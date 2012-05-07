using System;

using Csla;
using Csla.Data;
using Csla.Serialization;
#if !SILVERLIGHT
using System.Data;
#endif

namespace TreeBrowser.Entities
{

    [Serializable]
    public class ReadOnlyLineageGroup : ReadOnlyBase<ReadOnlyLineageGroup>
    {


        public ReadOnlyLineageGroup() { }


        public static PropertyInfo<int> IdProperty = RegisterProperty<int>(typeof(ReadOnlyLineageGroup), new PropertyInfo<int>("Id", "Id", 0));
        public int Id
        {
            get
            {
                return GetProperty(IdProperty);
            }
        }

        public static PropertyInfo<string> NameProperty = RegisterProperty<string>(typeof(ReadOnlyLineageGroup), new PropertyInfo<string>("Name", "Name", string.Empty));
        public string Name
        {
            get
            {
                return GetProperty(NameProperty);
            }
        }

        #region Data Access

#if !SILVERLIGHT

        public static ReadOnlyLineageGroup GetReadOnlyLineageGroup(DataRow row)
        {
            return Csla.DataPortal.FetchChild<ReadOnlyLineageGroup>(row);
        }

        private void Child_Fetch(DataRow row)
        {
            LoadProperty<int>(IdProperty, (int)row["Id"]);
            LoadProperty<string>(NameProperty, (string)row["Name"]);
        }

#endif

        #endregion

    }
}