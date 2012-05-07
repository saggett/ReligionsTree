using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
#if SILVERLIGHT
using System.Windows;
#endif
using Csla;
using Csla.Core;
using Csla.Serialization;

#if !SILVERLIGHT
using System.Data;
using TreeBrowser.DataPortal;
#endif
using TreeBrowser.Entities.Helpers;


namespace TreeBrowser.Entities
{

    [Serializable]
    public class Lineage : BusinessBase<Lineage>
    {


        #region Constructor

        public Lineage()
        {
        }


        #endregion

        #region Factory Methods
        
#if !SILVERLIGHT

        public static Lineage GetLineageLocally(int lineageId)
        {
            var lin = new Lineage();
            lin.PopulateLineage(lineageId);
            lin.BusinessRules.CheckObjectRules();
            return lin;
        }

#endif

        public static void GetLineage(int lineageId, EventHandler<DataPortalResult<Lineage>> handler)
        {
            DataPortal<Lineage> dp = new DataPortal<Lineage>();
            dp.FetchCompleted += handler;
            dp.BeginFetch(new SingleCriteria<Lineage, int>(lineageId));
        }

        public static Lineage NewLineage(int parentLineageId, int startYear)
        {
            Lineage returnValue = new Lineage();
            returnValue.LoadProperty(ParentLineageIdProperty, parentLineageId);
            returnValue.LoadProperty(StartYearProperty, startYear);
            returnValue.LoadProperty(HyperlinksProperty, LineageHyperlinks.NewLineageHyperlinks());
            returnValue.BusinessRules.CheckObjectRules();
            return returnValue;
        }

        #endregion

        #region Fields

        public static PropertyInfo<int> IdProperty = RegisterProperty<int>(typeof(Lineage), new PropertyInfo<int>("Id", "Id", 0));
        public int Id
        {
            get
            {
                return GetProperty(IdProperty);
            }
            set
            {
                SetProperty(IdProperty, value);
            }
        }

        public static PropertyInfo<string> NameProperty = RegisterProperty<string>(typeof(Lineage), new PropertyInfo<string>("Name", "Name", string.Empty));
        public string Name
        {
            get
            {
                return GetProperty(NameProperty);
            }
            set
            {
                SetProperty(NameProperty, value);
            }
        }

        public static PropertyInfo<int?> ParentLineageIdProperty = RegisterProperty<int?>(typeof(Lineage), new PropertyInfo<int?>("ParentLineageId", "Parent Lineage Id", new int?()));
        public int? ParentLineageId
        {
            get
            {
                return GetProperty(ParentLineageIdProperty);
            }
            set
            {
                SetProperty(ParentLineageIdProperty, value);
            }
        }

        public static PropertyInfo<int> StartYearProperty = RegisterProperty<int>(typeof(Lineage), new PropertyInfo<int>("StartYear", "Start Year", 0));
        public int StartYear
        {
            get
            {
                return GetProperty(StartYearProperty);
            }
            set
            {
                SetProperty(StartYearProperty, value);
            }
        }

        public static PropertyInfo<int?> EndYearProperty = RegisterProperty<int?>(typeof(Lineage), new PropertyInfo<int?>("EndYear", "End Year", new int?()));
        public int? EndYear
        {
            get
            {
                return GetProperty(EndYearProperty);
            }
            set
            {
                SetProperty(EndYearProperty, value);
            }
        }

        public static PropertyInfo<string> NotesProperty = RegisterProperty<string>(typeof(Lineage), new PropertyInfo<string>("Notes", "Notes", string.Empty));
        public string Notes
        {
            get
            {
                return GetProperty(NotesProperty);
            }
            set
            {
                SetProperty(NotesProperty, value);
            }
        }

        public static PropertyInfo<int?> LineageGroupIdProperty = RegisterProperty<int?>(typeof(Lineage), new PropertyInfo<int?>("LineageGroupId", "Lineage Group Id", new int?()));
        public int? LineageGroupId
        {
            get
            {
                return GetProperty(LineageGroupIdProperty);
            }
            set
            {
                SetProperty(LineageGroupIdProperty, value);
            }
        }

        public static PropertyInfo<bool> HasChildrenProperty = RegisterProperty<bool>(typeof(Lineage), new PropertyInfo<bool>("HasChildren", "Has Children"));
        public bool HasChildren
        {
            get
            {
                return GetProperty(HasChildrenProperty);
            }
        }

        #endregion

        #region Properties

        public static PropertyInfo<LineageHyperlinks> HyperlinksProperty = RegisterProperty<LineageHyperlinks>(typeof(Lineage), new PropertyInfo<LineageHyperlinks>("LineageHyperlinks", "Hyperlinks"));
        public LineageHyperlinks Hyperlinks
        {
            get
            {
                return GetProperty(HyperlinksProperty);
            }
        }

#if SILVERLIGHT

