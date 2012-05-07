using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Serialization;
using Csla;
#if !SILVERLIGHT
using System.Data;
using Csla;

#endif

namespace TreeBrowser.Entities
{

    [Serializable]
    public class ReadOnlyLineages : ReadOnlyListBase<ReadOnlyLineages, ReadOnlyLineage>
    {

        #region Logic Methods


        public bool ContainsDuplicates()
        {
            return this.GroupBy(li => li.Id).Any(grp => grp.Count() > 1);
        }

        public ReadOnlyLineage LookupLineage(int lineageId)
        {
            return this.Single(l => l.Id == lineageId);
        }

        public bool ContainsLineageWithId(int lineageId)
        {
            return this.Any(l => l.Id == lineageId);
        }

        public double MaxYear
        {
            get
            {
                return this.Max(li => li.MaxYear);
            }
        }


        public double MinYear
        {
            get
            {
                return this.Min(li => li.StartYear);
            }
        }

        #endregion

        #region Tree Methods


        public bool IsLineageAChildOfLineageB(int lineageAId, int lineageBId)
        {
            return LookupLineage(lineageBId).Children.Any(li => li.Id == lineageAId);
        }

        public ReadOnlyLineage GetRoot()
        {
            return this.SingleOrDefault(li => li.ParentLineageId == null);
        }

        public void RemoveRoot()
        {
            int rootId = GetRoot().Id;
            RaiseListChangedEvents = false;
            IsReadOnly = false;
            Remove(this.Single(l => l.Id == rootId));
            RaiseListChangedEvents = true;
            IsReadOnly = true;
        }

        #endregion

        #region Factory Methods

        public static void GetLineages(EventHandler<DataPortalResult<ReadOnlyLineages>> handler)
        {
            GetLineages(handler, null);
        }

        public static void GetLineages(EventHandler<DataPortalResult<ReadOnlyLineages>> handler, int? rootLineageId)
        {
            DataPortal<ReadOnlyLineages> dp = new DataPortal<ReadOnlyLineages>();
            if (handler != null)
                dp.FetchCompleted += handler;
            dp.BeginFetch(new SingleCriteria<ReadOnlyLineages, int?>(rootLineageId));
        }

#if !SILVERLIGHT

        public static ReadOnlyLineages GetLineagesLocally(int? rootId)
        {
            var lins = new ReadOnlyLineages();
            lins.Populate(FetchBranch(rootId));
            return lins;
        }

#endif

        #endregion

        protected override void OnDeserialized()
        {
            base.OnDeserialized();
            foreach (var child in this)
                child.SetParent(this);
        }

#if !SILVERLIGHT

        protected void DataPortal_Fetch(SingleCriteria<ReadOnlyLineages, int?> criteria)
        {
            Populate(FetchBranch(criteria.Value));
        }

        private void Populate(ReadOnlyLineage[] lins)
        {
            RaiseListChangedEvents = false;
            IsReadOnly = false;
            foreach (ReadOnlyLineage lin in lins)
            {
                Add(lin);
                lin.SetParent(this);
            }
            IsReadOnly = false;
            RaiseListChangedEvents = true;
        }

        public static ReadOnlyLineage[] FetchBranch(int? rootId)
        {
            return ConvertTableToLineages(TreeBrowser.DataPortal.BranchWorker.FetchBranch(rootId));
        }

        private static ReadOnlyLineage[] ConvertTableToLineages(DataTable lineageTable)
        {
            var lineages = new ReadOnlyLineage[lineageTable.Rows.Count];
            for (int rowIndex = 0; rowIndex < lineageTable.Rows.Count; rowIndex++)
                lineages[rowIndex] = ReadOnlyLineage.GetLineage(lineageTable.Rows[rowIndex]);
            return lineages;
        }

#endif

    }
}
