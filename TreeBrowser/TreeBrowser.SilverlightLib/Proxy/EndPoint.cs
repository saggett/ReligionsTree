using System;

namespace TreeBrowser.SilverlightLib.Proxy
{
    public static class EndPoint
    {

        public static event EventHandler<EventArgs> BeforeCallBegin;
        public static event EventHandler<EventArgs> BeforeCallComplete;


        private static void ServiceManager_BeforeCallComplete(object sender, EventArgs e)
        {
            if (BeforeCallComplete != null)
                BeforeCallComplete(sender, e);
        }

        private static void ServiceManager_BeforeCallBegin(object sender, EventArgs e)
        {
            if (BeforeCallBegin != null)
                BeforeCallBegin(sender, e);
        }

        private static ContractProxy proxy;
        public static ContractProxy Proxy
        {
            get
            {
                if (proxy == null)
                    proxy = new ContractProxy();
                return proxy;
            }
        }

        public static string EndPointName
        {
            get { return ServiceProxyHolder.EndPointName; }
            set
            {
                if (value == EndPointName)
                    return;
                ServiceProxyHolder.EndPointName = value;
                proxy = null;
            }
        }
        
        public static string Address
        {
            get { return ServiceProxyHolder.EndPointAddress; }
        }

        static EndPoint()
        {
            ServiceProxyHolder.BeforeCallBegin += ServiceManager_BeforeCallBegin;
            ServiceProxyHolder.BeforeCallComplete += ServiceManager_BeforeCallComplete;
        }

    }
}