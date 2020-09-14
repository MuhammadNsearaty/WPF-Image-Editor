using ImageProcessor;
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
        public static bool CheckInside(System.Windows.Point begin,Double w ,Double h, System.Windows.Point p)
        {
            if (p.X < w + begin.X-0.1 && p.Y < h + begin.Y-0.1 && p.X > begin.X+0.1 && p.Y > begin.Y+0.1)
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
        /*public static void Crop()
        {
            if (rectH == 0 || rectW == 0)
                return;
            else
            {
                Cursor = Cursors.Default;
                Bitmap bmp2 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pictureBox1.DrawToBitmap(bmp2, pictureBox1.ClientRectangle);
                Bitmap crpImg = new Bitmap(rectW, rectH);
                for (int i = 0; i < rectW; i++)
                {
                    for (int y = 0; y < rectH; y++)
                    {
                        Color pxlclr = bmp2.GetPixel(crpX + i, crpY + y);
                        crpImg.SetPixel(i, y, pxlclr);
                    }
                }
                pictureBox1.Image = (Image)crpImg;
                myBitMap = bmp2;
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            }
        }*/

        public static RenderTargetBitmap snipCanvas(Canvas canvas , System.Windows.Size size)
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
        public static Bitmap newSnip(Canvas canvas,System.Windows.Point p,double w,double h)
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
