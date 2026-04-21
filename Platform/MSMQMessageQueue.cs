// =============================================================================
// RULE ID   : cr-dotnet-0043
// RULE NAME : Message Queue
// CATEGORY  : Platform
// DESCRIPTION: FIXED - Replaced MSMQ with Amazon SQS
// =============================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;

namespace SyntheticLegacyApp.Platform
{
    public class OrderQueueProcessor
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly string _orderQueueUrl;
        private readonly string _retryQueueUrl;
        private readonly string _deadLetterQueueUrl;

        public OrderQueueProcessor(IAmazonSQS sqsClient = null)
        {
            _sqsClient = sqsClient ?? new AmazonSQSClient();
            
            // Queue URLs from environment variables or configuration
            _orderQueueUrl = Environment.GetEnvironmentVariable("ORDER_QUEUE_URL") ?? 
                           throw new InvalidOperationException("ORDER_QUEUE_URL not configured");
            _retryQueueUrl = Environment.GetEnvironmentVariable("RETRY_QUEUE_URL") ?? _orderQueueUrl;
            _deadLetterQueueUrl = Environment.GetEnvironmentVariable("DEADLETTER_QUEUE_URL") ?? _orderQueueUrl;
        }

        // FIXED: SQS queues are created via infrastructure (CloudFormation/Terraform)
        // No runtime queue creation needed
        public async Task EnsureQueueExistsAsync()
        {
            try
            {
                // Verify queue exists by getting attributes
                await _sqsClient.GetQueueAttributesAsync(new GetQueueAttributesRequest
                {
                    QueueUrl = _orderQueueUrl,
                    AttributeNames = new List<string> { "QueueArn" }
                });
            }
            catch (QueueDoesNotExistException)
            {
                throw new InvalidOperationException(
                    "Queue does not exist. Create queues via infrastructure-as-code (CloudFormation/Terraform)");
            }
        }

        public void EnsureQueueExists()
        {
            EnsureQueueExistsAsync().GetAwaiter().GetResult();
        }

        // FIXED: Send message to SQS instead of MSMQ
        public async Task EnqueueOrderAsync(string orderJson)
        {
            var request = new SendMessageRequest
            {
                QueueUrl = _orderQueueUrl,
                MessageBody = orderJson,
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    ["Timestamp"] = new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = DateTime.UtcNow.ToString("o")
                    },
                    ["MessageType"] = new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = "NewOrder"
                    }
                }
            };

            await _sqsClient.SendMessageAsync(request);
        }

        public void EnqueueOrder(string orderJson)
        {
            EnqueueOrderAsync(orderJson).GetAwaiter().GetResult();
        }

        // FIXED: Receive message from SQS with visibility timeout
        public async Task<string> DequeueNextOrderAsync()
        {
            var request = new ReceiveMessageRequest
            {
                QueueUrl = _orderQueueUrl,
                MaxNumberOfMessages = 1,
                WaitTimeSeconds = 10, // Long polling
                VisibilityTimeout = 30, // Message hidden for 30 seconds while processing
                MessageAttributeNames = new List<string> { "All" }
            };

            var response = await _sqsClient.ReceiveMessageAsync(request);
            
            if (response.Messages.Count > 0)
            {
                var message = response.Messages[0];
                
                // Delete message after successful retrieval (equivalent to MSMQ transactional receive)
                await _sqsClient.DeleteMessageAsync(new DeleteMessageRequest
                {
                    QueueUrl = _orderQueueUrl,
                    ReceiptHandle = message.ReceiptHandle
                });
                
                return message.Body;
            }
            
            return null;
        }

        public string DequeueNextOrder()
        {
            return DequeueNextOrderAsync().GetAwaiter().GetResult();
        }

        // FIXED: Get queue depth from SQS attributes
        public async Task LogQueueDepthAsync()
        {
            var request = new GetQueueAttributesRequest
            {
                QueueUrl = _orderQueueUrl,
                AttributeNames = new List<string> 
                { 
                    "ApproximateNumberOfMessages",
                    "ApproximateNumberOfMessagesNotVisible"
                }
            };

            var response = await _sqsClient.GetQueueAttributesAsync(request);
            
            var visible = int.Parse(response.Attributes["ApproximateNumberOfMessages"]);
            var inFlight = int.Parse(response.Attributes["ApproximateNumberOfMessagesNotVisible"]);
            
            Console.WriteLine($"Orders pending in SQS: {visible} (visible), {inFlight} (in-flight)");
        }

        public void LogQueueDepth()
        {
            LogQueueDepthAsync().GetAwaiter().GetResult();
        }

        // FIXED: Move message to dead-letter queue in SQS
        public async Task MoveToDeadLetterAsync(string receiptHandle)
        {
            // First, receive the message from retry queue
            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = _retryQueueUrl,
                MaxNumberOfMessages = 1,
                VisibilityTimeout = 30
            };

            var receiveResponse = await _sqsClient.ReceiveMessageAsync(receiveRequest);
            
            if (receiveResponse.Messages.Count > 0)
            {
                var message = receiveResponse.Messages[0];
                
                // Send to dead-letter queue
                await _sqsClient.SendMessageAsync(new SendMessageRequest
                {
                    QueueUrl = _deadLetterQueueUrl,
                    MessageBody = message.Body,
                    MessageAttributes = message.MessageAttributes
                });
                
                // Delete from retry queue
                await _sqsClient.DeleteMessageAsync(new DeleteMessageRequest
                {
                    QueueUrl = _retryQueueUrl,
                    ReceiptHandle = message.ReceiptHandle
                });
            }
        }

        public void MoveToDeadLetter(string messageId)
        {
            MoveToDeadLetterAsync(messageId).GetAwaiter().GetResult();
        }
    }
}
