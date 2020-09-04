using Newtonsoft.Json;

namespace LocalChachaAdminApi.Models
{
    public class ProductRequestModel
    {
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

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("isActive")]
        public int IsActive { get; set; }

        [JsonProperty("isPublished")]
        public int IsPublished { get; set; }
    }

    public class Product : ProductRequestModel
    {
        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }
    }

}