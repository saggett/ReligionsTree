﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TreeBrowser.SilverlightLib.Helpers;

namespace TreeBrowser.SilverlightLib.Controls
{
    public class ResetButton : HyperlinkButton
    {

        public ResetButton()
        {
            Loaded += new RoutedEventHandler(ResetButton_Loaded);
            
        }

        private void ResetButton_Loaded(object sender, RoutedEventArgs e)
        {
            Opacity = 0.5;
            Width = 35;
            Height = 35;
            this.BorderThickness = new Thickness(0);
            this.Background = new SolidColorBrush(Colors.Transparent);
            Content = new Image()
                {
                    Stretch = Stretch.None,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Source = new BitmapImage(new Uri(UrlHelper.SitePath + "Resources/resetarrow.png", UriKind.Absolute))
                };
            ToolTipService.SetToolTip(this, "Return to root");
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            Opacity = 1;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            Opacity = 0.5;
        }

    }
}
