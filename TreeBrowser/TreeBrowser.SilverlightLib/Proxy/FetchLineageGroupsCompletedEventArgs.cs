using TreeBrowser.Client.Entities;

namespace TreeBrowser.SilverlightLib.Proxy
{
    public class FetchLineageGroupsCompletedEventArgs : System.EventArgs
    {

        public ReadOnlyLineageGroups LineageGroups { get; private set; }


        public FetchLineageGroupsCompletedEventArgs(ReadOnlyLineageGroups lgs)
        {
            LineageGroups = lgs;
        }

    }
}
