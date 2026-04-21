// =============================================================================
// RULE ID   : cr-dotnet-0021
// RULE NAME : Thread.Start() Usage
// CATEGORY  : Threading
// DESCRIPTION: Application creates threads directly using Thread.Start() without
//              proper limits or resource management. In cloud environments with
//              shared resources, unbounded thread creation causes resource exhaustion.
// =============================================================================
using System.Threading;

namespace SyntheticLegacyApp.Threading
{
    public class ThreadStartUsage
    {
        public void ProcessOrdersInBackground(string[] orderIds)
        {
            foreach (string orderId in orderIds)
            {
                // VIOLATION cr-dotnet-0021: New Thread per item - unbounded thread creation
                var id = orderId;
                var thread = new Thread(() => ProcessSingleOrder(id));
                thread.Start();
            }
        }

        public void StartReportGenerationThread()
        {
            // VIOLATION cr-dotnet-0021: Long-running thread without lifecycle management
            var reportThread = new Thread(GenerateReports) { IsBackground = true };
            reportThread.Start();
        }

        public void RunParallel(System.Action[] actions)
        {
            var threads = new Thread[actions.Length];
            for (int i = 0; i < actions.Length; i++)
            {
                // VIOLATION cr-dotnet-0021: Array of threads without pooling
                threads[i] = new Thread(new ThreadStart(actions[i]));
                threads[i].Start();
            }
            foreach (var t in threads) t.Join();
        }

        private void ProcessSingleOrder(string orderId) { }
        private void GenerateReports() { }
    }
}
