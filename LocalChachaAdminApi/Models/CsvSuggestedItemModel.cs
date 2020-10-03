namespace LocalChachaAdminApi.Models
{
    public class CsvSuggestedItemModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int MaxQuantity { get; set; }

        public string Image { get; set; }

        public float Price { get; set; }

        public int IsActive { get; set; }

        public int IsPublished { get; set; }
    }
}