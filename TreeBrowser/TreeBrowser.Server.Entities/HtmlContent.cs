using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
#if !SILVERLIGHT
using TreeBrowser.DataPortal;
using System.Data;
#endif

namespace TreeBrowser.Entities
{

    [Serializable]
    public class HtmlContent : BusinessBase<HtmlContent>
    {

        

        #region Constructor

        public HtmlContent()
        {
        }


        #endregion

        #region Factory Methods
        
#if !SILVERLIGHT

        public static HtmlContent GetHtmlContentLocally(HtmlContentEnum htmlContentEnum)
        {
            var lin = new HtmlContent();
            lin.PopulateHtmlContent((int)htmlContentEnum);
            lin.BusinessRules.CheckObjectRules();
            return lin;
        }

#endif

        public static void GetHtmlContent(HtmlContentEnum htmlContentEnum, EventHandler<DataPortalResult<HtmlContent>> handler)
        {
            DataPortal<HtmlContent> dp = new DataPortal<HtmlContent>();
            dp.FetchCompleted += handler;
            dp.BeginFetch(new SingleCriteria<HtmlContent, int>((int)htmlContentEnum), htmlContentEnum);
        }

        public static HtmlContent NewHtmlContent()
        {
            HtmlContent returnValue = new HtmlContent();
            returnValue.BusinessRules.CheckObjectRules();
            return returnValue;
        }

        #endregion

        #region Fields

        public static PropertyInfo<int> IdProperty = RegisterProperty<int>(typeof(HtmlContent), new PropertyInfo<int>("Id", "Id", 0));
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

        public static PropertyInfo<string> NameProperty = RegisterProperty<string>(typeof(HtmlContent), new PropertyInfo<string>("Name", "Name", string.Empty));
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

        public static PropertyInfo<string> ContentProperty = RegisterProperty<string>(typeof(HtmlContent), new PropertyInfo<string>("Content", "Content", string.Empty));
        public string Content
        {
            get
            {
                return GetProperty(ContentProperty);
            }
            set
            {
                SetProperty(ContentProperty, value);
            }
        }

        #endregion

        public HtmlContentEnum HtmlContentEnum
        {
            get { return (HtmlContentEnum)Id; }
        }

        #region Methods

        public static HtmlContentEnum GetHtmlContentEnum(string contentName)
        {
            //TODO: Replace with ReadOnlyHtmlContentNameValues lookup
            switch (contentName)
            {
                case "About":
                    return HtmlContentEnum.About;
                case "Bibliography":
                    return HtmlContentEnum.Bibliography;
                case "Intro":
                    return HtmlContentEnum.Intro;
                default:
                    throw new NotSupportedException("Content name not recognized.");
            }
        }

        protected override void AddBusinessRules()
        {
            Csla.Rules.BusinessRules.AddRule(typeof(HtmlContent), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, "Admin"));
            Csla.Rules.BusinessRules.AddRule(typeof(HtmlContent), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, "Admin"));
            Csla.Rules.BusinessRules.AddRule(typeof(HtmlContent), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, "Admin"));
        }


        //public static void Create(EventHandler<DataPortalResult<HtmlContent>> handler, int parentHtmlContentId, int startYear)
        //{
        //    //TODO: should be local only portal
        //    DataPortal<HtmlContent> dp = new DataPortal<HtmlContent>();
        //    dp.CreateCompleted += handler;
        //    dp.BeginCreate(new HtmlContentCreateCriteria(parentHtmlContentId, startYear));
        //}

        #endregion

        #region Authorization


        public bool CanDelete
        {
            get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, typeof(HtmlContent)); }
        }

        public bool CanEdit
        {
            get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(HtmlContent)); }
        }

        public bool CanCreate
        {
            get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(HtmlContent)); }
        }

        #endregion


        #region Data Access

#if !SILVERLIGHT

        protected void DataPortal_Fetch(SingleCriteria<HtmlContent, int> criteria)
        {
            PopulateHtmlContent(criteria.Value);
        }

        private void PopulateHtmlContent(int htmlContentId)
        {
            var ds = TreeBrowser.DataPortal.HtmlContentWorker.FetchHtmlContent(htmlContentId);
            LoadProperties(ds.Tables[0].Rows[0]);
        }


        protected override void DataPortal_DeleteSelf()
        {
            throw new NotImplementedException();
        }

        protected override void DataPortal_Insert()
        {
            InsertUpdate(true);
        }

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
                    newId = DataPortal.HtmlContentWorker.SaveHtmlContent(Id, Name, Content, trans);
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

        internal static HtmlContent GetHtmlContent(DataRow row)
        {
            HtmlContent returnValue = new HtmlContent();
            returnValue.LoadProperties(row);
            returnValue.MarkOld();
            return returnValue;
        }

        private void LoadProperties(DataRow htmlContentRow)
        {
            LoadProperty(IdProperty, (int)htmlContentRow["Id"]);
            LoadProperty(NameProperty, (string) htmlContentRow["Name"]);
            LoadProperty(ContentProperty, (string) htmlContentRow["Content"]);
        }

#endif

        #endregion

    }
}
