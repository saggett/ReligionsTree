using TreeBrowser.Entities;

namespace TreeBrowser.SilverlightLib.EventArgs
{
    public class LineageEventArgs : System.EventArgs
    {
        public Lineage Lineage
        {
            get;
            private set;
        }

        public LineageEventArgs(Lineage lineage)
        {
            Lineage = lineage;
        }

    }
}