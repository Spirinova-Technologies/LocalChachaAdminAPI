using LocalChachaAdminApi.Core.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Services
{
    public class HttpService : IHttpService
    {
        private HttpClient httpClient;

        public HttpService(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient("localChacha");
        }

        public async Task<T> GetHttpClientResponse<T>(string path, object request, HttpMethod httpMethod, bool includeToken = false, string token = "")
        {
            var httpRequestMessage = new HttpRequestMessage(httpMethod, path)
            {
                Content = new StringContent(JsonConvert.SerializeObject(request),
                                    Encoding.UTF8,
                                    "application/json")
            };

            if (includeToken)
            {
                httpRequestMessage.Headers.Authorization =
                      new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await httpClient.SendAsync(httpRequestMessage);

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}