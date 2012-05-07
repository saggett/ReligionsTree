using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;


namespace TreeBrowser.DataPortal
{
    public class BranchWorker : WorkerBase
    {
        public static DataTable FetchBranch(int? rootId)
        {
            return FetchBranch(rootId, 0);
        }

        public static DataTable FetchBranch(int? rootId, int maxCount)
        {
            var db = CreateDB();
            var ds = db.ExecuteDataSet("FetchBranch", ConvertNullableIntToParam(rootId));
            return ds.Tables[0];
        }

        public static int SaveLineage(int lineageId, string name, int startYear, int? endYear, int? parentLineageId, string notes, int? lineageGroupId, DbTransaction trans)
        {
            var db = CreateDB();
            return Convert.ToInt32(db.ExecuteScalar(trans, "Lineage_Save",
                                    new object[]
                                        {
                                            lineageId, name, startYear, ConvertNullableIntToParam(endYear),
                                            ConvertNullableIntToParam(parentLineageId), notes,
                                            ConvertNullableIntToParam(lineageGroupId)
                                        }));
        }

        private static object ConvertNullableIntToParam(int? val)
        {
            return val.HasValue ? (object) val.Value : (object) DBNull.Value;
        }

        public static void DeleteLineage(int lineageId, IEnumerable<int> linHyperlinkIds)
        {
            var db = CreateDB();
            using (var connection = DbHelper.OpenConnection())
            {
                var trans = connection.BeginTransaction();
                try
                {
                    foreach (var linkId in linHyperlinkIds)
                        LineageHyperlinkWorker.DeleteLineageHyperlink(trans, linkId);
                    db.ExecuteNonQuery(trans, "Lineage_Delete", lineageId);
                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public static DataSet FetchLineage(int lineageId)
        {
            var db = CreateDB();
            var ds = db.ExecuteDataSet("Lineage_Fetch", lineageId);
            return ds;
        }

    }
}
