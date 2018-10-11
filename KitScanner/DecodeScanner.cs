using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Server;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using System.Xml;

namespace KitScanner
{
    public static partial class DecodeScanner
    {
        private static readonly char[] Split_Marks_Return = "\r\n".ToCharArray();
        private const char Split_Marks_Space = ' ';

        private static string[] GetSplitedArray(string scannerData)
        {
            if (scannerData.IndexOf("\r\n") > -1)
                return scannerData.Split(Split_Marks_Return, StringSplitOptions.RemoveEmptyEntries);
            else
                return scannerData.Split(Split_Marks_Space);
        }
        private static List<string> GetSplitedArray(string scannerData, string typeNumber)
        {
            string[] splitedData = GetSplitedArray(scannerData);

            List<string> list = new List<string>();
            foreach (string q in splitedData)
            {
                if (q.Length < 5)
                    continue;

                string recordType = q.Substring(2, 2);

                if (typeNumber == recordType)
                    list.Add(q);
            }

            return list;
        }
        public static List<string> DecodeTypeName(string scannerData)
        {
            string[] array = GetSplitedArray(scannerData); ;
            List<string> typeList = new List<string>();
            foreach (string q in array)
            {
                string recordType = q.Substring(2, 2);
                if (!typeList.Contains(recordType))
                    typeList.Add(recordType);
            }

            return typeList;
        }
        public static HeaderInfo DecodeType01(string scannerData)
        {
            //Type01
            HeaderInfo hi = new HeaderInfo();
            hi.Length = 85;

            hi.Type = Type01;
            hi.Content = scannerData.Substring(0, hi.Length);
            hi.CurrentDate = hi.Content.Substring(4, 6);
            hi.CurrentTime = hi.Content.Substring(10, 6);
            hi.PanelNumber = hi.Content.Substring(16, 6);
            hi.ConnectionType = hi.Content.Substring(23, 1);
            hi.FullPanelNumber = hi.Content.Substring(24, 12);
            // string filler = hi.Content.Substring(36, 4);
            hi.SerialNumber = hi.Content.Substring(53, 13);
            hi.ProgramVersion = hi.Content.Substring(40, 5);
            hi.Const = hi.Content.Substring(45, 8);
            hi.CallType = hi.Content.Substring(66, 1);
            hi.DayOfWeek = hi.Content.Substring(67, 9);
            hi.BatteryLevel = hi.Content.Substring(68, 5);
            hi.KitOS = hi.Content.Substring(74, 4);
            hi.NumberOfDial = hi.Content.Substring(79, 3);
            hi.ModemType = hi.Content.Substring(84, 1);

            return hi;
        }
        public static List<QuestionnaireInfo> DecodeType29(string scannerData)
        {
            List<QuestionnaireInfo> questionnaireList = new List<QuestionnaireInfo>();
            string[] questionnaireArray = GetSplitedArray(scannerData); ;

            foreach (string q in questionnaireArray)
            {
                if (q.Length < 5)
                    continue;

                string recordType = q.Substring(2, 2);

                if (recordType != Type29)
                    continue;

                QuestionnaireInfo qi = new QuestionnaireInfo();
                qi.Content = q;
                qi.Type = Type29;
                qi.Length = Convert.ToInt32(q.Substring(0, 2));

                qi.IsLengthVariable = true;
                qi.Number = q.Substring(4, 5);
                qi.UserCode = q.Substring(9, 1);
                qi.CompletionDate = q.Substring(10, 6);
                qi.CompletionTime = q.Substring(14, 4); //有问题s

                int qPos = q.IndexOf('Q');
                if (qPos > -1)
                    qi.QuestionAndAnswers = q.Substring(qPos);

                questionnaireList.Add(qi);
            }

            return questionnaireList;
        }
        public static List<NonCbkProductPurchaseInfo> DecodeType22(string scannerData)
        {
            //Type22
            List<NonCbkProductPurchaseInfo> nonCbkList = new List<NonCbkProductPurchaseInfo>();

            string[] purchaseArray = GetSplitedArray(scannerData); ;

            foreach (string p in purchaseArray)
            {
                if (p.Length < 5)
                    continue;

                string recordType = p.Substring(2, 2);

                if (recordType == Type22)
                {
                    NonCbkProductPurchaseInfo nonCbkProduct = new NonCbkProductPurchaseInfo();
                    nonCbkProduct.Content = p;
                    nonCbkProduct.Type = Type22;
                    nonCbkProduct.Length = Convert.ToInt32(p.Substring(0, 2));

                    nonCbkProduct.ProductCode = p.Substring(4, 13);
                    nonCbkProduct.EntryMethod = p.Substring(17, 1);
                    nonCbkProduct.SpecialOfferCode = p.Substring(18, 2);
                    nonCbkProduct.Quantity = p.Substring(20, 2);
                    nonCbkProduct.Price = p.Substring(22, 8);
                    nonCbkProduct.IsLengthVariable = true;
                    //nonCbkProduct.Count = nonCbkList.Count + 1;
                    nonCbkList.Add(nonCbkProduct);
                }

            }

            return nonCbkList;
        }


