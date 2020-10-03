using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LocalChachaAdminApi.Core.Models.DTO
{
    public class SuggestedItemResult
    {
        public SuggestedItemResult()
        {
            SuggestedItems = new List<SuggestedItem>();
        }

        public int TotalRecords { get; set; }
        public List<SuggestedItem> SuggestedItems { get; set; }
    }

    public class SuggestedItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("categoryId")]
        public string CategoryId { get; set; }

        [JsonProperty("merchantId")]
        public int MerchantId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("maxQuantity")]
        public int MaxQuantity { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("price")]
        public float Price { get; set; }

        [JsonProperty("isActive")]
        public int IsActive { get; set; }

        [JsonProperty("isPublished")]
        public int IsPublished { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("modified")]
        public DateTime Modified { get; set; }
    }
}