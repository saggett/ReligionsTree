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
using TreeBrowser.Entities;
using TreeBrowser.SilverlightLib.Cache;

namespace TreeBrowser.SilverlightLib.Controls
{
    public class HtmlIntroBlock : Liquid.RichTextBlock
    {

        public HtmlIntroBlock()
        {
            HtmlContentCache.Default.FetchHtmlContentCompleted += new EventHandler<Csla.DataPortalResult<HtmlContent>>(HtmlContentCache_FetchHtmlContentCompleted);
            HtmlContentCache.Default.BeginFetchOfContent(HtmlContentEnum.Intro);
            this.LinkClicked += new Liquid.RichTextBoxEventHandler(HtmlIntroBlock_LinkClicked);
            Background = new SolidColorBrush(Colors.Yellow);
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private void HtmlIntroBlock_LinkClicked(object sender, Liquid.RichTextBoxEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(e.Parameter.ToString()), "_blank");
        }

        

        private void HtmlContentCache_FetchHtmlContentCompleted(object sender, Csla.DataPortalResult<HtmlContent> e)
        {
            if (e.Object.HtmlContentEnum != HtmlContentEnum.Intro)
                return;
            HTML = e.Object.Content;
        }


    }
}
