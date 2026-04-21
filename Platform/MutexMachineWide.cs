// =============================================================================
// RULE ID   : cr-dotnet-0052
// RULE NAME : Mutex Machine-Wide
// CATEGORY  : Platform
// DESCRIPTION: Application uses machine-wide named mutexes through
//              System.Threading.Mutex for process synchronization. Machine-wide
//              mutexes don't work in distributed cloud systems where processes
//              run across multiple instances.
// =============================================================================

using System;
using System.Threading;

namespace SyntheticLegacyApp.Platform
{
    public class SingleInstanceGuard
    {
        // VIOLATION cr-dotnet-0052: Global\ prefix creates a machine-wide named mutex
        private const string MutexName = @"Global\LegacyApp_SingleInstance";
        private Mutex _mutex;

        public bool TryAcquireExclusiveLock()
        {
            // VIOLATION cr-dotnet-0052: Named mutex scoped to the entire Windows machine
            _mutex = new Mutex(initiallyOwned: false, name: MutexName,
                out bool createdNew);

            if (!createdNew)
            {
                Console.WriteLine("Another instance is already running on this machine.");
                return false;
            }

            Console.WriteLine("Acquired machine-wide mutex. Running as sole instance.");
            return true;
        }

        public void Release()
        {
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
        }
    }

    public class ExclusiveJobScheduler
    {
        // VIOLATION cr-dotnet-0052: Machine-wide mutex for distributed job coordination
        // In a cloud pod cluster, each pod acquires its own "machine" mutex independently
        private const string JobMutexName = @"Global\LegacyApp_NightlyBatchJob";

        public void RunNightlyBatch()
        {
            // VIOLATION cr-dotnet-0052: WaitOne assumes only one machine runs this job
            using (var mutex = new Mutex(false, JobMutexName))
            {
                bool acquired = mutex.WaitOne(TimeSpan.FromSeconds(10), exitContext: false);

                if (!acquired)
                {
                    Console.WriteLine("Batch already running on this machine. Skipping.");
                    return;
                }

                try
                {
                    Console.WriteLine("Running nightly batch job...");
                    // In multi-pod deployments, EVERY pod acquires its own mutex — no actual exclusion
                    Thread.Sleep(2000);
                    Console.WriteLine("Batch complete.");
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }
    }

    public class LegacyResourceLockManager
    {
        // VIOLATION cr-dotnet-0052: Named mutex used to serialise access to a shared local file
        public void WriteAuditLog(string entry)
        {
            using (var mutex = new Mutex(false, @"Global\LegacyApp_AuditLog_Lock"))
            {
                mutex.WaitOne();
                try
                {
                    System.IO.File.AppendAllText(
                        @"C:\Logs\audit.log", entry + Environment.NewLine);
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }
    }
}
