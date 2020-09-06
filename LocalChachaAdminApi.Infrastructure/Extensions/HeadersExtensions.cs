using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace LocalChachaAdminApi.Infrastructure.Extensions
{
    public static class HeadersExtensions
    {
        public static string GetHeaderValue(this IHeaderDictionary headers, string key)
        {
            if (headers == null || string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            StringValues stringValue;
            if (headers.TryGetValue(key, out stringValue))
            {
                return stringValue.ToString();
            }

            return string.Empty;
        }
    }
}