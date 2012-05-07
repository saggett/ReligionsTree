using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
#if !SILVERLIGHT
using System.Data;
#endif
using TreeBrowser.Entities.Helpers;

namespace TreeBrowser.Entities
{

    [Serializable]
    public class ReadOnlyLineage : ReadOnlyBase<ReadOnlyLineage>
    {


        #region Fields

        public static PropertyInfo<int> IdProperty = RegisterProperty<int>(typeof(ReadOnlyLineage), new PropertyInfo<int>("Id", "Id", 0));
        public int Id
        {
            get
            {
                return GetProperty(IdProperty);
            }
        }

        public static PropertyInfo<string> NameProperty = RegisterProperty<string>(typeof(ReadOnlyLineage), new PropertyInfo<string>("Name", "Name", string.Empty));
        public string Name
        {
            get
            {
                return GetProperty(NameProperty);
            }
        }

        public static PropertyInfo<int?> ParentLineageIdProperty = RegisterProperty<int?>(typeof(ReadOnlyLineage), new PropertyInfo<int?>("ParentLineageId", "Parent Lineage Id", new int?()));
        public int? ParentLineageId
        {
            get
            {
                return GetProperty(ParentLineageIdProperty);
            }
        }

        public static PropertyInfo<int> StartYearProperty = RegisterProperty<int>(typeof(ReadOnlyLineage), new PropertyInfo<int>("StartYear", "Start Year", 0));
        public int StartYear
        {
            get
            {
                return GetProperty(StartYearProperty);
            }
        }

        public static PropertyInfo<int?> EndYearProperty = RegisterProperty<int?>(typeof(ReadOnlyLineage), new PropertyInfo<int?>("EndYear", "End Year", new int?()));
        public int? EndYear
        {
            get
            {
                return GetProperty(EndYearProperty);
            }
        }

        public static PropertyInfo<int?> LineageGroupIdProperty = RegisterProperty<int?>(typeof(ReadOnlyLineage), new PropertyInfo<int?>("LineageGroupId", "Lineage Group Id", new int?()));
        public int? LineageGroupId
        {
            get
            {
                return GetProperty(LineageGroupIdProperty);
            }
        }

        #endregion

        #region Properties


        public ReadOnlyLineage ParentLineage
        {
            get
            {
                return ParentLineageId.HasValue ? ParentList.LookupLineage(ParentLineageId.Value) : null;
            }
        }

        public string ParentName
        {
            get
            {
                return ParentLineage != null ? ParentLineage.Name : string.Empty;
            }
        }

        public IEnumerable<ReadOnlyLineage> DirectChildren
        {
            get
            {
                return ParentList.Where(lin => lin.ParentLineageId == Id);
            }
        }

        [NonSerialized, NotUndoable]
        private ReadOnlyLineages _Parent;

        internal ReadOnlyLineages ParentList
        {
            get { return _Parent; }
        }

        internal void SetParent(ReadOnlyLineages parent)
        {
            _Parent = parent;
        }

        public IList<ReadOnlyLineage> Children
        {
            get
            {
                var output = new List<ReadOnlyLineage>();
                foreach (ReadOnlyLineage lineage in DirectChildren)
                {
                    output.Add(lineage);
                    output.AddRange(lineage.Children);
                }
                return output;
            }
        }

        public int EffectiveMaxYearOfBranch
        {
            get
            {
                List<int> endYears = new List<int>();
                endYears.Add(DateTime.Today.Year);
                if (EndYear.HasValue)
                    endYears.Add(EndYear.Value);
                if (ChildCount > 0)
                {
                    int? maxYear = Children.Max(lin => lin.EndYear);
                    if (maxYear.HasValue)
                        endYears.Add(maxYear.Value);
                }
                return endYears.Max();
            }
        }

        public int ChildCount
        {
            get { return Children.Count; }
        }

        public bool HasChildren
        {
            get { return ParentList.Any(lin => lin.ParentLineageId == Id); }
        }


        internal int MaxYear
        {
            get
            {
                if (!EndYear.HasValue)
                    return StartYear;
                return Math.Max(StartYear,
                                EndYear.Value);
            }
        }

        public string DisplayStartYear
        {
            get { return StartYear.ConvertToDisplayYear(); }
        }

        public string DisplayEndYear
        {
            get { return EndYear.ConvertToDisplayYear(); }
        }

        public string LineageGroupName
        {
            get { return LineageGroupId.HasValue ? ReadOnlyLineageGroups.CachedGroups.LookupName(LineageGroupId.Value) : string.Empty; }
        }

        public string Description
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append(Name);
                if (LineageGroupId.HasValue)
                    builder.AppendFormat(", part of {0}. I", LineageGroupName);
                else
                    builder.Append(", i");
                builder.AppendFormat("ntroduced {0}", DisplayStartYear);
                if (EndYear.HasValue)
                    builder.AppendFormat(", ended {0}", DisplayEndYear);
                builder.Append(". ");
                return builder.ToString();
            }
        }

        #endregion

        #region Data Access


#if !SILVERLIGHT


        internal static ReadOnlyLineage GetLineage(DataRow row)
        {
            ReadOnlyLineage returnValue = new ReadOnlyLineage();
            returnValue.LoadProperties(row);
            return returnValue;
        }

        private void LoadProperties(DataRow lineageRow)
        {
            LoadProperty(IdProperty, (int)lineageRow["Id"]);
            LoadProperty(NameProperty, (string)lineageRow["Name"]);
            LoadProperty(StartYearProperty, (int)lineageRow["StartYear"]);
            LoadProperty(EndYearProperty, !lineageRow.IsNull("EndYear") ? (int)lineageRow["EndYear"] : new int?());
            LoadProperty(ParentLineageIdProperty,
                         !lineageRow.IsNull("ParentLineageId") ? (int)lineageRow["ParentLineageId"] : new int?());
            //LoadProperty(NotesProperty, (string)lineageRow["Notes"]);
            LoadProperty(LineageGroupIdProperty, !lineageRow.IsNull("LineageGroupId") ? (int)lineageRow["LineageGroupId"] : new int?());
        }

#endif

        #endregion

    }
}
