using System.Security.Principal;
using Csla.Security;

namespace TreeBrowser.Entities.Security
{
  public partial class TreeBrowserPrincipal : Csla.Security.CslaPrincipal
  {
    public static void Login(string name, string password)
    {
        var identity = MembershipIdentity.GetMembershipIdentity<TreeBrowserIdentity>(name, password);
        SetPrincipal(identity);
    }


  }
}