using System;
using System.Windows;
using System.Windows.Controls;

namespace TreeBrowser.Silverlight.Application.Views
{
    public partial class LoginWindow : ChildWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            TreeBrowser.Entities.Security.TreeBrowserPrincipal.Login(LoginUsernameTextBox.Text,
                                 LoginPasswordTextBox.Password);
            this.DialogResult = true;
        }

    }
}