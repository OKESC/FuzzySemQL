/**
 * Copyright (c) OKESC, Alberto Romo Valverde
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

/// <summary>
/// Provides methods for semantic comparison of strings using word embeddings.
/// Loads a pre-trained embeddings model from an embedded resource and allows comparison operations such as cosine similarity.
/// </summary>
public class SemanticComparison
{
    private Dictionary<string, float[]> _embeddings;
    private int _vecSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticComparison"/> class.
    /// Loads word embeddings from the embedded resource file.
    /// </summary>
    public SemanticComparison()
    {
        _embeddings = new Dictionary<string, float[]>(100000);
        LoadModelFromResource();
    }

    /// <summary>
    /// Loads the embeddings model data from the embedded resource file.
    /// The resource name is hardcoded and must match the embedded vector file in the assembly.
    /// </summary>
    private void LoadModelFromResource()
    {
        /*
         * FastText default models
         * https://fasttext.cc/docs/en/crawl-vectors.html
         
            @inproceedings{grave2018learning,
              title={Learning Word Vectors for 157 Languages},
              author={Grave, Edouard and Bojanowski, Piotr and Gupta, Prakhar and Joulin, Armand and Mikolov, Tomas},
              booktitle={Proceedings of the International Conference on Language Resources and Evaluation (LREC 2018)},
              year={2018}
            }
         */
        var assembly = Assembly.GetExecutingAssembly();
        string resourceName = "FuzzySemQL.cc.en.300.short.vec";
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
            {
                string line;
                int lineNo = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (lineNo == 0 && line.Split().Length == 2) { lineNo++; continue; }
                    var parts = line.Trim().Split(' ');
                    if (parts.Length < 2) continue;
                    string word = parts[0];
                    int dims = parts.Length - 1;
                    if (_vecSize == 0) _vecSize = dims;

                    float[] vec = new float[dims];
                    for (int i = 0; i < dims; i++)
                        vec[i] = float.Parse(parts[i + 1], CultureInfo.InvariantCulture);

                    _embeddings[word] = vec;
                    lineNo++;
                }
            }
        }
    }

    /// <summary>
    /// Tokenizes the input text into lowercase alphanumeric tokens, removing punctuation and special characters.
    /// </summary>
    /// <param name="text">The input text to tokenize.</param>
    /// <returns>An array of string tokens. Returns an empty array if text is null or empty.</returns>
    public string[] Tokenize(string text)
    {
        if (string.IsNullOrEmpty(text)) return new string[0];
        string clean = Regex.Replace(text.ToLowerInvariant(), @"[^\w\dá-úüñ]", " ");
        string[] tokens = clean.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return tokens;
    }

    /// <summary>
    /// Computes the sentence embedding vector for a given text by averaging the word embeddings of its tokens.
    /// </summary>
    /// <param name="text">The input sentence or text.</param>
    /// <returns>
    /// A float array representing the averaged embedding of the sentence.
    /// Returns <c>null</c> if none of the tokens are found in the vocabulary.
    /// </returns>
    public float[] GetSentenceEmbedding(string text)
    {
        string[] tokens = Tokenize(text);
        float[] sum = new float[_vecSize];
        int count = 0;
        foreach (string token in tokens)
        {
            float[] v;
            if (_embeddings.TryGetValue(token, out v))
            {
                for (int i = 0; i < _vecSize; i++)
                    sum[i] += v[i];
                count++;
            }
        }
        if (count == 0) return null;
        for (int i = 0; i < _vecSize; i++)
            sum[i] /= count;
        return sum;
    }

    /// <summary>
    /// Calculates the cosine similarity between two float vectors.
    /// Cosine similarity is a measure of similarity between two non-zero vectors of an inner product space.
    /// </summary>
    /// <param name="v1">The first vector.</param>
    /// <param name="v2">The second vector.</param>
    /// <returns>
    /// A value between -1 and 1 indicating the similarity.
    /// Returns 0.0 if either vector is zero.
    /// </returns>
    /// <example>
    /// <code>
    /// float[] v1 = ...;
    /// float[] v2 = ...;
    /// double similarity = comp.CosineSimilarity(v1, v2);
    /// </code>
    /// </example>
    public double CosineSimilarity(float[] v1, float[] v2)
    {
        double dot = 0, norm1 = 0, norm2 = 0;
        for (int i = 0; i < v1.Length; i++)
        {
            dot += v1[i] * v2[i];
            norm1 += v1[i] * v1[i];
            norm2 += v2[i] * v2[i];
        }
        if (norm1 == 0 || norm2 == 0) return 0.0;
        return dot / (Math.Sqrt(norm1) * Math.Sqrt(norm2));
    }
}