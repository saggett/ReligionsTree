using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using TreeBrowser.Entities;
using TreeBrowser.Silverlight.Application.Views;
using TreeBrowser.SilverlightLib.Cache;
using TreeBrowser.SilverlightLib.Helpers;
using System.Windows.Browser;

namespace TreeBrowser.Silverlight.Application
{
    public partial class App : System.Windows.Application
    {

        public App()
        {
            InitEndpoint();
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;
            InitializeComponent();
        }

        private static void InitEndpoint()
        {
            Csla.Serialization.Mobile.MobileFormatter.UseBinaryXml = false;
            var bhb = new BasicHttpBinding
                {
                    Name = "BasicHttpBinding_IWcfPortal",
                    MaxBufferSize = 10000000,
                    MaxReceivedMessageSize = 10000000
                };

            bhb.Security.Mode = BasicHttpSecurityMode.None;
            Csla.DataPortalClient.WcfProxy.DefaultBinding = bhb;

            Csla.DataPortalClient.WcfProxy.DefaultUrl = UrlHelper.SitePath + "WcfPortal.svc";
            //System.Net.WebRequest.RegisterPrefix("http://", System.Net.Browser.WebRequestCreator.ClientHttp);
        }


        private string ContentNameFromQueryString
        {
            get
            {
                foreach (string key in HtmlPage.Document.QueryString.Keys)
                {
                    if (key == "contentName")
                        return HtmlPage.Document.QueryString["contentName"];
                }
                return null;
            }
        }
        
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (ContentNameFromQueryString != null)
                HtmlPage.Window.Navigate(new Uri(UrlHelper.SitePath + "#/" + ContentNameFromQueryString));

            this.RootVisual = new MainPage();
            ReadOnlyLineageGroups.GetLineageGroups();
            LineagesCache.Prefetch();
            HtmlContentCache.Default.PrefetchAll();
        }

        private ReadOnlyLineagesCache _lineagesCache;
        public ReadOnlyLineagesCache LineagesCache
        {
            get
            {
                if (_lineagesCache == null)
                {
                    _lineagesCache = new ReadOnlyLineagesCache();
                }
                return _lineagesCache;
            }
        }

        //private void Content_Resized(object sender, EventArgs e)
        //{
        //    var page = (SilverlightLib.Controls.Tree)RootVisual;
        //    page.Height = (int) Host.Content.ActualHeight;
        //    page.Width = (int) Host.Content.ActualWidth;
        //    //page.Draw();
        //}

        private void Application_Exit(object sender, EventArgs e)
        {

        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // a ChildWindow control.
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                ChildWindow errorWin = new ErrorWindow(e.ExceptionObject);
                errorWin.Show();
            }
        }

    }
}