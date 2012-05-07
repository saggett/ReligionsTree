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
using System.Windows.Shapes;
using TreeBrowser.SilverlightLib.Cache;
using TreeBrowser.Entities;

namespace TreeBrowser.SilverlightLib.Controls
{
    public partial class HtmlIntroControl : UserControl
    {
        public HtmlIntroControl()
        {
            InitializeComponent();
            HtmlContentCache.Default.FetchHtmlContentCompleted += new EventHandler<Csla.DataPortalResult<HtmlContent>>(HtmlContentCache_FetchHtmlContentCompleted);
            HtmlContentCache.Default.BeginFetchOfContent(HtmlContentEnum.Intro);
        }

        private void HtmlTextBlock_LinkClicked(object sender, Liquid.RichTextBoxEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(e.Parameter.ToString()), "_blank");
        }

        private void HtmlContentCache_FetchHtmlContentCompleted(object sender, Csla.DataPortalResult<HtmlContent> e)
        {
            if (e.Object.HtmlContentEnum != HtmlContentEnum.Intro)
                return;
            HtmlTextBlock.HTML = e.Object.Content;
        }

    }
}
