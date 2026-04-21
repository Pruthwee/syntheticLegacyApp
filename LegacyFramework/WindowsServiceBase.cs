// =============================================================================
// RULE ID   : cr-dotnet-0060
// RULE NAME : Windows Service Base
// CATEGORY  : LegacyFramework
// DESCRIPTION: Application inherits from System.ServiceProcess.ServiceBase class
//              to implement Windows Services. Windows Services architecture doesn't
//              work in cloud PaaS environments, which use different process models
//              and lifecycle management.
// =============================================================================

using System;
using System.ServiceProcess;
using System.Threading;
using System.Timers;

namespace SyntheticLegacyApp.LegacyFramework
{
    // VIOLATION cr-dotnet-0060: Inherits ServiceBase — Windows Service host model
    public class OrderProcessingService : ServiceBase
    {
        private System.Timers.Timer _processingTimer;
        private CancellationTokenSource _cts;

        public OrderProcessingService()
        {
            // VIOLATION cr-dotnet-0060: ServiceName, CanStop, CanPauseAndContinue are SCM properties
            ServiceName         = "LegacyApp.OrderProcessing";
            CanStop             = true;
            CanPauseAndContinue = true;
            AutoLog             = true;
        }

        // VIOLATION cr-dotnet-0060: OnStart is the Windows SCM start lifecycle hook
        protected override void OnStart(string[] args)
        {
            _cts = new CancellationTokenSource();

            _processingTimer = new System.Timers.Timer(30_000); // 30 seconds
            _processingTimer.Elapsed += ProcessOrders;
            _processingTimer.AutoReset = true;
            _processingTimer.Start();

            Console.WriteLine($"{ServiceName} started.");
        }

        // VIOLATION cr-dotnet-0060: OnStop is Windows SCM stop lifecycle hook
        protected override void OnStop()
        {
            _cts?.Cancel();
            _processingTimer?.Stop();
            _processingTimer?.Dispose();
            Console.WriteLine($"{ServiceName} stopped.");
        }

        // VIOLATION cr-dotnet-0060: OnPause/OnContinue are SCM pause hooks — no equivalent in containers
        protected override void OnPause()
        {
            _processingTimer?.Stop();
            Console.WriteLine($"{ServiceName} paused.");
        }

        protected override void OnContinue()
        {
            _processingTimer?.Start();
            Console.WriteLine($"{ServiceName} resumed.");
        }

        // VIOLATION cr-dotnet-0060: OnCustomCommand handles SCM custom control codes
        protected override void OnCustomCommand(int command)
        {
            Console.WriteLine($"Custom SCM command received: {command}");
        }

        private void ProcessOrders(object sender, ElapsedEventArgs e)
        {
            if (_cts.IsCancellationRequested) return;
            Console.WriteLine("Processing pending orders...");
        }
    }

    // VIOLATION cr-dotnet-0060: Entry point using ServiceBase.Run — Windows SCM registration
    public class WindowsServiceEntryPoint
    {
        public static void Main()
        {
            ServiceBase[] servicesToRun =
            {
                new OrderProcessingService()
            };

            // VIOLATION cr-dotnet-0060: ServiceBase.Run registers with Windows SCM
            ServiceBase.Run(servicesToRun);
        }
    }
}
