using System;
using System.Linq;
using System.ServiceModel;
using TreeBrowser.ServiceContract;

namespace TreeBrowser.SilverlightLib.Proxy
{
    /// <summary>
    /// This class is used to open a proxy only when needed
    /// </summary>
    /// <remarks>
    /// When using this code, you put a using block around any code that accesses the WCF service. This ensures that only one service proxy is opened for however many nested
    /// calls there are in the code. The outer service proxy used will open and close the proxy, all inner methods will just simply use the proxy and not have to open another.
    /// </remarks>
    public class ServiceProxyHolder : IDisposable
    {

        public static event EventHandler<EventArgs> BeforeCallBegin;
        public static event EventHandler<EventArgs> BeforeCallComplete;

        /// <summary>
        /// Flag indicating that we're the top of the chain so should close the proxy when disposed
        /// </summary>
        private bool _mustClose;

        /// <summary>
        /// This method opens a new proxy if necessary and returns a disposable class that ensures that the proxy is closed when necessary
        /// </summary>
        /// <returns></returns>
        public static IDisposable OpenProxy()
        {
            return new ServiceProxyHolder();
        }


        /// <summary>
        /// This needs to be thread static as we want to ensure that multiple threads get different proxies
        /// </summary>
        [ThreadStatic]
        private static ITreeService _proxy;

        /// <summary>
        /// Retrieve the current service proxy, if one is available
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if this is called without first calling the OpenProxy method</exception>
        public static ITreeService Proxy
        {
            get
            {
                if (null == _proxy)
                    throw new InvalidOperationException("There is no current ServiceProxy - you must surround calls in the ServiceProxyHolder.OpenProxy() method");
                return _proxy;
            }
        }

        public static string EndPointAddress
        {
            get
            {
                return ChannelFactory.Endpoint.Address.Uri.OriginalString;
            }
        }

        private static ChannelFactory<ITreeService> _channelFactory = null;
        private static ChannelFactory<ITreeService> ChannelFactory
        {
            get
            {
                if (null == _channelFactory)
                    _channelFactory = CreateFactory();
                return _channelFactory;
            }
        }

        private static ChannelFactory<ITreeService> CreateFactory()
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            return new ChannelFactory<ITreeService>(binding, new EndpointAddress(
                new Uri("http://localhost:51537/TreeBrowser.Service/Service.svc")));
        }

        private static string _endPointName;
        internal static string EndPointName
        {
            get { return _endPointName; }
            set
            {
                if (_endPointName == value)
                    return;
                if (_proxy != null)
                    throw new InvalidOperationException("Cannot switch endpoints while proxy is open. ");
                _channelFactory = null;
                _endPointName = value;
            }
        }

        /// <summary>
        /// Construct the proxy object and open it if necessary.
        /// </summary>
        private ServiceProxyHolder()
        {
            if (null == _proxy)
            {
                if (BeforeCallBegin != null)
                    BeforeCallBegin(this, new EventArgs());
                // Create a new proxy
                _proxy = ChannelFactory.CreateChannel();
                var channel = (IClientChannel)_proxy;
                channel.Open();
                _mustClose = true;
            }
            else
                _mustClose = false;
        }

        /// <summary>
        /// Ensure that the service proxy is closed if necessary
        /// </summary>
        public void Dispose()
        {
            if (_mustClose)
            {
                if (BeforeCallComplete != null)
                    BeforeCallComplete(this, new EventArgs());
                ITreeService temp = _proxy;
                _proxy = null;

                IClientChannel channel = temp as IClientChannel;
                if (null != channel)
                    CloseChannel(channel);
            }
        }

        static ServiceProxyHolder()
        {
            EndPointName = "basicHttpBinding_ITreeService";
        }

        private static void CloseChannel(IClientChannel channel)
        {
            try
            {
                channel.Close();
            }
            catch (TimeoutException)
            {
                channel.Abort();
            }
            catch (CommunicationException)
            {
                channel.Abort();
            }
            catch (Exception)
            {
                channel.Abort();
                throw;
            }
        }

    }
}