// =============================================================================
// RULE ID   : cr-dotnet-0124
// RULE NAME : Fixed Thread Affinity/Pinning
// CATEGORY  : Threading
// DESCRIPTION: Application attempts to control thread processor affinity using
//              Process.ProcessorAffinity or makes assumptions about CPU core binding.
//              Cloud environments have dynamic CPU allocation without guaranteed
//              core assignments, causing affinity operations to fail or be ignored.
// =============================================================================
using System;
using System.Diagnostics;
using System.Threading;

namespace SyntheticLegacyApp.Threading
{
    public class FixedThreadAffinity
    {
        public void PinProcessToCores()
        {
            // VIOLATION cr-dotnet-0124: CPU affinity mask - ignored or throws in cloud VMs
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0b0000_1111; // cores 0-3
        }

        public void RunCriticalWorkOnCore(int coreIndex, Action work)
        {
            var thread = new Thread(() =>
            {
                // VIOLATION cr-dotnet-0124: Assuming specific core will always be available
                Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << coreIndex);
                work();
            });
            thread.Start();
            thread.Join();
        }

        public void OptimizeForNumaNode()
        {
            // VIOLATION cr-dotnet-0124: NUMA node awareness assumes bare-metal topology
            int coreCount = Environment.ProcessorCount;
            long mask = (1L << (coreCount / 2)) - 1;
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)mask;
        }
    }
}
