using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TreeBrowser.Entities;
using TreeBrowser.Entities.Helpers;
using TreeBrowser.SilverlightLib.Cache;

namespace TreeBrowser.Silverlight.Application.Views
{
    public partial class HtmlContentPage : Page
    {
        public HtmlContentPage()
        {
            InitializeComponent();
            HtmlContentCache.Default.FetchHtmlContentCompleted += new EventHandler<Csla.DataPortalResult<HtmlContent>>(HtmlContentCache_FetchHtmlContentCompleted);
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ContentEnum = ExtractContentEnum();
            //TODO: Implement busy indicator
            HtmlContentCache.Default.BeginFetchOfContent(ContentEnum);
        }


        private void HtmlContentCache_FetchHtmlContentCompleted(object sender, Csla.DataPortalResult<HtmlContent> e)
        {
            if (e.Object.HtmlContentEnum != ContentEnum)
                return;
            DataContext = e.Object;
            Title = MetaDataHelper.CreateContentPageTitle(e.Object.Name);
        }

        public HtmlContentEnum ContentEnum
        {
            get;
            set;
        }

        private HtmlContentEnum ExtractContentEnum()
        {
            foreach (
                var dict in new[] { NavigationContext.QueryString, System.Windows.Browser.HtmlPage.Document.QueryString })
            {
                HtmlContentEnum contentEnum = ExtractContentEnum(dict);
                if (contentEnum != HtmlContentEnum.NotSpecified)
                    return contentEnum;
            }
            throw new InvalidOperationException("No content name was specified in the URL.");
        }

        private static HtmlContentEnum ExtractContentEnum(IDictionary<string, string> initParams)
        {
            if (!initParams.ContainsKey("contentName"))
                return HtmlContentEnum.NotSpecified;
            return HtmlContent.GetHtmlContentEnum(initParams["contentName"]);
        }

        private void ContentRichTextBlock_LinkClicked(object sender, Liquid.RichTextBoxEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(e.Parameter.ToString()), "_blank");
        }


    }
}