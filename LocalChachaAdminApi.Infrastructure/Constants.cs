using System.Collections.Generic;

namespace LocalChachaAdminApi.Infrastructure
{
    public static class Constants
    {
        public static string MerchantsS3Path = "merchants/Merchant.json";
        public static string LocationsS3Path = "merchants/Locations.csv";

        public static List<string> UserMobileNumbers = new List<string>
        {
          // "9175429576",
           "8605633693"
        };

        public static List<string> QuickBloxUsers = new List<string> {
          "localchacha-user-16",
          "localchacha-user-17",
          "localchacha-user-27",
          "localchacha-user-18",
          "localchacha-user-14"
        };
    }
}