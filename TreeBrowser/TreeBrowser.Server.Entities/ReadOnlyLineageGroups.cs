using System;
using System.Linq;
using Csla;
using Csla.Serialization;

#if !SILVERLIGHT
using TreeBrowser.DataPortal;
using System.Data;
#endif

namespace TreeBrowser.Entities
{

    [Serializable]
    public class ReadOnlyLineageGroups : ReadOnlyListBase<ReadOnlyLineageGroups, ReadOnlyLineageGroup>
    {


        public ReadOnlyLineageGroups() { }


        #region Methods
        
        public string LookupName(int groupId)
        {
            return CachedGroups.Single(lg => lg.Id == groupId).Name;
        }

        #endregion

        #region Data Access

#if !SILVERLIGHT


        private static ReadOnlyLineageGroups GetLineageGroupsLocally()
        {
            var groups = new ReadOnlyLineageGroups();
            groups.Populate();
            return groups;
        }

        private void DataPortal_Fetch()
        {
            Populate();
        }

        private void Populate()
        {
            RaiseListChangedEvents = false;
            IsReadOnly = false;
            AddRange(FetchLineageGroups());
            IsReadOnly = true;
            RaiseListChangedEvents = true;
        }

        private static ReadOnlyLineageGroup[] FetchLineageGroups()
        {
            return ConvertTableToLineageGroups(LineageGroupWorker.FetchLineageGroups());
        }

        private static ReadOnlyLineageGroup[] ConvertTableToLineageGroups(DataTable lineageGroupTable)
        {
            var lgs = new ReadOnlyLineageGroup[lineageGroupTable.Rows.Count];
            for (int rowIndex = 0; rowIndex < lineageGroupTable.Rows.Count; rowIndex++)
                lgs[rowIndex] = ReadOnlyLineageGroup.GetReadOnlyLineageGroup(lineageGroupTable.Rows[rowIndex]);
            return lgs;
        }


#endif

        #endregion

        public static void GetLineageGroups()
        {
            GetLineageGroups(null);
        }

        public static void GetLineageGroups(EventHandler<DataPortalResult<ReadOnlyLineageGroups>> handler)
        {
            //check cache first. if we have it just invoke the event handler
            if (CachedGroups != null)
            {
                if (handler != null)
                    handler.Invoke(null, new DataPortalResult<ReadOnlyLineageGroups>(CachedGroups, null, null));
                return;
            }
            DataPortal<ReadOnlyLineageGroups> dp = new DataPortal<ReadOnlyLineageGroups>();
            if (handler != null)
                dp.FetchCompleted += handler;
            dp.FetchCompleted += LineageGroups_FetchCompleted;
            dp.BeginFetch();
        }

        private static void LineageGroups_FetchCompleted(object sender, DataPortalResult<ReadOnlyLineageGroups> e)
        {
            CachedGroups = e.Object;
        }

        private static ReadOnlyLineageGroups _cachedGroups;
        public static ReadOnlyLineageGroups CachedGroups
        {
            get
            {
#if !SILVERLIGHT
                if (_cachedGroups == null)
                    _cachedGroups = GetLineageGroupsLocally();
#endif
                return _cachedGroups;
            }
            private set { _cachedGroups = value; }
        }

    }
}
