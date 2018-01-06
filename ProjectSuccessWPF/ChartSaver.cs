using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProjectSuccessWPF
{
    class ChartSaver
    {
        /// <summary>
        /// Converting control to bitmap
        /// </summary>
        /// <param name="target">Target UI control</param>
        /// <returns>System.Drawing.Image</returns>
        public static System.Drawing.Bitmap SaveChart(Visual target)
        {
            BitmapSource source = ControlToImage(target, 96, 96);
            return BitmapFromSource(source);
        }

        /// <summary>
        /// Converting control to memory stream
        /// </summary>
        /// <param name="target">Target UI control</param>
        /// <returns>Memory stream</returns>
        public static MemoryStream SaveChartToMS(Visual target)
        {
            BitmapSource source = ControlToImage(target, 96, 96);
            return BitmapFromSourceToMS(source);
        }

        /// <summary>
        /// Converts control to bitmap source
        /// </summary>
        /// <param name="target">control to be converted</param>
        /// <param name="dpiX">Points per inch on axis X</param>
        /// <param name="dpiY">points per inch on axis Y</param>
        /// <returns></returns>
        static BitmapSource ControlToImage(Visual target, double dpiX, double dpiY)
        {
            if (target == null)
            {
                return null;
            }
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
                                                            (int)(bounds.Height * dpiY / 96.0),
                                                            dpiX,
                                                            dpiY,
                                                            PixelFormats.Pbgra32);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(target);
                ctx.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }
            rtb.Render(dv);
            return rtb;
        }

        static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {

            return new System.Drawing.Bitmap(BitmapFromSourceToMS(bitmapsource));
        }

        static MemoryStream BitmapFromSourceToMS(BitmapSource bitmapsource)
        {
            MemoryStream outStream = new MemoryStream();
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bitmapsource));
            enc.Save(outStream);
            outStream.Position = 0;
            return outStream;
        }


        /// <summary>
        /// Convert UI control to System.Drawing.Bitmap
        /// </summary>
        /// <param name="target">Control to convert</param>
        /// <param name="dpiX"></param>
        /// <param name="dpiY"></param>
        /// <returns></returns>
        public static Bitmap ControlToBitmapImage(Visual target, double dpiX = 96, double dpiY = 96)
        {
            if (target == null)
            {
                return null;
            }
            // render control content
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
                                                            (int)(bounds.Height * dpiY / 96.0),
                                                            dpiX,
                                                            dpiY,
                                                            PixelFormats.Pbgra32);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(target);
                ctx.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }
            rtb.Render(dv);

            //convert image format
            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            encoder.Save(stream);

            return new Bitmap(stream);
        }

        public static void SaveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            EncodeVisual(visual, fileName, encoder);
        }

        private static void EncodeVisual(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            var bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            var frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);
            using (var stream = File.Create(fileName)) encoder.Save(stream);
        }
    }
}
