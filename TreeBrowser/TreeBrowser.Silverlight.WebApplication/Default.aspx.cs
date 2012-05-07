using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using TreeBrowser.Entities;
using TreeBrowser.Entities.Helpers;
using Csla.Web;

namespace TreeBrowser.Silverlight.WebApplication
{
    public partial class Default : System.Web.UI.Page
    {

        protected override void OnPreRenderComplete(EventArgs e)
        {
            base.OnPreRenderComplete(e);
            Csla.ApplicationContext.ContextManager = null;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                PageContent.Visible = false;
                ReligionContent.Visible = QueryHelper.SelectedContentEnum == HtmlContentEnum.NotSpecified;
                //TODO: Bibliography link should not be displayed when bibliography page is loaded, etc.
                if (QueryHelper.SelectedContentEnum != HtmlContentEnum.NotSpecified)
                {
                    PageContent.ContentEnum = QueryHelper.SelectedContentEnum;
                    PageContent.Visible = true;
                }
                else if (QueryHelper.SelectedLineageId == null)
                {
                    PageContent.ContentEnum = HtmlContentEnum.Intro;
                    PageContent.Visible = true;
                }

            }
        }


    }
}
