// =============================================================================
// RULE ID   : cr-dotnet-0047
// RULE NAME : MSBuild Custom Tasks
// CATEGORY  : LegacyFramework
// DESCRIPTION: Application build process depends on custom MSBuild tasks that
//              assume specific build server configurations. These dependencies
//              prevent successful builds in cloud build systems.
// =============================================================================

using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace SyntheticLegacyApp.LegacyFramework
{
    // VIOLATION cr-dotnet-0047: Custom MSBuild Task class — assumes MSBuild-aware build server
    public class DeployConfigTransformTask : Task
    {
        // VIOLATION cr-dotnet-0047: Required task input assumes environment variable from build agent
        [Required]
        public string TargetEnvironment { get; set; }

        // VIOLATION cr-dotnet-0047: Hard-coded local build server path input
        [Required]
        public string WebConfigPath { get; set; }

        public string TransformOutputPath { get; set; }

        public override bool Execute()
        {
            // VIOLATION cr-dotnet-0047: Reads local file system paths set up by build agent
            if (!File.Exists(WebConfigPath))
            {
                Log.LogError($"Web.config not found at: {WebConfigPath}");
                return false;
            }

            string transformFile = Path.Combine(
                Path.GetDirectoryName(WebConfigPath),
                $"Web.{TargetEnvironment}.config");

            if (!File.Exists(transformFile))
            {
                Log.LogWarning($"Transform file not found: {transformFile}. Skipping.");
                return true;
            }

            // VIOLATION cr-dotnet-0047: Invokes msdeploy.exe assumed to be on build agent
            string output = TransformOutputPath ?? Path.GetTempFileName();
            Log.LogMessage(MessageImportance.High,
                $"Applying {transformFile} to {WebConfigPath} → {output}");

            // Simulate msdeploy invocation
            System.Diagnostics.Process.Start("msdeploy.exe",
                $"-verb:sync -source:webConfig=\"{WebConfigPath}\" " +
                $"-dest:webConfig=\"{output}\" " +
                $"-enableRule:DoNotDeleteRule");

            return true;
        }
    }

    // VIOLATION cr-dotnet-0047: Second custom MSBuild task — stamps build info into AssemblyInfo
    public class StampBuildInfoTask : Task
    {
        [Required]
        public string AssemblyInfoPath { get; set; }

        // VIOLATION cr-dotnet-0047: BUILD_NUMBER injected by build agent environment variable
        public string BuildNumber { get; set; } = Environment.GetEnvironmentVariable("BUILD_NUMBER") ?? "0";

        public override bool Execute()
        {
            if (!File.Exists(AssemblyInfoPath))
            {
                Log.LogError($"AssemblyInfo.cs not found: {AssemblyInfoPath}");
                return false;
            }

            // VIOLATION cr-dotnet-0047: File-system manipulation assumed to work in all build envs
            string content = File.ReadAllText(AssemblyInfoPath);
            content = System.Text.RegularExpressions.Regex.Replace(
                content,
                @"AssemblyFileVersion\(""\d+\.\d+\.\d+\.\d+""\)",
                $@"AssemblyFileVersion(""1.0.0.{BuildNumber}"")");

            File.WriteAllText(AssemblyInfoPath, content);
            Log.LogMessage(MessageImportance.High, $"Build number {BuildNumber} stamped.");
            return true;
        }
    }
}
