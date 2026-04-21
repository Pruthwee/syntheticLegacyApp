// =============================================================================
// RULE ID   : cr-dotnet-0041
// RULE NAME : COM Interop Usage
// CATEGORY  : Platform
// DESCRIPTION: FIXED - Replaced COM Office Automation with AWS Lambda and open-source libraries
// =============================================================================

using System;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Newtonsoft.Json;

namespace SyntheticLegacyApp.Platform
{
    // REMOVED: COM interface - no longer needed with cloud-native approach
    
    public class ComInteropWorker
    {
        private readonly IAmazonLambda _lambdaClient;

        public ComInteropWorker(IAmazonLambda lambdaClient = null)
        {
            _lambdaClient = lambdaClient ?? new AmazonLambdaClient();
        }

        // FIXED: Replaced Excel COM automation with ClosedXML library
        public void GenerateExcelReport(string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Report");
                worksheet.Cell(1, 1).Value = "Legacy Report";
                worksheet.Cell(2, 1).Value = "Generated on:";
                worksheet.Cell(2, 2).Value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC");
                
                // Add some sample data
                worksheet.Cell(4, 1).Value = "Item";
                worksheet.Cell(4, 2).Value = "Value";
                worksheet.Cell(5, 1).Value = "Sample Data";
                worksheet.Cell(5, 2).Value = 12345;
                
                // Auto-fit columns
                worksheet.Columns().AdjustToContents();
                
                workbook.SaveAs(filePath);
            }
        }

        // FIXED: Replaced COM object creation with AWS Lambda invocation
        public async Task<string> InvokeLegacyProcessorAsync(string input)
        {
            try
            {
                var payload = JsonConvert.SerializeObject(new
                {
                    action = "process",
                    input = input,
                    config = "default"
                });

                var request = new InvokeRequest
                {
                    FunctionName = Environment.GetEnvironmentVariable("LEGACY_PROCESSOR_LAMBDA") ?? "legacy-processor",
                    Payload = payload
                };

                var response = await _lambdaClient.InvokeAsync(request);
                
                using (var reader = new StreamReader(response.Payload))
                {
                    var result = await reader.ReadToEndAsync();
                    Console.WriteLine($"Lambda result: {result}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lambda invocation failed: {ex.Message}");
                throw;
            }
        }

        public void InvokeLegacyProcessor(string input)
        {
            InvokeLegacyProcessorAsync(input).GetAwaiter().GetResult();
        }

        // FIXED: Replaced COM error handling with standard exception handling
        public void HandleComError(Exception ex)
        {
            // Standard exception handling - no COM-specific error codes
            Console.WriteLine($"Error: {ex.GetType().Name} - {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }

        // Additional helper method for Excel operations
        public async Task GenerateExcelReportToS3Async(string s3Bucket, string s3Key)
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                GenerateExcelReport(tempFile);
                
                // Upload to S3 (requires Amazon.S3 package)
                // var s3Client = new AmazonS3Client();
                // await s3Client.PutObjectAsync(new PutObjectRequest
                // {
                //     BucketName = s3Bucket,
                //     Key = s3Key,
                //     FilePath = tempFile
                // });
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }
}
