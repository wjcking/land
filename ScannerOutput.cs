using System;
using System.Collections.Generic;

using System.Text;
using System.Reflection;

namespace KitScanner
{
    public static partial class DecodeScanner
    {
        // 1 x [Type 01] “header” record 
        //1 x [Type 56] “versions” record 
        //  n x “diagnostic” and “shopping trip” records in increasing date/time: 
        //    [Type 57] “version change” diagnostic records 
        //    [Type 58] “time” diagnostic records 
        //    [Type 59] “call” diagnostic records 
        //    [Type 55] “panel number” diagnostic records 
        //    “Shopping Trip” records 
        //    1 x  [Type 21] “shop header” record 
        //      1 x [Type 26] “shop header extension” record 
        //      0,1,2 “custom text” records (according to code book configuration) 
        //        [Type 25] “custom text” records (“custom shop data”) 
        //      n x “purchase” records consisting of any combination of: 
        //        [Type 22] “non-codebook purchase” records 
        //        [Type 43] “coded random weight purchase” records 
        //        [Type 24] “non-codebook random weight” records 
        //        [Type 23] “codebook purchase” records 
        //      0,1,2 “custom text” records (according to code book configuration) 
        //        [Type 25] “custom text” records (“custom product data”) 
        //    “Questionnaire” records: 
        //      n x [Type 29] “questionnaire data” record 
        //1 x [Type 99] “end of transfer file” record 
        public const string Type01 = "01";
        public const string Type56 = "56";
        public const string Type21 = "21";
        public const string Type22 = "22";
        public const string Type23 = "23";
        //“custom text” records (“custom product data”) 
        public const string Type25 = "25";
        public const string Type26 = "26";
        //“questionnaire data” record 
        public const string Type29 = "29";
        public const string Type43 = "43";
        public const string Type57 = "57";
        public const string Type58 = "58";
        public const string Type59 = "59";
        public const string Type99 = "99";
        public const string Type02 = "02";


        public static Dictionary<string, string> Dictionary
        {
            get
            {

                Dictionary<string, string> list = new Dictionary<string, string>();
                list.Add(Type01, "header  record ");
                list.Add(Type56, "versions record ");
                list.Add(Type21, "shop header record  ");
                list.Add(Type22, "non-codebook purchase records ");
                list.Add(Type23, "codebook purchase” records ");
                list.Add(Type29, "questionnaire data” record  ");
                list.Add(Type43, "coded random wight purchase record ");
                list.Add(Type59, "“call” diagnostic records  ");
                list.Add(Type99, "end of transfer file” record  ");
                list.Add(Type02, "Cradle record ");

                return list;
            }
        }


        public static Dictionary<string, object> GetDataFileRecord(string scannerData)
        {

            if (scannerData == "")
                return null;

            if (String.IsNullOrEmpty(scannerData))
                return null;

            if (scannerData.Length <= 84)
                return null;


            //Type01
            Dictionary<string, object> dataList = new Dictionary<string, object>();
            HeaderInfo header = DecodeType01(scannerData);
            dataList.Add(Type01, header);

            VersionInfo version = DecodeType56(scannerData);
            dataList.Add(Type56, version);
            //Type29
            List<QuestionnaireInfo> questionnaireList = DecodeType29(scannerData);
            dataList.Add(Type29, questionnaireList);

            //Type22
            List<NonCbkProductPurchaseInfo> nonCbkList = DecodeType22(scannerData);
            dataList.Add(Type22, nonCbkList);

            //Type23  
           
            List<CbkProductPurchaseInfo> cbkList = DecodeType23(scannerData);
            dataList.Add(Type23, cbkList);

            //Type21
            // ShopInfo shopInfo = DecodeType21(scannerData);
            List<ShopInfo> shopList = DecodeType21(scannerData);
            dataList.Add(Type21, shopList);
            //Type43
            WeightPurchaseInfo wpi = DecodeType43(scannerData);
            dataList.Add(Type43, wpi);
            //Total 
            //List<RecordBaseInfo> rbList = new List<RecordBaseInfo>();

            //if (questionnaireList.Count > 1)
            //    rbList.

            return dataList;
        }


