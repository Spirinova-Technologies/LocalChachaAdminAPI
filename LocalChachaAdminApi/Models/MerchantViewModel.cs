namespace LocalChachaAdminApi.Models
{
    public class MerchantViewModel
    {
        public int QuickBloxId { get; set; }
        public string FullName { get; set; }
        public string ShopName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string InviteCode { get; set; }
        public string Type { get; set; }
        public string LoginType { get; set; }
        public string CategoryFileName { get; set; }
        public string ProductFileName { get; set; }
        public string MerchantSettingsFileName { get; set; }
        public int TotalRecords { get; set; }
        public int TotalMessages { get; set; }
        public string Categories { get; set; }
        public string Products { get; set; }
        public string Settings { get; set; }
    }
}