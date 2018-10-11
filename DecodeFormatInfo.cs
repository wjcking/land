using System;
using System.Collections.Generic;
using System.Text;

namespace KitScanner
{
    public class DecodeFormatInfo
    {
        public DecodeFormatInfo()
        { }

        public int Start { get; set; }
        public int Length { get; set; }
        public string Type { get; set; }
        public int PriceStart { get; set; }
        public int PriceLength { get; set; }
        public int WeightStart { get; set; }
        public int WeightLength { get; set; }
        public int QuanityStart { get; set; }
        public int QuanityLength { get; set; }
        public int OfferStart { get; set; }
        public int OfferLength { get; set; }
        public List<DecodeFormatInfo> ProductAttrList { get; set; }

        public string Name { get; set; }
    }
}