        public static DecodeFormatInfo GetDecodeFormat(string cbkVersion, string type, string cbField)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT * FROM sCodebook_decode WHERE  convert(int,rtrim(ltrim(replace(Cbk_Version,'	',''))))={0} and Record_Type={1} and CB_Field='{2}' ", Convert.ToInt32(cbkVersion), type, cbField);

            Database db = DatabaseFactory.CreateDatabase("wis");

            DbCommand dbCommand = db.GetSqlStringCommand(sql.ToString());
            dbCommand.CommandTimeout = 60 * 3;
            IDataReader dr = db.ExecuteReader(dbCommand);

            if (dr.Read())
            {
                string decodeFormat = dr["Decode_format"].ToString();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(decodeFormat);
                XmlNodeList nodeList = doc.SelectNodes("/Contents");
                XmlNode node = doc.SelectSingleNode("/Contents");
                // <Scanner_Price Start="1" Length="5" Type="F" />
                //<Weight Start="6" Length="7" Type="O" />
                //<N-328V10 Start="13" Length="2" Type="P" />
                //<Quantity Start="15" Length="2" Type="F" />
                //<Offer Start="17" Length="2" Type="F" />
                // if Type = 'P' then
                // N-328V10= substring.value~ S-220B12=substring.value
                // 328V10= substring.value~ S-220B12=substring.value (Remove 'N-')
                DecodeFormatInfo df = new DecodeFormatInfo();
                DecodeFormatInfo d1;
                foreach (XmlNode n in node.ChildNodes)
                {
                    if (n.Name.StartsWith("N-") || n.Name.StartsWith("S-"))
                    {
                        if (df.ProductAttrList == null)
                            df.ProductAttrList = new List<DecodeFormatInfo>();
                        d1 = new DecodeFormatInfo();
                        d1.Name = n.Name;
                        d1.Start = Convert.ToInt32(n.Attributes["Start"].Value);
                        d1.Length = Convert.ToInt32(n.Attributes["Length"].Value);
                        d1.Type = n.Attributes["Type"].Value;
                        df.ProductAttrList.Add(d1);
                        continue;
                    }

                    if (node["Scanner_Price"] != null)
                    {
                        df.PriceStart = Convert.ToInt32(node["Scanner_Price"].Attributes["Start"].Value) - 1;
                        df.PriceLength = Convert.ToInt32(node["Scanner_Price"].Attributes["Length"].Value);
                    }

                    if (node["Weight"] != null)
                    {
                        df.WeightStart = Convert.ToInt32(node["Weight"].Attributes["Start"].Value) - 1;
                        df.WeightLength = Convert.ToInt32(node["Weight"].Attributes["Length"].Value);
                    }

                    if (node["Quantity"] != null)
                    {
                        df.QuanityStart = Convert.ToInt32(node["Quantity"].Attributes["Start"].Value) - 1;
                        df.QuanityLength = Convert.ToInt32(node["Quantity"].Attributes["Length"].Value);
                    }

                        if (node["Offer"] != null)
                        {
                            df.OfferStart = Convert.ToInt32(node["Offer"].Attributes["Start"].Value) - 1;
                            df.OfferLength = Convert.ToInt32(node["Offer"].Attributes["Length"].Value);
                        }
                    }
                return df;
            }

            return null;
        }

