using System;
using System.Windows;
using System.Windows.Controls;
using TreeBrowser.Entities;
using Csla;
using TreeBrowser.SilverlightLib.Cache;

namespace TreeBrowser.Silverlight.Application.Views
{
    public partial class ContentEditorWindow : ChildWindow
    {
        public ContentEditorWindow()
        {
            InitializeComponent();
            IsBusy = true;
            HtmlContentNameValues.GetHtmlContentNameValues(HtmlContentNameValues_FetchCompleted);
        }

        public HtmlContent DataSource
        {
            get { return DataContext as HtmlContent; }
            private set
            {
                DataContext = value;
                OnAfterDataSourceBound(value);
            }
        }

        private void OnAfterDataSourceBound(HtmlContent content)
        {
            Dispatcher.BeginInvoke(() => { SaveButton.IsEnabled = content != null; });
            if (content != null)
                DataSource.BeginEdit();
        }

        private void HtmlContentNameValues_FetchCompleted(object sender, DataPortalResult<HtmlContentNameValues> e)
        {
            HcNamesComboBox.ItemsSource = e.Object;
            IsBusy = false;
        }

        private void HtmlContent_FetchCompleted(object sender, DataPortalResult<HtmlContent> e)
        {
            DataSource = e.Object;
            IsBusy = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DataSource.ApplyEdit();
            DataSource.BeginSave();
            HtmlContentCache.Default.Invalidate(SelectedHtmlContentEnum);
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataSource != null)
                DataSource.CancelEdit();
            DialogResult = false;
        }

        private void HcNamesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedHtmlContentEnum == HtmlContentEnum.NotSpecified)
            {
                DataSource = null;
                return;
            }
            IsBusy = true;
            HtmlContent.GetHtmlContent(SelectedHtmlContentEnum, HtmlContent_FetchCompleted);
            //DataSource = ((App)App.Current).HtmlContentCache.HtmlContentLookup[SelectedHtmlContentEnum];
        }


        private bool IsBusy
        {
            get { return (bool)BusyAnimation.IsRunning; }
            set { BusyAnimation.IsRunning = value; }
        }

        private HtmlContentEnum SelectedHtmlContentEnum
        {
            get
            {
                if (HcNamesComboBox.Items.Count == 0)
                    return HtmlContentEnum.NotSpecified;
                return (HtmlContentEnum)HcNamesComboBox.SelectedValue;
            }
        }

    }
}