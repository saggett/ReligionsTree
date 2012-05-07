using System;
using Csla;
using TreeBrowser.Entities;

namespace TreeBrowser.SilverlightLib.Cache
{
    public class ReadOnlyLineagesCache
    {
        private bool fetchingLineages;

        public event EventHandler<DataPortalResult<ReadOnlyLineages>> FetchLineagesCompleted;

        public ReadOnlyLineagesCache()
        {
            
        }

        public void Prefetch()
        {
            if (fetchingLineages)
                return;
            if (!DataFetched)
            {
                fetchingLineages = true;
                ReadOnlyLineages.GetLineages(Client_FetchLineagesCompleted);
            }
            else
                FetchLineagesCompleted(this, new DataPortalResult<ReadOnlyLineages>(Lineages, null, null));
        }

        public ReadOnlyLineages Lineages
        {
            get;
            private set;
        }

        public bool DataFetched
        {
            get;
            private set;
        }

        private void Client_FetchLineagesCompleted(object sender, DataPortalResult<ReadOnlyLineages> e)
        {
            fetchingLineages = false;
            if (e.Error != null)
            {
                if (e.Error.InnerException != null)
                    throw e.Error.InnerException;
                else
                    throw e.Error;
            }
            DataFetched = true;
            Lineages = e.Object;
            if (FetchLineagesCompleted != null)
                FetchLineagesCompleted(this, e);
        }

        public void Invalidate()
        {
            Lineages = null;
            DataFetched = false;
        }

    }
}