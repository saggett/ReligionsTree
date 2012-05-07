using System;
using System.ServiceModel;
using TreeBrowser.ServiceContract;

namespace TreeBrowser.SilverlightLib.Proxy
{
    public class WCFServiceManager
    {

        private const string EndPointName = "WSHttpBinding_ITreeService";

        private string _endPointAddress;

        public event EventHandler<EventArgs> BeforeCallBegin;
        public event EventHandler<EventArgs> BeforeCallComplete;


        public WCFServiceManager()
        {

        }

        public string EndPointAddress
        {
            get
            {
                if (_endPointAddress == null)
                {
                    var fact = new ChannelFactory<ITreeService>(EndPointName);
                    _endPointAddress = fact.Endpoint.Address.Uri.OriginalString;
                }
                return _endPointAddress;
            }
        }

        public ITreeService CreateProxy()
        {
            return BeginCall();
        }

        public ITreeService BeginCall()
        {
            if (BeforeCallBegin != null)
                BeforeCallBegin(this, new EventArgs());
            return ServiceProxyHolder.Proxy;
        }

        public void CompleteCall(ITreeService endPoint)
        {
            if (BeforeCallComplete != null)
                BeforeCallComplete(this, new EventArgs());
        }

    }
}