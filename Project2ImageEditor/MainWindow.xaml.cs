using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using ImageProcessor;
using System.IO;
using ImageProcessor.Imaging.Formats;
using MaterialDesignThemes.Wpf;
using Project2ImageEditor;
using ImageProcessor.Imaging.Filters.Photo;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Xml;

namespace Project2ImageEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<Layer> layersList = new List<Layer>();
        List<Canvas> canvasList = new List<Canvas>();
        List<int> currentLayers = new List<int>();
        
        BitmapImage bitmap = new BitmapImage();
        Point currentPoint = new Point();
        private Rectangle rectangle;
        private Ellipse cirlce;
        string flag = "none";
        int angel = 0;
        string path = "";
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            currentLayers.Add(0);
        }
        
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (flag)
            {
                case "pen":
                    {
                        if (e.ButtonState == MouseButtonState.Pressed)
                            currentPoint = e.GetPosition(canvas1);

                        break;
                    }
                case "rect":
                    {
                        currentPoint= e.GetPosition(canvas1);

                        // Initialize the rectangle.
                        // Set border color and width
                         SolidColorBrush br = new SolidColorBrush();
                         br.Color = (Color)clrPick.Color;
                        rectangle = new Rectangle
                        {
                            StrokeThickness = slider.Value
                            
                        };
                        rectangle.Stroke = br;
                        rectangle.Fill = br;
                        Canvas.SetLeft(rectangle, currentPoint.X);
                        Canvas.SetTop(rectangle, currentPoint.X);
                        canvas1.Children.Add(rectangle);
                        
                        break;
                    }
                case "circle":
                    {
                        currentPoint = e.GetPosition(canvas1);
                        SolidColorBrush br = new SolidColorBrush();
                        br.Color = (Color)clrPick.Color;
                        cirlce = new Ellipse
                        {
                            StrokeThickness = slider.Value
                        };
                        cirlce.Stroke = br;
                        cirlce.Fill = br;
                        Canvas.SetLeft(cirlce, currentPoint.X);
                        Canvas.SetTop(cirlce, currentPoint.X);
                        canvas1.Children.Add(cirlce);

                        break;

                    }
                case "none":
                    break;
            }
            
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                switch(this.flag){
                    case "pen":
                        {
                            Line line = new Line();
                            line.StrokeThickness = this.slider.Value;
                            SolidColorBrush br = new SolidColorBrush();
                            br.Color = (Color)clrPick.Color;
                            line.Stroke = br;
                            line.X1 = currentPoint.X;
                            line.Y1 = currentPoint.Y;
                            line.X2 = e.GetPosition(canvas1).X;
                            line.Y2 = e.GetPosition(canvas1).Y;

                            currentPoint = e.GetPosition(canvas1);
                            canvas1.Children.Add(line);
                            
                            break;
                        }
                    case "rect":
                        {
                            if (e.LeftButton == MouseButtonState.Released || rectangle == null)
                                return;

                            var pos = e.GetPosition(canvas1);

                            // Set the position of rectangle
                            var x = Math.Min(pos.X, currentPoint.X);
                            var y = Math.Min(pos.Y, currentPoint.Y);

                            // Set the dimenssion of the rectangle
                            var w = Math.Max(pos.X, currentPoint.X)-x;
                            var h = Math.Max(pos.Y, currentPoint.Y)-y;

                            rectangle.Width = w;
                            rectangle.Height = h;

                            Canvas.SetLeft(rectangle, x);
                            Canvas.SetTop(rectangle, y);
                            break;
                        }
                    case "circle":
                        {
                            if (e.LeftButton == MouseButtonState.Released || rectangle == null)
                                return;

                            var pos = e.GetPosition(canvas1);

                            // Set the position of rectangle
                            var x = Math.Min(pos.X, currentPoint.X);
                            var y = Math.Min(pos.Y, currentPoint.Y);

                            // Set the dimenssion of the rectangle
                            var w = Math.Max(pos.X, currentPoint.X)-x;
                            var h = Math.Max(pos.Y, currentPoint.Y)-y;

                            cirlce.Width = w;
                            cirlce.Height = h;

                            Canvas.SetLeft(cirlce, x);
                            Canvas.SetTop(cirlce, y);
                            break;

                        }
                    case "none":
                        break;
                }
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            canvas1.Children.Clear();
            path = "";
        }

        private void newSave(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap bmpCopied = ImageHelpers.snipCanvas(canvas1, bitmap, ImageViewer1);


            JpegBitmapEncoder jpg = new JpegBitmapEncoder();
            jpg.Frames.Add(BitmapFrame.Create(bmpCopied));
            using (Stream stm = File.Create("c:\\temp\\test.jpeg"))
            {
                jpg.Save(stm);
            }
        }

        private void rotateRight_Click(object sender, RoutedEventArgs e)
        {
            angel += 90;
            RotateTransform trans = new RotateTransform(angel);
            canvas1.RenderTransform = trans;
        }

        private void rotateLeft_Click(object sender, RoutedEventArgs e)
        {
            angel -= 90;
            RotateTransform trans = new RotateTransform(angel);
            canvas1.RenderTransform = trans;
        }

        private void penButton_Click(object sender, RoutedEventArgs e)
        {
            penButton.IsEnabled = false;
            circleButton.IsEnabled = true;
            RectButton.IsEnabled = true;

            this.flag = "pen";
        }

        private void RectButton_Click(object sender, RoutedEventArgs e)
        {
            RectButton.IsEnabled = false;
            penButton.IsEnabled = true;
            circleButton.IsEnabled = true;

            this.flag = "rect";
        }

        private void circleButton_Click(object sender, RoutedEventArgs e)
        {
            circleButton.IsEnabled = false;
            penButton.IsEnabled = true;
            RectButton.IsEnabled = true;

            this.flag = "circle";
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF;*.JPEG|" +
        "All files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == true)
            {
                path = dlg.FileName;
               // BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path);
                bitmap.EndInit();
                ImageViewer1.Source = bitmap;
            }
        }
        private void Comic_Click(object sender, RoutedEventArgs e)

        {
            ImageHelpers.applyFillter("Comic", path, ImageViewer1);
        }

    
        private void BlackWhiteButton_Click(object sender, RoutedEventArgs e)
        {
            ImageHelpers.applyFillter("BlackWhite", path, ImageViewer1);

        }

        private void Gotham_Click(object sender, RoutedEventArgs e)
        {
            ImageHelpers.applyFillter("Gotham", path, ImageViewer1);

        }

        private void GreyScale_Click(object sender, RoutedEventArgs e)
        {
            ImageHelpers.applyFillter("GreyScale", path, ImageViewer1);

        }

        private void HiSatch_Click(object sender, RoutedEventArgs e)
        {
            ImageHelpers.applyFillter("HiSatch", path, ImageViewer1);

        }

        private void Invert_Click(object sender, RoutedEventArgs e)
        {
            ImageHelpers.applyFillter("Invert", path, ImageViewer1);

        }

        private void Lomograph_Click(object sender, RoutedEventArgs e)
        {
            ImageHelpers.applyFillter("Lomograph", path, ImageViewer1);

        }

        private void LoSatch_Click(object sender, RoutedEventArgs e)
        {
            ImageHelpers.applyFillter("LoSatch", path, ImageViewer1);

        }

        private void Polaroid_Click(object sender, RoutedEventArgs e)
        {
            ImageHelpers.applyFillter("Polaroid", path, ImageViewer1);

        }

        private void Sepia_Click(object sender, RoutedEventArgs e)
        {
            ImageHelpers.applyFillter("Sepia", path, ImageViewer1);

        }

        private void addLayerButton_Click(object sender, RoutedEventArgs e)
        {
            Canvas newcanvas = new Canvas();
            newcanvas.Height = 130;
            newcanvas.Width = 164;

            newcanvas.Children.Clear();
            ImageBrush ib = new ImageBrush();

            BitmapImage bitmap = new BitmapImage();

            var stream = File.OpenRead(@"C:\Users\NSEARATY\Desktop\png.png");
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            stream.Close();
            stream.Dispose();

            ib.ImageSource = bitmap;
            newcanvas.Background = ib;
            var uilist = canvas1.Children.Cast<UIElement>().ToList();
            uilist.ForEach (p => newcanvas.Children.Add(ImageHelpers.cloneElement(p))) ;


            RenderTargetBitmap bmp = ImageHelpers.snipCanvas(newcanvas, bitmap, ImageViewer1);


            layersList.Add(new Layer(bmp,"Layer",true));

            layersListView.ItemsSource = null;
            layersListView.ItemsSource = layersList;

            this.currentLayers.Add(layersList.Count);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            handle(sender as CheckBox);
        }
        public void handle(CheckBox chk)
        {
            bool flag = chk.IsChecked.Value;
            var r = layersListView.Items.IndexOf(chk);
            if (flag)
            {

            }
        }
    }
}
