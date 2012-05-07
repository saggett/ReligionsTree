using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Navigation;
using TreeBrowser.Entities.Helpers;
using System.Windows.Browser;

namespace TreeBrowser.Silverlight.Application.Views
{
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
            ReligionsTree.NavigateRequest += ReligionsTree_NavigateRequest;
            ReligionsTree.RootBindComplete += ReligionsTree_RootBindComplete;
        }

        private void ReligionsTree_RootBindComplete(object sender, TreeBrowser.SilverlightLib.EventArgs.LineageIdEventArgs e)
        {
            Title = MetaDataHelper.CreateTitle(ReligionsTree.CurrentRootLineage.Name,
                                           ReligionsTree.CurrentRootLineage.ParentLineageId.HasValue);
        }

        private void ReligionsTree_NavigateRequest(object sender, TreeBrowser.SilverlightLib.EventArgs.LineageIdEventArgs e)
        {
            string path = "/Home";
            if (e.LineageId.HasValue)
                path += "?lineageId=" + e.LineageId.Value.ToString();
            ((MainPage)System.Windows.Application.Current.RootVisual).ContentFrame.Navigate(new Uri(path, UriKind.Relative));
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ReligionsTree.InitForRoot(ExtractRootLineageId());
        }

        private int? ExtractRootLineageId()
        {
            foreach (
                var dict in new[] {NavigationContext.QueryString, System.Windows.Browser.HtmlPage.Document.QueryString})
            {
                int? lineageId = ExtractRootLineageId(dict);
                if (lineageId != null)
                    return lineageId;
            }
            return null;
        }

        private static int? ExtractRootLineageId(IDictionary<string, string> initParams)
        {
            if (initParams.ContainsKey("lineageId"))
            {
                int result;
                if (Int32.TryParse(initParams["lineageId"], out result))
                    return result;
            }
            return null;
        }


    }
}