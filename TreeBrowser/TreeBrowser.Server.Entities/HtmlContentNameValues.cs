using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
#if !SILVERLIGHT
using TreeBrowser.DataPortal;
using Microsoft.Practices.EnterpriseLibrary.Data;
#endif

namespace TreeBrowser.Entities
{

    [Serializable]
    public class HtmlContentNameValues : Csla.NameValueListBase<HtmlContentEnum, string>
    {

#if !SILVERLIGHT

        private static HtmlContentNameValues GetHtmlContentNameValuesLocally()
        {
            var groups = new HtmlContentNameValues();
            groups.DataPortal_Fetch();
            return groups;
        }

        private void DataPortal_Fetch()
        {
            this.RaiseListChangedEvents = false;
            IsReadOnly = false;
            Database db = DbHelper.CreateDB();
            using (var reader = db.ExecuteReader(System.Data.CommandType.Text, "SELECT Id, Name FROM HtmlContent"))
            {
                while (reader.Read())
                {
                    Add(new NameValuePair((HtmlContentEnum)reader["Id"], (string)reader["Name"]));
                }
                reader.Close();
            }
            IsReadOnly = true;
            this.RaiseListChangedEvents = true;
        }



#endif

        public static void GetHtmlContentNameValues()
        {
            GetHtmlContentNameValues(null);
        }

        public static void GetHtmlContentNameValues(EventHandler<DataPortalResult<HtmlContentNameValues>> handler)
        {
            //check cache first. if we have it just invoke the event handler
            if (CachedNVs != null)
            {
                if (handler != null)
                    handler.Invoke(null, new DataPortalResult<HtmlContentNameValues>(CachedNVs, null, null));
                return;
            }
            DataPortal<HtmlContentNameValues> dp = new DataPortal<HtmlContentNameValues>();
            if (handler != null)
                dp.FetchCompleted += handler;
            dp.FetchCompleted += HtmlContentNameValues_FetchCompleted;
            dp.BeginFetch();
        }

        private static void HtmlContentNameValues_FetchCompleted(object sender, DataPortalResult<HtmlContentNameValues> e)
        {
            CachedNVs = e.Object;
        }

        private static HtmlContentNameValues _cachedNVs;
        public static HtmlContentNameValues CachedNVs
        {
            get
            {
#if !SILVERLIGHT
                if (_cachedNVs == null)
                    _cachedNVs = GetHtmlContentNameValuesLocally();
#endif
                return _cachedNVs;
            }
            private set { _cachedNVs = value; }
        }

    }
}
