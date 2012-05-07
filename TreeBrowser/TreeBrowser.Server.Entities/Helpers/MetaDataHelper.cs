using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TreeBrowser.Entities.Helpers
{
    public static class MetaDataHelper
    {

        private const string StaticTitle = "Religions Tree";

        public static string CreateTitle(string rootName, bool hasParent)
        {
            if (!String.IsNullOrEmpty(rootName) && hasParent)
            {
                string caption = rootName;
                caption = FormattingHelper.StripHtmlTags(caption);
                return CreateContentPageTitle(caption);
            }
            return StaticTitle;
        }

        public static string CreateTitle()
        {
            return CreateTitle(null, false);
        }

        public static string CreateContentPageTitle(string caption)
        {
            return String.Format("{0}: {1}", StaticTitle, caption);
        }


    }
}
