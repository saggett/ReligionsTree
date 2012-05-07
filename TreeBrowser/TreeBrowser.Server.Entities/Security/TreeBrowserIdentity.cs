using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Security;
using Csla.Serialization;

namespace TreeBrowser.Entities.Security
{
    
    [Serializable]
    public class TreeBrowserIdentity : MembershipIdentity
    {

#if !SILVERLIGHT

        public override void LoadCustomData()
        {
            base.LoadCustomData();
        }

#endif

    }
}
