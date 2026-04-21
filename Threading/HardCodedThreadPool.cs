// =============================================================================
// RULE ID   : cr-dotnet-0022
// RULE NAME : Hard-coded ThreadPool Size
// CATEGORY  : Threading
// DESCRIPTION: Application configures ThreadPool with fixed minimum and maximum
//              thread counts. Hard-coded values do not adapt to cloud resource
//              availability and prevent optimal utilization across instance types.
// =============================================================================
using System.Threading;

namespace SyntheticLegacyApp.Threading
{
    public class HardCodedThreadPool
    {
        public void ConfigureThreadPool()
        {
            // VIOLATION cr-dotnet-0022: ThreadPool sized for specific on-prem server spec
            ThreadPool.SetMinThreads(workerThreads: 50, completionPortThreads: 50);
            ThreadPool.SetMaxThreads(workerThreads: 200, completionPortThreads: 200);
        }

        public void ConfigureForBatchProcessing()
        {
            // VIOLATION cr-dotnet-0022: Fixed pool size assumes known CPU count
            int cpuCount = System.Environment.ProcessorCount;
            ThreadPool.SetMinThreads(cpuCount * 4, cpuCount * 4);
            ThreadPool.SetMaxThreads(cpuCount * 8, cpuCount * 8);
        }

        public void BoostForPeakLoad()
        {
            // VIOLATION cr-dotnet-0022: Hard-coded peak value overrides adaptive sizing
            ThreadPool.SetMaxThreads(500, 500);
        }
    }
}
