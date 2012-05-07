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

namespace TreeBrowser.SilverlightLib.Proxy
{
    public class DeleteLineageCompletedArgs : System.EventArgs
    {


        public int NewRootLineageId { get; private set; }

        public DeleteLineageCompletedArgs(int newRootLineageId)
        {
            NewRootLineageId = newRootLineageId;
        }



    }
}
