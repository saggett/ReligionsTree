using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TreeBrowser.SilverlightLib.EventArgs;
using TreeBrowser.SilverlightLib.Helpers;
using TreeBrowser.SilverlightLib;
using TreeBrowser.Entities;

namespace TreeBrowser.Silverlight.Application.Controls
{
    public partial class LineageControl : UserControl
    {

        private const bool NavigateOnSecondClick = false;

        public event EventHandler<LineageIdEventArgs> Selected;
        public event EventHandler<LineageIdEventArgs> NavigateRequest;


        public LineageControl()
        {
            InitializeComponent();
        }

        public LineageControl(LineageCoords lineageInfo) : this()
        {
            DataContext = lineageInfo;
        }
        
        private void Ctrl_MouseLeave(object sender, MouseEventArgs e)
        {
            LineageInfo.IsMouseOver = false;
        }

        private void Ctrl_MouseEnter(object sender, MouseEventArgs e)
        {
            LineageInfo.IsMouseOver = true;
        }

        public LineageCoords LineageInfo
        {
            get { return (LineageCoords) DataContext; }
        }

        public int LineageId
        {
            get { return LineageInfo.LineageId; }
        }

        public int MinX
        {
            get { return LineageInfo.BranchAbsoluteMinX; }
        }

        public int MaxX
        {
            get { return LineageInfo.BranchAbsoluteMaxX; }
        }

        private int MinY
        {
            get { return LineageInfo.BranchAbsoluteMinY; }
        }

        private int MaxY
        {
            get { return LineageInfo.BranchAbsoluteMaxY; }
        }

        //public int BranchWidth
        //{
        //    get { return MaxX - MinX; }
        //}

        public bool IsSelected
        {
            get { return LineageInfo.IsSelected; }
            set
            {
                bool oldValue = LineageInfo.IsSelected;
                if (oldValue != value)
                {
                    LineageInfo.IsSelected = value;
                    if (value && Selected != null)
                        Selected(this, new LineageIdEventArgs(LineageId));
                }
            }
        }

        public int BranchHeight
        {
            get { return MaxY - MinY; }
        }

        public bool BranchesLeft    
        {
            get { return LineageInfo.BranchesLeft; }
        }

        public int StartYear
        {
            get { return LineageInfo.Lineage.StartYear; }
        }

        private void Line_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Select();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Select();
        }

        private void Select()
        {
            bool alreadySelected = IsSelected;
            IsSelected = true;
            if (NavigateOnSecondClick && alreadySelected & NavigateRequest != null)
                NavigateRequest(this, new LineageIdEventArgs(LineageId));
        }

        public bool IsInHierarchy(LineageControl lc)
        {
            return LineageInfo.SelfAndAllChildren.Contains(lc.LineageInfo);
        }

        private void Ctrl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            DisplayContextMenu(e.GetPosition(System.Windows.Application.Current.RootVisual));
        }

        private void DisplayContextMenu(Point displayPos)
        {
            Select();
            LineTooltip.IsOpen = false;
            LabelToolTip.IsOpen = false;
            var menu = new ContextMenu();
            var setRootMi = new MenuItem() { Header = "Set religion as root" };
            setRootMi.Click += new RoutedEventHandler(setRootMi_Click);
            menu.Items.Add(setRootMi);
            if (IsDisplayedRoot && HasParent)
            {
                var setParentAsRootMi = new MenuItem { Header = "Go to parent religion" };
                setParentAsRootMi.Click += new RoutedEventHandler(setParentAsRootMi_Click);
                menu.Items.Add(setParentAsRootMi);
            }
            //if (RootHasParent)
            //{
            //    var resetMi = new MenuItem() { Header = "Reset" };
            //    resetMi.Click += new RoutedEventHandler(resetMi_Click);
            //    menu.Items.Add(resetMi);
            //}
            menu.HorizontalOffset = displayPos.X;
            menu.VerticalOffset = displayPos.Y;
            menu.IsOpen = menu.Items.Count > 0;
        }

        private void setParentAsRootMi_Click(object sender, RoutedEventArgs e)
        {
            NavigateRequest(this, new LineageIdEventArgs(LineageInfo.Lineage.ParentLineageId.Value));
        }

        private void resetMi_Click(object sender, RoutedEventArgs e)
        {
            //root id: ((App)System.Windows.Application.Current).LineagesCache.Lineages.GetRoot().Id
            NavigateRequest(this, new LineageIdEventArgs(null));
        }

        private bool IsDisplayedRoot
        {
            get { return ParentTree.CurrentRootLineageId == LineageId; }
        }

        private bool HasParent
        {
            get { return LineageInfo.Lineage.ParentLineage != null; }
        }

        private ReadOnlyLineage LookupLineage(int lineageId)
        {
            return ((App)System.Windows.Application.Current).LineagesCache.Lineages.LookupLineage(lineageId);
        }

        private Tree ParentTree
        {
            get { return FindParentTree(this); }
        }

        private bool RootHasParent
        {
            get { return ParentTree.CurrentRootLineage.ParentLineage != null; }
        }

        private Tree FindParentTree(FrameworkElement element)
        {
            return element is Tree ? (Tree)element : FindParentTree((FrameworkElement)element.Parent);
        }

        private void setRootMi_Click(object sender, RoutedEventArgs e)
        {
            NavigateRequest(this, new LineageIdEventArgs(LineageId));
        }

    }

}