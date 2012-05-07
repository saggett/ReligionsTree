using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;
using FluxJpeg.Core.Encoder;
using FluxJpeg.Core;

namespace TreeBrowser.SilverlightLib.Helpers
{
    public static class ControlHelper
    {

        private const int JpgQuality = 90;

        public static Rect GetBounds(this FrameworkElement element)
        {
            var topLeft = new Point(0, 0);
            var topRight = new Point(element.ActualWidth, 0);
            var bottomRight = new Point(element.ActualWidth, element.ActualHeight);
            var bottomLeft = new Point(0, element.ActualHeight);

            double boundsLeft = Math.Min(topLeft.X, Math.Min(topRight.X, Math.Min(bottomRight.X, bottomLeft.X)));
            double boundsTop = Math.Min(topLeft.Y, Math.Min(topRight.Y, Math.Min(bottomRight.Y, bottomLeft.Y)));
            double boundsRight = Math.Max(topLeft.X, Math.Max(topRight.X, Math.Max(bottomRight.X, bottomLeft.X)));
            double boundsBottom = Math.Max(topLeft.Y, Math.Max(topRight.Y, Math.Max(bottomRight.Y, bottomLeft.Y)));

            return new Rect(boundsLeft, boundsTop, boundsRight - boundsLeft, boundsBottom - boundsTop);
        }

        public static void SaveToJPG(this UIElement element)
        {
            WriteableBitmap bitmap = new WriteableBitmap(element, element.RenderTransform);
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "JPG";
            saveDialog.Filter = "JPG Files|*.jpg";
            bool? result = saveDialog.ShowDialog();
            if (!result.HasValue || !result.Value)
                return;
            using (Stream stream = saveDialog.OpenFile())
            {
                SaveToJPG(bitmap, stream);
            }
        }

        private static void SaveToJPG(WriteableBitmap bitmap, Stream stream)
        {
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int bands = 3;
            byte[][,] raster = new byte[bands][,];

            for (int i = 0; i < bands; i++)
            {
                raster[i] = new byte[width, height];
            }

            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    int pixel = bitmap.Pixels[width * row + column];
                    raster[0][column, row] = (byte)(pixel >> 16);
                    raster[1][column, row] = (byte)(pixel >> 8);
                    raster[2][column, row] = (byte)pixel;
                }
            }

            ColorModel model = new ColorModel { colorspace = ColorSpace.RGB };
            FluxJpeg.Core.Image img = new FluxJpeg.Core.Image(model, raster);
            JpegEncoder encoder = new JpegEncoder(img, JpgQuality, stream);
            
            encoder.Encode();

        }

    }
}
