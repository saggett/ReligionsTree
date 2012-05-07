using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeBrowser.Entities.Helpers
{
    public class ConnectionHelper
    {

        public static string ConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["Main.ConnectionString"].ConnectionString;
            }
        }

    }
}