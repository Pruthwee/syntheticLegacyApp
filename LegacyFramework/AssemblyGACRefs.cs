// =============================================================================
// RULE ID   : cr-dotnet-0049
// RULE NAME : Assembly GAC References
// CATEGORY  : LegacyFramework
// DESCRIPTION: Application references assemblies installed in the Global Assembly
//              Cache (GAC) through strong-named references. GAC doesn't exist in
//              cloud platforms, causing assembly loading failures and preventing
//              application startup.
// =============================================================================

using System;
using System.Reflection;

namespace SyntheticLegacyApp.LegacyFramework
{
    public class GACAssemblyLoader
    {
        // VIOLATION cr-dotnet-0049: Strong-named GAC reference — assembly must be installed in GAC
        private const string GacAssemblyFullName =
            "Microsoft.ReportViewer.Common, Version=12.0.0.0, Culture=neutral, " +
            "PublicKeyToken=89845dcd8080cc91";

        // VIOLATION cr-dotnet-0049: Loading assembly by full strong name — resolves from GAC
        public Assembly LoadReportViewerFromGac()
        {
            return Assembly.Load(GacAssemblyFullName);
        }

        // VIOLATION cr-dotnet-0049: Runtime GAC probe for versioned assembly
        public Type GetReportViewerType()
        {
            Assembly asm = Assembly.Load(GacAssemblyFullName);
            return asm.GetType("Microsoft.Reporting.WinForms.ReportViewer");
        }

        // VIOLATION cr-dotnet-0049: Loading Crystal Reports runtime from GAC
        public Assembly LoadCrystalRuntimeFromGac()
        {
            return Assembly.Load(
                "CrystalDecisions.CrystalReports.Engine, Version=13.0.2000.0, " +
                "Culture=neutral, PublicKeyToken=692fbea5521e1304");
        }

        // VIOLATION cr-dotnet-0049: Checking GAC-registered version of a system component
        public string GetGacAssemblyVersion(string assemblyName)
        {
            try
            {
                Assembly asm = Assembly.Load(assemblyName);
                return asm.GetName().Version?.ToString() ?? "unknown";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GAC load failed for '{assemblyName}': {ex.Message}");
                return null;
            }
        }

        // VIOLATION cr-dotnet-0049: Enumerating GAC via reflection heuristic
        public void LogGacDependencies()
        {
            string[] gacAssemblies =
            {
                "Microsoft.ReportViewer.Common, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91",
                "Microsoft.VisualBasic.PowerPacks.Vs, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                "Interop.ADODB, Version=7.0.3300.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
            };

            foreach (var name in gacAssemblies)
            {
                try
                {
                    Assembly.Load(name);
                    Console.WriteLine($"[OK] {name}");
                }
                catch
                {
                    Console.WriteLine($"[MISSING] {name}");
                }
            }
        }
    }
}