        public static List<CbkProductPurchaseInfo> DecodeType23(string scannerData)
        {
            //Type23

            List<CbkProductPurchaseInfo> cbkList = new List<CbkProductPurchaseInfo>();
            string[] purchaseArray = GetSplitedArray(scannerData); ;
            VersionInfo versionInfo = DecodeType56(scannerData);
            //string version = versionInfo.CodeBook.Length > 4 ? versionInfo.CodeBook.Substring(1) : versionInfo.CodeBook;
            string version = versionInfo.CodeBook;
            foreach (string p in purchaseArray)
            {
                if (p.Length < 5)
                    continue;

                string recordType = p.Substring(2, 2);


                if (recordType == Type23)
                {
                    CbkProductPurchaseInfo cbkProduct = new CbkProductPurchaseInfo();
                    cbkProduct.Content = p;
                    cbkProduct.Type = Type23;
                    cbkProduct.Length = Convert.ToInt32(p.Substring(0, 2));

                    cbkProduct.ProductCode = p.Substring(4, 4);


                    if (p.Length > 8)
                        cbkProduct.VarCombinations = p.Substring(8);
                    //2623L790
                    //11111
                    //22222
                    //0018
                    //33
                    //16
                    DecodeFormatInfo df = GetDecodeFormat(version, Type23, cbkProduct.ProductCode);
                    if (df != null)
                    {
                        cbkProduct.Price = cbkProduct.VarCombinations.Substring(df.PriceStart, df.PriceLength);
                        cbkProduct.Weight = cbkProduct.VarCombinations.Substring(df.WeightStart, df.WeightLength);
                        cbkProduct.Quantity = cbkProduct.VarCombinations.Substring(df.QuanityStart, df.QuanityLength);
                        if (df.OfferStart > cbkProduct.VarCombinations.Length)
                            cbkProduct.Offer = cbkProduct.VarCombinations.Substring(df.OfferStart, df.OfferLength);
                        if (df.ProductAttrList != null)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (var pa in df.ProductAttrList)
                            {
                                if (pa.Start > cbkProduct.VarCombinations.Length)
                                    continue;
                                sb.Append(pa.Name.Replace("N-", ""));
                                sb.Append("=");
                                sb.Append(cbkProduct.VarCombinations.Substring(pa.Start, pa.Length));
                                sb.Append("~");
                            }
                            cbkProduct.ProductAttr = sb.ToString().Trim('~');
                        }
                    }
                    //cbkProduct.Price = p.Substring(8, 5);
                    //if (cbkProduct.Length > 20)
                    //{
                    //    cbkProduct.Weight = p.Substring(13, 5);
                    //    cbkProduct.Quantity = p.Substring(24, 2);
                    //    cbkProduct.Unit = p.Substring(18, 4);
                    //    cbkProduct.Quantity = p.Substring(22, 2);
                    //    cbkProduct.Offer = p.Substring(24);
                    //}
                    cbkProduct.IsLengthVariable = true;
                    // cbkProduct.Count = cbkList.Count + 1;
                    cbkList.Add(cbkProduct);
                }
            }

