using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using TreeBrowser.Entities;
using TreeBrowser.Entities.Helpers;
using Csla.Web;
using TreeBrowser.Silverlight.WebApplication.Helpers;

namespace TreeBrowser.Silverlight.WebApplication
{
    public partial class ReligionDetail : System.Web.UI.UserControl
    {

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private int CurrentRootId
        {
            get { return QueryHelper.SelectedLineageId.HasValue ? QueryHelper.SelectedLineageId.Value : ChildLineages.GetRoot().Id; }
        }



        private ReadOnlyLineages _childLineages;
        private ReadOnlyLineages ChildLineages
        {
            get
            {
                if (_childLineages == null)
                    _childLineages = ReadOnlyLineages.GetLineagesLocally(QueryHelper.SelectedLineageId);
                return _childLineages;
            }
        }

        protected void ReadOnlyLineagesDataSource_SelectObject(object sender, SelectObjectArgs e)
        {
            ReadOnlyLineages linsWithoutRoot = ChildLineages.Clone();
            var root = linsWithoutRoot.Single(l => l.Id == CurrentRootId);
            linsWithoutRoot.Remove(root);
            e.BusinessObject = linsWithoutRoot;
            ChildReligionsLabel.Visible = linsWithoutRoot.Any();
        }

        protected void RootLineageDataSource_SelectObject(object sender, SelectObjectArgs e)
        {
            Lineage lineage = Lineage.GetLineageLocally(CurrentRootId);
            e.BusinessObject = lineage;
            InitHeader(lineage);
        }

        private void InitHeader(Lineage root)
        {
            //if we're on the landing page don't do the animism info
            if (QueryHelper.SelectedLineageId == null)
                return;
            Page.Header.Title = MetaDataHelper.CreateTitle(root.Name, root.ParentLineageId.HasValue);
            Page.Header.Controls.Add(PageHelper.CreateDescriptionMetatag(root.FoundingText + root.Notes));
            Page.Header.Controls.Add(PageHelper.CreateKeywordsMetatag(root.Name, root.LineageGroupName));
        }


    }
}