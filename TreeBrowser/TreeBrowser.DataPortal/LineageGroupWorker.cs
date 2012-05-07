using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TreeBrowser.DataPortal
{
    public class LineageGroupWorker : WorkerBase
    {

        public static DataTable FetchLineageGroups()
        {
            var db = CreateDB();
            var ds = db.ExecuteDataSet("LineageGroups_Fetch");
            return ds.Tables[0];
        }

    }

}
