using System.Windows;
using System;
using System.Windows.Controls;

namespace Silverlight.Controls
{
    /// <summary>
    /// Represents a control that creates a pop-up window that displays information for an element in the UI.
    /// </summary>
    public class ToolTip : System.Windows.Controls.ToolTip
    {

        /// <summary>
        /// Identifies the ToolTip.ShowDuration dependency property.
        /// </summary>
        /// <remarks>Default value is 5 seconds.</remarks>
        public static readonly DependencyProperty ShowDurationProperty
            = DependencyProperty.RegisterAttached("ShowDuration", typeof(int), typeof(ToolTip),
                                                  new PropertyMetadata(5, OnShowDurationPropertyChanged));

        /// <summary>
        /// Identifies the ToolTip.InitialDelay dependency property.
        /// </summary>
        /// <remarks>Default value is 1 second.</remarks>
        public static readonly DependencyProperty InitialDelayProperty
            = DependencyProperty.RegisterAttached("InitialDelay", typeof(int), typeof(ToolTip),
                                                  new PropertyMetadata(1, OnInitialDelayPropertyChanged));

        private static void OnInitialDelayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ToolTip))
                throw new InvalidOperationException("You can only set ToolTip.ShowDurationProperty on ToolTip object.");
        }
        private static void OnShowDurationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(!(d is ToolTip))
                throw new InvalidOperationException("You can only set ToolTip.ShowDurationProperty on ToolTip object.");
        }

        /// <summary>
        /// Gets or sets the display time of this ToolTip instance in seconds.
        /// </summary>
        public int ShowDuration
        {
            get { return (int)GetValue(ShowDurationProperty); }
            set { SetValue(ShowDurationProperty, value); }
        }
        /// <summary>
        /// Gets or sets the initial delay for the tooltip to show in seconds.
        /// </summary>
        public int InitialDelay
        {
            get { return (int)GetValue(InitialDelayProperty); }
            set { SetValue(InitialDelayProperty, value); }
        }

    }
}