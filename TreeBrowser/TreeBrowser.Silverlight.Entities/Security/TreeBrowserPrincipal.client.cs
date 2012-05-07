using System;
using System.Security.Principal;
using Csla;
using Csla.Security;

namespace TreeBrowser.Entities.Security
{
    public partial class TreeBrowserPrincipal : CslaPrincipal
    {

        public static event EventHandler<LoginCompletedEventArgs> LoginCompleted;

        public static void Login(string username, string password)
        {
            MembershipIdentity.GetMembershipIdentity<TreeBrowserIdentity>(username, password, 
                (o, e) =>
                OnGetIdentityComplete(e, (EventHandler<DataPortalResult<TreeBrowserPrincipal>>)null));
        }

        public static void Logoff()
        {
            SetPrincipal(CslaIdentity.UnauthenticatedIdentity());
        }

        private static void OnGetIdentityComplete<T>(DataPortalResult<T> e,
                                                     EventHandler<DataPortalResult<TreeBrowserPrincipal>> completed)
            where T : IIdentity
        {
            if (e.Error == null)
                SetPrincipal(e.Object);
            else
            {
                SetPrincipal(CslaIdentity.UnauthenticatedIdentity());
                throw e.Error;
            }
            if (LoginCompleted != null)
                LoginCompleted((object)e.Object,
                               new LoginCompletedEventArgs(ApplicationContext.User.Identity.IsAuthenticated, e.Error));
        }

    }
}