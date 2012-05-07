using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Printing;
using Csla;
using TreeBrowser.Entities;
using TreeBrowser.Entities.Helpers;
using TreeBrowser.Silverlight.Controls.DataVisualization.Charting;
using TreeBrowser.SilverlightLib;
using TreeBrowser.SilverlightLib.Controls;
using TreeBrowser.SilverlightLib.EventArgs;
using TreeBrowser.SilverlightLib.Helpers;

namespace TreeBrowser.Silverlight.Application.Controls
{
    public partial class Tree : UserControl
    {

        PrintDocument printDocument = new PrintDocument();

        public event EventHandler<LineageIdEventArgs> NavigateRequest;
        public event EventHandler<LineageIdEventArgs> RootBindComplete;

        private const double AnimationDuration = 1.2;
        private const bool AnimationEnabled = true;
        private const double MaxEndYearMultipler = 1.2d;

        public Tree()
        {
            InitializeComponent();
            //MouseWheel += new System.Windows.Input.MouseWheelEventHandler(Tree_MouseWheel);
            LineageEditorPanel.Deleted += LineageEditorPanel_Deleted;
            LineageEditorPanel.Saved += LineageEditorPanel_Saved;
            LineageEditorPanel.NavigateRequest += new EventHandler<LineageIdEventArgs>(LineageEditorPanel_NavigateRequest);
            printDocument.PrintPage += new EventHandler<PrintPageEventArgs>(printDocument_PrintPage);
            ((App)System.Windows.Application.Current).LineagesCache.FetchLineagesCompleted += LineagesCache_FetchLineagesCompleted;
            //MouseMove += new System.Windows.Input.MouseEventHandler(Tree_MouseMove);
            //Application.Current.Host.Content.Resized += new EventHandler(Content_Resized);
            //LoadData();
        }

        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.PageVisual = TreeGrid;
        }

        private void LineageEditorPanel_NavigateRequest(object sender, LineageIdEventArgs e)
        {
            HandleNavigationRequest(e);
        }

