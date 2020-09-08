using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocalChachaAdminApi.Core.Models
{
    public class MerchantSettingsRequestModel
    {
        [JsonProperty("pickupFromShop")]
        public int PickupFromShop { get; set; }
        [JsonProperty("showProductCatalogue")]
        public int PhowProductCatalogue { get; set; }
        [JsonProperty("showProductRate")]
        public int ShowProductRate { get; set; }
        [JsonProperty("onlyAllowAvailableProduct")]
        public int OnlyAllowAvailableProduct { get; set; }
        [JsonProperty("autoPopulateCatalogue")]
        public int AutoPopulateCatalogue { get; set; }
        [JsonProperty("cashOnDelivery")]
        public int CashOnDelivery { get; set; }
        [JsonProperty("takeShopOffline")]
        public int TakeShopOffline { get; set; }
        [JsonProperty("goOffline")]
        public int GoOffline { get; set; }
        [JsonProperty("getWhatsupAlert")]
        public int GetWhatsupAlert { get; set; }
        [JsonProperty("enableHomeDelivery")]
        public int EnableHomeDelivery { get; set; }
        [JsonProperty("noHomeDelivery")]
        public int NoHomeDelivery { get; set; }
        [JsonProperty("standardDeliveryCharges")]
        public int StandardDeliveryCharges { get; set; }
        [JsonProperty("enableUrgentDelivery")]
        public int EnableUrgentDelivery { get; set; }
        [JsonProperty("urgentDeliveryCharges")]
        public int UrgentDeliveryCharges { get; set; }
        [JsonProperty("minimumOrder")]
        public int MinimumOrder { get; set; }
        [JsonProperty("enableFreeDelivery")]
        public int EnableFreeDelivery { get; set; }
        [JsonProperty("sendOrderDetails")]
        public int SendOrderDetails { get; set; }
    }
}
