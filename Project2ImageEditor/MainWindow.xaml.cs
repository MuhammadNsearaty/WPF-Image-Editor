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
        int currentIdx = 0;
        int idx = 0;
        int nxtId = 0;
        
        BitmapImage bitmap = new BitmapImage();
        Point currentPoint = new Point();
        private Rectangle rectangle;
        Rectangle newRect;

        private Ellipse cirlce;
        Ellipse newCir;
        string flag = "none";
        int angel = 0;
        string path = "";
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
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
                        Canvas.SetTop(rectangle, currentPoint.Y);
                        rectangle.Uid = ""+nxtId;
                        canvas1.Children.Add(rectangle);

                        newRect =  new Rectangle { StrokeThickness = slider.Value };
                        newRect.Uid = ""+nxtId;
                        nxtId++;

                        newRect.Stroke = br;
                        newRect.Fill = br;
                        double dimW = this.layersList[this.currentIdx].canvas.ActualWidth / canvas1.ActualWidth ;
                        double dimH = this.layersList[this.currentIdx].canvas.ActualHeight / canvas1.ActualHeight;

                        Canvas.SetLeft(newRect, currentPoint.X * dimW);
                        Canvas.SetTop(newRect, currentPoint.Y * dimH);

                        this.layersList[this.currentIdx].canvas.Children.Add(newRect);
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
                        cirlce.Uid = ""+nxtId;
                        canvas1.Children.Add(cirlce);

                        newCir = new Ellipse
                        {
                            StrokeThickness = slider.Value
                        };
                        newCir.Uid = ""+nxtId;
                        nxtId++;
                        double dimW = this.layersList[this.currentIdx].canvas.ActualWidth / canvas1.ActualWidth;
                        double dimH = this.layersList[this.currentIdx].canvas.ActualHeight / canvas1.ActualHeight;




                        newCir.Stroke = br;
                        newCir.Fill = br;
                        Canvas.SetLeft(newCir, currentPoint.X*dimW);
                        Canvas.SetTop(newCir, currentPoint.X*dimH);

                        this.layersList[this.currentIdx].canvas.Children.Add(newCir);

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
                            line.Uid = "" + nxtId;
                            
                            canvas1.Children.Add(line);

                            Line newLine = new Line();
                            double dimW = this.layersList[this.currentIdx].canvas.ActualWidth / canvas1.ActualWidth;
                            double dimH = this.layersList[this.currentIdx].canvas.ActualHeight / canvas1.ActualHeight;

                            newLine.StrokeThickness = this.slider.Value;
                            newLine.Stroke = br;
                            newLine.X1 = currentPoint.X*dimW;
                            newLine.Y1 = currentPoint.Y*dimH;
                            newLine.X2 = e.GetPosition(canvas1).X*dimW;
                            newLine.Y2 = e.GetPosition(canvas1).Y*dimH;
                            newLine.Uid = "" + nxtId;
                            nxtId++;
                           
                            this.layersList[this.currentIdx].canvas.Children.Add(newLine);

                            currentPoint = e.GetPosition(canvas1);


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


                            double dimW = this.layersList[this.currentIdx].canvas.ActualWidth / canvas1.ActualWidth;
                            double dimH = this.layersList[this.currentIdx].canvas.ActualHeight / canvas1.ActualHeight;

                            Point point = new Point(currentPoint.X,currentPoint.Y);
                            point.X *= dimW;
                            point.Y *= dimH;
                            // Set the position of rectangle
                            var x1 = Math.Min(pos.X*dimW, point.X);
                            var y1 = Math.Min(pos.Y * dimH, point.Y);

                            // Set the dimenssion of the rectangle
                            var w1 = Math.Max(pos.X * dimW, point.X) - x1;
                            var h1 = Math.Max(pos.Y * dimH, point.Y) - y1;

                            newRect.Width = w1;
                            newRect.Height = h1;

                            Canvas.SetLeft(newRect, x1);
                            Canvas.SetTop(newRect, y1);




                            break;
                        }
                    case "circle":
                        {
                            if (e.LeftButton == MouseButtonState.Released || cirlce == null)
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

                            double dimW = this.layersList[this.currentIdx].canvas.ActualWidth / canvas1.ActualWidth;
                            double dimH = this.layersList[this.currentIdx].canvas.ActualHeight / canvas1.ActualHeight;
                            Point point = new Point(currentPoint.X*dimW, currentPoint.Y*dimH);
                            
                            // Set the position of rectangle
                            var x1 = Math.Min(pos.X*dimW, point.X);
                            var y1 = Math.Min(pos.Y*dimH, point.Y);

                            // Set the dimenssion of the rectangle
                            var w1 = Math.Max(pos.X * dimW, point.X) - x1;
                            var h1 = Math.Max(pos.Y*dimH, point.Y) - y1;

                            newCir.Width = w1;
                            newCir.Height = h1;

                            Canvas.SetLeft(newCir, x1);
                            Canvas.SetTop(newCir, y1);
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
            Canvas newCanvas = new Canvas();
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = bitmap;
            newCanvas.Background = ib;
            this.layersList.Add(new Layer(newCanvas,"Layer 0",true,0));
            this.layersListView.ItemsSource = null;
            this.layersListView.ItemsSource = layersList;
            this.currentIdx = 0;
            this.idx++;
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
            
            layersList.Add(new Layer(newcanvas,"Layer "+idx,true,idx++));

            layersListView.ItemsSource = null;
            layersListView.ItemsSource = layersList;

            this.currentIdx = layersList.Count-1;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            handle(sender as CheckBox);
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            handle(sender as CheckBox);
        }

        public void handle(CheckBox chk)
        {
            bool flag = chk.IsChecked.Value;
            int itemId = int.Parse(chk.Uid);
            if (flag)
            {
                var uilist = layersList[itemId].canvas.Children.Cast<UIElement>().ToList();

                foreach (UIElement layerItem in uilist)
                {
                    double top = (double)layerItem.GetValue(Canvas.TopProperty);
                    double left = (double)layerItem.GetValue(Canvas.LeftProperty);

                    string itemXaml = XamlWriter.Save(layerItem);
                    StringReader stringReader = new StringReader(itemXaml);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    UIElement newItem = (UIElement)XamlReader.Load(xmlReader);

                    double dimW =  canvas1.ActualWidth/ layersList[itemId].canvas.ActualWidth;
                    double dimH =  canvas1.ActualHeight/ layersList[itemId].canvas.ActualHeight;

                    Line line = newItem as Line;
                    if (line != null)
                    {
                        (newItem as Line).X1 *= dimW;
                        (newItem as Line).Y1 *= dimH;
                        (newItem as Line).X2 *= dimW;
                        (newItem as Line).Y2 *= dimH;

                    }
                    else
                    {
                        (newItem as System.Windows.Shapes.Shape).Width *= dimW;
                        (newItem as System.Windows.Shapes.Shape).Height *= dimH;
                        Canvas.SetTop(newItem, top * dimH);
                        Canvas.SetLeft(newItem, left * dimW);
                    }
                    

                    this.canvas1.Children.Add(newItem);

                }
            }
            else
            {
                var uilist = layersList[itemId].canvas.Children.Cast<UIElement>().ToList();
                var ancestList = canvas1.Children.Cast<UIElement>().ToList();


                foreach(UIElement layerItem in uilist)
                {
                    foreach(UIElement mainItem in ancestList)
                    {
                        
                        if (layerItem.Uid.Equals(mainItem.Uid))
                        {
                            this.canvas1.Children.Remove(mainItem);
                            break;
                        }
                    }
                }
            }
        }

        

        private void listView_Click(object sender,MouseButtonEventArgs e)
        {
            String id = (sender as UIElement).Uid;
            this.currentIdx = int.Parse(id);
        }

    }
}
