using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Csla;
using Csla.Core;
using TreeBrowser.Entities;
using TreeBrowser.SilverlightLib.EventArgs;
using Csla.Rules;


namespace TreeBrowser.SilverlightLib.Controls
{
    public partial class LineageEditor : UserControl
    {

        public enum PanelModeEnum
        {
            NotSpecified = 0,
            LineageDisplay = 1,
            LineageEditing = 2,
            Intro = 3
        }


        public event EventHandler<EditModeEventArgs> EditModeChanged;
        public event EventHandler<LineageEventArgs> Saved;
        public event EventHandler<LineageEventArgs> Deleted;
        public event EventHandler<LineageIdEventArgs> NavigateRequest;

        private int? lineageIdBeingFetched;

        public LineageEditor()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(LineageEditor_Loaded);
            TreeBrowser.Entities.Security.TreeBrowserPrincipal.LoginCompleted += TreeBrowserPrincipal_LoginCompleted;
            //SetAsReadOnly();
        }

        private void LineageEditor_Loaded(object sender, RoutedEventArgs e)
        {
            ReadOnlyLineageGroups.GetLineageGroups(LineageGroups_FetchCompleted);
            SetPanelMode(PanelModeEnum.Intro);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private void TreeBrowserPrincipal_LoginCompleted(object sender, Entities.Security.LoginCompletedEventArgs e)
        {
            Refresh();
        }

        private void LineageGroups_FetchCompleted(object sender, DataPortalResult<ReadOnlyLineageGroups> e)
        {
            LineageGroupComboBox.DataContext = e.Object;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Edit();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void LineageGroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object value = ((SilverlightLib.Controls.ComboBox)sender).SelectedValue;
            DataSource.LineageGroupId = value != null ? Convert.ToInt32(value) : new int?();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        private void AddOffshootButton_Click(object sender, RoutedEventArgs e)
        {
            Create(DataSource);
        }

        private void Create(Lineage parentLineage)
        {
            var newLin = Lineage.NewLineage(parentLineage.Id, parentLineage.StartYear);
            DataSource = newLin;
            LineageGroupComboBox.SelectedValue = DataSource.LineageGroupId.HasValue
                             ? (object)DataSource.LineageGroupId.Value
                             : null;
            SetPanelMode(PanelModeEnum.LineageEditing);
        }

        private void Edit()
        {
            DataSource.BeginEdit();
            LineageGroupComboBox.SelectedValue = DataSource.LineageGroupId.HasValue
                                                     ? (object)DataSource.LineageGroupId.Value
                                                     : null;
            SetPanelMode(PanelModeEnum.LineageEditing);
        }

        public Lineage DataSource
        {
            get { return DataContext as Lineage; }
            private set
            {
                DataContext = value;
                OnAfterLineageBound(value);
            }
        }

        public void SetLineageSource(int lineageId, bool isCurrentRoot)
        {
            if (lineageIdBeingFetched.HasValue && lineageIdBeingFetched.Value == lineageId)
                return;
            bool sourceChanging = DataSource == null || DataSource.Id != lineageId;
            if (DataSource != null && !sourceChanging)
                return;
            if (sourceChanging)
                Cancel();
            IsBusy = true;
            lineageIdBeingFetched = lineageId;
            IsCurrentRoot = isCurrentRoot;
            Lineage.GetLineage(lineageId, Lineage_FetchForBindCompleted);
        }

        private void Lineage_FetchForBindCompleted(object sender, DataPortalResult<Lineage> e)
        {
            lineageIdBeingFetched = null;
            if (e.Error != null)
                throw e.Error;
            DataSource = e.Object;
            IsBusy = false;
        }


        private void OnAfterLineageBound(Lineage dataSource)
        {
            SetPanelMode(PanelModeEnum.LineageDisplay);
        }


        private void Save()
        {
            DataSource.ApplyEdit();
            IsBusy = true;
            DataSource.BeginSave(Lineage_SaveCompleted);
        }

        private void Lineage_SaveCompleted(object sender, SavedEventArgs eventArgs)
        {
            DataSource = (Lineage)eventArgs.NewObject;
            SetPanelMode(PanelModeEnum.LineageDisplay);
            IsBusy = false;
            if (Saved != null)
                Saved(this, new LineageEventArgs(DataSource));
        }

        private bool IsBusy
        {
            get { return (bool)LineageBusyAnimation.IsRunning; }
            set { LineageBusyAnimation.IsRunning = value; }
        }

        private bool _isCurrentRoot = false;
        private bool IsCurrentRoot
        {
            get { return _isCurrentRoot; }
            set
            {
                Dispatcher.BeginInvoke(() => { SetAsRootHyperlinkButton.Visibility = value ? Visibility.Collapsed : Visibility.Visible; });
                _isCurrentRoot = value;
            }
        }

        public void Refresh()
        {
            if (DataSource == null)
                return;
            int sourceId = DataSource.Id;
            Clear();
            SetLineageSource(sourceId, IsCurrentRoot);
        }

        public void Clear()
        {
            DataSource = null;
        }

        public void Cancel()
        {
            if (DataSource == null)
                return;
            DataSource.CancelEdit();
            SetPanelMode(PanelModeEnum.LineageDisplay);
        }

        private void Delete()
        {
            DataSource.Delete();
            DataSource.ApplyEdit();
            IsBusy = true;
            DataSource.BeginSave(Lineage_DeleteCompleted, DataSource.ParentLineageId);
        }

        private void Lineage_DeleteCompleted(object sender, SavedEventArgs e)
        {
            IsBusy = false;
            SetPanelMode(PanelModeEnum.LineageDisplay);
            if (Deleted != null)
                Deleted(this, new LineageEventArgs(DataSource));
        }

        private void InitLineageDisplayMode()
        {
            LayoutRoot.Visibility = System.Windows.Visibility.Visible;
            Visibility editOptionsVis = DataSource != null && BusinessRules.HasPermission(AuthorizationActions.EditObject, DataSource)
                                                           ? Visibility.Visible
                                                           : Visibility.Collapsed;
            IntroTextBlock.Visibility = Visibility.Collapsed;
            EditorControls.Visibility = Visibility.Collapsed;
            LineageDetailsPanel.Visibility = Visibility.Visible;
            foreach (UIElement child in LineageDetailsPanel.Children)
            {
                if (child == SetAsRootHyperlinkButton)
                    SetAsRootHyperlinkButton.Visibility = IsCurrentRoot ? Visibility.Collapsed : Visibility.Visible;
                else
                    child.Visibility = Visibility.Visible;
            }
            ExternalLinksLabel.Visibility = DataSource != null ? DataSource.ExternalHyperlinksVisibility : System.Windows.Visibility.Collapsed;
            EditButton.Visibility = editOptionsVis;
            SaveButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;
            AddOffshootButton.Visibility = editOptionsVis;
        }

        private void InitLineageEditingMode(bool isNew, bool hasChildren)
        {
            LayoutRoot.Visibility = System.Windows.Visibility.Visible;
            IntroTextBlock.Visibility = Visibility.Collapsed;
            EditorControls.Visibility = Visibility.Visible;
            foreach (UIElement child in LineageDetailsPanel.Children)
                child.Visibility = Visibility.Collapsed;
            LineageDetailsPanel.Visibility = Visibility.Collapsed;
            EditButton.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = !isNew & !hasChildren ? Visibility.Visible : Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Visible;
            AddOffshootButton.Visibility = Visibility.Collapsed;
        }

        private void InitIntroMode()
        {
            LayoutRoot.Visibility = System.Windows.Visibility.Visible;
            IntroTextBlock.Visibility = Visibility.Visible;
            EditorControls.Visibility = Visibility.Collapsed;
            LineageDetailsPanel.Visibility = Visibility.Collapsed;
            foreach (UIElement child in LineageDetailsPanel.Children)
                child.Visibility = Visibility.Collapsed;
            EditButton.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;
            AddOffshootButton.Visibility = Visibility.Collapsed;

        }

        private void SetPanelMode(PanelModeEnum mode)
        {
            //override to intro when we don't have a data source
            if (DataSource == null)
                mode = PanelModeEnum.Intro;
            if (mode == PanelMode)
                return;
            //check whether we're changing to or from edit mode
            bool editModeSwitch = mode == PanelModeEnum.LineageEditing || PanelMode == PanelModeEnum.LineageEditing;
            PanelMode = mode;
            switch (mode)
            {
                case PanelModeEnum.LineageDisplay:
                    Dispatcher.BeginInvoke(() => { InitLineageDisplayMode(); });
                    break;
                case PanelModeEnum.LineageEditing:
                    Dispatcher.BeginInvoke(() => { InitLineageEditingMode(DataSource.IsNew, DataSource.HasChildren); });
                    break;
                case PanelModeEnum.Intro:
                    Dispatcher.BeginInvoke(() => { InitIntroMode(); });
                    break;
                default:
                    throw new NotSupportedException();
            }
            if (editModeSwitch && EditModeChanged != null)
                EditModeChanged(this, new EditModeEventArgs(mode == PanelModeEnum.LineageEditing));
        }

        public PanelModeEnum PanelMode
        {
            get;
            private set;
        }

        private void DeleteLinkRow_Click(object sender, RoutedEventArgs e)
        {
            DataSource.Hyperlinks.Remove((LineageHyperlink)((Button)sender).Tag);
        }

        private void AddLinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataSource == null)
                return;
            DataSource.Hyperlinks.AddNew();
        }

        private void SetAsRootHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigateRequest != null)
                NavigateRequest(this, new LineageIdEventArgs(DataSource.Id));
        }


    }

}