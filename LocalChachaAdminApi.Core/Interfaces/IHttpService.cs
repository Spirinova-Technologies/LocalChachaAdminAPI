using System.Net.Http;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Interfaces
{
    public interface IHttpService
    {
        Task<T> GetHttpClientResponse<T>(string path, object request, HttpMethod httpMethod, bool includeToken = false, string token = "");
    }
}