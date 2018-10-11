using System;
using System.Collections.Generic;
using System.Web;

namespace KitScanner
{
    /// <summary>
    ///  n x [Type 29] “questionnaire data” record 
    /// </summary>
    public class QuestionnaireInfo : RecordBaseInfo
    {
        /// <summary>
        /// from the questionnaire number
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// not used
        /// </summary>
        public string UserCode { get; set; }
        public string CompletionDate { get; set; }
        /// <summary>
        /// from the timestamp of the last question
        /// </summary>
        public string CompletionTime { get; set; }

        /// <summary>
        /// Var1=xx~var2=yuyyyy
        /// Where Var1 & Var2 are variable names from the question table and 
        /// xx && yy are the respective answers, each question and answer should then be separated by ~ 
        /// </summary>
        public string QuestionAndAnswers { get; set; }

        public QuestionInfo[] QuestionList
        {
            get
            {
                if (String.IsNullOrEmpty(QuestionAndAnswers) || QuestionAndAnswers == "")
                    return null;

                if (QuestionAndAnswers.IndexOf('~') > -1)
                {
                    string[] qaPairs = QuestionAndAnswers.Replace("AR",String.Empty).Split('~');
                    QuestionInfo[] questionList = new QuestionInfo[qaPairs.Length];

                    for (int i = 0; i < qaPairs.Length; i++)
                    {
                        if (qaPairs[i].IndexOf('=') > -1)
                        {
                            string[] qaArray = qaPairs[i].Split('=');

                            questionList[i] = new QuestionInfo();
                            questionList[i].Question = qaArray[0];
                            questionList[i].Answer = qaArray[1];
                        }
                    }
                    return questionList;
                }

                if (QuestionAndAnswers.IndexOf('=') > -1)
                {
                    QuestionInfo qi = new QuestionInfo();
                    string[] qaArray = QuestionAndAnswers.Split('=');
                    qi = new QuestionInfo();
                    qi.Question = qaArray[0].Length > 3 ? qaArray[0].Substring(0,2):qaArray[0];
                    qi.Answer = qaArray[1];

                    return new QuestionInfo[1] {qi};
                }

                return null;
            }
        }

    }

    /// <summary>
    /// for spliting questions and answers ,  could be more than one
    /// </summary>
    public class QuestionInfo
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
