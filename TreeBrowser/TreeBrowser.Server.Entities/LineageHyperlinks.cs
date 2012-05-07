using System;
using System.Collections.Generic;
#if !SILVERLIGHT
using System.Data;
#endif
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace TreeBrowser.Entities
{
    [Serializable]
    public class LineageHyperlinks : BusinessListBase<LineageHyperlinks, LineageHyperlink>
    {

#if !SILVERLIGHT
        private LineageHyperlinks() { MarkAsChild(); }
        protected override LineageHyperlink AddNewCore()
        {
            LineageHyperlink newLink = LineageHyperlink.NewLineageHyperlink();
            Add(newLink);
            return newLink;
        }
#else
        public LineageHyperlinks() { MarkAsChild(); }
        protected override void AddNewCore()
        {
            LineageHyperlink newLink = LineageHyperlink.NewLineageHyperlink();
            Add(newLink);
        }
#endif


        internal static LineageHyperlinks NewLineageHyperlinks()
        {
            return new LineageHyperlinks();
        }

#if !SILVERLIGHT
        internal static LineageHyperlinks GetLineageHyperlinks(DataTable table)
        {
            return Csla.DataPortal.FetchChild<LineageHyperlinks>(table);
        }

        private void Child_Fetch(DataTable table)
        {
            this.RaiseListChangedEvents = false;
            foreach (DataRow row in table.Rows)
                Add(LineageHyperlink.GetLineageHyperlink(row));
            this.RaiseListChangedEvents = true;
        }

#endif

    }
}
