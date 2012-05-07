using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace TreeBrowser.DataPortal
{
    public static class DbHelper 
    {

        public static Database CreateDB()
        {
            return DatabaseFactory.CreateDatabase("Main.ConnectionString");
        }

        public static DbConnection OpenConnection()
        {
            var connection = CreateDB().CreateConnection();
            connection.Open();
            return connection;
        }

    }
}
