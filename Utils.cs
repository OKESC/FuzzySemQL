/**
 * Copyright (c) OKESC, Alberto Romo Valverde
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System;

/// <summary>
/// Provides utility functions for string processing, including fuzzy matching operations.
/// </summary>
public class Utils
{
    /// <summary>
    /// Computes the Levenshtein distance between two strings.
    /// The Levenshtein distance is a measure of the minimum number of single-character edits
    /// (insertions, deletions or substitutions) required to change one string into the other.
    /// </summary>
    /// <param name="s">The first string to compare.</param>
    /// <param name="t">The second string to compare.</param>
    /// <returns>
    /// The Levenshtein distance between the two input strings. If one of the strings is null or empty,
    /// returns the length of the other string.
    /// </returns>
    /// <example>
    /// <code>
    /// int distance = Utils.Levenshtein("kitten", "sitting");
    /// // distance == 3
    /// </code>
    /// </example>
    public static int Levenshtein(string s, string t)
    {
        if (string.IsNullOrEmpty(s)) return (t ?? "").Length;
        if (string.IsNullOrEmpty(t)) return s.Length;

        int n = s.Length, m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; i++) d[i, 0] = i;
        for (int j = 0; j <= m; j++) d[0, j] = j;

        for (int i = 1; i <= n; i++)
            for (int j = 1; j <= m; j++)
            {
                int cost = s[i - 1] == t[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(
                      d[i - 1, j] + 1,
                      d[i, j - 1] + 1),
                      d[i - 1, j - 1] + cost);
            }

        return d[n, m];
    }
}
