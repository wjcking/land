using System;
using System.Collections.Generic;
using System.Text;

using System.Globalization;
namespace KitScanner
{
   public class C
    {
           private const string DiagnosticTimeFormat = "yyMMddHHmmss";
        public static DateTimeFormatInfo DateTimeFormat;
        public static DateTimeFormatInfo DateFormat;
        public static DateTimeFormatInfo TimeFormat;
        static C()
        {
            DateTimeFormat = new DateTimeFormatInfo();
            DateTimeFormat.ShortDatePattern = DiagnosticTimeFormat;

            DateFormat = new DateTimeFormatInfo();
            DateFormat.ShortDatePattern = "yyMMdd";


            TimeFormat = new DateTimeFormatInfo();
            TimeFormat.ShortTimePattern = "HHmmss";
        }
    }
}
