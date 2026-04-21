// =============================================================================
// RULE ID   : cr-dotnet-0024
// RULE NAME : Timer Without Synchronization
// CATEGORY  : Threading
// DESCRIPTION: Application uses System.Threading.Timer or System.Timers.Timer for
//              scheduled operations without coordination across multiple instances.
//              In horizontally scaled cloud deployments, all instances execute
//              the same operations simultaneously.
// =============================================================================
using System;
using System.Threading;
using System.Timers;

namespace SyntheticLegacyApp.Threading
{
    public class TimerNoSync
    {
        // VIOLATION cr-dotnet-0024: No distributed lock around timer callback
        private System.Threading.Timer _cleanupTimer;
        private System.Timers.Timer    _reportTimer;

        public void StartTimers()
        {
            // VIOLATION cr-dotnet-0024: Every scaled instance will run cleanup simultaneously
            _cleanupTimer = new System.Threading.Timer(
                RunCleanupJob, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));

            // VIOLATION cr-dotnet-0024: Timers.Timer also uncoordinated across pods
            _reportTimer = new System.Timers.Timer(interval: 3_600_000); // 1 hour
            _reportTimer.Elapsed  += GenerateDailyReport;
            _reportTimer.AutoReset = true;
            _reportTimer.Start();
        }

        private void RunCleanupJob(object state)      { /* No distributed lock */ }
        private void GenerateDailyReport(object s, ElapsedEventArgs e) { /* No leader election */ }
    }
}
