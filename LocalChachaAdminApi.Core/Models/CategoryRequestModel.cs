using Newtonsoft.Json;

namespace LocalChachaAdminApi.Core.Models
{
    public class CategoryRequestModel
    {
        [JsonProperty("merchantId")]
        public int MerchantId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
