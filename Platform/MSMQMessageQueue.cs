// =============================================================================
// RULE ID   : cr-dotnet-0043
// RULE NAME : Message Queue
// CATEGORY  : Platform
// DESCRIPTION: Application depends on Microsoft Message Queuing (MSMQ) through
//              System.Messaging for asynchronous communication. MSMQ service
//              doesn't exist in cloud platforms, causing message queue operations
//              to fail completely.
// =============================================================================

using System;
using System.Messaging;

namespace SyntheticLegacyApp.Platform
{
    public class OrderQueueProcessor
    {
        // VIOLATION cr-dotnet-0043: Hard-coded MSMQ queue path — MSMQ is Windows-only
        private const string OrderQueuePath  = @".\private$\orders";
        private const string RetryQueuePath  = @".\private$\orders_retry";
        private const string DeadLetterPath  = @".\private$\orders_deadletter";

        // VIOLATION cr-dotnet-0043: MessageQueue.Create — requires Windows MSMQ service
        public void EnsureQueueExists()
        {
            if (!MessageQueue.Exists(OrderQueuePath))
                MessageQueue.Create(OrderQueuePath, true); // transactional
        }

        // VIOLATION cr-dotnet-0043: Sending message via System.Messaging.MessageQueue
        public void EnqueueOrder(string orderJson)
        {
            using (var queue = new MessageQueue(OrderQueuePath))
            {
                queue.Formatter = new XmlMessageFormatter(new[] { typeof(string) });

                var msg = new Message
                {
                    Body       = orderJson,
                    Label      = "NewOrder_" + DateTime.UtcNow.Ticks,
                    Recoverable = true
                };

                // VIOLATION cr-dotnet-0043: Send inside MessageQueueTransaction — MSMQ-only
                var txn = new MessageQueueTransaction();
                txn.Begin();
                queue.Send(msg, txn);
                txn.Commit();
            }
        }

        // VIOLATION cr-dotnet-0043: Receive — blocking dequeue from MSMQ
        public string DequeueNextOrder()
        {
            using (var queue = new MessageQueue(OrderQueuePath))
            {
                queue.Formatter = new XmlMessageFormatter(new[] { typeof(string) });

                Message msg = queue.Receive(TimeSpan.FromSeconds(10));
                return msg?.Body?.ToString();
            }
        }

        // VIOLATION cr-dotnet-0043: Peek without consuming — uses MSMQ cursor internally
        public void LogQueueDepth()
        {
            using (var queue = new MessageQueue(OrderQueuePath))
            {
                var msgs   = queue.GetAllMessages();
                Console.WriteLine($"Orders pending in MSMQ: {msgs.Length}");
            }
        }

        // VIOLATION cr-dotnet-0043: Moving failed messages to dead-letter via MSMQ
        public void MoveToDeadLetter(string messageId)
        {
            using (var source = new MessageQueue(RetryQueuePath))
            using (var target = new MessageQueue(DeadLetterPath))
            {
                source.Formatter = new XmlMessageFormatter(new[] { typeof(string) });
                Message msg = source.ReceiveById(messageId);
                target.Send(msg, MessageQueueTransactionType.Single);
            }
        }
    }
}
