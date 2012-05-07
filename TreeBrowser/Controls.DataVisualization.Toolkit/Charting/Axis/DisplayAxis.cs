// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TreeBrowser.SilverlightLib.Axis.Charting
{
    /// <summary>
    /// An axis that has a range.
    /// </summary>
    public abstract class DisplayAxis : Axis
    {
        /// <summary>
        /// Maximum intervals per 200 pixels.
        /// </summary>
        protected const double MaximumAxisIntervalsPer200Pixels = 8;

        /// <summary>
        /// The name of the axis grid template part.
        /// </summary>
        protected const string AxisGridName = "AxisGrid";

        /// <summary>
        /// The name of the axis title template part.
        /// </summary>
        protected const string AxisTitleName = "AxisTitle";

        #region public Style AxisLabelStyle

        /// <summary>
        /// Gets or sets the style used for the axis labels.
        /// </summary>
        public Style AxisLabelStyle
        {
            get { return GetValue(AxisLabelStyleProperty) as Style; }
            set { SetValue(AxisLabelStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the AxisLabelStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisLabelStyleProperty =
            DependencyProperty.Register(
                "AxisLabelStyle",
                typeof (Style),
                typeof (DisplayAxis),
                new PropertyMetadata(null, OnAxisLabelStylePropertyChanged));

        /// <summary>
        /// AxisLabelStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">DisplayAxis that changed its AxisLabelStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnAxisLabelStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (DisplayAxis) d;
            var oldValue = (Style) e.OldValue;
            var newValue = (Style) e.NewValue;
            source.OnAxisLabelStylePropertyChanged(oldValue, newValue);
        }

        /// <summary>
        /// AxisLabelStyleProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnAxisLabelStylePropertyChanged(Style oldValue, Style newValue)
        {
        }

        #endregion public Style AxisLabelStyle

        /// <summary>
        /// Gets the actual length.
        /// </summary>
        protected double ActualLength
        {
            get { return GetLength(new Size(ActualWidth, ActualHeight)); }
        }

        /// <summary>
        /// Returns the length of the axis given an available size.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>The length of the axis given an available size.</returns>
        protected double GetLength(Size availableSize)
        {
            if (ActualHeight == 0.0 && ActualWidth == 0.0)
            {
                return 0.0;
            }
            if (Orientation == AxisOrientation.X)
            {
                return availableSize.Width;
            }
            else if (Orientation == AxisOrientation.Y)
            {
                return availableSize.Height;
            }
            else
            {
                throw new InvalidOperationException(
                    Properties.Resources.DisplayAxis_GetLength_CannotDetermineTheLengthOfAnAxisWithAnOrientationOfNone);
            }
        }

        #region public Style MajorTickMarkStyle

        /// <summary>
        /// Gets or sets the style applied to the Axis tick marks.
        /// </summary>
        /// <value>The Style applied to the Axis tick marks.</value>
        public Style MajorTickMarkStyle
        {
            get { return GetValue(MajorTickMarkStyleProperty) as Style; }
            set { SetValue(MajorTickMarkStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the MajorTickMarkStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorTickMarkStyleProperty =
            DependencyProperty.Register(
                "MajorTickMarkStyle",
                typeof (Style),
                typeof (DisplayAxis),
                new PropertyMetadata(null, OnMajorTickMarkStylePropertyChanged));

        /// <summary>
        /// MajorTickMarkStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">DisplayAxis that changed its MajorTickMarkStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMajorTickMarkStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (DisplayAxis) d;
            var oldValue = (Style) e.OldValue;
            var newValue = (Style) e.NewValue;
            source.OnMajorTickMarkStylePropertyChanged(oldValue, newValue);
        }

        /// <summary>
        /// MajorTickMarkStyleProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnMajorTickMarkStylePropertyChanged(Style oldValue, Style newValue)
        {
        }

        #endregion public Style MajorTickMarkStyle

        #region public object Title

        /// <summary>
        /// Gets or sets the title property.
        /// </summary>
        public object Title
        {
            get { return GetValue(TitleProperty) as object; }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Identifies the Title dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof (object),
                typeof (DisplayAxis),
                new PropertyMetadata(null, OnTitlePropertyChanged));

        /// <summary>
        /// TitleProperty property changed handler.
        /// </summary>
        /// <param name="d">DisplayAxis that changed its Title.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (DisplayAxis) d;
            var oldValue = (object) e.OldValue;
            var newValue = (object) e.NewValue;
            source.OnTitlePropertyChanged(oldValue, newValue);
        }

        /// <summary>
        /// TitleProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnTitlePropertyChanged(object oldValue, object newValue)
        {
            if (AxisTitle != null)
            {
                AxisTitle.Content = Title;
            }
        }

        #endregion public object Title

        /// <summary>
        /// Gets or sets the LayoutTransformControl used to rotate the title.
        /// </summary>
        private LayoutTransformControl TitleLayoutTransformControl { get; set; }

        #region public Style TitleStyle

        /// <summary>
        /// Gets or sets the style applied to the Axis title.
        /// </summary>
        /// <value>The Style applied to the Axis title.</value>
        public Style TitleStyle
        {
            get { return GetValue(TitleStyleProperty) as Style; }
            set { SetValue(TitleStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the TitleStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleStyleProperty =
            DependencyProperty.Register(
                "TitleStyle",
                typeof (Style),
                typeof (DisplayAxis),
                null);

        #endregion

        #region public Style GridLineStyle

        /// <summary>
        /// Gets or sets the Style of the Axis's gridlines.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GridLine",
            Justification = "Current casing is the expected one.")]
        public Style GridLineStyle
        {
            get { return GetValue(GridLineStyleProperty) as Style; }
            set { SetValue(GridLineStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the GridlineStyle dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GridLine",
            Justification = "Current casing is the expected one.")] public static readonly DependencyProperty
            GridLineStyleProperty =
                DependencyProperty.Register(
                    "GridLineStyle",
                    typeof (Style),
                    typeof (DisplayAxis),
                    null);

        #endregion

        /// <summary>
        /// The grid used to layout the axis.
        /// </summary>
        private Grid _grid;

        /// <summary>
        /// Gets or sets the grid used to layout the axis.
        /// </summary>
        private Grid AxisGrid
        {
            get { return _grid; }
            set
            {
                if (_grid != value)
                {
                    if (_grid != null)
                    {
                        _grid.Children.Clear();
                    }

                    _grid = value;

                    if (_grid != null)
                    {
                        _grid.Children.Add(OrientedPanel);
                        if (AxisTitle != null)
                        {
                            _grid.Children.Add(AxisTitle);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a grid to lay out the dependent axis.
        /// </summary>
        private Grid DependentAxisGrid { get; set; }

        /// <summary>
        /// Gets the oriented panel used to layout the axis labels.
        /// </summary>
        internal OrientedPanel OrientedPanel { get; private set; }

        /// <summary>
        /// The control used to display the axis title.
        /// </summary>
        private Title _axisTitle;

        /// <summary>
        /// Gets or sets the title control used to display the title.
        /// </summary>
        private Title AxisTitle
        {
            get { return _axisTitle; }
            set
            {
                if (_axisTitle != value)
                {
                    if (_axisTitle != null)
                    {
                        _axisTitle.Content = null;
                    }

                    _axisTitle = value;
                    if (Title != null)
                    {
                        _axisTitle.Content = Title;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a major axis tick mark.
        /// </summary>
        /// <returns>A line to used to render a tick mark.</returns>
        protected virtual Line CreateMajorTickMark()
        {
            return CreateTickMark(MajorTickMarkStyle);
        }

        /// <summary>
        /// Creates a tick mark and applies a style to it.
        /// </summary>
        /// <param name="style">The style to apply.</param>
        /// <returns>The newly created tick mark.</returns>
        protected Line CreateTickMark(Style style)
        {
            var line = new Line();
            line.Style = style;
            if (Orientation == AxisOrientation.Y)
            {
                line.Y1 = 0.5;
                line.Y2 = 0.5;
            }
            else if (Orientation == AxisOrientation.X)
            {
                line.X1 = 0.5;
                line.X2 = 0.5;
            }
            return line;
        }

        /// <summary>
        /// This method is used to share the grid line coordinates with the
        /// internal grid lines control.
        /// </summary>
        /// <returns>A sequence of the major grid line coordinates.</returns>
        internal IEnumerable<UnitValue> InternalGetMajorGridLinePositions()
        {
            return GetMajorGridLineCoordinates(new Size(ActualWidth, ActualHeight));
        }

        /// <summary>
        /// Returns the coordinates to use for the grid line control.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of coordinates at which to draw grid lines.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GridLine",
            Justification = "This is the expected capitalization.")]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
            Justification = "Returns the coordinates of the grid lines.")]
        protected abstract IEnumerable<UnitValue> GetMajorGridLineCoordinates(Size availableSize);

#if !SILVERLIGHT
    /// <summary>
    /// Initializes the static members of the DisplayAxis class.
    /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Dependency properties are initialized in-line.")]
        static DisplayAxis()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DisplayAxis), new FrameworkPropertyMetadata(typeof(DisplayAxis)));
        }

#endif

        /// <summary>
        /// Instantiates a new instance of the DisplayAxis class.
        /// </summary>
        internal DisplayAxis()
        {
            OrientedPanel = new OrientedPanel();
#if SILVERLIGHT
            DefaultStyleKey = typeof (DisplayAxis);
            OrientedPanel.UseLayoutRounding = true;
#endif

            DependentAxisGrid = new Grid();

            TitleLayoutTransformControl = new LayoutTransformControl();
            TitleLayoutTransformControl.HorizontalAlignment = ((FrameworkElement) this).HorizontalAlignment.Center;
            TitleLayoutTransformControl.VerticalAlignment = ((FrameworkElement) this).VerticalAlignment.Center;

            SizeChanged += new SizeChangedEventHandler(DisplayAxisSizeChanged);
        }

        /// <summary>
        /// If display axis has just become visible, invalidate.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void DisplayAxisSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width == 0.0 && e.PreviousSize.Height == 0.0)
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Creates an axis label.
        /// </summary>
        /// <returns>The new axis label.</returns>
        protected virtual Control CreateAxisLabel()
        {
            return new AxisLabel();
        }

        /// <summary>
        /// Updates the grid lines element if a suitable dependent axis has
        /// been added to a radial axis.
        /// </summary>
        protected override void OnDependentAxesCollectionChanged()
        {
            base.OnDependentAxesCollectionChanged();
        }

        /// <summary>
        /// Prepares an axis label to be plotted.
        /// </summary>
        /// <param name="label">The axis label to prepare.</param>
        /// <param name="dataContext">The data context to use for the axis 
        /// label.</param>
        protected virtual void PrepareAxisLabel(Control label, object dataContext)
        {
            label.DataContext = dataContext;
            label.SetStyle(AxisLabelStyle);
        }

        /// <summary>
        /// Retrieves template parts and configures layout.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AxisGrid = GetTemplateChild(AxisGridName) as Grid;
            AxisTitle = GetTemplateChild(AxisTitleName) as Title;
            if (AxisTitle != null && AxisGrid.Children.Contains(AxisTitle))
            {
                AxisGrid.Children.Remove(AxisTitle);
                TitleLayoutTransformControl.Child = AxisTitle;
                AxisGrid.Children.Add(TitleLayoutTransformControl);
            }

            ArrangeAxisGrid();
        }

        /// <summary>
        /// When the size of the oriented panel changes invalidate the axis.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void OnOrientedPanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Invalidate();
        }

        /// <summary>
        /// Arranges the grid when the location property is changed.
        /// </summary>
        /// <param name="oldValue">The old location.</param>
        /// <param name="newValue">The new location.</param>
        protected override void OnLocationPropertyChanged(AxisLocation oldValue, AxisLocation newValue)
        {
            ArrangeAxisGrid();
            base.OnLocationPropertyChanged(oldValue, newValue);
        }

        /// <summary>
        /// Arranges the elements in the axis grid.
        /// </summary>
        private void ArrangeAxisGrid()
        {
            if (AxisGrid != null)
            {
                AxisGrid.ColumnDefinitions.Clear();
                AxisGrid.RowDefinitions.Clear();
                AxisGrid.Children.Clear();

                if (Orientation == AxisOrientation.Y)
                {
                    OrientedPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
                    OrientedPanel.IsReversed = true;

                    if (Location == AxisLocation.Left || Location == AxisLocation.Right)
                    {
                        TitleLayoutTransformControl.Transform = new RotateTransform {Angle = -90.0};

                        OrientedPanel.IsInverted = !(Location == AxisLocation.Right);
                        AxisGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        AxisGrid.RowDefinitions.Add(new RowDefinition());

                        int column = 0;
                        if (AxisTitle != null)
                        {
                            AxisGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            Grid.SetRow(TitleLayoutTransformControl, 0);
                            Grid.SetColumn(TitleLayoutTransformControl, 0);
                            column++;
                        }
                        Grid.SetRow(OrientedPanel, 0);
                        Grid.SetColumn(OrientedPanel, column);

                        AxisGrid.Children.Add(TitleLayoutTransformControl);
                        AxisGrid.Children.Add(OrientedPanel);

                        if (Location == AxisLocation.Right)
                        {
                            AxisGrid.Mirror(System.Windows.Controls.Orientation.Vertical);
                            TitleLayoutTransformControl.Transform = new RotateTransform {Angle = 90};
                        }
                    }
                }
                else if (Orientation == AxisOrientation.X)
                {
                    OrientedPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                    OrientedPanel.IsReversed = false;

                    if (Location == AxisLocation.Top || Location == AxisLocation.Bottom)
                    {
                        OrientedPanel.IsInverted = (Location == AxisLocation.Top);
                        TitleLayoutTransformControl.Transform = new RotateTransform {Angle = 0};

                        AxisGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        AxisGrid.RowDefinitions.Add(new RowDefinition());

                        if (AxisTitle != null)
                        {
                            AxisGrid.RowDefinitions.Add(new RowDefinition());
                            Grid.SetColumn(TitleLayoutTransformControl, 0);
                            Grid.SetRow(TitleLayoutTransformControl, 1);
                        }

                        Grid.SetColumn(OrientedPanel, 0);
                        Grid.SetRow(OrientedPanel, 0);

                        AxisGrid.Children.Add(TitleLayoutTransformControl);
                        AxisGrid.Children.Add(OrientedPanel);

                        if (Location == AxisLocation.Top)
                        {
                            AxisGrid.Mirror(System.Windows.Controls.Orientation.Horizontal);
                        }
                    }
                }

                Invalidate();
            }
        }

        /// <summary>
        /// Renders the axis.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>The required size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            RenderAxis(availableSize);
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Reformulates the grid when the orientation is changed.  Grid is
        /// either separated into two columns or two rows.  The title is 
        /// inserted with the outermost section from the edge and an oriented
        /// panel is inserted into the innermost section.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnOrientationPropertyChanged(AxisOrientation oldValue, AxisOrientation newValue)
        {
            ArrangeAxisGrid();
            base.OnOrientationPropertyChanged(oldValue, newValue);
        }

        /// <summary>
        /// Updates the visual appearance of the axis when it is invalidated.
        /// </summary>
        /// <param name="args">Information for the invalidated event.</param>
        protected override void OnInvalidated(RoutedEventArgs args)
        {
            InvalidateMeasure();
            base.OnInvalidated(args);
        }

        /// <summary>
        /// Renders the axis if there is a valid value for orientation.
        /// </summary>
        /// <param name="availableSize">The available size in which to render 
        /// the axis.</param>
        private void RenderAxis(Size availableSize)
        {
            if (Orientation != AxisOrientation.None && Location != AxisLocation.Auto)
            {
                Render(availableSize);
            }
        }

        /// <summary>
        /// Renders the axis labels, tick marks, and other visual elements.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        protected abstract void Render(Size availableSize);

        /// <summary>
        /// Invalidates the axis.
        /// </summary>
        protected void Invalidate()
        {
            OnInvalidated(new RoutedEventArgs());
        }
    }
}