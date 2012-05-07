using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace TreeBrowser.DataPortal
{
    public abstract class WorkerBase
    {


        protected static Database CreateDB()
        {
            return DbHelper.CreateDB();
        }
    }
}
