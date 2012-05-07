using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace TreeBrowser.SilverlightLib.Proxy
{
    public class Lineages : ObservableCollection<Lineage>
    {

        public Lineage LookupLineage(int lineageId)
        {
            return this.Single(l => l.Id == lineageId);
        }

        public bool ContainsLineageWithId(int lineageId)
        {
            return this.Any(l => l.Id == lineageId);
        }

        public Lineage GetRoot()
        {
            return this.SingleOrDefault(li => li.ParentLineageId == null);
        }

        public void RemoveWithId(int lineageId)
        {
            Remove(LookupLineage(lineageId));
        }

        public bool IsLineageAChildOfLineageB(int lineageAId, int lineageBId)
        {
            return LookupLineage(lineageBId).Children.Any(li => li.Id == lineageAId);
        }

        public double MaxYear
        {
            get
            {
                return this.Max(li => Math.Max(li.StartYear, li.EndYear.HasValue ? li.EndYear.Value : 0));
            }
        }

    }

}
