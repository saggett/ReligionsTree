using System.Linq;
using System.Runtime.Serialization;

using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
using System;
#if !SILVERLIGHT
using System.Data;
#endif


namespace TreeBrowser.Entities
{

    [Serializable]
    public class Lineages : EditableRootListBase<Lineage>
    {

public Lineages() {}

#if SILVERLIGHT

protected override void AddNewCore()
{
    Add(Lineage.NewLineage());
}

#endif

        protected override void SetItem(int index, Lineage item)
        {
            base.SetItem(index, item);
            ContainsDuplicates();
        }

        public bool ContainsDuplicates()
        {
            return this.GroupBy(li => li.Id).Any(grp => grp.Count() > 1);
        }
        
        public Lineage LookupLineage(int lineageId)
        {
            return this.Single(l => l.Id == lineageId);
        }

        public bool ContainsLineageWithId(int lineageId)
        {
            return this.Any(l => l.Id == lineageId);
        }

        public Lineage GetRoot()
        {
            return this.SingleOrDefault(li => li.ParentLineageId == null);
        }

        public void RemoveWithId(int lineageId)
        {
            Remove(LookupLineage(lineageId));
        }

        public bool IsLineageAChildOfLineageB(int lineageAId, int lineageBId)
        {
            return LookupLineage(lineageBId).Children.Any(li => li.Id == lineageAId);
        }

        public double MaxYear
        {
            get
            {
                return this.Max<Lineage>(li => li.MaxYear);
            }
        }


        public static void GetLineages(EventHandler<DataPortalResult<Lineages>> handler)
        {
            GetLineages(handler, null);
        }

        public static void GetLineages(EventHandler<DataPortalResult<Lineages>> handler, int? rootLineageId)
        {
            ////check cache first. if we have it just invoke the event handler
            //if (CachedLineages != null)
            //{
            //    if (handler != null)
            //        handler.Invoke(null, new DataPortalResult<Lineages>(CachedLineages, null, null));
            //    return;
            //}
            DataPortal<Lineages> dp = new DataPortal<Lineages>();
            if (handler != null)
                dp.FetchCompleted += handler;
            //dp.FetchCompleted += Lineages_FetchCompleted;
            dp.BeginFetch(new SingleCriteria<Lineages, int?>(rootLineageId));
        }

    //public static event EventHandler<DataPortalResult<Lineages>> FetchLineagesCompleted;

    //private static void Lineages_FetchCompleted(object sender, DataPortalResult<Lineages> e)
    //{
    //    CachedLineages = e.Object;
    //    if (FetchLineagesCompleted != null)
    //        FetchLineagesCompleted(sender, e);
    //}

    //public static void InvalidateCache()
    //{
    //    CachedLineages = null;
    //    GetLineages();
    //}

    //public static Lineages CachedLineages { get; private set; }


#if !SILVERLIGHT

        protected void DataPortal_Fetch(SingleCriteria<Lineages,int?> criteria)
        {
            RaiseListChangedEvents = false;
            AddRange(FetchBranch(criteria.Value));
            RaiseListChangedEvents = true;
        }

        public static Lineage[] FetchBranch(int? rootId)
        {
            return ConvertTableToLineages(BranchWorker.FetchBranch(rootId));
        }

        private static Lineage[] ConvertTableToLineages(DataTable lineageTable)
        {
            var lineages = new Lineage[lineageTable.Rows.Count];
            for (int rowIndex = 0; rowIndex < lineageTable.Rows.Count; rowIndex++)
                lineages[rowIndex] = Lineage.GetLineage(lineageTable.Rows[rowIndex]);
            return lineages;
        }

#endif

        public static void GetLineages()
        {
            GetLineages(null);
        }
    }
}
