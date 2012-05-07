using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Csla;
using TreeBrowser.Entities.Security;
using TreeBrowser.Silverlight.Application.Views;

namespace TreeBrowser.Silverlight.Application
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            TreeBrowserPrincipal.LoginCompleted += new EventHandler<LoginCompletedEventArgs>(TreeBrowserPrincipal_LoginCompleted);
        }


        private void TreeBrowserPrincipal_LoginCompleted(object sender, LoginCompletedEventArgs e)
        {
            Visibility loginButtonVis = ApplicationContext.User.Identity.IsAuthenticated
                                             ? Visibility.Collapsed
                                             : Visibility.Visible;
            Visibility adminOptionsVis = ApplicationContext.User.Identity.IsAuthenticated
                                             ? Visibility.Visible
                                             : Visibility.Collapsed;
            ShowLoginButton.Visibility = loginButtonVis;
            Divider2.Visibility = loginButtonVis;
            EditContentButton.Visibility = adminOptionsVis;
            Divider3.Visibility = adminOptionsVis;
        }

        // After the Frame navigates, ensure the HyperlinkButton representing the current page is selected
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            foreach (UIElement child in LinksStackPanel.Children)
            {
                HyperlinkButton hb = child as HyperlinkButton;
                if (hb != null && hb.NavigateUri != null)
                {
                    if (hb.NavigateUri.ToString().Equals(e.Uri.ToString()))
                    {
                        VisualStateManager.GoToState(hb, "ActiveLink", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(hb, "InactiveLink", true);
                    }
                }
            }
        }

        // If an error occurs during navigation, show an error window
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            ChildWindow errorWin = new ErrorWindow(e.Uri);
            errorWin.Show();
        }

        private void ShowLoginButton_Click(object sender, RoutedEventArgs e)
        {
            ChildWindow loginWin = new LoginWindow();
            loginWin.Show();
        }

        private void EditContentButton_Click(object sender, RoutedEventArgs e)
        {
            ChildWindow contentEditorWin = new ContentEditorWindow();
            contentEditorWin.Show();
        }

    }
}