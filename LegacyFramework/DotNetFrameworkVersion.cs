// =============================================================================
// RULE ID   : cr-dotnet-0025
// RULE NAME : .NET Framework < 4.6.1
// CATEGORY  : LegacyFramework
// DESCRIPTION: Application targets .NET Framework versions prior to 4.6.1, missing
//              important cloud-friendly features like improved TLS support, enhanced
//              cryptography, and compatibility improvements essential for reliable
//              cloud deployment.
// =============================================================================

// VIOLATION cr-dotnet-0025: TargetFrameworkAttribute declares targeting .NET 4.5
// The equivalent of this in the project file would be:
//   <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
// which predates 4.6.1's TLS 1.2 default, async improvements, and WCF compatibility fixes.

using System;
using System.Net;
using System.Runtime.Versioning;

// VIOLATION cr-dotnet-0025: Assembly-level target framework attribute pinned to < 4.6.1
[assembly: TargetFramework(".NETFramework,Version=v4.5",
    FrameworkDisplayName = ".NET Framework 4.5")]

namespace SyntheticLegacyApp.LegacyFramework
{
    public class LegacyFrameworkBootstrap
    {
        // VIOLATION cr-dotnet-0025: Explicit TLS 1.0/1.1 because .NET 4.5 does not default to TLS 1.2
        public void ConfigureNetworkSecurity()
        {
            // In .NET < 4.6.1, TLS 1.2 is not the default — must be set manually
            // Older apps often set Ssl3 | Tls (1.0) as a workaround, which is insecure
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;

            Console.WriteLine("Network security configured for .NET 4.5 (TLS 1.0/SSL3).");
        }

        // VIOLATION cr-dotnet-0025: Using obsolete async patterns absent from < 4.6.1 improvements
        public IAsyncResult BeginLegacyOperation(AsyncCallback callback, object state)
        {
            // APM (Asynchronous Programming Model) — superseded in 4.6.1+ with better TAP support
            return new LegacyAsyncResult(callback, state);
        }

        public void EndLegacyOperation(IAsyncResult result)
        {
            // No-op: represents async completion pre-4.6.1 style
        }

        // VIOLATION cr-dotnet-0025: Checking runtime version at runtime due to targeting constraints
        public void ValidateFrameworkVersion()
        {
            string runtimeVer = System.Runtime.InteropServices.RuntimeEnvironment
                .GetRuntimeDirectory();

            Console.WriteLine($"CLR runtime: {runtimeVer}");

            // Guard that reveals the app was designed for < 4.6.1
            if (Environment.Version.Major < 4 ||
               (Environment.Version.Major == 4 && Environment.Version.Minor < 6))
            {
                Console.WriteLine("WARNING: Running on .NET < 4.6 — TLS 1.2 not guaranteed.");
            }
        }
    }

    internal class LegacyAsyncResult : IAsyncResult
    {
        private readonly AsyncCallback _callback;
        public LegacyAsyncResult(AsyncCallback cb, object state) { _callback = cb; AsyncState = state; }
        public bool       IsCompleted      => true;
        public System.Threading.WaitHandle AsyncWaitHandle => null;
        public object     AsyncState        { get; }
        public bool       CompletedSynchronously => true;
    }
}
