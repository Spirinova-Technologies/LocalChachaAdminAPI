using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LocalChachaAdminApi.Core.Models
{
    public class QuickbloxUser
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("website")]
        public object Website { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("last_request_at")]
        public DateTime LastRequestAt { get; set; }

        [JsonProperty("external_user_id")]
        public object ExternalUserId { get; set; }

        [JsonProperty("facebook_id")]
        public string FacebookId { get; set; }

        [JsonProperty("twitter_id")]
        public object TwitterId { get; set; }

        [JsonProperty("blob_id")]
        public object BlobId { get; set; }

        [JsonProperty("custom_data")]
        public string CustomData { get; set; }
    }
}