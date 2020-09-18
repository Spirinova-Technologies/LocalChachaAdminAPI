using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace LocalChachaAdminApi.Core.Models
{
    [JsonObject]
    public class QuickBloxUserRequest
    {
        [JsonProperty(PropertyName = "full_name")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "old_password")]
        public string OldPassword { get; set; }

        [JsonProperty(PropertyName = "external_user_id")]
        public long? ExternalUserId { get; set; }

        [JsonProperty(PropertyName = "facebook_id")]
        public string FacebookId { get; set; }

        [JsonProperty(PropertyName = "twitter_id")]
        public string TwitterId { get; set; }

        [JsonProperty(PropertyName = "external_id")]
        public long ExternalId { get; set; }

        [JsonProperty(PropertyName = "website")]
        public string Website { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "tag_list")]
        public string TagList { get; set; }
    }
}