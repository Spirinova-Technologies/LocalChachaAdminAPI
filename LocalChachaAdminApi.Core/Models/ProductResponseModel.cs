using Newtonsoft.Json;

namespace LocalChachaAdminApi.Core.Models
{
    public class ProductResponseModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }
}