using System;
using System.Diagnostics;
using System.IO;
using FluentAssertions;

namespace SFA.DAS.Testing.PackageScanning;

/// <summary>
/// Functions for checking and asserting against vulnerable and deprecated package references
/// </summary>
public static class PackageChecker
{
    /// <summary>
    /// Asserts that projects in the given directory, including subdirectories, have no vulnerable or deprecated packages.
    /// Also logs the full package scan result to console.
    /// </summary>
    /// <param name="directory">The directory to look for csproj files in, e.g. the sln folder.</param>
    public static void AssertNoVulnerableOrDeprecatedPackages(string directory)
    {
        var scanResult = GetVulnerableAndDeprecatedPackages(directory);

        Console.WriteLine(scanResult.VulnerablePackageResults);
        Console.WriteLine(scanResult.DeprecatedPackageResults);

        foreach (var packageResult in scanResult.VulnerablePackageResults)
            HasFailure(packageResult).Should().BeFalse();

        foreach (var packageResult in scanResult.DeprecatedPackageResults)
            HasFailure(packageResult).Should().BeFalse();
    }

    /// <summary>
    /// Gets all vulnerable and deprecated packages in the given directory, including subdirectories.
    /// </summary>
    /// <param name="directory">The directory to look for csproj files in, e.g. the sln folder.</param>
    /// <returns>A PackageScanResult which contains a list of vulnerable and deprecated scan results for each project scanned.</returns>
    public static PackageScanResult GetVulnerableAndDeprecatedPackages(string directory)
    {
        var scanResult = new PackageScanResult();

        var csprojs = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories);

        foreach (var csprojPath in csprojs)
        {
            var vulnerableResult = RunCheck(csprojPath, "vulnerable");
            vulnerableResult.Should().NotBeNullOrWhiteSpace();
            scanResult.VulnerablePackageResults.Add(vulnerableResult);

            var deprecatedResult = RunCheck(csprojPath, "deprecated");
            deprecatedResult.Should().NotBeNullOrWhiteSpace();
            scanResult.DeprecatedPackageResults.Add(deprecatedResult);
        }

        return scanResult;
    }

    private static string RunCheck(string projectFilePath, string checkType)
    {
        var processStartInfo = new ProcessStartInfo {
            FileName = "dotnet",
            Arguments = $"list \"{projectFilePath}\" package --source nuget.org --{checkType}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processStartInfo);
        var output = process!.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception($"Command Failed: {processStartInfo.FileName} {processStartInfo.Arguments}:\n{error}");
        }

        return output;
    }

    private static bool HasFailure(string packageResult)
    {
        return packageResult.Contains("has the following");
    }
}