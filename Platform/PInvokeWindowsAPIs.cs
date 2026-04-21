// =============================================================================
// RULE ID   : cr-dotnet-0042
// RULE NAME : P/Invoke Windows APIs
// CATEGORY  : Platform
// DESCRIPTION: Application uses Platform Invoke (P/Invoke) to call Windows-specific
//              APIs through DllImport attributes targeting kernel32.dll, user32.dll,
//              or other Windows system libraries. These APIs don't exist on
//              non-Windows cloud platforms, causing native call failures.
// =============================================================================

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SyntheticLegacyApp.Platform
{
    public class NativeWindowsInterop
    {
        // VIOLATION cr-dotnet-0042: DllImport targeting kernel32.dll — Windows-only DLL
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetCurrentProcess();

        // VIOLATION cr-dotnet-0042: kernel32 memory status — unavailable outside Windows
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        // VIOLATION cr-dotnet-0042: user32.dll call — Windows UI subsystem, not present in containers
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        // VIOLATION cr-dotnet-0042: advapi32.dll — Windows security/registry APIs
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LookupAccountName(
            string systemName, string accountName,
            byte[] sid, ref int cbSid,
            StringBuilder domainName, ref int cbDomainName,
            out int peUse);

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        public long GetAvailablePhysicalMemory()
        {
            // VIOLATION cr-dotnet-0042: Calls Windows-only GlobalMemoryStatusEx via P/Invoke
            var memStatus = new MEMORYSTATUSEX { dwLength = 64 };
            if (GlobalMemoryStatusEx(ref memStatus))
                return (long)memStatus.ullAvailPhys;
            throw new InvalidOperationException("GlobalMemoryStatusEx failed: " +
                Marshal.GetLastWin32Error());
        }

        public void ShowNativeAlert(string message)
        {
            // VIOLATION cr-dotnet-0042: user32 MessageBox — requires Windows GUI subsystem
            MessageBox(IntPtr.Zero, message, "Legacy Alert", 0);
        }

        public IntPtr GetProcessHandle()
        {
            // VIOLATION cr-dotnet-0042: kernel32 GetCurrentProcess — not portable
            return GetCurrentProcess();
        }

        public string ResolveWindowsAccount(string accountName)
        {
            // VIOLATION cr-dotnet-0042: advapi32 LookupAccountName — AD/Windows only
            byte[] sid = new byte[256];
            int cbSid = 256;
            var domain = new StringBuilder(256);
            int cbDomain = 256;

            LookupAccountName(null, accountName, sid, ref cbSid,
                domain, ref cbDomain, out _);

            return domain.ToString();
        }
    }
}
