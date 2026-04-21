// =============================================================================
// RULE ID   : cr-dotnet-0037
// RULE NAME : Synchronous HttpClient
// CATEGORY  : Memory
// DESCRIPTION: Application uses HttpClient synchronously with .Result, .Wait(), or
//              GetAwaiter().GetResult() for HTTP operations. Blocking HTTP calls
//              reduce application throughput in high-concurrency cloud environments.
// =============================================================================

using System;
using System.Net.Http;
using System.Text;

namespace SyntheticLegacyApp.Memory
{
    public class ExternalApiClient
    {
        private readonly HttpClient _httpClient = new HttpClient();

        // VIOLATION cr-dotnet-0037: .Result blocks the calling thread
        public string GetUserProfile(int userId)
        {
            var response = _httpClient
                .GetAsync($"https://api.internal.corp/users/{userId}")
                .Result;

            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }

        // VIOLATION cr-dotnet-0037: .Wait() on PostAsync — thread is blocked
        public bool SubmitOrder(string orderJson)
        {
            var content  = new StringContent(orderJson, Encoding.UTF8, "application/json");
            var postTask = _httpClient.PostAsync("https://api.internal.corp/orders", content);
            postTask.Wait();

            return postTask.Result.IsSuccessStatusCode;
        }

        // VIOLATION cr-dotnet-0037: GetAwaiter().GetResult() is semantically identical to .Result
        public byte[] DownloadReport(string reportId)
        {
            return _httpClient
                .GetByteArrayAsync($"https://reports.internal.corp/download/{reportId}")
                .GetAwaiter()
                .GetResult();
        }

        // VIOLATION cr-dotnet-0037: Nested .Result calls across multiple awaitable operations
        public string GetAuthToken(string clientId, string secret)
        {
            var formData = new StringContent(
                $"client_id={clientId}&client_secret={secret}&grant_type=client_credentials",
                Encoding.UTF8, "application/x-www-form-urlencoded");

            var tokenResponse = _httpClient
                .PostAsync("https://auth.internal.corp/token", formData)
                .Result;

            return tokenResponse.Content.ReadAsStringAsync().Result;
        }

        // VIOLATION cr-dotnet-0037: Synchronous loop over multiple API calls — thread pool starvation risk
        public void SyncBulkFetch(int[] ids)
        {
            foreach (int id in ids)
            {
                string result = _httpClient
                    .GetStringAsync($"https://api.internal.corp/items/{id}")
                    .Result;

                Console.WriteLine($"Fetched item {id}: {result.Length} bytes");
            }
        }
    }
}
