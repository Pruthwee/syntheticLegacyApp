// =============================================================================
// RULE ID   : cr-dotnet-0127
// RULE NAME : Missing Graceful Shutdown Hooks
// CATEGORY  : Runtime
// DESCRIPTION: Application lacks proper shutdown event handling through
//              AppDomain.ProcessExit, Console.CancelKeyPress, IHostedService.StopAsync,
//              or other graceful shutdown mechanisms. This affects proper application
//              lifecycle management in cloud environments.
// =============================================================================

using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;

namespace SyntheticLegacyApp.Runtime
{
    public class OrderBatchProcessor
    {
        private SqlConnection _dbConnection;
        private StreamWriter  _auditWriter;
        private bool          _isRunning;

        public void Start()
        {
            // VIOLATION cr-dotnet-0127: No AppDomain.ProcessExit handler registered
            // No Console.CancelKeyPress handler registered
            // No IHostedService.StopAsync implementation
            // In Kubernetes, SIGTERM will kill the process without any cleanup

            _dbConnection = new SqlConnection("Server=sqlserver;Database=Orders;...");
            _dbConnection.Open();

            _auditWriter = new StreamWriter(@"C:\Logs\batch_audit.log", append: true);
            _isRunning = true;

            Console.WriteLine("Batch processor started — no shutdown hook registered.");
            ProcessLoop();
        }

        // VIOLATION cr-dotnet-0127: Infinite loop with no cooperative cancellation token
        private void ProcessLoop()
        {
            while (_isRunning) // no CancellationToken check
            {
                try
                {
                    ProcessNextBatch();
                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    // VIOLATION cr-dotnet-0127: Swallowing all exceptions — no graceful drain
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private void ProcessNextBatch()
        {
            // Simulated batch work — in-flight transactions will be lost on SIGTERM
            using (var cmd = new SqlCommand("exec usp_ProcessNextBatch", _dbConnection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        // VIOLATION cr-dotnet-0127: Cleanup method exists but is never wired to any shutdown event
        public void Cleanup()
        {
            _isRunning = false;
            _dbConnection?.Close();
            _dbConnection?.Dispose();
            _auditWriter?.Flush();
            _auditWriter?.Dispose();
            Console.WriteLine("Cleanup completed — but this method is never called on SIGTERM.");
        }
    }

    public class LegacyHttpServer
    {
        private System.Net.HttpListener _listener;

        public void Start(string prefix)
        {
            _listener = new System.Net.HttpListener();
            _listener.Prefixes.Add(prefix);
            _listener.Start();

            // VIOLATION cr-dotnet-0127: No graceful drain of in-flight requests on shutdown
            // Kubernetes sends SIGTERM → pod terminates immediately, active requests are dropped
            Console.WriteLine($"Listening on {prefix} — no graceful drain on shutdown.");

            while (true) // VIOLATION cr-dotnet-0127: Blocking loop, no cancellation support
            {
                var context = _listener.GetContext(); // blocking — cannot be interrupted cleanly
                System.Threading.Tasks.Task.Run(() => HandleRequest(context));
            }
        }

        private void HandleRequest(System.Net.HttpListenerContext ctx)
        {
            ctx.Response.StatusCode = 200;
            ctx.Response.Close();
        }
    }

    public class ApplicationEntryPoint
    {
        public static void Main(string[] args)
        {
            // VIOLATION cr-dotnet-0127: No AppDomain.CurrentDomain.ProcessExit registration
            // VIOLATION cr-dotnet-0127: No Console.CancelKeyPress handler
            // VIOLATION cr-dotnet-0127: No Environment.Exit code cleanup path

            var processor = new OrderBatchProcessor();
            processor.Start(); // blocks — any SIGTERM kills the process immediately
        }
    }
}
