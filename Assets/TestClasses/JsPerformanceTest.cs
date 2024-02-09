using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using JsTools;
using UnityEngine;

public class JsPerformanceTest : MonoBehaviour
{
    IEnumerator Start()
    {
        var iterations = 100;
        using var puerTests = new PuertsTests("puer_tests.mjs", iterations);
        puerTests.Initialize();
        using var v8Tests = new BanzaiJsTests(EngineTag.V8, iterations);
        v8Tests.Initialize();
        var tests = new List<IScriptTest> { puerTests, v8Tests };

        // run jsc tests on iOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            using var jscTests = new BanzaiJsTests(EngineTag.JavaScriptCore, iterations);
            tests.Add(jscTests);
        }

        yield return new WaitForSeconds(1);
        Debug.Log("Starting tests...");
        for (var i = 0; i < 20; i++)
        {
            Debug.Log(DoTest(tests, test => test.TestGetPlayerProgress(),
                "TestGetPlayerProgress"));
            yield return null;
            Debug.Log(DoTest(tests, test => test.TestSetPlayerProgress(),
                "TestSetPlayerProgress"));
            yield return null;
            Debug.Log(DoTest(tests, test => test.TestFibonacci(Mathf.Min((i + 1) * 2, 20)),
                "TestFibonacci"));
            yield return null;
        }

        Debug.Log("Tests finished.");
    }

    public string DoTest(IEnumerable<IScriptTest> scriptTests, Func<IScriptTest, long> exec,
        string testMethodName)
    {
        var results = scriptTests.Select(x => (x.GetType().Name, exec(x)));
        return FormatResult(results, testMethodName);
    }

    private string FormatResult(IEnumerable<(string TestName, long TestResult)> args, string testMethodName = "")
    {
        long fastestTime = long.MaxValue;
        long slowestTime = long.MinValue;

        // Find the fastest and slowest times
        foreach (var (_, time) in args)
        {
            fastestTime = Math.Min(fastestTime, time);
            slowestTime = Math.Max(slowestTime, time);
        }

        // Format results with color based on test performance
        var formattedResults = new List<string>();
        foreach (var (testName, time) in args)
        {
            formattedResults.Add($"{testName}: {FormatResult(time, fastestTime, slowestTime)}");
        }

        return $"{testMethodName}: {string.Join(", ", formattedResults)}";
    }

    private string FormatResult(long time, long fastestTime, long slowestTime)
    {
        // Calculate the performance ratio
        double performanceRatio = (fastestTime - time) / (double)(fastestTime - slowestTime);
        // Map the execution time to color intensity (inverse relation)
        int red = (int)(255 * performanceRatio);
        int green = 255 - red;

        // Format the result with the interpolated color
        return $"<color=#{ToHex(red)}{ToHex(green)}00>{time}</color>";
    }

    // Helper method to convert an integer to a two-digit hexadecimal representation
    private string ToHex(int value)
    {
        return value.ToString("X2");
    }
}