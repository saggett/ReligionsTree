using System.Windows;
using System.Windows.Controls;

namespace TreeBrowser.SilverlightLib.Controls
{
    public class DataBindingTooltip
    {
        public static DependencyProperty TooltipProperty;

        static DataBindingTooltip()
        {
            TooltipProperty = DependencyProperty.RegisterAttached
                ("Tooltip", typeof (object),
                 typeof (DataBindingTooltip),
                 new PropertyMetadata(TooltipChanged));
        }

        public static void SetTooltip(DependencyObject d, object value)
        {
            d.SetValue(TooltipProperty, value);
        }

        public static object GetToolTip(DependencyObject d)
        {
            return d.GetValue(TooltipProperty);
        }

        private static void TooltipChanged(DependencyObject sender,
                                           DependencyPropertyChangedEventArgs e)
        {
            if (sender is FrameworkElement)
            {
                var owner = sender as FrameworkElement;
                // wait for it to be in the visual tree so that
                // context can be established
                owner.Loaded += Owner_Loaded;
            }
        }

        private static void Owner_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement)
            {
                var owner = sender as FrameworkElement;
                // remove the event handler
                owner.Loaded -= Owner_Loaded;

                var tooltip =
                    owner.GetValue(TooltipProperty) as DependencyObject;
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