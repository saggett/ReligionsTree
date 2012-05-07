using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml;
using TreeBrowser.Entities;

namespace TreeBrowser.Silverlight.WebApplication
{
    public partial class Sitemap : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "text/xml";
            WriteXml();
            Response.End();
        }

        private void WriteXml()
        {
            using (var writer = new XmlTextWriter(Response.OutputStream, Encoding.UTF8))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("urlset");
                writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

                WriteSitemapXmlForPage(writer, new Uri(Request.Url, "Default.aspx?contentName=About"), "0.5");
                WriteSitemapXmlForPage(writer, new Uri(Request.Url, "Default.aspx?contentName=Bibliography"), "0.5");
                foreach (var lineage in ReadOnlyLineages.GetLineagesLocally(null))
                    WriteSitemapXmlForLineage(writer, lineage);

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }
        }

        private void WriteSitemapXmlForLineage(XmlTextWriter writer, ReadOnlyLineage lineage)
        {
            var linAddress = new Uri(Request.Url,
                                     "Default.aspx?lineageId=" +
                                     HttpUtility.UrlEncode(lineage.Id.ToString()));
            WriteSitemapXmlForPage(writer, linAddress, "1.0");
        }

        private static void WriteSitemapXmlForPage(XmlTextWriter writer, Uri address, string priority)
        {
            writer.WriteStartElement("url");
            writer.WriteElementString("loc", address.ToString());
            //writer.WriteElementString("lastmod", String.Format("{0:yyyy-MM-dd}", rdr[0]));
            //writer.WriteElementString("changefreq", "weekly");
            writer.WriteElementString("priority", priority);
            writer.WriteEndElement();
        }

    }
}