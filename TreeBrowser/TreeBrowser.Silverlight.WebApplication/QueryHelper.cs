using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TreeBrowser.Entities;

namespace TreeBrowser.Silverlight.WebApplication
{
    internal static class QueryHelper
    {

        public static int? SelectedLineageId
        {
            get { return HttpContext.Current.Request.QueryString.AllKeys.Contains("lineageId") ? Convert.ToInt32(HttpContext.Current.Request.QueryString["lineageId"]) : new int?(); }
        }


        public static string SelectedContentName
        {
            get { return HttpContext.Current.Request.QueryString.AllKeys.Contains("contentName") ? HttpContext.Current.Request.QueryString["contentName"] : String.Empty; }
        }

        public static HtmlContentEnum SelectedContentEnum
        {
            get
            {
                if (String.IsNullOrEmpty(SelectedContentName))
                    return HtmlContentEnum.NotSpecified;
                return HtmlContent.GetHtmlContentEnum(SelectedContentName);
            }
        }

    }
}