        private void Tree_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CurrentMousePosition = e.GetPosition(TreeCanvas);
        }

        protected Point CurrentMousePosition { get; set; }


        //private void Content_Resized(object sender, System.EventArgs e)
        //{
        //    Height = Application.Current.Host.Content.ActualHeight;
        //    Width = Application.Current.Host.Content.ActualWidth;
        //}


        #region Draw Methods

        //private void BeginInitialDraw()
        //{
        //    BeginDraw(null, false);
        //}


        public void InitForRoot(int? rootLineageId)
        {
            if (rootLineageId.HasValue)
                CurrentRootLineageId = rootLineageId.Value;
            if (!LineagesFetched)
            {
                BeginFetch();
                return;
            }
            Dispatcher.BeginInvoke(() => Draw(CurrentRootLineageId));
        }

        private void AnimateMove(int rootLineageId)
        {
            AnimateMove(rootLineageId, false);
        }

        private void AnimateMove(int rootLineageId, bool startFromMousePos)
        {
            int previousRootLineageId = CurrentRootLineageId;
            CurrentRootLineageId = rootLineageId;

            //deselect lineage and reset lineage editor panel if we're changing root
            LineageEditorPanel.Cancel();
            //hide go to parent button during animation
            Dispatcher.BeginInvoke(() => HideNavigationButtons());

            if (!SelectedLineageId.HasValue)
                SelectedLineageId = rootLineageId;

            AnimateRootChange(previousRootLineageId, CurrentRootLineageId, startFromMousePos);
        }


        private void Draw(int rootId)
        {
            _lineageIdLookup.Clear();
            TreeCanvas.Children.Clear();
            //BUG: Max Year can be negative
            var maxYear = CalcTreeMaxYear();

            //Reset transforms
            ResetLineageTransforms();

            ReadOnlyLineage rootLineage = DataSource.LookupLineage(rootId);

            DrawTree(TreePlot.CreateTreePlot(rootLineage, maxYear,
                                             (int)TreeCanvas.ActualHeight, (int)TreeCanvas.ActualWidth), rootLineage,
                     SelectedLineageId);
            InitAxisMinAndMax(rootLineage);
            if (LineageEditorPanel.PanelMode != LineageEditor.PanelModeEnum.LineageEditing)
                UpdateDataSource();
            if (RootBindComplete != null)
                RootBindComplete(this, new LineageIdEventArgs(rootId));
        }

        private int CalcTreeMaxYear()
        {
            return CalcTreeMaxYear(CurrentRootLineage);
        }

        private int CalcTreeMaxYear(ReadOnlyLineage lin)
        {
            double result = ((lin.EffectiveMaxYearOfBranch - lin.StartYear) * MaxEndYearMultipler) + lin.StartYear;
            return (int)result;
        }

        private void ResetLineageTransforms()
        {
            ChangeCanvasScale(null, null, 1, 1);
            foreach (LineageControl lc in TreeCanvas.Children.OfType<LineageControl>())
                lc.Opacity = 1;
        }

        private void DrawTree(IEnumerable<LineageCoords> treePlot, ReadOnlyLineage root, int? selectedLineageId)
        {
            foreach (LineageCoords plot in treePlot)
            {
                var linCtrl = new LineageControl(plot);
                if (selectedLineageId.HasValue && plot.LineageId == selectedLineageId.Value)
                    linCtrl.IsSelected = true;
                linCtrl.RenderTransform = LineageTransformGroup;
                Canvas.SetTop(linCtrl, plot.AbsoluteOriginY);
                Canvas.SetLeft(linCtrl, plot.AbsoluteOriginX);

                _lineageIdLookup.Add(linCtrl, plot.LineageId);
                linCtrl.Selected += LinCtrl_Selected;
                linCtrl.NavigateRequest += linCtrl_NavigateRequest;

                TreeCanvas.Children.Add(linCtrl);
                if (plot.LineageId == root.Id && root.ParentLineageId.HasValue)
                {
                    AddGoToParentButton(plot);
                    AddResetButton(plot);
                }
                //TODO: Try to give canvas focus on tree load so that it'll respond to mouse wheel events
                //if (plot.LineageId == root.Id && selectedLineageId == null)
                //{
                //    Dispatcher.BeginInvoke(() => { linCtrl.Focus(); });
                //}
            }
        }

        private void AddGoToParentButton(LineageCoords plot)
        {
            //if the lineage occurs more than 3/4 of the way to the right of the canvas, plot the buttons to the left of the line
            bool plotToLeft = plot.AbsoluteXStart > TreeCanvas.ActualWidth * 0.85;
            int xOffset = plotToLeft ? -100 : 20;
            var button = new GoToParentButton();
            button.Name = "GoToParentButton";
            button.Click += GotoParentButton_Click;
            Canvas.SetTop(button, plot.AbsoluteYStart - 40);
            Canvas.SetLeft(button, plot.AbsoluteXStart + xOffset);
            TreeCanvas.Children.Add(button);
        }

        private void AddResetButton(LineageCoords plot)
        {
            //if the lineage occurs more than 3/4 of the way to the right of the canvas, plot the buttons to the left of the line
            bool plotToLeft = plot.AbsoluteXStart > TreeCanvas.ActualWidth * 0.85;
            int xOffset = plotToLeft ? -60 : 60;
            var rButton = new ResetButton();
            rButton.Name = "ResetButton";
            rButton.Click += ResetButton_Click;
            Canvas.SetTop(rButton, plot.AbsoluteYStart - 40);
            Canvas.SetLeft(rButton, plot.AbsoluteXStart + xOffset);
            TreeCanvas.Children.Add(rButton);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetTree();
        }


        #endregion

        #region Methods

        private bool TryToSetLineageControlSelection(int lineageId, bool isSelected)
        {
            var lc = LookupLineageControl(lineageId);
            if (lc != null)
                lc.IsSelected = isSelected;
            return lc != null;
        }

        private void GoToParent()
        {
            ReadOnlyLineage lineage = DataSource.LookupLineage(CurrentRootLineageId);
            if (!lineage.ParentLineageId.HasValue)
                throw new InvalidOperationException("Current root lineage has no parent. ");
            if (AnimationEnabled)
                AnimateMove(lineage.ParentLineageId.Value);
            else
                if (NavigateRequest != null)
                    NavigateRequest(this, new LineageIdEventArgs(lineage.ParentLineageId.Value));

        }

        private void BranchFetchCompleted(DataPortalResult<ReadOnlyLineages> e)
        {
            myBusyAnimation.IsRunning = false;
            DataSource = e.Object;
            if (CurrentRootLineageId == 0)
                CurrentRootLineageId = LookupRootLineageId();
            //if there's no selected lineage and we're not looking at the home tree (the ultimate root), select the root
            if (!SelectedLineageId.HasValue && CurrentRootLineage.ParentLineageId.HasValue)
                SelectedLineageId = CurrentRootLineageId;
            InitForRoot(CurrentRootLineageId);
        }

        //private void InitAxisMinAndMax(ReadOnlyLineage root)
        //{
        //    TreeAxis.Minimum = root.StartYear;
        //    TreeAxis.Maximum = CalcTreeMaxYear();
        //}

        private void InitAxisMinAndMax(ReadOnlyLineage root)
        {

            int startYear = root.StartYear;
            int endYear = CalcTreeMaxYear();

            if (CanvasScaleTransform.ScaleY == 1)
            {
                TreeAxis.Maximum = endYear;
                TreeAxis.Minimum = startYear;
                return;
            }

            Rect rect = TreeCanvas.GetBounds();
            Rect transBounds = CanvasScaleTransform.TransformBounds(rect);

            TreeAxis.Minimum = -2500;

            TreeAxis.Maximum = endYear - ((endYear - startYear) * (Math.Abs(transBounds.Top) / transBounds.Height));
            TreeAxis.Minimum = endYear - ((endYear - startYear) * ((rect.Height + Math.Abs(transBounds.Top)) / transBounds.Height));


            //double zoomedHeight = TreeCanvas.ActualHeight / scaleY;
            ////if center takes the zoom area outside of the canvas bounds, adjust the center so that it doesn't
            //if (centerY + (zoomedHeight / 2d) > TreeCanvas.ActualHeight)
            //    centerY = TreeCanvas.ActualHeight - (zoomedHeight / 2d);
            //else if (centerY - (zoomedHeight / 2d) < 0)
            //    centerY = zoomedHeight / 2d;
            ////SA 17.07.10 BUG: Doesn't calculate correct min / max year when zoom is in middle of canvas
            //TreeAxis.Minimum = ConvertYCoordToYear(centerY + (zoomedHeight / 2d), startYear, endYear);
            //TreeAxis.Maximum = ConvertYCoordToYear(centerY - (zoomedHeight / 2d), startYear, endYear);

        }

        private int ConvertYCoordToYear(double yCoord, int startYear, int endYear)
        {
            return endYear - (int)((double)(endYear - startYear) * (yCoord / TreeCanvas.ActualHeight));
        }

        private void UpdateDataSource()
        {
            if (SelectedLineageId.HasValue)
                LineageEditorPanel.SetLineageSource(SelectedLineageId.Value, SelectedLineageId.Value == CurrentRootLineageId);
            else
                LineageEditorPanel.Clear();
        }



        private LineageControl LookupLineageControl(int lineageId)
        {
            return TreeCanvas.Children.OfType<LineageControl>().SingleOrDefault(lc => lc.LineageId == lineageId);
        }

        private bool LineagesDrawn
        {
            get { return TreeCanvas.Children.OfType<LineageControl>().Any(); }
        }

        private void HideNavigationButtons()
        {
            GoToParentButton gtpButton = (GoToParentButton)FindName("GoToParentButton");
            if (gtpButton != null)
                gtpButton.Visibility = Visibility.Collapsed;
            ResetButton rButton = (ResetButton)FindName("ResetButton");
            if (rButton != null)
                rButton.Visibility = Visibility.Collapsed;
        }

        private bool LineagesFetched
        {
            get { return ((App)System.Windows.Application.Current).LineagesCache.DataFetched & DataSource != null; }
        }

        private int LookupRootLineageId()
        {
            return DataSource.GetRoot().Id;
        }

        private bool IsReverseMove(int oldLineageId, int newLineageId)
        {
            return DataSource.IsLineageAChildOfLineageB(oldLineageId, newLineageId);
        }

        #endregion

        #region Event Handlers

        private void LineageEditorPanel_Saved(object sender, LineageEventArgs e)
        {
            ((App)System.Windows.Application.Current).LineagesCache.Invalidate();
            NavigateRequest(this, new LineageIdEventArgs(CurrentRootLineageId));
        }

        private void LineageEditorPanel_Deleted(object sender, LineageEventArgs e)
        {
            if (SelectedLineageId == e.Lineage.Id)
                SelectedLineageId = null;
            ((App)Application.App.Current).LineagesCache.Invalidate();
            NavigateRequest(this, new LineageIdEventArgs(CurrentRootLineageId));
        }

        private void TreeCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitForRoot(CurrentRootLineageId);
        }

        private void GotoParentButton_Click(object sender, RoutedEventArgs e)
        {
            GoToParent();
        }

        private void LinCtrl_Selected(object sender, LineageIdEventArgs e)
        {
            SelectedLineageId = e.LineageId;
        }

        private void linCtrl_NavigateRequest(object sender, LineageIdEventArgs e)
        {
            HandleNavigationRequest(e);
            //NavigateRequest(this, new LineageIdEventArgs(e.LineageId));
            //BeginDraw(e.LineageId, true);
        }

        private void HandleNavigationRequest(LineageIdEventArgs e)
        {
            if (!e.LineageId.HasValue)
                throw new InvalidOperationException("Lineage navigation requests must specify an id. ");
            if (e.LineageId.Value == CurrentRootLineageId)
                return;
            if (AnimationEnabled & CanvasScaleTransform.ScaleX == 1d & CanvasScaleTransform.ScaleY == 1d)
                AnimateMove(e.LineageId.Value);
            else
                //Root change animation is not supported while the canvas is zoomed by user (ScaleX/Y != 1) so navigate without animation in this case
                if (NavigateRequest != null)
                    NavigateRequest(this, new LineageIdEventArgs(e.LineageId.Value));
        }


        private void Tree_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta == 0)
                return;
            Point p = e.GetPosition(App.Current.RootVisual);
            var elems = VisualTreeHelper.FindElementsInHostCoordinates(p, App.Current.RootVisual);
            if (!elems.Contains(CanvasBorder))
                return;
            e.Handled = true;
            DoZoom(Math.Sign(e.Delta));
        }

        private void LineagesCache_FetchLineagesCompleted(object sender, DataPortalResult<ReadOnlyLineages> e)
        {
            BranchFetchCompleted(e);
        }

        #endregion

        #region Properties


        private readonly Dictionary<LineageControl, int> _lineageIdLookup = new Dictionary<LineageControl, int>();

        internal int CurrentRootLineageId { get; private set; }

        private ReadOnlyLineages DataSource
        {
            get { return (ReadOnlyLineages)DataContext; }
            set { DataContext = value; }
        }

        #endregion

        #region Animation Creation

        private void AnimateRootChange(int oldRootId, int newRootId, bool startFromMousePos)
        {
            if (!IsReverseMove(oldRootId, newRootId))
                AnimateForwardRootChange(oldRootId, newRootId, startFromMousePos);
            else
                AnimateReverseRootChange(oldRootId, newRootId, startFromMousePos);
        }

        private void AnimateForwardRootChange(int oldRootId, int newRootId, bool startFromMousePos)
        {
            LineageControl oldRoot = LookupLineageCtrlOfLineage(oldRootId);
            LineageControl newRoot = LookupLineageCtrlOfLineage(newRootId);

            //Point targetPoint = newRoot.LineageOriginPoint;
            //Point curPoint = LookupLineageCtrlOfLineage(oldRootId).LineageOriginPoint;
            if (startFromMousePos)
                CanvasScaleTransform.CenterX = CalcZoomCenter(CurrentMousePosition.X, TreeCanvas.ActualWidth, false);
            else
                CanvasScaleTransform.CenterX = newRoot.BranchesLeft ? newRoot.MinX : newRoot.MaxX;

            var board = new Storyboard();
            board.Duration = new Duration(TimeSpan.FromSeconds(AnimationDuration));
            board.SpeedRatio = 0.5;
            //double xZoomScale = Convert.ToDouble(oldRoot.BranchWidth) / newRoot.BranchWidth;
            //use min y coords on both
            double yZoomScale = Convert.ToDouble(oldRoot.LineageInfo.BranchAbsoluteMaxY) / newRoot.LineageInfo.BranchAbsoluteMaxY;
            board.Children.Add(CreateChangeToAnim(CanvasScaleTransform, ScaleTransform.ScaleYProperty, yZoomScale));
            board.Children.Add(CreateChangeToAnim(CanvasScaleTransform, ScaleTransform.ScaleXProperty, yZoomScale));

            //we move to max year of old root because up until redraw after animation completion we are staying with that tree
            //board.Children.Add(CreateChangeToAnim(TreeAxis, NumericAxis.MaximumProperty, CalcTreeMaxYear(oldRoot.LineageInfo.Lineage)));
            board.Children.Add(CreateChangeToAnim(TreeAxis, NumericAxis.MinimumProperty, newRoot.StartYear));

            foreach (
                LineageControl lc in
                    TreeCanvas.Children.OfType<LineageControl>().Where(lc => !newRoot.IsInHierarchy(lc)))
                board.Children.Add(CreateChangeToAnim(lc, OpacityProperty, 0));

            board.Completed += RootChangeStoryboard_Completed;
            board.Begin();
        }

        private void AnimateForwardRootChange(int oldRootId, int newRootId)
        {
            AnimateForwardRootChange(oldRootId, newRootId, false);
        }

        //SA TEST
        private void DoZoom(int delta)
        {
            double centerX = CalcZoomCenter(CurrentMousePosition.X, TreeCanvas.ActualWidth, false);
            double centerY = CalcZoomCenter(CurrentMousePosition.Y, TreeCanvas.ActualHeight, true);

            double scaleFactor = delta > 0 ? 0.2 : -0.2;

            double newXScale = CanvasScaleTransform.ScaleX + (scaleFactor * CanvasScaleTransform.ScaleX);
            double newYScale = CanvasScaleTransform.ScaleY + (scaleFactor * CanvasScaleTransform.ScaleY);

            if (newXScale > 40 | newYScale > 40)
                //stop scaling
                return;

            ChangeCanvasScale(centerX, centerY, newXScale, newYScale);
        }

        private static double CalcZoomCenter(double mouseCoord, double dimensionLength, bool isY)
        {
            if (!isY)
                return mouseCoord;
            if ((mouseCoord / dimensionLength) > 0.95d)
                return dimensionLength;
            else if ((mouseCoord / dimensionLength) < 0.05d)
                return 0d;
            else
                return mouseCoord;
        }

        //private void ZoomStoryboard_Completed(object sender, EventArgs e)
        //{
        //    UpdateCanvasScaleFactorValuesOnLinControls();
        //}

        private void ChangeCanvasScale(double? centerX, double? centerY, double? scaleX, double? scaleY)
        {
            if (centerX.HasValue)
                CanvasScaleTransform.CenterX = centerX.Value;
            if (centerY.HasValue)
                CanvasScaleTransform.CenterY = centerY.Value;
            if (scaleX.HasValue)
                CanvasScaleTransform.ScaleX = Math.Max(scaleX.Value, 1d);
            if (scaleY.HasValue)
                CanvasScaleTransform.ScaleY = Math.Max(scaleY.Value, 1d);
            //do something here
            if (scaleY.HasValue)
                InitAxisMinAndMax(CurrentRootLineage);
        }

        private void AnimateReverseRootChange(int oldRootId, int newRootId, bool startFromMousePos)
        {
            //start by drawing tree of new root
            Draw(newRootId);

            LineageControl oldRoot = LookupLineageCtrlOfLineage(oldRootId);
            LineageControl newRoot = LookupLineageCtrlOfLineage(newRootId);

            //calculate and set zoomed in transform values
            double yZoomScale = Convert.ToDouble(newRoot.BranchHeight) / oldRoot.BranchHeight;
            double centerX;
            if (startFromMousePos)
                centerX = CalcZoomCenter(CurrentMousePosition.X, TreeCanvas.ActualWidth, false);
            else
                centerX = oldRoot.BranchesLeft ? oldRoot.MinX : oldRoot.MaxX;

            ChangeCanvasScale(centerX, null, yZoomScale, yZoomScale);

            foreach (LineageControl lc in
                TreeCanvas.Children.OfType<LineageControl>().Where(lc => !oldRoot.IsInHierarchy(lc)))
                lc.Opacity = 0;


            //SA TEST: This line can cause an exception after user scaling
            //TreeAxis.Minimum = oldRoot.StartYear;


            var board = new Storyboard();
            board.Duration = new Duration(TimeSpan.FromSeconds(AnimationDuration));
            board.SpeedRatio = 0.5;
            board.Children.Add(CreateChangeToAnim(CanvasScaleTransform, ScaleTransform.ScaleYProperty, 1));
            board.Children.Add(CreateChangeToAnim(CanvasScaleTransform, ScaleTransform.ScaleXProperty, 1));

            board.Children.Add(CreateChangeToAnim(TreeAxis, NumericAxis.MaximumProperty, CalcTreeMaxYear(newRoot.LineageInfo.Lineage)));
            board.Children.Add(CreateChangeToAnim(TreeAxis, NumericAxis.MinimumProperty, newRoot.StartYear));

            foreach (
                LineageControl lc in
                    TreeCanvas.Children.OfType<LineageControl>().Where(lc => lc.Opacity == 0))
                board.Children.Add(CreateChangeToAnim(lc, OpacityProperty, 1));

            board.Completed += RootChangeStoryboard_Completed;
            board.Begin();
        }

        private void RootChangeStoryboard_Completed(object sender, System.EventArgs e)
        {
            if (NavigateRequest != null)
                NavigateRequest(this, new LineageIdEventArgs(CurrentRootLineageId));
        }

        private static DoubleAnimation CreateChangeToAnim(DependencyObject ctrl, DependencyProperty targetProp,
                                                          double changeTo)
        {
            DoubleAnimation changeAnim = CreateAnimation(ctrl, targetProp);
            changeAnim.To = changeTo;
            return changeAnim;
        }

        private static DoubleAnimation CreateChangeByAnim(DependencyObject ctrl, DependencyProperty targetProp,
                                                          double changeBy)
        {
            DoubleAnimation changeAnim = CreateAnimation(ctrl, targetProp);
            changeAnim.By = changeBy;
            return changeAnim;
        }

        private static DoubleAnimation CreateAnimation(DependencyObject ctrl, DependencyProperty targetProp)
        {
            var changeAnim = new DoubleAnimation();
            Storyboard.SetTarget(changeAnim, ctrl);
            Storyboard.SetTargetProperty(changeAnim, new PropertyPath(targetProp));
            return changeAnim;
        }

        private LineageControl LookupLineageCtrlOfLineage(int lineageId)
        {
            return TreeCanvas.Children.OfType<LineageControl>().Single(lc => lc.LineageId == lineageId);
        }

        #endregion

        #region Properties


        private int? _selectedLineageId;

        public int? SelectedLineageId
        {
            get { return _selectedLineageId; }
            set
            {
                if (value != _selectedLineageId)
                {
                    int? oldValue = _selectedLineageId;
                    _selectedLineageId = value;
                    if (LineagesDrawn)
                    {
                        if (oldValue.HasValue)
                            TryToSetLineageControlSelection(oldValue.Value, false);
                        if (_selectedLineageId.HasValue)
                            TryToSetLineageControlSelection(_selectedLineageId.Value, true);
                    }
                    UpdateDataSource();
                }
            }
        }

        public ReadOnlyLineage CurrentRootLineage
        {
            get { return DataSource != null ? DataSource.LookupLineage(CurrentRootLineageId) : null; }
        }

        #endregion

        private void BeginFetch()
        {
            myBusyAnimation.IsRunning = true;
            ((App)System.Windows.Application.Current).LineagesCache.Prefetch();
        }

        private void LineageEditorPanel_EditModeChanged(object sender, EditModeEventArgs e)
        {
            Dispatcher.BeginInvoke(() => UpdateSidebarWidth(e.IsInEditMode));
        }

        private void UpdateSidebarWidth(bool isInEditMode)
        {
            LayoutGrid.ColumnDefinitions[1].Width = isInEditMode
                                                        ? new GridLength(0.68, GridUnitType.Star)
                                                        : new GridLength(0.22, GridUnitType.Star);
            //LayoutGrid.UpdateLayout();
        }

        private void LayoutGrid_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(System.Windows.Application.Current.RootVisual);
            //if (!WasTreeClicked(pos))
            //    return;
            e.Handled = true;
            DisplayContextMenu(pos);
        }

        private void DisplayContextMenu(Point point)
        {
            var menu = new ContextMenu();
            if (CurrentRootLineage.ParentLineage != null || CanvasScaleTransform.ScaleY != 1)
            {
                var resetMi = new MenuItem() { Header = "Reset" };
                resetMi.Click += new RoutedEventHandler(resetMi_Click);
                menu.Items.Add(resetMi);
            }
            var printMi = new MenuItem() {Header = "Print"};
            printMi.Click += new RoutedEventHandler(printMi_Click);
            menu.Items.Add(printMi);
            //var saveJpgMi = new MenuItem() { Header = "Save as JPG" };
            //saveJpgMi.Click += new RoutedEventHandler(saveJpgMi_Click);
            //menu.Items.Add(saveJpgMi);
            menu.HorizontalOffset = point.X;
            menu.VerticalOffset = point.Y;
            menu.IsOpen = menu.Items.Count > 0;
        }

        private void printMi_Click(object sender, RoutedEventArgs e)
        {
            printDocument.Print("Tree", new PrinterFallbackSettings(){ForceVector = true}, false);
        }

        //private void saveJpgMi_Click(object sender, RoutedEventArgs e)
        //{
        //    CanvasBorder.SaveToJPG();
        //}

        private void resetMi_Click(object sender, RoutedEventArgs e)
        {
            ResetTree();
        }

        private void ResetTree()
        {
            AnimateMove(DataSource.GetRoot().Id, true);
        }

        private void CanvasBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectedLineageId = null;
        }



    }
}