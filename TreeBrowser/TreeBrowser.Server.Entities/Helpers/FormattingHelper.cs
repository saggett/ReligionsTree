using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TreeBrowser.Entities.Helpers
{
    public static class FormattingHelper
    {

        public static string ConvertToDisplayYear(this int year)
        {
            string adSuffix = year < 0 ? "BCE" : "CE";
            if (year < 0)
                year = year * -1;
            return String.Format("{0} {1}", year, adSuffix);
        }

        public static string ConvertToDisplayYear(this int? year)
        {
            return year.HasValue ? year.Value.ConvertToDisplayYear() : String.Empty;
        }

        public static string StripHtmlTags(string htmlString)
        {
            const string pattern = @"<(.|\n)*?>";
            return Regex.Replace(htmlString, pattern, string.Empty);
        }

    }
}