            return cbkList;
        }
        public static WeightPurchaseInfo DecodeType43(string scannerData)
        {
            WeightPurchaseInfo wpi = new WeightPurchaseInfo();
            string[] sliceData = GetSplitedArray(scannerData); ;

            foreach (string p in sliceData)
            {
                if (p.Length < 5)
                    continue;

                string recordType = p.Substring(2, 2);

                if (recordType != Type43)
                    continue;
                wpi.Content = p;
                wpi.Type = Type43;
                wpi.IsLengthVariable = true;
                wpi.Length = p.Length;
                wpi.OriginalBarcode = p.Substring(4, 13);
                wpi.EntryMethod = p.Substring(17, 1);
                wpi.CodeBookProduct = p.Substring(18, 4);
                wpi.LeftFromCodeBook = p.Substring(22);

                return wpi;
            }

            return wpi;
        }
        public static VersionInfo DecodeType56(string scannerData)
        {
            VersionInfo version = new VersionInfo();
            version.Type = Type56;

            if (scannerData.IndexOf("\r\n") > -1)
            {
                string[] versionArray = scannerData.Split(Split_Marks_Return);
                foreach (string v in versionArray)
                {
                    if (v.Length < 5)
                        continue;

                    string recordType = v.Substring(2, 2);

                    if (recordType == Type56)
                    {
                        version.Length = v.Length;
                        version.Content = v.Substring(0, 59);
                        version.PanelScan = v.Substring(4, 5);
                        version.CodeBook = v.Substring(14, 5);
                        version.ShopCode = v.Substring(19, 5);
                        version.Connection = v.Substring(24, 5);
                        version.UserEAN = v.Substring(29, 5);
                        version.AddtionalEAN = v.Substring(34, 5);
                        version.EANQuestions = v.Substring(39, 5);
                        version.FontFile = v.Substring(54, 5);
                        break;
                    }
                }
            }
            else
            {
                string v = scannerData.Substring(138, 59);
                version.Length = v.Length;
                version.Content = v.Substring(0, 59);
                version.PanelScan = v.Substring(4, 5);
                version.CodeBook = v.Substring(14, 5);
                version.ShopCode = v.Substring(19, 5);
                version.Connection = v.Substring(24, 5);
                version.UserEAN = v.Substring(29, 5);
                version.AddtionalEAN = v.Substring(34, 5);
                version.EANQuestions = v.Substring(39, 5);
                version.FontFile = v.Substring(54, 5);
            }

            return version;
        }
        public static List<ShopInfo> DecodeType21(string scannerData)
        {
            List<ShopInfo> shopList = new List<ShopInfo>();
            VersionInfo versionInfo = DecodeType56(scannerData);
            string version = versionInfo.CodeBook.Length > 4 ? versionInfo.CodeBook.Substring(1) : versionInfo.CodeBook;
            string[] shopArray = GetSplitedArray(scannerData); ;

            for (int i = 0; i < shopArray.Length; i++)
            {
                string shopString = shopArray[i];
                ShopInfo si = new ShopInfo();
                si.Type = Type21;

                int position7321 = shopString.LastIndexOf("7321");

                if (position7321 < 0)
                    continue;

                si.Content = shopString.Substring(position7321, 73);
                si.Length = si.Content.Length;
                si.PurchaserCode = si.Content.Substring(4, 1);
                si.ShopCode = si.Content.Substring(5, 3);
                si.BranchCode = si.Content.Substring(8, 5);
                si.USIFlag = si.Content.Substring(13, 1);
                si.TotalAmountSpent = si.Content.Substring(14, 8);
                si.ShoppingTripStartDate = si.Content.Substring(22, 6);
                si.ShoppingTripStartTime = si.Content.Substring(28, 4);
                si.ShoppingTripEndTime = si.Content.Substring(32, 4);
                si.LoyaltyCard = si.Content.Substring(37, 1);
                si.MethodOfPayment = si.Content.Substring(38, 2);
                si.LanguageCode = si.Content.Substring(40, 2);
                si.TripTermination = si.Content.Substring(44, 1);

                //购买批次下面的条码商品
                for (int n = i + 1; n < shopArray.Length; n++)
                {
                    short barcodeProductCount = 0;
                    NonCbkProductPurchaseInfo ncpp = GetType22(shopArray[n]);
                    if (ncpp != null)
                        barcodeProductCount++;

                    CbkProductPurchaseInfo cpp = GetType23(version, shopArray[n]);
                    if (cpp != null)
                        barcodeProductCount++;

                    if (barcodeProductCount == 0)
                        break;

                    if (si.NonCbkProductPurchaseList == null)
                        si.NonCbkProductPurchaseList = new List<NonCbkProductPurchaseInfo>();

                    if (si.CbkProductPurchaseList == null)
                        si.CbkProductPurchaseList = new List<CbkProductPurchaseInfo>();

                    si.NonCbkProductPurchaseList.Add(ncpp);
                    si.CbkProductPurchaseList.Add(cpp);
                }

                shopList.Add(si);

            }

            return shopList;
        }


        // 获得购买批次下条码商品

        private static NonCbkProductPurchaseInfo GetType22(string type22Content)
        {
            string recordType = type22Content.Substring(2, 2);

            if (recordType == Type22)
            {
                NonCbkProductPurchaseInfo nonCbkProduct = new NonCbkProductPurchaseInfo();
                nonCbkProduct.Content = type22Content;
                nonCbkProduct.Type = Type22;
                nonCbkProduct.Length = Convert.ToInt32(type22Content.Substring(0, 2));

                nonCbkProduct.ProductCode = type22Content.Substring(4, 13);
                nonCbkProduct.EntryMethod = type22Content.Substring(17, 1);
                nonCbkProduct.SpecialOfferCode = type22Content.Substring(18, 2);
                nonCbkProduct.Quantity = type22Content.Substring(20, 2);
                nonCbkProduct.Price = type22Content.Substring(22, 8);
                if (type22Content.Length > 31)
                    nonCbkProduct.VarCombinations = type22Content.Substring(31);
                return nonCbkProduct;
            }

            return null;
        }
        //获得购买批次下codebook上的商品信息
        private static CbkProductPurchaseInfo GetType23(string version, string type23Content)
        {
            //Type23            
            if (type23Content.Length < 5)
                return null;

            string recordType = type23Content.Substring(2, 2);

            if (recordType == Type23)
            {
                CbkProductPurchaseInfo cbkProduct = new CbkProductPurchaseInfo();
                cbkProduct.Content = type23Content;
                cbkProduct.Type = Type23;
                cbkProduct.Length = Convert.ToInt32(type23Content.Substring(0, 2));
                cbkProduct.VarCombinations = type23Content.Substring(8);
                cbkProduct.ProductCode = type23Content.Substring(4, 4);
                DecodeFormatInfo df = GetDecodeFormat(version, Type23, cbkProduct.ProductCode);
                if (df != null)
                {
                    cbkProduct.Price = cbkProduct.VarCombinations.Substring(df.PriceStart, df.PriceLength);
                    cbkProduct.Weight = cbkProduct.VarCombinations.Substring(df.WeightStart, df.WeightLength);
                    cbkProduct.Quantity = cbkProduct.VarCombinations.Substring(df.QuanityStart, df.QuanityLength);
                    if (df.OfferStart > cbkProduct.VarCombinations.Length)
                        cbkProduct.Offer = cbkProduct.VarCombinations.Substring(df.OfferStart, df.OfferLength);

                }

                //if (cbkProduct.Length > 20)
                //{

                //    cbkProduct.Price = type23Content.Substring(8, 5);
                //    cbkProduct.Weight = type23Content.Substring(13, 5);
                //    cbkProduct.Unit = type23Content.Substring(18, 4);
                //    cbkProduct.Quantity = type23Content.Substring(22, 2);
                //    cbkProduct.Offer = type23Content.Substring(24);
                //}
                cbkProduct.IsLengthVariable = true;

                return cbkProduct;
            }

            return null;
        }



        public static List<DiagVersionChange> DecodeType57(string scannerData)
        {
            var list = GetSplitedArray(scannerData, Type57);
            var dtList = new List<DiagVersionChange>();
            DiagVersionChange dvc;


            foreach (var d in list)
            {
                dvc = new DiagVersionChange();
                dvc.Length = Convert.ToInt32(d.Substring(0, 2));
                dvc.Type = Type57;
                dvc.OldVersionNumber = d.Substring(19, 5);
                dvc.NewVersionNumber = d.Substring(24, 5);
                dvc.FileType = d.Substring(16, 3);
                dvc.Content = d;
            }

            return dtList;
        }

        public static List<DiagTime> DecodeType58(string scannerData)
        {
            var list = GetSplitedArray(scannerData, Type58);
            var dtList = new List<DiagTime>();
            DiagTime dt;

            foreach (var d in list)
            {
                dt = new DiagTime();
                dt.Type = Type58;
                dt.Length = Convert.ToInt32(d.Substring(0, 2));
                dt.DTBeforeCorrection = d.Substring(4, 12);
                dt.DTAfterCorrection = d.Substring(16, 12);
                dt.Content = d;
                dtList.Add(dt);
            }

            return dtList;
        }

        public static List<DiagCall> DecodeType59(string scannerData)
        {
            var list = GetSplitedArray(scannerData, Type59);
            var dcList = new List<DiagCall>();
            DiagCall dc;
            foreach (var d in list)
            {
                dc = new DiagCall();
                dc.Content = d;
                dc.Length = Convert.ToInt32(d.Substring(0, 2));
                dc.Type = Type59;
                dc.DateTime = d.Substring(4, 16);
                dc.Status = d.Substring(16, 1);
                dc.CommunicationState = d.Substring(17, 2);
                dc.CommunicationCode = d.Substring(19, 2);
                dc.TelephoneNumber = d.Substring(21, 20);
                dcList.Add(dc);
            }

            return dcList;
        }

        public static CradleInfo DecodeType02(string scannerData)
        {
            CradleInfo ci = new CradleInfo();
            var list = GetSplitedArray(scannerData, Type02);

            foreach (var d in list)
            {
                ci.CradleSerialNumber = d.Substring(4, 20);
                ci.SimNumber = d.Substring(24, 25);
            }
            return ci;
        }

        public static ShopHeaderExten DecodeType26(string scannerData)
        {
            var she = new ShopHeaderExten();
            var list = GetSplitedArray(scannerData, Type26);

            //            she.Fascia=
            //she.VarCombinations = list[0]
            if (list.Count > 1)
                she.VarCombinations = list[0].Substring(44);
            return she;
        }
    }
}
