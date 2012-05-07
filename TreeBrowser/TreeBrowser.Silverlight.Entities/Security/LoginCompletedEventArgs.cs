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

namespace TreeBrowser.Entities.Security
{
    public class LoginCompletedEventArgs : EventArgs
    {

        public bool Successful { get; private set; }

        public Exception Error { get; private set; }

        public LoginCompletedEventArgs(bool successful, Exception error)
        {
            Successful = successful;
            Error = error;
        }

    }
}
