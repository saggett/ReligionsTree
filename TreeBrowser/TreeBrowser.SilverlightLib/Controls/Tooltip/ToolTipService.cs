using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Silverlight.Controls
{
    /// <summary>
    /// Represents a service that provides static methods to display a tooltip.
    /// </summary>
    public static class ToolTipService
    {
        private static readonly Dictionary<UIElement, ToolTipTimer> ElementsAndTimers;
        private static readonly Dictionary<UIElement, ToolTip> ElementsAndToolTips;
        internal static readonly DependencyProperty ToolTipObjectProperty;

        /// <summary>
        /// Identifies the ToolTipService.ToolTip dependency property.
        /// </summary>
        public static readonly DependencyProperty ToolTipProperty;

        private static UIElement CurrentElement;
        private static ToolTip CurrentToolTip;
        private static Size LastSize;
        private static FrameworkElement rootVisual;

        static ToolTipService()
        {
            ElementsAndTimers = new Dictionary<UIElement, ToolTipTimer>();
            ElementsAndToolTips = new Dictionary<UIElement, ToolTip>();
            ToolTipProperty = DependencyProperty.RegisterAttached("ToolTip", typeof (object), typeof (ToolTipService),
                                                                  new PropertyMetadata(
                                                                      new PropertyChangedCallback(
                                                                          OnToolTipPropertyChanged)));
            ToolTipObjectProperty = DependencyProperty.RegisterAttached("ToolTipObject", typeof (object),
                                                                        typeof (ToolTipService), null);
        }

        internal static Point MousePosition { get; set; }

        internal static FrameworkElement RootVisual
        {
            get
            {
                SetRootVisual();
                return rootVisual;
            }
        }

        /// <summary>
        /// Gets the tooltip for an object.
        /// </summary>
        /// <param name="element">The UIElement from which the property value is read.</param>
        public static ToolTip GetToolTip(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (ToolTip) element.GetValue(ToolTipProperty);
        }

        /// <summary>
        /// Sets the tooltip for an object.
        /// </summary>
        /// <param name="element">The UIElement to which the attached property is written.</param>
        /// <param name="value">The value to set.</param>
        public static void SetToolTip(UIElement element, ToolTip value)
        {
            SetToolTipInternal(element, value);
        }

        private static void OnElementIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ToolTipTimer timer = ElementsAndTimers[(UIElement) sender];
            if (!(bool) e.NewValue && timer.IsEnabled)
                timer.StopAndReset();
        }

        private static void OnTimerStopped(object sender, EventArgs e)
        {
            CurrentToolTip.IsOpen = false;
        }

        private static void OnToolTipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = (FrameworkElement) d;
            object newValue = e.NewValue;
            if (e.OldValue != null)
                UnregisterToolTip(owner);
            if (newValue != null)
                RegisterToolTip(owner, newValue);
        }

        private static void OnElementMouseEnter(object sender, MouseEventArgs e)
        {
            MousePosition = e.GetPosition(null);
            CurrentElement = (UIElement) sender;
            CurrentToolTip = ElementsAndToolTips[CurrentElement];

            SetRootVisual();

            if (CurrentToolTip.InitialDelay.Equals(0))
                CurrentToolTip.IsOpen = true;

            ElementsAndTimers[CurrentElement].StartAndReset();
        }

        private static void OnTimerTick(object sender, EventArgs e)
        {
            if (CurrentToolTip.InitialDelay == ((ToolTipTimer) sender).CurrentTick)
                CurrentToolTip.IsOpen = true;
        }

        private static void OnElementMouseLeave(object sender, MouseEventArgs e)
        {
            var element = (UIElement) sender;
            ToolTipTimer timer = ElementsAndTimers[element];
            if (timer.IsEnabled)
                timer.StopAndReset();

            if (GetToolTip(element) != CurrentToolTip) return;
            CurrentToolTip.IsOpen = false;
        }

        private static void OnRootMouseMove(object sender, MouseEventArgs e)
        {
            MousePosition = e.GetPosition(null);
        }

        private static void OnRootVisualSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (CurrentToolTip == null) return;
            if (CurrentToolTip.Parent == null) return;

            PerformPlacement(CurrentToolTip.HorizontalOffset, CurrentToolTip.VerticalOffset);
        }

        private static void OnToolTipSizeChanged(object sender, SizeChangedEventArgs e)
        {
            LastSize = e.NewSize;
            if (CurrentToolTip.Parent != null)
                PerformPlacement(CurrentToolTip.HorizontalOffset, CurrentToolTip.VerticalOffset);
        }

        private static ToolTip ConvertToToolTip(object o)
        {
            if (o is ToolTip)
                return o as ToolTip;
            return new ToolTip {Content = o};
        }

        private static void PerformPlacement(double horizontalOffset, double verticalOffset)
        {
            double num = MousePosition.Y + new TextBlock().FontSize + verticalOffset;
            double x = MousePosition.X + horizontalOffset;

            num = Math.Max(2.0, num);
            x = Math.Max(2.0, x);

            double actualHeight = RootVisual.ActualHeight;
            double actualWidth = RootVisual.ActualWidth;
            var rect = new Rect(x, num, LastSize.Width, LastSize.Height);
            var rect2 = new Rect(0.0, 0.0, actualWidth, actualHeight);
            rect2.Intersect(rect);

            var parentPopup = (Popup) CurrentToolTip.Parent;
            if ((Math.Abs(rect2.Width - rect.Width) < 2.0) && (Math.Abs(rect2.Height - rect.Height) < 2.0))
            {
                parentPopup.VerticalOffset = num;
                parentPopup.HorizontalOffset = x;
            }
            else
            {
                if ((num + rect.Height) > actualHeight)
                    num = (actualHeight - rect.Height) - 2.0;
                if (num < 0.0)
                    num = 0.0;
                if ((x + rect.Width) > actualWidth)
                    x = (actualWidth - rect.Width) - 2.0;
                if (x < 0.0)
                    x = 0.0;
                parentPopup.VerticalOffset = num;
                parentPopup.HorizontalOffset = x;
                double num5 = ((num + rect.Height) + 2.0) - actualHeight;
                double num6 = ((x + rect.Width) + 2.0) - actualWidth;
                if ((num6 >= 2.0) || (num5 >= 2.0))
                {
                    num6 = Math.Max(0.0, num6);
                    num5 = Math.Max(0.0, num5);
                    PerformClipping(new Size(rect.Width - num6, rect.Height - num5));
                }
            }
        }

        private static void PerformClipping(Size size)
        {
            var child = VisualTreeHelper.GetChild(CurrentToolTip, 0) as Border;
            if (child == null) return;

            if (size.Width < child.ActualWidth)
                child.Width = size.Width;
            if (size.Height < child.ActualHeight)
                child.Height = size.Height;
        }

        private static void RegisterToolTip(UIElement owner, object toolTip)
        {
            owner.MouseEnter += OnElementMouseEnter;
            owner.MouseLeave += OnElementMouseLeave;
            if (owner is FrameworkElement)
            {
                var fe = (FrameworkElement) owner;
                fe.Loaded += Owner_Loaded;
            }
            ToolTip tooltip = ConvertToToolTip(toolTip);
            owner.SetValue(ToolTipObjectProperty, tooltip);
        }

        private static void Owner_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement))
                return;
            var owner = sender as FrameworkElement;
            // remove the event handler
            owner.Loaded -= Owner_Loaded;

            var tooltip =
                owner.GetValue(ToolTipObjectProperty) as ToolTip;
            if (tooltip != null)
            {
                // assign the data context of the current owner control to the tooltip's datacontext
                tooltip.SetValue(FrameworkElement.DataContextProperty,
                                 owner.GetValue(FrameworkElement.DataContextProperty));
            }
            SetToolTipInternal(owner, tooltip);
        }

        private static void SetRootVisual()
        {
            if ((rootVisual != null) || (Application.Current == null)) return;

            rootVisual = Application.Current.RootVisual as FrameworkElement;
            if (rootVisual == null) return;

            rootVisual.MouseMove += OnRootMouseMove;
            rootVisual.SizeChanged += OnRootVisualSizeChanged;
        }

        private static void SetToolTipInternal(UIElement element, ToolTip value)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            if (value == null)
            {
                ElementsAndToolTips.Remove(element);
                ElementsAndTimers.Remove(element);
                SetToolTipInternal(element, value);
                return;
            }
            var timer = new ToolTipTimer(value.ShowDuration, value.InitialDelay);

            timer.Tick += OnTimerTick;
            timer.Stopped += OnTimerStopped;
            value.SizeChanged += OnToolTipSizeChanged;
            if (element is Control)
                ((Control) element).IsEnabledChanged += OnElementIsEnabledChanged;

            if (ElementsAndTimers.ContainsKey(element))
                ElementsAndTimers.Remove(element);
            if (ElementsAndToolTips.ContainsKey(element))
                ElementsAndToolTips.Remove(element);

            ElementsAndTimers.Add(element, timer);
            ElementsAndToolTips.Add(element, value);

            element.SetValue(ToolTipProperty, value);
        }

        private static void UnregisterToolTip(UIElement owner)
        {
            owner.MouseEnter -= OnElementMouseEnter;
            owner.MouseLeave -= OnElementMouseLeave;
            if (owner.GetValue(ToolTipObjectProperty) == null) return;
            var tip = (ToolTip) owner.GetValue(ToolTipObjectProperty);
            if (tip.IsOpen)
                tip.IsOpen = false;
            owner.ClearValue(ToolTipObjectProperty);
        }
    }
}