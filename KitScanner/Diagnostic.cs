using System;
using System.Collections.Generic;
using System.Text;

namespace KitScanner
{
    public class DiagVersionChange : RecordBaseInfo
    {
        public string DateTime { get; set; }
        public string OldVersionNumber { get; set; }
        public string NewVersionNumber { get; set; }
        public DateTime EDateTime
        {
            get
            {

                return RecordBaseInfo.ConvertToDateTime(DateTime);

            }
        }
        public string FileType { get; set; }
    }

    public class DiagTime : RecordBaseInfo
    {

        public string DTBeforeCorrection { get; set; }
        public string DTAfterCorrection { get; set; }

        public DateTime EDTBeforeCorrection { get { return RecordBaseInfo.ConvertToDateTime(DTBeforeCorrection); } }
        public DateTime EDTAfterCorrection { get { return RecordBaseInfo.ConvertToDateTime(DTAfterCorrection); } }
    }


    public class DiagCall : RecordBaseInfo
    {
        public string DateTime { get; set; }

        public DateTime EDateTime
        {
            get
            {

            return     RecordBaseInfo.ConvertToDateTime(DateTime);
                   
            }
        }
        public string Status { get; set; }
        public string CommunicationState { get; set; }
        public string CommunicationCode { get; set; }
        public string TelephoneNumber { get; set; }

    }
}