        public static string Debug(string dataString, string typeID, bool isWindow)
        {
            string returnMark = isWindow ? "\r\n" : "<br />";

            string highlightedTitle = isWindow ? "[{0}]" : "<b>{0}</b>";


            StringBuilder output = new StringBuilder();
            Dictionary<string, object> dataList = DecodeScanner.GetDataFileRecord(dataString);

            if (dataList == null)
                return "";
            
            List<string> typeList = DecodeTypeName(dataString);

            foreach (string type in typeList)
            {
                if (Dictionary.ContainsKey(type))
                    output.Append(Dictionary[type]).Append(returnMark);
                else
                    output.Append(type).Append(returnMark);
            }
            //Type:
            output.Append(returnMark);
            output.Append("-".PadRight(100, '-'));
            output.Append(returnMark);
            output.Append(dataString);


            output.Append(GetSpliter(1, Type01));
            HeaderInfo hri = dataList[DecodeScanner.Type01] as HeaderInfo;
            foreach (PropertyInfo pi in typeof(HeaderInfo).GetProperties())
            {
                output.Append(String.Format(highlightedTitle, pi.Name));
                output.Append(":");
                output.Append(pi.GetValue(hri, null));
                output.Append(returnMark);
            }
            //Type43: 

            output.Append(GetSpliter(1, Type43));

            WeightPurchaseInfo wpi = dataList[DecodeScanner.Type43] as WeightPurchaseInfo;
            foreach (PropertyInfo pi in typeof(WeightPurchaseInfo).GetProperties())
            {
                output.Append(String.Format(highlightedTitle, pi.Name));
                output.Append(":");
                try
                {
                    output.Append(pi.GetValue(wpi, null));
                }
                catch (Exception e)
                {
                    output.Append(e.Message);
                }
                output.Append(returnMark);
            }
            //Type21: 
            // ShopInfo shopInfo = dataList[ScannerOutput.Type21] as ShopInfo;
            List<ShopInfo> shopList = dataList[DecodeScanner.Type21] as List<ShopInfo>;

            output.Append(GetSpliter(shopList.Count, Type21));
            foreach (ShopInfo shopInfo in shopList)
            {
                foreach (PropertyInfo pi in typeof(ShopInfo).GetProperties())
                {
                    if (pi.Name == "NonCbkProductPurchaseList")
                        continue;
                    if (pi.Name == "CbkProductPurchaseList")
                        continue;
                    if (pi.Name == "IsLengthVariable")
                        continue;
                    if (pi.Name == "Name")
                        continue;
                    if (pi.Name == "Content")
                        continue;

                    output.Append(String.Format(highlightedTitle, pi.Name));
                    output.Append(":");
                    output.Append(pi.GetValue(shopInfo, null));
                    output.Append(returnMark);

                }


                if (shopInfo.NonCbkProductPurchaseList != null)
                for (int i = 0; i < shopInfo.NonCbkProductPurchaseList.Count; i++)
                {
                    if (shopInfo.NonCbkProductPurchaseList[i] == null)
                        continue;
                    foreach (PropertyInfo pi in typeof(NonCbkProductPurchaseInfo).GetProperties())
                    {
                        output.Append(String.Format(highlightedTitle, pi.Name));
                        output.Append(":");
                        object o = pi.GetValue(shopInfo.NonCbkProductPurchaseList[i], null);
                        if (o!=null)
                        output.Append(o);
                        output.Append(" ");
                    }
                    output.Append(returnMark);
                }

                if ( shopInfo.CbkProductPurchaseList != null)
                for (int i = 0; i < shopInfo.CbkProductPurchaseList.Count; i++)
                {
                    if (shopInfo.CbkProductPurchaseList[i] == null)
                        continue;
                    foreach (PropertyInfo pi in typeof(CbkProductPurchaseInfo).GetProperties())
                    {
                        output.Append(String.Format(highlightedTitle, pi.Name));
                        output.Append(":");
                        object o = pi.GetValue(shopInfo.CbkProductPurchaseList[i], null);
                        if (o!=null)
                        output.Append(o);
                        output.Append(" ");
                    }
                    output.Append(returnMark);
                }
                output.Append(returnMark);
            }

            //Type56:
            output.Append(GetSpliter( 1, Type56));
          

            VersionInfo version = dataList[DecodeScanner.Type56] as VersionInfo;
            foreach (PropertyInfo pi in typeof(VersionInfo).GetProperties())
            {
                output.Append(String.Format(highlightedTitle, pi.Name));
                output.Append(":");
                output.Append(pi.GetValue(version, null));
                output.Append(returnMark);
            }


            //Type22:
            List<NonCbkProductPurchaseInfo> nonCbkProductList = dataList[DecodeScanner.Type22] as List<NonCbkProductPurchaseInfo>;

            output.Append(GetSpliter(nonCbkProductList.Count, Type22));
          
            for (int i = 0; i < nonCbkProductList.Count; i++)
            {
                foreach (PropertyInfo pi in typeof(NonCbkProductPurchaseInfo).GetProperties())
                {
                    output.Append(String.Format(highlightedTitle, pi.Name));
                    output.Append(":");
                    output.Append(pi.GetValue(nonCbkProductList[i], null));
                    output.Append(" ");
                }
                output.Append(returnMark);
            }
            //    
            //Type23:
            List<CbkProductPurchaseInfo> cbkProductList = dataList[DecodeScanner.Type23] as List<CbkProductPurchaseInfo>;

            output.Append(GetSpliter(cbkProductList.Count, Type23));
            if (!dataList.ContainsKey(DecodeScanner.Type23))
                return "";


            for (int i = 0; i < cbkProductList.Count; i++)
            {
                foreach (PropertyInfo pi in typeof(CbkProductPurchaseInfo).GetProperties())
                {
                    output.Append(String.Format(highlightedTitle, pi.Name));
                    output.Append(":");
                    output.Append(pi.GetValue(cbkProductList[i], null));
                    output.Append(" ");
                }
                output.Append(returnMark);
            }


            // Type29:

            List<QuestionnaireInfo> questionnaireList = dataList[DecodeScanner.Type29] as List<QuestionnaireInfo>;

            output.Append(GetSpliter(questionnaireList.Count, Type29));

            if (!dataList.ContainsKey(DecodeScanner.Type29))
                return "";


            for (int i = 0; i < questionnaireList.Count; i++)
            {
                foreach (PropertyInfo pi in typeof(QuestionnaireInfo).GetProperties())
                {
                    output.Append(String.Format(highlightedTitle, pi.Name));
                    output.Append(":");
                    output.Append(pi.GetValue(questionnaireList[i], null));
                    output.Append(" ");
                }
                output.Append(returnMark);
            }

            // Type29 questionlist
            output.Append(GetSpliter(questionnaireList.Count, Type29));

            for (int i = 0; i < questionnaireList.Count; i++)
            {
                if (questionnaireList[i].QuestionList != null)
                {
                    foreach (QuestionInfo q in questionnaireList[i].QuestionList)
                    {
                        output.Append(String.Format(highlightedTitle, q.Question));
                        output.Append(":");
                        output.Append(q.Answer);
                        output.Append(" ");
                    }
                }
                output.Append(returnMark);
            }


            return output.ToString();
        }

        private static string GetSpliter(int count, string type)
        {
            StringBuilder output = new StringBuilder();
            output.Append("\r\n");
            output.Append(count);
            output.Append(" x ");
            output.Append(Dictionary[type]);
            output.Append("\r\n");
            output.Append("-".PadRight(100, '-'));
            output.Append("\r\n");

            return output.ToString();
        }
        public static string Debug(string dataString, string typeID)
        {
            return Debug(dataString, typeID, false);
        }
    }
}
