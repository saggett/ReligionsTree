using System;
using System.Collections.Generic;
#if !SILVERLIGHT
    using System.Data;
    using System.Data.Common;
using System.Data.SqlClient;
#endif

using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace TreeBrowser.Entities
{

    [Serializable]
    public class LineageHyperlink : BusinessBase<LineageHyperlink>
    {

        public LineageHyperlink()
        {
            MarkAsChild();
        }


        public static LineageHyperlink NewLineageHyperlink()
        {
            var link = new LineageHyperlink();
            link.BusinessRules.CheckRules();
            return link;
        }


        #region Fields

        public static PropertyInfo<int> IdProperty = RegisterProperty<int>(typeof(LineageHyperlink), new PropertyInfo<int>("Id", "Id"));
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

        public static PropertyInfo<int> LineageIdProperty = RegisterProperty<int>(typeof(LineageHyperlink), new PropertyInfo<int>("LineageId", "Lineage Id"));
        public int LineageId
        {
            get
            {
                return GetProperty(LineageIdProperty);
            }
            set
            {
                SetProperty(LineageIdProperty, value);
            }
        }

        public static PropertyInfo<string> CaptionProperty = RegisterProperty<string>(typeof(LineageHyperlink), new PropertyInfo<string>("Caption", "Caption", string.Empty));
        public string Caption
        {
            get
            {
                return GetProperty(CaptionProperty);
            }
            set
            {
                SetProperty(CaptionProperty, value);
            }
        }

        public static PropertyInfo<string> DescriptionProperty = RegisterProperty<string>(typeof(LineageHyperlink), new PropertyInfo<string>("Description", "Description", string.Empty));
        public string Description
        {
            get
            {
                return GetProperty(DescriptionProperty);
            }
            set
            {
                SetProperty(DescriptionProperty, value);
            }
        }

        public static PropertyInfo<string> UrlProperty = RegisterProperty<string>(typeof(LineageHyperlink), new PropertyInfo<string>("Url", "URL", string.Empty));
        public string Url
        {
            get
            {
                return GetProperty(UrlProperty);
            }
            set
            {
                SetProperty(UrlProperty, value);
            }
        }

#endregion


        protected override void AddBusinessRules()
        {
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(CaptionProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(UrlProperty));
            Csla.Rules.BusinessRules.AddRule(typeof(LineageHyperlink), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, "Admin"));
            Csla.Rules.BusinessRules.AddRule(typeof(LineageHyperlink), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, "Admin"));
            Csla.Rules.BusinessRules.AddRule(typeof(LineageHyperlink), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, "Admin"));
        }


#if !SILVERLIGHT

    internal static LineageHyperlink GetLineageHyperlink(DataRow linkRow)
    {
        return Csla.DataPortal.FetchChild<LineageHyperlink>(linkRow);
    }

    private void Child_Fetch(DataRow linkRow)
    {
        LoadProperty<int>(IdProperty, (int)linkRow["Id"]);
        LoadProperty<int>(LineageIdProperty, (int)linkRow["LineageId"]);
        LoadProperty<string>(CaptionProperty, linkRow["Caption"].ToString());
        LoadProperty<string>(DescriptionProperty, linkRow["Description"].ToString());
        LoadProperty<string>(UrlProperty, linkRow["Url"].ToString());
    }


    private void Child_Insert(Lineage parent, DbTransaction transaction)
    {
        LoadProperty<int>(IdProperty,
                          InsertUpdate(parent, transaction));
    }

    private int InsertUpdate(Lineage parent, DbTransaction transaction)
    {
        return TreeBrowser.DataPortal.LineageHyperlinkWorker.SaveLineageHyperlink(transaction,
                                                                                  ReadProperty(IdProperty),
                                                                                  parent.Id,
                                                                                  ReadProperty(
                                                                                      CaptionProperty),
                                                                                  ReadProperty(
                                                                                      DescriptionProperty),
                                                                                  ReadProperty(UrlProperty));
    }


    private void Child_Update(Lineage parent, DbTransaction transaction)
    {
        InsertUpdate(parent, transaction);
    }

    private void Child_DeleteSelf(Lineage parent, DbTransaction transaction)
    {
        TreeBrowser.DataPortal.LineageHyperlinkWorker.DeleteLineageHyperlink(transaction, ReadProperty(IdProperty));
    }

#endif

    }
}
