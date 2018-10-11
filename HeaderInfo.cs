using System;
using System.Collections.Generic;
using System.Web;

namespace KitScanner
{
    /// <summary>
    /// 1 x [Type 01] “header” record 
    /// </summary>
    public class HeaderInfo : RecordBaseInfo
    {
        /// <summary>
        /// Date of file create
        /// </summary>
        public string CurrentDate { get; set; }

        /// <summary>
        /// Time of file create
        /// </summary>
        public string CurrentTime { get; set; }

        public DateTime ECurrentDateTime
        {
            get
            {
                try
                {
                    if ((CurrentDate + CurrentTime).Trim() != "")
                        return RecordBaseInfo.ConvertToDateTime(CurrentDate + CurrentTime);
                }
                catch
                {
                    return DateTime.Now.AddYears(-100);
                }
                return DateTime.Now.AddYears(-100);
            }
        }
        public string Minute
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentTime))
                return "";

                return CurrentTime.Substring(2, 2);
            }
        }
        /// <summary>
        /// last panel number userd - truncated to 6 digits
        /// </summary>
        public string FullPanelNumber { get; set; }

        /// <summary>
        /// 1=analog , 2=GPRS , and 3 = GSM
        /// </summary>
        public string ConnectionType { get; set; }

        /// <summary>
        /// e.g. '00130' is V1.30
        /// </summary>
        public string ProgramVersion { get; set; }

        public string Const { get; set; }

        /// <summary>
        /// 13-chars of terminal serial number
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// 1= auto-dial 2=manual/test, 3= auto transfer at end of entry
        /// </summary>
        public string CallType { get; set; }

        /// <summary>
        /// 0= Sunday 6= Saturday
        /// </summary>
        public string DayOfWeek { get; set; }

        /// <summary>
        /// 3-digit "battery level " with leading zeroes
        /// </summary>
        public string BatteryLevel { get; set; }

        /// <summary>
        /// user for kit type and OS
        /// </summary>
        public string KitOS { get; set; }

        public string NumberOfDial { get; set; }

        /// <summary>
        /// A=Analog G=GPRS
        /// </summary>
        public string ModemType {get;set;}
        public string PanelNumber { get; set; }
    }


    public class CradleInfo : RecordBaseInfo
    {
        public string CradleSerialNumber { get; set; }
        public string SimNumber { get; set; }
    }

    public class ShopHeaderExten : RecordBaseInfo
    {
        public string Fascia;
        public string Location;
    }
}
