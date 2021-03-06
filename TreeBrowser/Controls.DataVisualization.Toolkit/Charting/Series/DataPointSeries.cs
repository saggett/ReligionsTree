﻿// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Collections;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents a control that contains a dynamic data series.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public abstract partial class DataPointSeries : Series
    {
        /// <summary>
        /// The name of the template part with the plot area.
        /// </summary>
        protected const string PlotAreaName = "PlotArea";

#if !SILVERLIGHT
        /// <summary>
        /// This event is raised when the selection is changed.
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent;
#endif
        /// <summary>
        /// Queue of hide/reveal storyboards to play.
        /// </summary>
        private StoryboardQueue _storyBoardQueue = new StoryboardQueue();

        /// <summary>
        /// The binding used to identify the dependent value binding.
        /// </summary>
        private Binding _dependentValueBinding;

        /// <summary>
        /// Gets or sets the Binding to use for identifying the dependent value.
        /// </summary>
        public Binding DependentValueBinding
        {
            get
            {
                return _dependentValueBinding;
            }
            set
            {
                if (value != _dependentValueBinding)
                {
                    _dependentValueBinding = value;
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Data points collection sorted by object.
        /// </summary>
        private MultipleDictionary<object, DataPoint> _dataPointsByObject = 
            new MultipleDictionary<object, DataPoint>(
                true,
                new GenericEqualityComparer<object>(
                    (left, right) =>
                        left.Equals(right),
                    (obj) => obj.GetHashCode()),
                new GenericEqualityComparer<DataPoint>(
                    (left, right) =>
                        object.ReferenceEquals(left, right),
                    (obj) => obj.GetHashCode()));

        /// <summary>
        /// Gets or sets the Binding Path to use for identifying the dependent value.
        /// </summary>
        public string DependentValuePath
        {
            get
            {
                return (null != DependentValueBinding) ? DependentValueBinding.Path.Path : null;
            }
            set
            {
                if (null == value)
                {
                    DependentValueBinding = null;
                }
                else
                {
                    DependentValueBinding = new Binding(value);
                }
            }
        }

        /// <summary>
        /// The binding used to identify the independent value binding.
        /// </summary>
        private Binding _independentValueBinding;

        /// <summary>
        /// Gets or sets the Binding to use for identifying the independent value.
        /// </summary>
        public Binding IndependentValueBinding
        {
            get
            {
                return _independentValueBinding;
            }
            set
            {
                if (_independentValueBinding != value)
                {
                    _independentValueBinding = value;
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Binding Path to use for identifying the independent value.
        /// </summary>
        public string IndependentValuePath
        {
            get
            {
                return (null != IndependentValueBinding) ? IndependentValueBinding.Path.Path : null;
            }
            set
            {
                if (null == value)
                {
                    IndependentValueBinding = null;
                }
                else
                {
                    IndependentValueBinding = new Binding(value);
                }
            }
        }

        #region public IEnumerable ItemsSource
        /// <summary>
        /// Gets or sets a collection used to contain the data points of the Series.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(DataPointSeries),
                new PropertyMetadata(OnItemsSourceChanged));

        /// <summary>
        /// ItemsSourceProperty property changed callback.
        /// </summary>
        /// <param name="o">Series for which the ItemsSource changed.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnItemsSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((DataPointSeries)o).OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        /// <summary>
        /// Called when the ItemsSource property changes.
        /// </summary>
        /// <param name="oldValue">Old value of the ItemsSource property.</param>
        /// <param name="newValue">New value of the ItemsSource property.</param>
        protected virtual void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            // Remove handler for oldValue.CollectionChanged (if present)
            INotifyCollectionChanged oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;
            if (null != oldValueINotifyCollectionChanged)
            {
                // Detach the WeakEventListener
                if (null != _weakEventListener)
                {
                    _weakEventListener.Detach();
                    _weakEventListener = null;
                }
            }

            // Add handler for newValue.CollectionChanged (if possible)
            INotifyCollectionChanged newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (null != newValueINotifyCollectionChanged)
            {
                // Use a WeakEventListener so that the backwards reference doesn't keep this object alive
                _weakEventListener = new WeakEventListener<DataPointSeries, object, NotifyCollectionChangedEventArgs>(this);
                _weakEventListener.OnEventAction = (instance, source, eventArgs) => instance.ItemsSourceCollectionChanged(source, eventArgs);
                _weakEventListener.OnDetachAction = (weakEventListener) => newValueINotifyCollectionChanged.CollectionChanged -= weakEventListener.OnEvent;
                newValueINotifyCollectionChanged.CollectionChanged += _weakEventListener.OnEvent;
            }

            if (TemplateApplied)
            {
                Refresh();
            }
        }
        #endregion public IEnumerable ItemsSource

        #region public AnimationSequence AnimationSequence
        /// <summary>
        /// Gets or sets the animation sequence to use for the DataPoints of the Series.
        /// </summary>
        public AnimationSequence AnimationSequence
        {
            get { return (AnimationSequence)GetValue(AnimationSequenceProperty); }
            set { SetValue(AnimationSequenceProperty, value); }
        }

        /// <summary>
        /// Gets a stream of the active data points in the plot area.
        /// </summary>
        protected virtual IEnumerable<DataPoint> ActiveDataPoints
        {
            get
            {
                return (null != PlotArea) ?
                    PlotArea.Children.OfType<DataPoint>().Where(dataPoint => dataPoint.IsActive) :
                    Enumerable.Empty<DataPoint>();
            }
        }

        /// <summary>
        /// Gets the number of active data points in the plot area.
        /// </summary>
        protected int ActiveDataPointCount { get; private set; }

        #region public bool IsSelectionEnabled
        /// <summary>
        /// Gets or sets a value indicating whether elements in the series can
        /// be selected.
        /// </summary>
        public bool IsSelectionEnabled
        {
            get { return (bool)GetValue(IsSelectionEnabledProperty); }
            set { SetValue(IsSelectionEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the IsSelectionEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectionEnabledProperty =
            DependencyProperty.Register(
                "IsSelectionEnabled",
                typeof(bool),
                typeof(DataPointSeries),
                new PropertyMetadata(false, OnIsSelectionEnabledPropertyChanged));

        /// <summary>
        /// IsSelectionEnabledProperty property changed handler.
        /// </summary>
        /// <param name="d">DynamicSeries that changed its IsSelectionEnabled.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsSelectionEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPointSeries source = (DataPointSeries)d;
            bool oldValue = (bool)e.OldValue;
            bool newValue = (bool)e.NewValue;
            source.OnIsSelectionEnabledPropertyChanged(oldValue, newValue);
        }

        /// <summary>
        /// IsSelectionEnabledProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnIsSelectionEnabledPropertyChanged(bool oldValue, bool newValue)
        {
            foreach (DataPoint dataPoint in ActiveDataPoints)
            {
                dataPoint.IsSelectionEnabled = newValue;
            }
        }
        #endregion public bool IsSelectionEnabled

        /// <summary>
        /// Identifies the AnimationSequence dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationSequenceProperty =
            DependencyProperty.Register(
                "AnimationSequence",
                typeof(AnimationSequence),
                typeof(DataPointSeries),
                new PropertyMetadata(AnimationSequence.Simultaneous));
        #endregion public AnimationSequence AnimationSequence

        /// <summary>
        /// WeakEventListener used to handle INotifyCollectionChanged events.
        /// </summary>
        private WeakEventListener<DataPointSeries, object, NotifyCollectionChangedEventArgs> _weakEventListener;

        /// <summary>
        /// The plot area canvas.
        /// </summary>
        private Panel _plotArea;

        /// <summary>
        /// Gets the plot area canvas.
        /// </summary>
        internal Panel PlotArea
        {
            get
            {
                return _plotArea;
            }
            private set
            {
                Panel oldValue = _plotArea;
                _plotArea = value;
                if (_plotArea != oldValue)
                {
                    OnPlotAreaChanged(oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets the size of the plot area.
        /// </summary>
        /// <remarks>
        /// Use this method instead of PlotArea.ActualWidth/ActualHeight
        /// because the ActualWidth and ActualHeight properties are set after
        /// the SizeChanged handler runs.
        /// </remarks>
        protected Size PlotAreaSize { get; private set; }

        /// <summary>
        /// An event raised when the selection has been changed.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Tracks whether a call to OnSelectedItemPropertyChanged is already in progress.
        /// </summary>
        private bool _processingOnSelectedItemPropertyChanged;

        #region public object SelectedItem
        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty) as object; }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(DataPointSeries),
                new PropertyMetadata(null, OnSelectedItemPropertyChanged));

        /// <summary>
        /// Called when the value of the SelectedItem property changes.
        /// </summary>
        /// <param name="d">DynamicSeries that changed its SelectedItem.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPointSeries source = (DataPointSeries)d;
            object oldValue = (object)e.OldValue;
            object newValue = (object)e.NewValue;
            source.OnSelectedItemPropertyChanged(oldValue, newValue);
        }

        /// <summary>
        /// Called when the value of the SelectedItem property changes.
        /// </summary>
        /// <param name="oldValue">The old selected index.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnSelectedItemPropertyChanged(object oldValue, object newValue)
        {
            DataPoint dataPoint = null;
            if (null != newValue)
            {
                // Find the corresponding Control
                dataPoint = _dataPointsByObject[newValue].Where(dp => dp.IsActive).FirstOrDefault();
                if (null == dataPoint)
                {
                    // None; clear SelectedItem
                    try
                    {
                        _processingOnSelectedItemPropertyChanged = true;
                        SelectedItem = null;
                        // Clear newValue so the SelectionChanged event will be correct (or suppressed)
                        newValue = null;
                    }
                    finally
                    {
                        _processingOnSelectedItemPropertyChanged = false;
                    }
                }
            }
            // Unselect everything else
            foreach (DataPoint dataPointUnselect in ActiveDataPoints.Where(activeDataPoint => (activeDataPoint != dataPoint) && activeDataPoint.IsSelected))
            {
                dataPointUnselect.IsSelectedChanged -= OnDataPointIsSelectedChanged;
                dataPointUnselect.IsSelected = false;
                dataPointUnselect.IsSelectedChanged += OnDataPointIsSelectedChanged;
            }
            if ((null != dataPoint) && !dataPoint.IsSelected)
            {
                // Select the new data point
                dataPoint.IsSelectedChanged -= OnDataPointIsSelectedChanged;
                dataPoint.IsSelected = true;
                dataPoint.IsSelectedChanged += OnDataPointIsSelectedChanged;
            }

            // Fire SelectionChanged (if appropriate)
            SelectionChangedEventHandler handler = SelectionChanged;
            if ((null != handler) && !_processingOnSelectedItemPropertyChanged && (oldValue != newValue))
            {
                IList oldValues = new List<object>();
                if (oldValue != null)
                {
                    oldValues.Add(oldValue);
                }
                IList newValues = new List<object>();
                if (newValue != null)
                {
                    newValues.Add(newValue);
                }
#if SILVERLIGHT
                handler(this, new SelectionChangedEventArgs(oldValues, newValues));
#else
                handler(this, new SelectionChangedEventArgs(DataPointSeries.SelectionChangedEvent, oldValues, newValues));
#endif
            }
        }
        #endregion public object SelectedItem

        /// <summary>
        /// Gets or sets a value indicating whether the template has been
        /// applied.
        /// </summary>
        private bool TemplateApplied { get; set; }

        #region public Style LegendItemStyle
        /// <summary>
        /// Gets or sets the style to use for the legend items.
        /// </summary>
        public Style LegendItemStyle
        {
            get { return GetValue(LegendItemStyleProperty) as Style; }
            set { SetValue(LegendItemStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the LegendItemStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendItemStyleProperty =
            DependencyProperty.Register(
                "LegendItemStyle",
                typeof(Style),
                typeof(DataPointSeries),
                new PropertyMetadata(null, OnLegendItemStylePropertyChanged));

        /// <summary>
        /// LegendItemStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">DataPointSeries that changed its LegendItemStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnLegendItemStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPointSeries source = d as DataPointSeries;
            source.OnLegendItemStylePropertyChanged((Style)e.OldValue, (Style)e.NewValue);
        }

        /// <summary>
        /// Called when the value of the LegendItemStyle property changes.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnLegendItemStylePropertyChanged(Style oldValue, Style newValue)
        {
            foreach (LegendItem legendItem in LegendItems.OfType<LegendItem>())
            {
                legendItem.SetStyle(newValue);
            }
        }
        #endregion public Style LegendItemStyle

        /// <summary>
        /// Gets or sets the Geometry used to clip DataPoints to the PlotArea bounds.
        /// </summary>
        private RectangleGeometry ClipGeometry { get; set; }

        /// <summary>
        /// Indicates whether a call to Refresh is required when the control's
        /// size changes.
        /// </summary>
        private bool _needRefreshWhenSizeChanged = true;

        #region public TimeSpan TransitionDuration
        /// <summary>
        /// Gets or sets the duration of the value Transition animation.
        /// </summary>
        public TimeSpan TransitionDuration
        {
            get { return (TimeSpan)GetValue(TransitionDurationProperty); }
            set { SetValue(TransitionDurationProperty, value); }
        }

        /// <summary>
        /// Identifies the TransitionDuration dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionDurationProperty =
            DependencyProperty.Register(
                "TransitionDuration",
                typeof(TimeSpan),
                typeof(DataPointSeries),
                new PropertyMetadata(TimeSpan.FromSeconds(0.5)));
        #endregion public TimeSpan TransitionDuration

#if !SILVERLIGHT
        /// <summary>
        /// Initializes the static members of the DataPointSeries class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Dependency properties are initialized in-line.")]
        static DataPointSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataPointSeries), new FrameworkPropertyMetadata(typeof(DataPointSeries)));
        }

#endif
        /// <summary>
        /// Initializes a new instance of the Series class.
        /// </summary>
        internal DataPointSeries()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(DataPointSeries);
#endif
            ClipGeometry = new RectangleGeometry();
            Clip = ClipGeometry;
        }

        /// <summary>
        /// Adds an object to the series host by creating a corresponding data point
        /// for it.
        /// </summary>
        /// <param name="dataContext">The object to add to the series host.</param>
        /// <returns>The data point created for the object.</returns>
        protected virtual DataPoint AddObject(object dataContext)
        {
            if (ShouldCreateDataPoint(dataContext))
            {
                DataPoint dataPoint = CreateAndPrepareDataPoint(dataContext);
                _dataPointsByObject.Add(dataContext, dataPoint);
                AddDataPoint(dataPoint);
                return dataPoint;
            }
            return null;
        }

        /// <summary>
        /// Returns whether a data point should be created for the data context.
        /// </summary>
        /// <param name="dataContext">The data context that will be used for the
        /// data point.</param>
        /// <returns>A value indicating whether a data point should be created
        /// for the data context.</returns>
        protected virtual bool ShouldCreateDataPoint(object dataContext)
        {
            return true;
        }

        /// <summary>
        /// Returns the index at which to insert data point in the plot area
        /// child collection.
        /// </summary>
        /// <param name="dataPoint">The data point to retrieve the insertion
        /// index for.</param>
        /// <returns>The insertion index.</returns>
        protected virtual int GetInsertionIndex(DataPoint dataPoint)
        {
            return PlotArea.Children.Count;
        }

        /// <summary>
        /// Adds a data point to the plot area.
        /// </summary>
        /// <param name="dataPoint">The data point to add to the plot area.
        /// </param>
        protected virtual void AddDataPoint(DataPoint dataPoint)
        {
            if (dataPoint.IsSelected)
            {
                Select(dataPoint);
            }

            if (PlotArea != null)
            {
                // Positioning data point outside the visible area.
                Canvas.SetLeft(dataPoint, float.MinValue);
                Canvas.SetTop(dataPoint, float.MinValue);
                dataPoint.IsSelectionEnabled = IsSelectionEnabled;
                AttachEventHandlersToDataPoint(dataPoint);
                PlotArea.Children.Insert(GetInsertionIndex(dataPoint), dataPoint);
                ActiveDataPointCount++;
            }
        }

        /// <summary>
        /// Retrieves the data point corresponding to the object passed as the
        /// parameter.
        /// </summary>
        /// <param name="dataContext">The data context used for the point.
        /// </param>
        /// <returns>The data point associated with the object.</returns>
        protected virtual DataPoint GetDataPoint(object dataContext)
        {
            DataPoint dataPoint = _dataPointsByObject[dataContext].FirstOrDefault();
            return dataPoint;
        }

        /// <summary>
        /// Creates and prepares a data point.
        /// </summary>
        /// <param name="dataContext">The object to use as the data context
        /// of the data point.</param>
        /// <returns>The newly created data point.</returns>
        private DataPoint CreateAndPrepareDataPoint(object dataContext)
        {
            DataPoint dataPoint = CreateDataPoint();
            PrepareDataPoint(dataPoint, dataContext);
            return dataPoint;
        }

        /// <summary>
        /// Returns a Control suitable for the Series.
        /// </summary>
        /// <returns>The DataPoint instance.</returns>
        protected abstract DataPoint CreateDataPoint();

        /// <summary>
        /// Creates a legend item.
        /// </summary>
        /// <returns>A legend item for insertion in the legend items collection.
        /// </returns>
        protected virtual LegendItem CreateLegendItem()
        {
            LegendItem legendItem = new LegendItem();
            legendItem.SetStyle(LegendItemStyle);

            return legendItem;
        }

        /// <summary>
        /// Method that handles the ObservableCollection.CollectionChanged event for the ItemsSource property.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Pass notification on
            OnItemsSourceCollectionChanged(ItemsSource, e);
        }

        /// <summary>
        /// Updates data points collection with items retrieved from items
        /// source and removes the old items.
        /// </summary>
        /// <param name="newItems">The items to load.</param>
        /// <param name="oldItems">The items to remove.</param>
        protected void LoadDataPoints(IEnumerable newItems, IEnumerable oldItems)
        {
            if ((PlotArea != null) && (SeriesHost != null))
            {
                IList<DataPoint> removedDataPoints = new List<DataPoint>();
                if (oldItems != null)
                {
                    if (oldItems != null)
                    {
                        // Remove existing objects from internal collections.
                        foreach (object dataContext in oldItems)
                        {
                            DataPoint removedDataPoint = RemoveObject(dataContext);
                            _dataPointsByObject.Remove(dataContext, removedDataPoint);
                            if (removedDataPoint != null)
                            {
                                removedDataPoints.Add(removedDataPoint);
                            }
                        }
                    }
                    StaggeredStateChange(removedDataPoints, removedDataPoints.Count, DataPointState.Hiding);
                }

                IList<DataPoint> addedDataPoints = new List<DataPoint>();
                if (newItems != null)
                {
                    foreach (object dataContext in newItems)
                    {
                        DataPoint dataPoint = AddObject(dataContext);
                        if (dataPoint != null)
                        {
                            addedDataPoints.Add(dataPoint);
                        }
                    }
                }

                OnDataPointsChanged(addedDataPoints, removedDataPoints);
            }
        }

        /// <summary>
        /// Attaches handler plot area after loading it from XAML.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Get reference to new ChartArea and hook its SizeChanged event
            PlotArea = GetTemplateChild(PlotAreaName) as Panel;

            if (!TemplateApplied)
            {
                TemplateApplied = true;
                SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
            }
        }

        /// <summary>
        /// Invokes an action when the plot area's layout is updated.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        internal void InvokeOnLayoutUpdated(Action action)
        {
            EventHandler handler = null;
            handler = delegate
            {
                this.PlotArea.LayoutUpdated -= handler;
                action();
            };

            this.PlotArea.LayoutUpdated += handler;
        }

        /// <summary>
        /// Handles changes to the SeriesHost property.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected override void OnSeriesHostPropertyChanged(ISeriesHost oldValue, ISeriesHost newValue)
        {
            base.OnSeriesHostPropertyChanged(oldValue, newValue);

            if (null == newValue)
            {
                // Reset flag to prepare for next addition to a series host
                _needRefreshWhenSizeChanged = true;
            }
        }

        /// <summary>
        /// Called after data points have been loaded from the items source.
        /// </summary>
        /// <param name="newDataPoints">New active data points.</param>
        /// <param name="oldDataPoints">Old inactive data points.</param>
        protected virtual void OnDataPointsChanged(IList<DataPoint> newDataPoints, IList<DataPoint> oldDataPoints)
        {
            StaggeredStateChange(newDataPoints, newDataPoints.Count(), DataPointState.Showing);
        }

        /// <summary>
        /// Method called when the ItemsSource collection changes.
        /// </summary>
        /// <param name="collection">New value of the collection.</param>
        /// <param name="e">Information about the change.</param>
        protected virtual void OnItemsSourceCollectionChanged(IEnumerable collection, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                IList<DataPoint> updatedDataPoints = new List<DataPoint>();

                for (int index = 0; index < e.OldItems.Count; index++)
                {
                    DataPoint dataPointToUpdate = _dataPointsByObject[e.OldItems[index]].Except(updatedDataPoints).FirstOrDefault();
                    if (null != dataPointToUpdate)
                    {
                        updatedDataPoints.Add(dataPointToUpdate);
                        dataPointToUpdate.DataContext = e.NewItems[index];
                        _dataPointsByObject.Remove(e.OldItems[index], dataPointToUpdate);
                        _dataPointsByObject.Add(e.NewItems[index], dataPointToUpdate);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove)
            {
                LoadDataPoints(
                    e.NewItems,
                    e.OldItems);
            }
            else
            {
                Refresh();
            }
        }

        /// <summary>
        /// Removes items from the existing plot area and adds items to new
        /// plot area.
        /// </summary>
        /// <param name="oldValue">The previous plot area.</param>
        /// <param name="newValue">The new plot area.</param>
        protected virtual void OnPlotAreaChanged(Panel oldValue, Panel newValue)
        {
            if (oldValue != null)
            {
                foreach (DataPoint dataPoint in ActiveDataPoints)
                {
                    oldValue.Children.Remove(dataPoint);
                }
            }

            if (newValue != null)
            {
                foreach (DataPoint dataPoint in ActiveDataPoints)
                {
                    newValue.Children.Add(dataPoint);
                }
            }
        }

        /// <summary>
        /// Updates the visual appearance of all the data points when the size
        /// changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PlotAreaSize = e.NewSize;
            ClipGeometry.Rect = new Rect(0, 0, PlotAreaSize.Width, PlotAreaSize.Height);
            if (null != PlotArea)
            {
                PlotArea.Width = PlotAreaSize.Width;
                PlotArea.Height = PlotAreaSize.Height;

                if (_needRefreshWhenSizeChanged)
                {
                    _needRefreshWhenSizeChanged = false;
                    Refresh();
                }
                else
                {
                    UpdateDataPoints(ActiveDataPoints);
                }
            }
        }

        /// <summary>
        /// Refreshes data from data source and renders the series.
        /// </summary>
        public override void Refresh()
        {
            try
            {
                IEnumerable itemsSource = ItemsSource;
                LoadDataPoints(itemsSource, ActiveDataPoints.Select(dataPoint => dataPoint.DataContext));
            }
            catch
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                {
                    // Suppress exception to improve the design-time experience
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Removes an object from the series host by removing its corresponding
        /// data point.
        /// </summary>
        /// <param name="dataContext">The object to remove from the series data
        /// source.</param>
        /// <returns>The data point corresponding to the removed object.
        /// </returns>
        protected virtual DataPoint RemoveObject(object dataContext)
        {
            DataPoint dataPoint = GetDataPoint(dataContext);

            if (dataPoint != null)
            {
                RemoveDataPoint(dataPoint);
            }
            return dataPoint;
        }

        /// <summary>
        /// Removes a data point from the plot area.
        /// </summary>
        /// <param name="dataPoint">The data point to remove.</param>
        protected virtual void RemoveDataPoint(DataPoint dataPoint)
        {
            if (dataPoint.IsSelected)
            {
                Unselect(dataPoint);
            }

            ActiveDataPointCount--;
            dataPoint.State = DataPointState.PendingRemoval;
        }

        /// <summary>
        /// Gets a value indicating whether all data points are being
        /// updated.
        /// </summary>
        protected bool UpdatingDataPoints { get; private set; }

        /// <summary>
        /// Updates the visual representation of all data points in the plot
        /// area.
        /// </summary>
        /// <param name="dataPoints">A sequence of data points to update.
        /// </param>
        protected virtual void UpdateDataPoints(IEnumerable<DataPoint> dataPoints)
        {
            UpdatingDataPoints = true;

            DetachEventHandlersFromDataPoints(dataPoints);
            try
            {
                OnBeforeUpdateDataPoints();

                foreach (DataPoint dataPoint in dataPoints)
                {
                    UpdateDataPoint(dataPoint);
                }

                OnAfterUpdateDataPoints();
            }
            finally
            {
                AttachEventHandlersToDataPoints(dataPoints);
                UpdatingDataPoints = false;
            }
        }

        /// <summary>
        /// Attaches event handlers to the data points.
        /// </summary>
        /// <param name="dataPoints">A sequence of data points.</param>
        private void AttachEventHandlersToDataPoints(IEnumerable<DataPoint> dataPoints)
        {
            foreach (DataPoint dataPoint in dataPoints)
            {
                AttachEventHandlersToDataPoint(dataPoint);
            }
        }

        /// <summary>
        /// Detaches event handlers from the data points.
        /// </summary>
        /// <param name="dataPoints">A sequence of data points.</param>
        private void DetachEventHandlersFromDataPoints(IEnumerable<DataPoint> dataPoints)
        {
            foreach (DataPoint dataPoint in dataPoints)
            {
                DetachEventHandlersFromDataPoint(dataPoint);
            }
        }

        /// <summary>
        /// Attaches event handlers to a data point.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        protected virtual void AttachEventHandlersToDataPoint(DataPoint dataPoint)
        {
            dataPoint.IsSelectedChanged += OnDataPointIsSelectedChanged;
            dataPoint.ActualDependentValueChanged += OnDataPointActualDependentValueChanged;
            dataPoint.ActualIndependentValueChanged += OnDataPointActualIndependentValueChanged;
            dataPoint.DependentValueChanged += OnDataPointDependentValueChanged;
            dataPoint.IndependentValueChanged += OnDataPointIndependentValueChanged;
            dataPoint.StateChanged += OnDataPointStateChanged;
        }

        /// <summary>
        /// Unselects a data point.
        /// </summary>
        /// <param name="dataPoint">The data point to unselect.</param>
        private void Unselect(DataPoint dataPoint)
        {
            if (dataPoint.DataContext.Equals(SelectedItem))
            {
                SelectedItem = null;
            }
        }

        /// <summary>
        /// Selects a data point.
        /// </summary>
        /// <param name="dataPoint">The data point to select.</param>
        private void Select(DataPoint dataPoint)
        {
            SelectedItem = dataPoint.DataContext;
        }

        /// <summary>
        /// Method executed when a data point is either selected or unselected.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void OnDataPointIsSelectedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            DataPoint dataPoint = sender as DataPoint;

            if (e.NewValue)
            {
                Select(dataPoint);
            }
            else
            {
                Unselect(dataPoint);
            }
        }

        /// <summary>
        /// Detaches event handlers from a data point.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        protected virtual void DetachEventHandlersFromDataPoint(DataPoint dataPoint)
        {
            dataPoint.IsSelectedChanged -= OnDataPointIsSelectedChanged;
            dataPoint.ActualDependentValueChanged -= OnDataPointActualDependentValueChanged;
            dataPoint.ActualIndependentValueChanged -= OnDataPointActualIndependentValueChanged;
            dataPoint.DependentValueChanged -= OnDataPointDependentValueChanged;
            dataPoint.IndependentValueChanged -= OnDataPointIndependentValueChanged;
            dataPoint.StateChanged -= OnDataPointStateChanged;
        }

        /// <summary>
        /// This method that executes before data points are updated.
        /// </summary>
        protected virtual void OnBeforeUpdateDataPoints()
        {
        }

        /// <summary>
        /// This method that executes after data points are updated.
        /// </summary>
        protected virtual void OnAfterUpdateDataPoints()
        {
        }

        /// <summary>
        /// Updates the visual representation of a single data point in the plot
        /// area.
        /// </summary>
        /// <param name="dataPoint">The data point to update.</param>
        protected abstract void UpdateDataPoint(DataPoint dataPoint);

        /// <summary>
        /// Prepares a data point by extracting binding it to a data context
        /// object.
        /// </summary>
        /// <param name="dataPoint">A data point.</param>
        /// <param name="dataContext">A data context object.</param>
        protected virtual void PrepareDataPoint(DataPoint dataPoint, object dataContext)
        {
            // Create a Control with DataContext set to the data source
            dataPoint.DataContext = dataContext;

            // Set bindings for IndependentValue/DependentValue
            if (IndependentValueBinding != null)
            {
                dataPoint.SetBinding(DataPoint.IndependentValueProperty, IndependentValueBinding);
            }

            if (DependentValueBinding == null)
            {
                dataPoint.SetBinding(DataPoint.DependentValueProperty, new Binding());
            }
            else
            {
                dataPoint.SetBinding(DataPoint.DependentValueProperty, DependentValueBinding);
            }
        }

        /// <summary>
        /// Reveals data points using a storyboard.
        /// </summary>
        /// <param name="dataPoints">The data points to change the state of.
        /// </param>
        /// <param name="dataPointCount">The number of data points in the sequence.</param>
        /// <param name="newState">The state to change to.</param>
        private void StaggeredStateChange(IEnumerable<DataPoint> dataPoints, int dataPointCount, DataPointState newState)
        {
            if (PlotArea == null || dataPointCount == 0)
            {
                return;
            }

            string guid = Guid.NewGuid().ToString();
            Storyboard stateChangeStoryBoard = new Storyboard();
            stateChangeStoryBoard.Completed +=
                (sender, args) =>
                {
                    PlotArea.Resources.Remove(guid);
                };

            dataPoints.ForEachWithIndex((dataPoint, count) =>
            {
                // Create an Animation
                ObjectAnimationUsingKeyFrames objectAnimationUsingKeyFrames = new ObjectAnimationUsingKeyFrames();
                Storyboard.SetTarget(objectAnimationUsingKeyFrames, dataPoint);
                Storyboard.SetTargetProperty(objectAnimationUsingKeyFrames, new PropertyPath("State"));

                // Create a key frame
                DiscreteObjectKeyFrame discreteObjectKeyFrame = new DiscreteObjectKeyFrame();
                discreteObjectKeyFrame.Value = newState;

                // Create the specified animation type
                switch (AnimationSequence)
                {
                    case AnimationSequence.Simultaneous:
                        discreteObjectKeyFrame.KeyTime = TimeSpan.Zero;
                        break;
                    case AnimationSequence.FirstToLast:
                        discreteObjectKeyFrame.KeyTime = TimeSpan.FromMilliseconds(1000 * ((double)count / dataPointCount));
                        break;
                    case AnimationSequence.LastToFirst:
                        discreteObjectKeyFrame.KeyTime = TimeSpan.FromMilliseconds(1000 * ((double)(dataPointCount - count - 1) / dataPointCount));
                        break;
                }

                // Add the Animation to the Storyboard
                objectAnimationUsingKeyFrames.KeyFrames.Add(discreteObjectKeyFrame);
                stateChangeStoryBoard.Children.Add(objectAnimationUsingKeyFrames);
            });

            _storyBoardQueue.Enqueue(stateChangeStoryBoard);
        }

        /// <summary>
        /// Handles data point state property change.
        /// </summary>
        /// <param name="sender">The data point.</param>
        /// <param name="args">Information about the event.</param>
        private void OnDataPointStateChanged(object sender, RoutedPropertyChangedEventArgs<DataPointState> args)
        {
            OnDataPointStateChanged(sender as DataPoint, args.OldValue, args.NewValue);
        }

        /// <summary>
        /// Handles data point state property change.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnDataPointStateChanged(DataPoint dataPoint, DataPointState oldValue, DataPointState newValue)
        {
            if (dataPoint.State == DataPointState.Hidden)
            {
                DetachEventHandlersFromDataPoint(dataPoint);
                PlotArea.Children.Remove(dataPoint);
            }
        }

        /// <summary>
        /// Handles data point actual dependent value property changes.
        /// </summary>
        /// <param name="sender">The data point.</param>
        /// <param name="args">Information about the event.</param>
        private void OnDataPointActualDependentValueChanged(object sender, RoutedPropertyChangedEventArgs<IComparable> args)
        {
            OnDataPointActualDependentValueChanged(sender as DataPoint, args.OldValue, args.NewValue);
        }

        /// <summary>
        /// Handles data point actual dependent value property change.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnDataPointActualDependentValueChanged(DataPoint dataPoint, IComparable oldValue, IComparable newValue)
        {
        }

        /// <summary>
        /// Handles data point actual independent value property changes.
        /// </summary>
        /// <param name="sender">The data point.</param>
        /// <param name="args">Information about the event.</param>
        private void OnDataPointActualIndependentValueChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
        {
            OnDataPointActualIndependentValueChanged(sender as DataPoint, args.OldValue, args.NewValue);
        }

        /// <summary>
        /// Handles data point actual independent value property change.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnDataPointActualIndependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
        }

        /// <summary>
        /// Handles data point dependent value property changes.
        /// </summary>
        /// <param name="sender">The data point.</param>
        /// <param name="args">Information about the event.</param>
        private void OnDataPointDependentValueChanged(object sender, RoutedPropertyChangedEventArgs<IComparable> args)
        {
            OnDataPointDependentValueChanged(sender as DataPoint, args.OldValue, args.NewValue);
        }

        /// <summary>
        /// Handles data point dependent value property change.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnDataPointDependentValueChanged(DataPoint dataPoint, IComparable oldValue, IComparable newValue)
        {
        }

        /// <summary>
        /// Handles data point independent value property changes.
        /// </summary>
        /// <param name="sender">The data point.</param>
        /// <param name="args">Information about the event.</param>
        private void OnDataPointIndependentValueChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
        {
            OnDataPointIndependentValueChanged(sender as DataPoint, args.OldValue, args.NewValue);
        }

        /// <summary>
        /// Handles data point independent value property change.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnDataPointIndependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
        }
    }
}