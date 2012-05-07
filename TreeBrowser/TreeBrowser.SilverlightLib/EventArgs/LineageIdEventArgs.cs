namespace TreeBrowser.SilverlightLib.EventArgs
{
    public class LineageIdEventArgs : System.EventArgs
    {

        public int? LineageId
        {
            get;
            private set;
        }

        public LineageIdEventArgs(int? lineageId)
        {
            LineageId = lineageId;
        }

    }
}
