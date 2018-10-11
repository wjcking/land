using System;
using System.Collections.Generic;
using System.Text;

namespace KitScanner
{
    /// <summary>
    /// 1 x [Type 56] “versions” record 
    /// </summary>
    public class VersionInfo : RecordBaseInfo
    {
        public string PanelScan { get; set; }
        public string CodeBook { get; set; }
        public string ShopCode { get; set; }
        public string Connection { get; set; }
        public string UserEAN { get; set; }
        public string AddtionalEAN { get; set; }
        public string EANQuestions { get; set; }

        public string FontFile { get; set; }
    }
}
