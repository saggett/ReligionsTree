using TreeBrowser.Client.Entities;

namespace TreeBrowser.SilverlightLib.Proxy
{
    public class FetchLineagesCompletedEventArgs : System.EventArgs
    {

        private Lineages _result;
        public Lineages Result { get { return _result; } }

        public FetchLineagesCompletedEventArgs(Lineages result)
        {
            _result = result;
        }

    }
}
