using System;
using System.Collections.Generic;

namespace LocalChachaAdminApi.Models
{
    public class SuggestedItemResultModel
    {
        public SuggestedItemResultModel()
        {
            SuggestedItemModels = new List<SuggestedItemModel>();
        }

        public int TotalRecords { get; set; }

        public List<SuggestedItemModel> SuggestedItemModels { get; set; }
    }


    public class SuggestedItemModel
    {
        public int Id { get; set; }

        public string CategoryId { get; set; }

        public int MerchantId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int MaxQuantity { get; set; }

        public string Image { get; set; }

        public float Price { get; set; }

        public int IsActive { get; set; }

        public int IsPublished { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }
    }
}