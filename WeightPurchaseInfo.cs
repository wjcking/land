using System;
using System.Collections.Generic;
using System.Text;

namespace KitScanner
{
    /// <summary>
    ///  [Type 43] “coded random weight purchase” records 
    /// </summary>
     public  class WeightPurchaseInfo : RecordBaseInfo
    {
         public string OriginalBarcode { get; set; }
         /// <summary>
         /// 0=scanned 1= keyed
         /// </summary>
         public string EntryMethod { get; set; }

         public string CodeBookProduct { get; set; }

         /// <summary>
         /// variable
         /// </summary>
         public string LeftFromCodeBook { get; set; }
    }
}
