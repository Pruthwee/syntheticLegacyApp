// =============================================================================
// RULE ID   : cr-dotnet-0050
// RULE NAME : Performance Counters
// CATEGORY  : Platform
// DESCRIPTION: Application uses Windows Performance Counters through
//              System.Diagnostics.PerformanceCounter for monitoring and
//              instrumentation. Performance Counters don't exist on non-Windows
//              cloud platforms, causing monitoring failures.
// =============================================================================

using System;
using System.Diagnostics;

namespace SyntheticLegacyApp.Platform
{
    public class ApplicationMetricsCollector
    {
        // VIOLATION cr-dotnet-0050: PerformanceCounter instances bound to Windows PerfMon
        private readonly PerformanceCounter _requestsPerSecond;
        private readonly PerformanceCounter _errorRate;
        private readonly PerformanceCounter _cpuUsage;
        private readonly PerformanceCounter _availableMemory;

        public ApplicationMetricsCollector()
        {
            // VIOLATION cr-dotnet-0050: Creating custom category — requires Windows PerfMon registry
            const string CategoryName = "LegacyApp";

            if (!PerformanceCounterCategory.Exists(CategoryName))
            {
                var counters = new CounterCreationDataCollection
                {
                    new CounterCreationData("RequestsPerSec",
                        "Requests processed per second",
                        PerformanceCounterType.RateOfCountsPerSecond32),
                    new CounterCreationData("ErrorRate",
                        "Errors per second",
                        PerformanceCounterType.RateOfCountsPerSecond32)
                };
                // VIOLATION cr-dotnet-0050: PerformanceCounterCategory.Create — Windows admin rights required
                PerformanceCounterCategory.Create(CategoryName,
                    "LegacyApp metrics", PerformanceCounterCategoryType.SingleInstance, counters);
            }

            // VIOLATION cr-dotnet-0050: Instantiating writeable custom performance counters
            _requestsPerSecond = new PerformanceCounter(CategoryName, "RequestsPerSec", false);
            _errorRate         = new PerformanceCounter(CategoryName, "ErrorRate", false);

            // VIOLATION cr-dotnet-0050: Reading built-in Windows system counters
            _cpuUsage       = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _availableMemory = new PerformanceCounter("Memory", "Available MBytes");
        }

        public void RecordRequest()
        {
            // VIOLATION cr-dotnet-0050: Incrementing counter — no-op on non-Windows platforms
            _requestsPerSecond.Increment();
        }

        public void RecordError()
        {
            _errorRate.Increment();
        }

        public void LogSystemHealth()
        {
            // VIOLATION cr-dotnet-0050: Reading system counters — fail silently or throw on Linux/containers
            float cpu = _cpuUsage.NextValue();
            float mem = _availableMemory.NextValue();
            Console.WriteLine($"CPU: {cpu:F1}%  Available Memory: {mem:F0} MB");
        }

        public void Dispose()
        {
            _requestsPerSecond?.Dispose();
            _errorRate?.Dispose();
            _cpuUsage?.Dispose();
            _availableMemory?.Dispose();
        }
    }
}
