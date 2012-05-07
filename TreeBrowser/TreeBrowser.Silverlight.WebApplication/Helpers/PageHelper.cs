using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using TreeBrowser.Entities.Helpers;

namespace TreeBrowser.Silverlight.WebApplication.Helpers
{
    public static class PageHelper
    {


        public static string[] GetKeywords(string lineageName, string lineageGroupName)
        {
            List<string> keywords = new List<string>();
            lineageName = FormattingHelper.StripHtmlTags(lineageName);
            if (!string.IsNullOrEmpty(lineageName))
                keywords.Add(lineageName);
            lineageGroupName = FormattingHelper.StripHtmlTags(lineageGroupName);
            if (!string.IsNullOrEmpty(lineageGroupName))
                keywords.Add(lineageGroupName);
            keywords.AddRange(new[] { "Religion", "World", "Tree", "History", "Beliefs", "Evolution", "Silverlight" });
            return keywords.ToArray();
        }

        public static string CreateDescriptionMetaData(string desc)
        {
            desc = FormattingHelper.StripHtmlTags(desc);
            //256 characters max is recommended by yahoo, not sure about google.
            if (desc.Length > 256)
            {
                desc = desc.Substring(0, 256);
                if (desc.LastIndexOf('.') != -1)
                {
                    desc = desc.Substring(0, desc.LastIndexOf('.') + 1);
                    if (desc.Length == 0)
                        throw new Exception("Application error when parsing description string. ");
                }
            }
            return desc;
        }


        public static HtmlMeta CreateDescriptionMetatag(string desc)
        {
            return new HtmlMeta { Name = "Description", Content = CreateDescriptionMetaData(desc) };
        }

        public static HtmlMeta CreateKeywordsMetatag(params string[] contentName)
        {
            return new HtmlMeta() { Name = "Keywords", Content = String.Join(",", contentName) };
        }

    }
}
