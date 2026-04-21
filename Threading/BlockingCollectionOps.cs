// =============================================================================
// RULE ID   : cr-dotnet-0039
// RULE NAME : Blocking Collection Operations
// CATEGORY  : Threading
// DESCRIPTION: Application uses blocking operations on collections like
//              BlockingCollection<T>.Take() or producer-consumer patterns without
//              timeout handling. Blocking operations reduce scalability in cloud.
// =============================================================================
using System.Collections.Concurrent;
using System.Threading;

namespace SyntheticLegacyApp.Threading
{
    public class BlockingCollectionOps
    {
        // VIOLATION cr-dotnet-0039: BlockingCollection as work queue without timeout
        private readonly BlockingCollection<string> _workQueue
            = new BlockingCollection<string>(boundedCapacity: 1000);

        public void ConsumeOrders()
        {
            while (true)
            {
                // VIOLATION cr-dotnet-0039: Take() blocks thread indefinitely
                string orderId = _workQueue.Take();
                ProcessOrder(orderId);
            }
        }

        public void ConsumeBatch()
        {
            // VIOLATION cr-dotnet-0039: GetConsumingEnumerable blocks until collection complete
            foreach (string item in _workQueue.GetConsumingEnumerable())
                ProcessOrder(item);
        }

        public string WaitForNextItem()
        {
            // VIOLATION cr-dotnet-0039: Blocking Take without CancellationToken
            return _workQueue.Take(CancellationToken.None);
        }

        private void ProcessOrder(string orderId) { }
    }
}
