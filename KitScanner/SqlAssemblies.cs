using Microsoft.SqlServer.Server;
using System.Collections;
using System.Data.SqlTypes;
using KitScanner;
using System.Collections.Generic;
using System.Reflection;

public class KitSqlAssemblies
{
    //[SqlFunction(IsDeterministic = true, IsPrecise = true)]
    //public static string RegExMatch(string matchString)
    //{
    //    return matchString;
    //}


    [SqlFunction(DataAccess = DataAccessKind.Read, FillRowMethodName = "FillBarcodeRow", TableDefinition = "Type varchar(4), Barcode nvarchar(20)")]
    public static IEnumerable GetBarcodeList(SqlString scannerData)
    {
        ArrayList resultCollection = new ArrayList();
        
        string dat = scannerData.ToString();

        List<NonCbkProductPurchaseInfo> type22List = DecodeScanner.DecodeType22(dat);
        List<CbkProductPurchaseInfo> type23List = DecodeScanner.DecodeType23(dat);

        foreach (NonCbkProductPurchaseInfo nc in type22List)
        {
            SqlProductPurchaseInfo ppi = new SqlProductPurchaseInfo();
            ppi.Type = nc.Type;
            ppi.ProductCode = nc.ProductCode;
        //    ppi.Name = nc.Name;
            
            resultCollection.Add(ppi);
        }

        foreach (CbkProductPurchaseInfo c in type23List)
        {
            SqlProductPurchaseInfo ppi = new SqlProductPurchaseInfo();
            ppi.Type = c.Type;
            ppi.ProductCode = c.ProductCode;
       //     ppi.Name = c.Name;
            resultCollection.Add(ppi);
        }
        return resultCollection;
    }

    [SqlFunction(DataAccess = DataAccessKind.Read, FillRowMethodName = "FillHeaderRow", TableDefinition = "Description nvarchar(85), Value nvarchar(85)")]
    public static IEnumerable GetHeaderInfo(SqlString scannerData)
    {        
        ArrayList resultCollection = new ArrayList();
        HeaderInfo hi = DecodeScanner.DecodeType01(scannerData.ToString());

        foreach (PropertyInfo pi in typeof(HeaderInfo).GetProperties())
        {
            SqlHeaderInfo shi = new SqlHeaderInfo();
            shi.Description = pi.Name;

            object o = pi.GetValue(hi, null);
            shi.Value = o == null ? "" : o.ToString();
            resultCollection.Add(shi);
        }

        return resultCollection;
    }

    [SqlFunction(DataAccess = DataAccessKind.Read, FillRowMethodName = "FillShopRow", TableDefinition = "Description nvarchar(100), Value nvarchar(100)")]
    public static IEnumerable GetShopInfo(SqlString scannerData)
    {
        ArrayList resultCollection = new ArrayList();
     //   ShopInfo si = ScannerOutput.DecodeType21(scannerData.ToString());
        List<ShopInfo> shopList = new List<ShopInfo>();
        foreach (ShopInfo si in shopList)
        {
            foreach (PropertyInfo pi in typeof(ShopInfo).GetProperties())
            {
                SqlShopInfo shi = new SqlShopInfo();
                shi.Description = pi.Name;

                object o = pi.GetValue(si, null);
                shi.Value = o == null ? "" : o.ToString();
                resultCollection.Add(shi);
            }
        }

        return resultCollection;
    }

    [SqlFunction(DataAccess = DataAccessKind.Read, FillRowMethodName = "FillVersionRow", TableDefinition = "Description nvarchar(59), Value nvarchar(59)")]
    public static IEnumerable GetVersionInfo(SqlString scannerData)
    {
        ArrayList resultCollection = new ArrayList();
        VersionInfo vi = DecodeScanner.DecodeType56(scannerData.ToString());

        foreach (PropertyInfo pi in typeof(VersionInfo).GetProperties())
        {
            SqlVersionInfo svi = new SqlVersionInfo();
            svi.Description = pi.Name;

            object o = pi.GetValue(vi, null);
            svi.Value = o == null ? "" : o.ToString();
            resultCollection.Add(svi);
        }

        return resultCollection;
    }

