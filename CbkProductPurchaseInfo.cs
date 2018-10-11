using System;
using System.Collections.Generic;
using System.Web;

namespace KitScanner
{
    public class CbkProductPurchaseInfo   : RecordBaseInfo
    {
        /// <summary>
        /// [Type 23] “codebook purchase” records  scanned from codebook
        /// </summary>
        public string ProductCode { get; set; }
        public string ProductAttr { get; set; }
        public string Price { get; set; }
        public float  EPrice
        {
            get
            {
                if (Price == null)
                    return 0;
                string price = Price.TrimStart('0');
                if (price.Length > 2)
                {
               return     Convert.ToSingle(price.Insert(price.Length - 2, "."));
                }

                return Convert.ToSingle(price);
            }
        }
        public string Weight { get; set; }
        public string Quantity { get; set; }
        public string Offer { get; set; }
        public string ProductAttribute { get; set; }

        public string Unit { get; set; }
    }
}
