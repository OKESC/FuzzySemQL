/**
 * Copyright (c) OKESC, Alberto Romo Valverde
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System;

/// <summary>
/// Singleton wrapper for the <see cref="SemanticComparison"/> class.
/// Ensures that only one instance of <see cref="SemanticComparison"/> exists throughout the application.
/// </summary>
public class SemanticComparisonSingleton
{
    /// <summary>
    /// The singleton instance of <see cref="SemanticComparison"/>.
    /// </summary>
    private static volatile SemanticComparison _instance = null;
    /// <summary>
    /// An object used to synchronize thread access to the instance.
    /// </summary>
    private static object syncRoot = new Object();

    /// <summary>
    /// Gets the singleton instance of <see cref="SemanticComparison"/>.
    /// The instance is created in a thread-safe manner if it does not already exist.
    /// </summary>
    public static SemanticComparison Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (syncRoot)
                {
                    if (_instance == null)
                        _instance = new SemanticComparison();
                }
            }
            return _instance;
        }
    }
}