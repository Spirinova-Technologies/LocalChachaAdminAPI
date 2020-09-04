using Newtonsoft.Json;

namespace LocalChachaAdminApi.Models
{
    public class MerchantResponseModel
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }

        [JsonProperty("merchant")]
        public Merchant Merchant { get; set; }

        [JsonProperty("basePathImage")]
        public string BasePathImage { get; set; }
    }

    public class Merchant: MerchantRequestModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("isActive")]
        public int IsActive { get; set; }

        [JsonProperty("socialId")]
        public string SocialId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("otp")]
        public string Otp { get; set; }

        [JsonProperty("isVerify")]
        public string IsVerify { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
