using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TreeBrowser.Silverlight.Entities;

namespace TreeBrowser.SilverlightLib.Proxy
{
    public static class Cache
    {

        static Cache()
        {
            ContractProxy.Current.FetchLineageGroupsCompleted += new EventHandler<FetchLineageGroupsCompletedEventArgs>(ContractProxy_FetchLineageGroupsCompleted);
            ContractProxy.Current.FetchLineagesCompleted += new EventHandler<FetchLineagesCompletedEventArgs>(ContractProxy_FetchLineagesCompleted);
            Prefetch();
        }

        private static void ContractProxy_FetchLineagesCompleted(object sender, FetchLineagesCompletedEventArgs e)
        {
            _Lineages = e.Result;
        }

        private static void ContractProxy_FetchLineageGroupsCompleted(object sender, FetchLineageGroupsCompletedEventArgs e)
        {
            _lineageGroups = new List<LineageGroup>(e.LineageGroups);
        }

        private static ReadOnlyLineageGroups _lineageGroups;
        public static ReadOnlyLineageGroups LineageGroups
        {
            get
            {
                if (_lineageGroups == null)
                    throw new InvalidOperationException("Lineage Groups are not yet fetched. ");
                return _lineageGroups;
            }
        }

        private static Lineages _Lineages;
        public static Lineages Lineages
        {
            get
            {
                if (_Lineages == null)
                    throw new InvalidOperationException("Lineages are not yet fetched. ");
                return _Lineages;
            }
        }


        public static string LookupLineageGroupName(int lineageGroupId)
        {
            return LineageGroups.Single(lg => lg.Id == lineageGroupId).Name;
        }

        public static bool PrefetchComplete
        {
            get { return _Lineages != null && _lineageGroups != null; }
        }

        public static void Prefetch()
        {
            ContractProxy.Current.BeginFetchLineageGroups();
            ContractProxy.Current.BeginFetchBranch(null);
        }

    }
}
