
using System.Data.SqlTypes;

namespace KitScanner
{
    public class SqlRecordInfoBase
    {
        public SqlInt32 Length { get; set; }
        public SqlString Type { get; set; }
        public SqlString Name { get; set; }
        public string Content { get; set; }
    }

    /// <summary>
    /// type23, type22
    /// </summary>
    public class SqlProductPurchaseInfo : SqlRecordInfoBase
    {
        public SqlString ProductCode { get; set; }
    }

    public class SqlHeaderInfo : SqlRecordInfoBase
    {
        public SqlString Description {get;set;}
        public SqlString Value { get; set; }
    }

    public class SqlVersionInfo : SqlRecordInfoBase
    {
        public SqlString Description { get; set; }
        public SqlString Value { get; set; }
    }
    public class SqlShopInfo : SqlRecordInfoBase
    {
        public SqlString Description { get; set; }
        public SqlString Value { get; set; }
    }
    public class SqlQuestionnaireInfo
    {
        /// <summary>
        /// from the questionnaire number
        /// </summary>
        public SqlString Number { get; set; }
        /// <summary>
        /// not used
        /// </summary>
        public SqlString UserCode { get; set; }
        public SqlString CompletionDate { get; set; }
        /// <summary>
        /// from the timestamp of the last question
        /// </summary>
        public SqlString CompletionTime { get; set; }

        /// <summary>
        /// Var1=xx~var2=yuyyyy
        /// Where Var1 & Var2 are variable names from the question table and 
        /// xx && yy are the respective answers, each question and answer should then be separated by ~ 
        /// </summary>
        public SqlString QuestionAndAnswers { get; set; }
    }

    public class SqlQuestionInfo
    {
        public SqlString Question { get; set; }
        public SqlString Answer { get; set; }
    }
}
