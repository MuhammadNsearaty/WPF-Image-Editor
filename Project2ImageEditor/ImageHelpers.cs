using ImageEditor;
using ImageProcessor;
using ImageProcessor.Imaging.Filters.Photo;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace Project2ImageEditor
{
    public static class ImageHelpers
    {

        public static void SaveStreamAsFile(Stream inputStream)
        {

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Image files (*.jpg, *.jpeg,*.png) | *.jpg; *.jpeg; *.png";
            if (saveDialog.ShowDialog() == true)
            {
                if (saveDialog.FileName == "")
                    return;
                using (FileStream outputFileStream = new FileStream(saveDialog.FileName, FileMode.Create))
                {
                    inputStream.CopyTo(outputFileStream);
                }
            }
        }

        public static void saveImage(BitmapSource bmp)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Image files (*.jpg, *.jpeg,*.png) | *.jpg; *.jpeg; *.png";
            if (saveDialog.ShowDialog() == true)
            {
                if (saveDialog.FileName == "")
                    return;
                BitmapEncoder encoder;
                if (System.IO.Path.GetExtension(saveDialog.FileName) == "png")
                    encoder = new PngBitmapEncoder();

                else
                    encoder = new JpegBitmapEncoder();


                encoder.Frames.Add(BitmapFrame.Create(bmp));
                using (var fileStream = new System.IO.FileStream(saveDialog.FileName, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }



            }
        }
        public static T CloneXaml<T>(T source)
        {
            string xaml = XamlWriter.Save(source);
            StringReader sr = new StringReader(xaml);
            XmlReader xr = XmlReader.Create(sr);
            return (T)XamlReader.Load(xr);
        }
        public static Bitmap bitmapFromBitmapImage(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        public static BitmapImage Bitmap2BitmapImage(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
        public static System.Drawing.Bitmap BitmapImage2Bitmap(RenderTargetBitmap bmp)
        {
            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            encoder.Save(stream);

            Bitmap bitmap = new Bitmap(stream);
            return bitmap;
        }
        public static System.Drawing.Bitmap interpolate(System.Drawing.Bitmap bitmap, string type, int w, int h)
        {
            Filters filters = new Filters();
            System.Drawing.Bitmap res = null;
            switch (type)
            {
                case "Bicubic":
                    {
                        res = filters.ResizeBicubic(bitmap, w, h);
                        break;
                    }
                case "Bilinear":
                    {
                        res = filters.ResizeBilinear(bitmap, w, h);
                        break;
                    }
                case "Nearest Neighbor":
                    {
                        res = filters.ResizeNearestNeighbor(bitmap, w, h);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return res;
        }
        public static bool CheckInside(System.Windows.Point begin, Double w, Double h, System.Windows.Point p)
        {
            if (p.X < w + begin.X - 0.1 && p.Y < h + begin.Y - 0.1 && p.X > begin.X + 0.1 && p.Y > begin.Y + 0.1)
                return true;
            return false;
        }
        public static BitmapSource Convert(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }


        public static RenderTargetBitmap snipCanvas(Canvas canvas, System.Windows.Size size)
        {
            //int width = (int)canvas.ActualWidth;
            //int height = (int)canvas.ActualHeight;
            canvas.Measure(size);
            canvas.Arrange(new Rect(size)); // This is important
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(canvas);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), new System.Windows.Size(size.Width, size.Height)));
            }
            bmpCopied.Render(dv);

            return bmpCopied;
        }
        public static Bitmap newSnip(Canvas canvas, System.Windows.Point p, double w, double h)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.RenderSize.Width,
    (int)canvas.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(canvas);
            Bitmap myBitmap;
            var crop = new CroppedBitmap(rtb, new Int32Rect((int)p.X, (int)p.Y, (int)w, (int)h));

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(crop));

            using (Stream s = new MemoryStream())
            {
                pngEncoder.Save(s);
                myBitmap = new Bitmap(s);
            }
            return myBitmap;
        }
        public static UIElement cloneElement(UIElement orig)
        {
            if (orig == null)
            {
                return (null);
            }

            var s = System.Windows.Markup.XamlWriter.Save(orig);
            var stringReader = new System.IO.StringReader(s);
            var xmlReader = System.Xml.XmlTextReader.Create(stringReader, new System.Xml.XmlReaderSettings());

            return (UIElement)System.Windows.Markup.XamlReader.Load(xmlReader);

        }
        public static void applyFillter(string fillterType, System.Drawing.Bitmap bmp, Canvas canvas)
        {
            System.Drawing.Image newImage = bmp;
            var imageFactory = new ImageFactory(false);

            switch (fillterType)
            {

                case "Blur":
                    imageFactory.Load(newImage).GaussianBlur(10);
                    break;
                case "BlackWhite":
                    imageFactory.Load(newImage)
                   .Filter(MatrixFilters.BlackWhite);
                    break;
                case "Comic":
                    imageFactory.Load(newImage)
                   .Filter(MatrixFilters.Comic);
                    break;
                case "Gotham":
                    imageFactory.Load(newImage)
                   .Filter(MatrixFilters.Gotham);
                    break;
                case "GreyScale":
                    imageFactory.Load(newImage)
                   .Filter(MatrixFilters.GreyScale);
                    break;
                case "HiSatch":
                    imageFactory.Load(newImage)
                   .Filter(MatrixFilters.HiSatch);
                    break;
                case "Invert":
                    imageFactory.Load(newImage)
                   .Filter(MatrixFilters.Invert);
                    break;
                case "Lomograph":
                    imageFactory.Load(newImage)
                   .Filter(MatrixFilters.Lomograph);
                    break;
                case "LoSatch":
                    imageFactory.Load(newImage)
                   .Filter(MatrixFilters.LoSatch);
                    break;
                case "Polaroid":
                    imageFactory.Load(newImage)
                   .Filter(MatrixFilters.Polaroid);
                    break;
                case "Sepia":
                    imageFactory.Load(newImage)
                   .Filter(MatrixFilters.Sepia);
                    break;


            }

            System.Drawing.Image tmp;
            if (fillterType == "Orgin")
            {
                tmp = bmp;
            }
            else
            {
                tmp = imageFactory.Image;
            }

            BitmapImage source = ImageHelpers.Bitmap2BitmapImage(new System.Drawing.Bitmap(tmp));
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = source;
            canvas.Background = ib;

        }
        public static BitmapSource GetImageStream(System.Drawing.Image myImage)
        {
            var bitmap = new Bitmap(myImage);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
             System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                   bmpPt,
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());

            //freeze bitmapSource and clear memory to avoid memory leaks
            bitmapSource.Freeze();
            //  DeleteObject(bmpPt);

            return bitmapSource;
        }
        public static BitmapImage ToBitmapImage(this WriteableBitmap wbm)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }
        public static void CreateSaveBitmap(Canvas canvas, string filename)
        {

            var height = canvas.ActualHeight;
            var width = canvas.ActualWidth;
            /* foreach (UIElement child in canvas.Children)
             {
                 if(child.GetType() == typeof(Image))
                 {
                     Image im = (Image)child;
                     height = im.ActualHeight;
                     width = im.ActualWidth;
                 }

                 // ...
             }*/

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
             (int)width, (int)height,
             96d, 96d, PixelFormats.Pbgra32);
            // needed otherwise the image output is black
            canvas.Measure(new System.Windows.Size((int)width, (int)height));
            canvas.Arrange(new Rect(new System.Windows.Size((int)width, (int)height)));

            renderBitmap.Render(canvas);

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            //PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                using (FileStream file = File.Create(saveFileDialog.FileName))
                {
                    encoder.Save(file);
                }
                MessageBox.Show("Saved");
            }
        }
    }
}