        public Visibility ExternalHyperlinksVisibility
        {
            get { return Hyperlinks.Any() ? Visibility.Visible : Visibility.Collapsed; }
        }

#endif

        #endregion

        #region Methods


        protected override void AddBusinessRules()
        {
            Csla.Rules.BusinessRules.AddRule(typeof(Lineage), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, "Admin"));
            Csla.Rules.BusinessRules.AddRule(typeof(Lineage), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, "Admin"));
            Csla.Rules.BusinessRules.AddRule(typeof(Lineage), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, "Admin"));
        }


        //public static void Create(EventHandler<DataPortalResult<Lineage>> handler, int parentLineageId, int startYear)
        //{
        //    //TODO: should be local only portal
        //    DataPortal<Lineage> dp = new DataPortal<Lineage>();
        //    dp.CreateCompleted += handler;
        //    dp.BeginCreate(new LineageCreateCriteria(parentLineageId, startYear));
        //}

        #endregion

        #region Authorization


        public bool CanDelete
        {
            get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, typeof(Lineage)) && !HasChildren; }
        }

        public bool CanEdit
        {
            get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(Lineage)); }
        }

        public bool CanCreate
        {
            get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(Lineage)); }
        }

        #endregion


        #region Data Access

#if !SILVERLIGHT

        protected void DataPortal_Fetch(SingleCriteria<Lineage, int> criteria)
        {
            PopulateLineage(criteria.Value);
        }

        private void PopulateLineage(int lineageId)
        {
            var ds = TreeBrowser.DataPortal.BranchWorker.FetchLineage(lineageId);
            LoadProperties(ds.Tables[0].Rows[0]);
            LoadProperty(HyperlinksProperty, LineageHyperlinks.GetLineageHyperlinks(ds.Tables[1]));
        }

        //SA TODO: Start using single transaction or get this working
        //[Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_DeleteSelf()
        {
            DataPortal.BranchWorker.DeleteLineage(Id, ReadProperty(HyperlinksProperty).Select(link => link.Id));
        }

        //SA TODO: Start using single transaction or get this working
        //[Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Insert()
        {
            InsertUpdate(true);
        }

        //SA TODO: Start using single transaction or get this working
        //[Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Update()
        {
            InsertUpdate(false);
        }

        private void InsertUpdate(bool isInsert)
        {
            int newId;
            using (var connection = DbHelper.OpenConnection())
            {
                var trans = connection.BeginTransaction();
                try
                {
                    newId = DataPortal.BranchWorker.SaveLineage(Id, Name, StartYear, EndYear, ParentLineageId, Notes,
                                                                LineageGroupId, trans);
                    Csla.DataPortal.UpdateChild(ReadProperty(HyperlinksProperty), this, trans);
                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
            if (isInsert)
                LoadProperty(IdProperty, newId);
        }

        internal static Lineage GetLineage(DataRow row)
        {
            Lineage returnValue = new Lineage();
            returnValue.LoadProperties(row);
            returnValue.MarkOld();
            return returnValue;
        }

        private void LoadProperties(DataRow lineageRow)
        {
            LoadProperty(IdProperty, (int)lineageRow["Id"]);
            LoadProperty(NameProperty, (string) lineageRow["Name"]);
            LoadProperty(StartYearProperty, (int) lineageRow["StartYear"]);
            LoadProperty(EndYearProperty, !lineageRow.IsNull("EndYear") ? (int) lineageRow["EndYear"] : new int?());
            LoadProperty(ParentLineageIdProperty,
                         !lineageRow.IsNull("ParentLineageId") ? (int) lineageRow["ParentLineageId"] : new int?());
            LoadProperty(NotesProperty, (string) lineageRow["Notes"]);
            LoadProperty(LineageGroupIdProperty, !lineageRow.IsNull("LineageGroupId") ? (int) lineageRow["LineageGroupId"] : new int?());
            LoadProperty(HasChildrenProperty, Convert.ToBoolean(lineageRow["HasChildren"]));
        }

#endif

        #endregion

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

        public string FoundingText
        {
            get
            {
                var builder = new StringBuilder();
                if (LineageGroupId.HasValue)
                    builder.AppendFormat("Part of {0}. ", LineageGroupName);
                builder.AppendFormat("Introduced {0}", DisplayStartYear);
                if (EndYear.HasValue)
                    builder.AppendFormat(", ended {0}", DisplayEndYear);
                builder.Append(". ");
                return builder.ToString();
            }
        }

    }

}
