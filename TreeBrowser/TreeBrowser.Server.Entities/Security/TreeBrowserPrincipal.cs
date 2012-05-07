using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Csla;
using Csla.Serialization;

namespace TreeBrowser.Entities.Security
{

    [Serializable]
    public partial class TreeBrowserPrincipal : Csla.Security.CslaPrincipal
    {

    #region Constructor

    public TreeBrowserPrincipal(){}
    public TreeBrowserPrincipal(IIdentity identity)
      : base(identity)
    { }

    #endregion


    #region Login/Logout

    private static bool SetPrincipal(IIdentity identity)
    {
        if (identity == null)
            return false;

        ApplicationContext.User =
          identity.IsAuthenticated ?
            new TreeBrowserPrincipal(identity) :
            new TreeBrowserPrincipal(new TreeBrowserIdentity());

        return identity.IsAuthenticated;
    }
    public static void Logout()
    {
        ApplicationContext.User = new TreeBrowserPrincipal(new TreeBrowserIdentity());
    }


    #endregion

    [Serializable]
    public class Criteria : Csla.Core.MobileObject
    {
      private Criteria() { }
      public string Name { get; set; }
      public string Password { get; set; }
      public string ProviderType { get; set; }
      public Criteria(string name, string password)
      {
        Name = name;
        Password = password;
        ProviderType = string.Empty;
      }
      public Criteria(string name, string password, string providerType)
      {
        Name = name;
        Password = password;
        ProviderType = providerType;
      }

      protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info, Csla.Core.StateMode mode)
      {
        info.AddValue("Principal.Criteria.Name", Name);
        info.AddValue("Principal.Criteria.Password", Password);
        info.AddValue("Principal.Criteria.ProviderType", ProviderType);
        base.OnGetState(info, mode);
      }
      protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info, Csla.Core.StateMode mode)
      {
        base.OnSetState(info, mode);
        Name = info.GetValue<string>("Principal.Criteria.Name");
        Password = info.GetValue<string>("Principal.Criteria.Password");
        ProviderType = info.GetValue<string>("Principal.Criteria.ProviderType");
      }
    }



    }
}
