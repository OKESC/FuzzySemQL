/**
 * Copyright (c) OKESC, Alberto Romo Valverde
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using Microsoft.SqlServer.Server;
using System;
using System.Data.SqlTypes;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Main class exposing fuzzy and semantic comparison functions for SQL Server integration.
/// Provides SQL CLR functions to be called directly from SQL Server queries.
/// </summary>
public partial class FuzzySemQL
{
    /// <summary>
    /// Performs a fuzzy and semantic similarity comparison between two strings.
    /// This SQL CLR function combines LIKE, normalized Levenshtein, and semantic embedding (cosine similarity) scores,
    /// and returns a combined similarity score between 0 and 1.
    /// </summary>
    /// <param name="stringOne">The first string for comparison (typically the query or search term).</param>
    /// <param name="stringTwo">The second string for comparison (typically the candidate or result).</param>
    /// <param name="embeddingOne">Optional embedding for the first string as a <see cref="SqlBytes"/> (nullable).</param>
    /// <param name="embeddingTwo">Optional embedding for the second string as a <see cref="SqlBytes"/> (nullable).</param>
    /// <returns>
    /// A <see cref="SqlDouble"/> representing the similarity score between the two input strings, normalized between 0 and 1.
    /// If either string is null or empty, returns 0.
    /// </returns>
    /// <remarks>
    /// The function gives priority to an exact or substring match (returns 1),
    /// then combines the maximum of normalized Levenshtein and semantic scores,
    /// and applies corrections based on coverage of search tokens in the target string.
    /// </remarks>
    [Microsoft.SqlServer.Server.SqlFunction(IsDeterministic = true, IsPrecise = false)]
    public static SqlDouble FuzzySemantic(SqlString stringOne, SqlString stringTwo, SqlBytes embeddingOne, SqlBytes embeddingTwo)
    {
        string search = stringOne.Value;
        string name = stringTwo.Value;

        if (string.IsNullOrEmpty(search) || string.IsNullOrEmpty(name)) return 0;

        // 1. LIKE test
        if (name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
            search.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
            return 1;

        // 2. Norm Levenshtein
        int lev = Utils.Levenshtein(search.ToLowerInvariant(), name.ToLowerInvariant());
        int maxLen = Math.Max(search.Length, name.Length);
        if (maxLen == 0) return 0;
        double levScore = 1.0 - ((double)lev / maxLen);

        // 3. Semantic Embedding
        float[] embA = null;
        float[] embB = null;

        if (embeddingOne != null && !embeddingOne.IsNull)
        {
            byte[] bytes = embeddingOne.Value;
            int dim = bytes.Length / sizeof(float);
            float[] tmp = new float[dim];
            Buffer.BlockCopy(bytes, 0, tmp, 0, bytes.Length);
            embA = tmp;
        }
        else
        {
            embA = SemanticComparisonSingleton.Instance.GetSentenceEmbedding(search);
        }
        if (embeddingTwo != null && !embeddingTwo.IsNull)
        {
            byte[] bytes = embeddingTwo.Value;
            int dim = bytes.Length / sizeof(float);
            float[] tmp = new float[dim];
            Buffer.BlockCopy(bytes, 0, tmp, 0, bytes.Length);
            embB = tmp;
        }
        else
        {
            embB = SemanticComparisonSingleton.Instance.GetSentenceEmbedding(name);
        }

        double semSim = 0;
        if (embA != null && embB != null)
            semSim = SemanticComparisonSingleton.Instance.CosineSimilarity(embA, embB);

        // 4. score = MAX(levScore, semSim) (optional) threshold 0.3-0.4
        double finalScore = Math.Max(levScore, semSim);

        string[] queryTokens = Regex.Matches(search.ToLowerInvariant(), @"\w+")
                    .Cast<Match>().Select(m => m.Value).ToArray();
        string nombreLower = name.ToLowerInvariant();

        int hits = 0;
        foreach (string token in queryTokens)
        {
            // Alone or inside word as Contains
            if (nombreLower.Contains(token))
                hits++;
        }
        double tokenCoverage = queryTokens.Length == 0 ? 0 : (double)hits / queryTokens.Length;

        if (tokenCoverage == 1.0)
            finalScore = Math.Max(finalScore, 0.98);
        else if (tokenCoverage > 0.5)
        {
            // Lineal bonus between 0.6 y 0.9 upon tokenCoverage
            double lexBonus = 0.5 + 0.35 * tokenCoverage; // [0.8 ... 0.96] for [0.5 ... 0.9+]
            finalScore = Math.Max(finalScore, lexBonus);
        }

        return Math.Min(finalScore, 1.0);
    }

    /// <summary>
    /// Generates a sentence embedding for the given text as a binary array compatible with SQL Server.
    /// This SQL CLR function converts a text string into its float embedding, then serializes it to a <see cref="SqlBytes"/> object.
    /// </summary>
    /// <param name="text">The input text to embed.</param>
    /// <returns>
    /// A <see cref="SqlBytes"/> object containing the serialized embedding as a byte array,
    /// or <see cref="SqlBytes.Null"/> if the embedding could not be generated.
    /// </returns>
    [SqlFunction]
    public static SqlBytes GetEmbedding(SqlString text)
    {
        if (text.IsNull) return SqlBytes.Null;
        var embedder = SemanticComparisonSingleton.Instance;
        float[] vector = embedder.GetSentenceEmbedding(text.Value);
        if (vector == null) return SqlBytes.Null;

        // Vector to byte[] serialization
        byte[] bytes = new byte[vector.Length * sizeof(float)];
        Buffer.BlockCopy(vector, 0, bytes, 0, bytes.Length);

        return new SqlBytes(bytes);
    }
};
