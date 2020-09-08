﻿using ImageProcessor;
using ImageProcessor.Imaging.Filters.Photo;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Project2ImageEditor
{
    public static class ImageHelpers
    {
        public static RenderTargetBitmap snipCanvas(Canvas canvas ,BitmapImage bitmap,System.Windows.Controls.Image ImageViewer1)
        {
            double width = bitmap.Width;
            double height = bitmap.Height;
            System.Windows.Size size = ImageViewer1.RenderSize;
            canvas.Measure(size);
            canvas.Arrange(new Rect(size)); // This is important

            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(canvas);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), new System.Windows.Size(width, height)));
            }
            bmpCopied.Render(dv);

            return bmpCopied;
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
        public static void applyFillter(string fillterType , string path , System.Windows.Controls.Image imageView)
        {

            if (path.Equals(""))
                return;
            System.Drawing.Image newImage = System.Drawing.Image.FromFile(path);
            var imageFactory = new ImageFactory(false);

            switch (fillterType)
            {
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


            System.Drawing.Image tmp = imageFactory.Image;

            BitmapSource source = ImageHelpers.GetImageStream(tmp);
            imageView.Source = source;

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
