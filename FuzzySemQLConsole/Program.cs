/**
 * Copyright (c) OKESC, Alberto Romo Valverde
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System;

/// <summary>
/// Console application for testing the FuzzySemantic functionality of the FuzzySemQL class.
/// Executes example sentence pair comparisons and prints the resulting similarity scores.
/// </summary>
class Program
{
    /// <summary>
    /// Entry point of the application.
    /// Runs several test cases using the FuzzySemQL.FuzzySemantic method to compare sentence similarity.
    /// Results are printed to the console for evaluation.
    /// </summary>
    /// <param name="args">Command line arguments (not used).</param>
    static void Main(string[] args)
    {
        // Test 1: Very similar sentences, expect a high similarity score
        double result = FuzzySemQL.FuzzySemantic("The boy is playing in the park", "A boy played in the playground", null, null).Value;
        System.Console.WriteLine(result);

        // Test 2: Sentences with moderate semantic similarity
        result = FuzzySemQL.FuzzySemantic("The weather is very hot today", "It is sunny and warm outside", null, null).Value;
        System.Console.WriteLine(result);

        // Test 3: Sentences with little or no semantic relation, expect a low similarity score
        result = FuzzySemQL.FuzzySemantic("The boy is playing in the park", "People are eating breakfast", null, null).Value;
        System.Console.WriteLine(result);

        // Hold the console open until a key is pressed to view results
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}