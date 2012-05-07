using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace TreeBrowser.DataPortal
{
    public class LineageHyperlinkWorker : WorkerBase
    {


        public static int SaveLineageHyperlink(DbTransaction transaction, int lineageHyperlinkId, int lineageId, string caption, string description, string url)
        {
            var db = CreateDB();
            return Convert.ToInt32(db.ExecuteScalar(transaction, "LineageHyperlink_Save",
                                    new object[]
                                        {
                                            lineageHyperlinkId, lineageId, caption, description, url
                                        }));
        }

        public static void DeleteLineageHyperlink(DbTransaction transaction, int lineageHyperlinkId)
        {
            var db = CreateDB();
            db.ExecuteNonQuery(transaction, "LineageHyperlink_Delete", lineageHyperlinkId);
        }

    }
}
