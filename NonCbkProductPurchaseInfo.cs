using System;
using System.Collections.Generic;
using System.Web;

namespace KitScanner
{
    public class NonCbkProductPurchaseInfo : RecordBaseInfo
    {
        /// <summary>
        ///  [Type 22] “non-codebook purchase” records  scanned or keyed product code see databar update
        /// </summary>
        public string ProductCode { get; set; }
        public string EntryMethod { get; set; }
        public string SpecialOfferCode { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
  

        public float EPrice
        {
            get
            {
                if (Price == null)
                    return 0;
                string price = Price.TrimStart('0');

                if (string.IsNullOrEmpty(price))
                    return 0;
                if (price.Length > 2)
                {
               return      Convert.ToSingle(price.Insert(price.Length - 2, "."));
                }

                return Convert.ToSingle(price);
            }
        }

        public string ProductAttr { get; set; }
    }
}
