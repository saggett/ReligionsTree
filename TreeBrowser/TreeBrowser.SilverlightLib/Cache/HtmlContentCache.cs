using System;
using Csla;
using TreeBrowser.Entities;
using System.Collections.Generic;

namespace TreeBrowser.SilverlightLib.Cache
{
    public class HtmlContentCache
    {

        private Dictionary<HtmlContentEnum, bool> _fetchingContentLookup = CreateFetchingLookup();
        private Dictionary<HtmlContentEnum, bool> IsFetchingHtmlContent { get { return _fetchingContentLookup; } }

        public event EventHandler<DataPortalResult<HtmlContent>> FetchHtmlContentCompleted;

        private HtmlContentCache()
        {

        }


        private static HtmlContentCache _htmlContentCache;
        public static HtmlContentCache Default
        {
            get
            {
                if (_htmlContentCache == null)
                {
                    _htmlContentCache = new HtmlContentCache();
                }
                return _htmlContentCache;
            }
        }


        private static Dictionary<HtmlContentEnum, bool> CreateFetchingLookup()
        {
            var dict = new Dictionary<HtmlContentEnum, bool>();
            foreach (HtmlContentEnum hce in new[] { HtmlContentEnum.About, HtmlContentEnum.Bibliography, HtmlContentEnum.Intro })
                dict.Add(hce, false);
            return dict;
        }

        public void PrefetchAll()
        {
            foreach (HtmlContentEnum hce in new[] { HtmlContentEnum.About, HtmlContentEnum.Bibliography, HtmlContentEnum.Intro })
                BeginFetchOfContent(hce);
        }

        public void BeginFetchOfContent(HtmlContentEnum htmlContentEnum)
        {
            if (IsFetchingHtmlContent[htmlContentEnum])
                return;
            if (HtmlContentLookup.ContainsKey(htmlContentEnum))
                FetchHtmlContentCompleted(this, new DataPortalResult<HtmlContent>(HtmlContentLookup[htmlContentEnum], null, htmlContentEnum));
            IsFetchingHtmlContent[htmlContentEnum] = true;
            HtmlContent.GetHtmlContent(htmlContentEnum, Client_FetchHtmlContentCompleted);
        }

        private readonly Dictionary<HtmlContentEnum, HtmlContent> _HtmlContentLookup = new Dictionary<HtmlContentEnum, HtmlContent>();
        public Dictionary<HtmlContentEnum, HtmlContent> HtmlContentLookup
        {
            get { return _HtmlContentLookup; }
        }

        private void Client_FetchHtmlContentCompleted(object sender, DataPortalResult<HtmlContent> e)
        {
            IsFetchingHtmlContent[(HtmlContentEnum)e.UserState] = false;
            if (e.Error != null)
            {
#if DEBUG
                if (e.Error.InnerException != null)
                    throw e.Error.InnerException;
                else
                    throw e.Error;
#else
                return;
#endif
            }
            if (!HtmlContentLookup.ContainsKey(e.Object.HtmlContentEnum))
                HtmlContentLookup.Add(e.Object.HtmlContentEnum, e.Object);
            else
                HtmlContentLookup[e.Object.HtmlContentEnum] = e.Object;
            if (FetchHtmlContentCompleted != null)
                FetchHtmlContentCompleted(this, e);
        }

        public void Invalidate(HtmlContentEnum htmlContentEnum)
        {
            HtmlContentLookup.Clear();
        }

    }
}
