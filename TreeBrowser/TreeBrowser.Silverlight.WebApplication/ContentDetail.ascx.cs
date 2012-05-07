using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TreeBrowser.Entities;
using Csla.Web;
using TreeBrowser.Entities.Helpers;
using System.Web.UI.HtmlControls;
using TreeBrowser.Silverlight.WebApplication.Helpers;

namespace TreeBrowser.Silverlight.WebApplication
{
    public partial class ContentDetail : System.Web.UI.UserControl
    {

        public HtmlContentEnum ContentEnum { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            EnsureChildControls();
        }

        private HtmlContent _htmlContent;
        private HtmlContent HtmlContent
        {
            get
            {
                if (_htmlContent == null)
                    _htmlContent = HtmlContent.GetHtmlContentLocally(ContentEnum);
                return _htmlContent;
            }
        }

        protected void HtmlContentDataSource_SelectObject(object sender, SelectObjectArgs e)
        {
            e.BusinessObject = HtmlContent;
            InitHeader(HtmlContent);
        }

        private void InitHeader(HtmlContent content)
        {
            Page.Header.Title = MetaDataHelper.CreateContentPageTitle(content.HtmlContentEnum != HtmlContentEnum.Intro ? content.Name : "Home");
            Page.Header.Controls.Add(PageHelper.CreateDescriptionMetatag(content.Content));
            Page.Header.Controls.Add(PageHelper.CreateKeywordsMetatag(content.Name));
        }

    }
}