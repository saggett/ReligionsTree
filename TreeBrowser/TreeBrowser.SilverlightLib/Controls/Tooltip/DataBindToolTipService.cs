using System.Windows;
using System.Windows.Controls;

namespace TreeBrowser.SilverlightLib.Controls.Tooltip
{
    public class DataBindToolTipService
    {

        public static DependencyProperty ToolTipProperty;

        static DataBindToolTipService()
        {
            ToolTipProperty = DependencyProperty.RegisterAttached
                ("ToolTip", typeof(object),
                typeof(DataBindToolTipService),
                new PropertyMetadata(ToolTipChanged));
        }

        public static void SetToolTip(DependencyObject d, object value)
        {
            d.SetValue(DataBindToolTipService.ToolTipProperty, value);
        }

        public static object GetToolTip(DependencyObject d)
        {
            return d.GetValue(DataBindToolTipService.ToolTipProperty);
        }

        private static void ToolTipChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            if (sender is FrameworkElement)
            {
                FrameworkElement owner = sender as FrameworkElement;
                // wait for it to be in the visual tree so that
                // context can be established
                owner.Loaded += new RoutedEventHandler(owner_Loaded);
            }

        }

        static void owner_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement)
            {
                FrameworkElement owner = sender as FrameworkElement;
                // remove the event handler
                owner.Loaded -= new RoutedEventHandler(owner_Loaded);

                DependencyObject tooltip =
                    owner.GetValue(DataBindToolTipService.ToolTipProperty) as DependencyObject;
                if (tooltip != null)
                {
                    // assign the data context of the current owner control to the tooltip's datacontext
                    tooltip.SetValue(FrameworkElement.DataContextProperty,
                        owner.GetValue(FrameworkElement.DataContextProperty));
                }
                ToolTipService.SetToolTip(owner, tooltip);
            }
        }

    }
}
