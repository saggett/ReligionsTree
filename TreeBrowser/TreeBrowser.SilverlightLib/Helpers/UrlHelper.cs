using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;

namespace TreeBrowser.SilverlightLib.Helpers
{
    public static class UrlHelper
    {

        public static string SitePath
        {
            get
            {
                var uriBuilder = new UriBuilder(HtmlPage.Document.DocumentUri);
                string path = uriBuilder.Path;
                //remove last slash and everything after it
                path = path.Remove(path.LastIndexOf("/")) + "/";
                return "http://" + uriBuilder.Host + path;
            }
        }

        public static Uri SitePathUri
        {
            get { return new Uri(SitePath, UriKind.Absolute); }
        }
        

    }
}
