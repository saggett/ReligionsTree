using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TreeBrowser.DataPortal
{
    public class HtmlContentWorker : WorkerBase
    {


        public static DataSet FetchHtmlContent(int htmlContentId)
        {
            var db = CreateDB();
            var ds = db.ExecuteDataSet("HtmlContent_Fetch", htmlContentId);
            return ds;
        }


        public static int SaveHtmlContent(int id, string name, string content, System.Data.Common.DbTransaction trans)
        {
            var db = CreateDB();
            return Convert.ToInt32(db.ExecuteScalar(trans, "HtmlContent_Save",
                                    new object[]
                                        {
                                            id, name, content
                                        }));
        }
    }
}
