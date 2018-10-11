using System;
using System.Collections.Generic;
using System.Text;

namespace KitScanner
{
    /// <summary>
    /// 1 x  [Type 21] “shop header” record 
    /// </summary>
    public class ShopInfo : RecordBaseInfo
    {
        public string PurchaserCode { get; set; }
        public string ShopCode { get; set; }
        public string BranchCode { get; set; }
        public int EShopCode
        {
            get
            {
                string shopCode = ShopCode +BranchCode;
                if (shopCode.Length > 7)
                  return  Convert.ToInt32(shopCode.Substring(0, 7));
                else
                    return Convert.ToInt32(shopCode);
            }
        }
        public string USIFlag {get;set;}
        public string TotalAmountSpent {get;set;}
        public string ShoppingTripStartDate { get; set; }
        public string ShoppingTripStartTime { get; set; }
        public string ShoppingTripEndTime { get; set; }
        public string LoyaltyCard { get; set; }
        public string MethodOfPayment { get; set; }
        public string LanguageCode { get; set; }
        public string TripTermination { get; set; }
        public List<NonCbkProductPurchaseInfo> NonCbkProductPurchaseList { get; set; }
        public List<CbkProductPurchaseInfo> CbkProductPurchaseList { get; set; }
    }
}
