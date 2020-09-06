using Newtonsoft.Json;

namespace LocalChachaAdminApi.Core.Models
{
    public class MerchantRequestModel
    {
        [JsonProperty("quickbloxId")]
        public int QuickBloxId { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("shop_name")]
        public string ShopName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("loginType")]
        public string LoginType { get; set; }
     
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("lat")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("inviteCode")]
        public string InviteCode { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("categoryFileName")]
        public string CategoryFileName { get; set; }

        [JsonProperty("productFileName")]
        public string ProductFileName { get; set; }
    }
    
}
