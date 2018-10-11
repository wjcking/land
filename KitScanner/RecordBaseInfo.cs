using System;
using System.Collections.Generic;
using System.Web;
using System.Reflection;

namespace KitScanner
{
    public class RecordBaseInfo
    {
        public RecordBaseInfo() { }

        public int Length { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public string VarCombinations { get; set; }
        public string Name
        {
            get
            {
                return DecodeScanner.Dictionary.ContainsKey(Type) ? DecodeScanner.Dictionary[Type] : "";
            }
        }
        private bool isLengthVariable = false;

        public bool IsLengthVariable
        {
            get { return isLengthVariable; }
            set { isLengthVariable = value; }
        }

        public virtual string Output()
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();

            Type type = GetType();

            return output.ToString();
        }

        public static DateTime ConvertToDateTime(string dt)
        {
            try
            {
                var dtList = new List<int>();
                for (int i = 0; i < dt.Length; i += 2)
                {
                    dtList.Add(Convert.ToInt32(dt.Substring(i, 2)));
                    }


                DateTime dateTime;
                if (dtList.Count < 3)
                    dateTime = new DateTime(dtList[2] + 2000, dtList[1], dtList[0]);
                else
                    dateTime = new DateTime(dtList[2] + 2000, dtList[1], dtList[0], dtList[3], dtList[4], dtList[5]);

                return dateTime;
            }
            catch
            {

            }
            return DateTime.Now.AddYears(-200);
        }
    }
}