    [SqlFunction(DataAccess = DataAccessKind.Read, FillRowMethodName = "FillQuestionnaireRow", TableDefinition = " Number nvarchar(20),   userCode nvarchar(20) , completionDate nvarchar(20) , completionTime nvarchar(20) ,   questionAndAnswer nvarchar(max)  ")]
    public static IEnumerable GetQuestionnaireList(SqlString scannerData)
    {
        ArrayList resultCollection = new ArrayList();
        List<QuestionnaireInfo> questionnaireList = DecodeScanner.DecodeType29(scannerData.ToString());

        for (int i = 0; i < questionnaireList.Count; i++)
        {
            SqlQuestionnaireInfo sqi = new SqlQuestionnaireInfo();
            sqi.Number = questionnaireList[i].Number;
            sqi.UserCode = questionnaireList[i].UserCode;
            sqi.CompletionDate = questionnaireList[i].CompletionDate;
            sqi.CompletionTime = questionnaireList[i].CompletionTime;
            sqi.QuestionAndAnswers = questionnaireList[i].QuestionAndAnswers;
            resultCollection.Add(sqi);
        }

        return resultCollection;
    }

    [SqlFunction(DataAccess = DataAccessKind.Read, FillRowMethodName = "FillQuestionRow", TableDefinition = "Question nvarchar(20), Answer nvarchar(20)")]
    public static IEnumerable GetQuestionList(SqlString scannerData)
    {
        ArrayList resultCollection = new ArrayList();
        List<QuestionnaireInfo> questionnaireList = DecodeScanner.DecodeType29(scannerData.ToString());

        for (int i = 0; i < questionnaireList.Count; i++)
        {
            foreach (QuestionInfo qi in questionnaireList[i].QuestionList)
            {
                SqlQuestionInfo sqi = new SqlQuestionInfo();
                sqi.Question = qi.Question;
                sqi.Answer = qi.Answer;
                resultCollection.Add(sqi);
            }
        }

        return resultCollection;
    }


    public static void FillBarcodeRow(object resultObj, out SqlString type, out SqlString barcode)
    {
        SqlProductPurchaseInfo ppi = resultObj as SqlProductPurchaseInfo;
        type = ppi.Type ;
        barcode = ppi.ProductCode;
    }

    public static void FillHeaderRow(object resultObj, out SqlString desc, out SqlString value)
    {
        SqlHeaderInfo hi = resultObj as SqlHeaderInfo;
        desc = hi.Description;
        value = hi.Value;
    }

    public static void FillVersionRow(object resultObj, out SqlString desc, out SqlString value)
    {
        SqlVersionInfo svi = resultObj as SqlVersionInfo;
        desc = svi.Description;
        value = svi.Value;
    }
    public static void FillShopRow(object resultObj, out SqlString desc, out SqlString value)
    {
        SqlShopInfo ssi = resultObj == null ? new SqlShopInfo() : resultObj as SqlShopInfo;
        desc = ssi.Description == null ? "" : ssi.Description;
        value = ssi.Value== null ? "" : ssi.Value;
    }
    public static void FillQuestionnaireRow(object resultObj, out SqlString number, out SqlString userCode, out SqlString completionDate, out SqlString completionTime, out SqlString questionAndAnswers)
    {
        SqlQuestionnaireInfo svi = resultObj as SqlQuestionnaireInfo;
        number = svi.Number;
        userCode = svi.UserCode;
        completionDate = svi.CompletionDate;
        completionTime = svi.CompletionTime;
        questionAndAnswers = svi.QuestionAndAnswers;        
    }

    public static void  FillQuestionRow(object resultObj, out SqlString question, out SqlString answer)
    {
        SqlQuestionInfo qi = resultObj as SqlQuestionInfo;
        question = qi.Question;
        answer = qi.Answer;
    }
}