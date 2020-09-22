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
using AForge.Imaging.Filters;

namespace Project2ImageEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SRWindow srWindow;
        User user;
        Login loginPage;
        chooseWindow choose;
        List<FeedItem> feedItems;
        Signup signupPage;
        UserProfile profile = new UserProfile();
        bool signedIn = false;
        List<Filter> ListOFFilters = new List<Filter>();
        Resize resizeWindow = new Resize();
        bool getout = false;
        Rectangle selectionBox = null;
        Rectangle cropBox = null;
        List<Layer> layersList = new List<Layer>();
        int currentIdx = 0;
        int idx = 0;
        int nxtId = 2;
        
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
            //User user = new User { Email = "adnankattan9@gmail.com", Password = "password" };
            //try
            //{
            //    Console.WriteLine("---------------------------");
            //    user.Login();
               
            //    Console.WriteLine(user);
            //    //Console.WriteLine(user);
            //    List<FeedItem> feedItems = Comunicator.GetFeedItems(user).Data;
            //    foreach (FeedItem item in feedItems)
            //    {
            //        Console.WriteLine(item);
            //        //var stream = Comunicator.DownLoadOriginalImage(user, item);
                    
            //        Console.WriteLine("------------------------------------------------");
            //    }

            //    //Console.WriteLine(user.AuthToken);
            //    //DownLoadOriginalImage(user, feedItems[0]);
            //    //DownLoadEnhancedImage(user, feedItems[0]);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(flag != "select")
            {
                if (selectionBox != null)
                {
                    currentPoint = e.GetPosition(canvas1);
                    Point begin = new Point((double)selectionBox.GetValue(Canvas.LeftProperty), (double)selectionBox.GetValue(Canvas.TopProperty));
                    bool res = ImageHelpers.CheckInside(begin, selectionBox.Width, selectionBox.Height, currentPoint);
                    if (!res)
                    {
                        getout = true;
                        return;
                    }
                    getout = false;
                }
            }
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
                        currentPoint = e.GetPosition(canvas1);
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
                        rectangle.Uid = "" + nxtId;
                        canvas1.Children.Add(rectangle);
                        newRect = new Rectangle { StrokeThickness = slider.Value };
                        newRect.Uid = "" + nxtId;
                        nxtId++;
                        newRect.Stroke = br;
                        newRect.Fill = br;
                        double dimW = this.layersList[this.currentIdx].canvas.ActualWidth / canvas1.ActualWidth;
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
                        cirlce.Uid = "" + nxtId;
                        canvas1.Children.Add(cirlce);

                        newCir = new Ellipse
                        {
                            StrokeThickness = slider.Value
                        };
                        newCir.Uid = "" + nxtId;
                        nxtId++;
                        double dimW = this.layersList[this.currentIdx].canvas.ActualWidth / canvas1.ActualWidth;
                        double dimH = this.layersList[this.currentIdx].canvas.ActualHeight / canvas1.ActualHeight;




                        newCir.Stroke = br;
                        newCir.Fill = br;
                        Canvas.SetLeft(newCir, currentPoint.X * dimW);
                        Canvas.SetTop(newCir, currentPoint.X * dimH);

                        this.layersList[this.currentIdx].canvas.Children.Add(newCir);

                        break;

                    }
                case "select":
                    {
                        
                        currentPoint = e.GetPosition(canvas1);

                        // Initialize the rectangle.
                        // Set border color and width
                        SolidColorBrush br = new SolidColorBrush();
                        br.Color = Colors.LightGray;
                        selectionBox = new Rectangle
                        {
                            StrokeThickness = slider.Value
                        };
                        selectionBox.Stroke = br;
                        selectionBox.StrokeDashArray = new DoubleCollection() { 2 };


                        Canvas.SetLeft(selectionBox, currentPoint.X);
                        Canvas.SetTop(selectionBox, currentPoint.Y);
                        selectionBox.Uid = "" + 0;
                        canvas1.Children.Add(selectionBox);


                        newRect = new Rectangle { StrokeThickness = slider.Value };
                        newRect.Uid = "" + 0;

                        newRect.Stroke = br;
                        double dimW = this.layersList[this.currentIdx].canvas.ActualWidth / canvas1.ActualWidth;
                        double dimH = this.layersList[this.currentIdx].canvas.ActualHeight / canvas1.ActualHeight;

                        Canvas.SetLeft(newRect, currentPoint.X * dimW);
                        Canvas.SetTop(newRect, currentPoint.Y * dimH);

                        this.layersList[this.currentIdx].canvas.Children.Add(newRect);
                        break;
                    }
                case "crop":
                    {
                        currentPoint = e.GetPosition(canvas1);
                        SolidColorBrush br = new SolidColorBrush();
                        br.Color = Colors.LightGray;
                        cropBox = new Rectangle
                        {
                            StrokeThickness = slider.Value
                        };
                        cropBox.Stroke = br;
                        
                        cropBox.StrokeDashArray = new DoubleCollection() { 2 };
                        cropBox.Uid = "" + 1;
                        Canvas.SetLeft(cropBox, currentPoint.X);
                        Canvas.SetTop(cropBox, currentPoint.Y);

                        canvas1.Children.Add(cropBox);
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
                if (flag != "select")
                {
                    if (selectionBox != null)
                    {
                        Point tmp;
                        if (flag == "pen")
                            tmp = currentPoint;
                        else
                            tmp = e.GetPosition(canvas1);
                        Point begin = new Point((double)selectionBox.GetValue(Canvas.LeftProperty), (double)selectionBox.GetValue(Canvas.TopProperty));
                        bool res = ImageHelpers.CheckInside(begin, selectionBox.Width, selectionBox.Height, tmp);
                        Console.WriteLine(tmp);
                        if (!res)
                        {
                            Console.WriteLine("fuck my life get out");
                            return;
                        }
                       
                    }
                }
                switch (this.flag) {
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
                            newLine.X1 = currentPoint.X * dimW;
                            newLine.Y1 = currentPoint.Y * dimH;
                            newLine.X2 = e.GetPosition(canvas1).X * dimW;
                            newLine.Y2 = e.GetPosition(canvas1).Y * dimH;
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
                            var w = Math.Max(pos.X, currentPoint.X) - x;
                            var h = Math.Max(pos.Y, currentPoint.Y) - y;

                            rectangle.Width = w;
                            rectangle.Height = h;

                            Canvas.SetLeft(rectangle, x);
                            Canvas.SetTop(rectangle, y);



                            double dimW = this.layersList[this.currentIdx].canvas.ActualWidth / canvas1.ActualWidth;
                            double dimH = this.layersList[this.currentIdx].canvas.ActualHeight / canvas1.ActualHeight;

                            Point point = new Point(currentPoint.X, currentPoint.Y);
                            point.X *= dimW;
                            point.Y *= dimH;
                            // Set the position of rectangle
                            var x1 = Math.Min(pos.X * dimW, point.X);
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
                            var w = Math.Max(pos.X, currentPoint.X) - x;
                            var h = Math.Max(pos.Y, currentPoint.Y) - y;

                            cirlce.Width = w;
                            cirlce.Height = h;

                            Canvas.SetLeft(cirlce, x);
                            Canvas.SetTop(cirlce, y);

                            double dimW = this.layersList[this.currentIdx].canvas.ActualWidth / canvas1.ActualWidth;
                            double dimH = this.layersList[this.currentIdx].canvas.ActualHeight / canvas1.ActualHeight;
                            Point point = new Point(currentPoint.X * dimW, currentPoint.Y * dimH);

                            // Set the position of rectangle
                            var x1 = Math.Min(pos.X * dimW, point.X);
                            var y1 = Math.Min(pos.Y * dimH, point.Y);

                            // Set the dimenssion of the rectangle
                            var w1 = Math.Max(pos.X * dimW, point.X) - x1;
                            var h1 = Math.Max(pos.Y * dimH, point.Y) - y1;

                            newCir.Width = w1;
                            newCir.Height = h1;

                            Canvas.SetLeft(newCir, x1);
                            Canvas.SetTop(newCir, y1);
                            break;

                        }
                    case "select":
                        { 
                        if (e.LeftButton == MouseButtonState.Released || selectionBox == null || selectionBox == null)
                            return;

                        var pos = e.GetPosition(canvas1);

                        // Set the position of rectangle
                        var x = Math.Min(pos.X, currentPoint.X);
                        var y = Math.Min(pos.Y, currentPoint.Y);

                        // Set the dimenssion of the rectangle
                        var w = Math.Max(pos.X, currentPoint.X) - x;
                        var h = Math.Max(pos.Y, currentPoint.Y) - y;

                        selectionBox.Width = w;
                        selectionBox.Height = h;





                        Canvas.SetLeft(selectionBox, x);
                        Canvas.SetTop(selectionBox, y);



                        double dimW = this.layersList[this.currentIdx].canvas.ActualWidth / canvas1.ActualWidth;
                        double dimH = this.layersList[this.currentIdx].canvas.ActualHeight / canvas1.ActualHeight;

                        Point point = new Point(currentPoint.X, currentPoint.Y);
                        point.X *= dimW;
                        point.Y *= dimH;
                        // Set the position of rectangle
                        var x1 = Math.Min(pos.X * dimW, point.X);
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
                    case "crop":
                        {

                            if (e.LeftButton == MouseButtonState.Released || cropBox == null)
                                return;

                            var pos = e.GetPosition(canvas1);

                            // Set the position of rectangle
                            var x = Math.Min(pos.X, currentPoint.X);
                            var y = Math.Min(pos.Y, currentPoint.Y);

                            // Set the dimenssion of the rectangle
                            var w = Math.Max(pos.X, currentPoint.X) - x;
                            var h = Math.Max(pos.Y, currentPoint.Y) - y;

                            cropBox.Width = w;
                            cropBox.Height = h;

                            Canvas.SetLeft(cropBox, x);
                            Canvas.SetTop(cropBox, y);
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
            RenderTargetBitmap bmpCopied = ImageHelpers.snipCanvas(canvas1, new System.Windows.Size((int)bitmap.Width,(int)bitmap.Height));

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Image files (*.jpg, *.jpeg) | *.jpg; *.jpeg";
            if (saveDialog.ShowDialog() == true)
            {
                if (saveDialog.FileName == "")
                    return;

                JpegBitmapEncoder jpg = new JpegBitmapEncoder();
                jpg.Frames.Add(BitmapFrame.Create(bmpCopied));
                using (Stream stm = File.Create(saveDialog.FileName))
                {
                    jpg.Save(stm);
                }
            }
        }
        private void cropButton_Click(object sender, RoutedEventArgs e)
        {
            cropButton.IsEnabled = false;
            RectButton.IsEnabled = true;
            penButton.IsEnabled = true;
            circleButton.IsEnabled = true;
            this.flag = "crop";
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
            cropButton.IsEnabled = true;
            circleButton.IsEnabled = true;
            RectButton.IsEnabled = true;
            this.flag = "pen";
        }

        private void RectButton_Click(object sender, RoutedEventArgs e)
        {
            RectButton.IsEnabled = false;

            cropButton.IsEnabled = true; 
            penButton.IsEnabled = true;
            circleButton.IsEnabled = true;
            this.flag = "rect";
        }

        private void circleButton_Click(object sender, RoutedEventArgs e)
        {
            circleButton.IsEnabled = false;

            cropButton.IsEnabled = true; 
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
            }
            Canvas newCanvas = new Canvas();
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = bitmap;
            newCanvas.Background = ib;
            canvas1.Background = ib;
            this.layersList.Add(new Layer(newCanvas,"Layer 0",true,0));
            this.layersListView.ItemsSource = null;
            this.layersListView.ItemsSource = layersList;
            this.currentIdx = 0;
            this.idx++;
        }
        private void Orgin_Click(object sender, RoutedEventArgs e)
        {
            ImageHelpers.applyFillter("Orgin", ImageHelpers.bitmapFromBitmapImage(bitmap), canvas1);

        }
        private void Comic_Click(object sender, RoutedEventArgs e)

        {
            ImageHelpers.applyFillter("Comic", ImageHelpers.bitmapFromBitmapImage(bitmap), canvas1);
        }
    
        private void BlackWhiteButton_Click(object sender, RoutedEventArgs e)
        {
            ImageHelpers.applyFillter("BlackWhite", ImageHelpers.bitmapFromBitmapImage(bitmap), canvas1);

        }

        private void Gotham_Click(object sender, RoutedEventArgs e)
        {
            ImageHelpers.applyFillter("Gotham", ImageHelpers.bitmapFromBitmapImage(bitmap), canvas1);

        }

        private void GreyScale_Click(object sender, RoutedEventArgs e)
        {
            ImageHelpers.applyFillter("GreyScale", ImageHelpers.bitmapFromBitmapImage(bitmap), canvas1);

        }

        private void HiSatch_Click(object sender, RoutedEventArgs e)

        {
            ImageHelpers.applyFillter("HiSatch", ImageHelpers.bitmapFromBitmapImage(bitmap), canvas1);

        }

        private void Invert_Click(object sender, RoutedEventArgs e)

        {
            ImageHelpers.applyFillter("Invert", ImageHelpers.bitmapFromBitmapImage(bitmap), canvas1);

        }

        private void Lomograph_Click(object sender, RoutedEventArgs e)

        {
            ImageHelpers.applyFillter("Lomograph", ImageHelpers.bitmapFromBitmapImage(bitmap), canvas1);

        }

        private void LoSatch_Click(object sender, RoutedEventArgs e)

        {
            ImageHelpers.applyFillter("LoSatch", ImageHelpers.bitmapFromBitmapImage(bitmap), canvas1);

        }

        private void Polaroid_Click(object sender, RoutedEventArgs e)

        {
            ImageHelpers.applyFillter("Polaroid", ImageHelpers.bitmapFromBitmapImage(bitmap), canvas1);

        }

        private void Sepia_Click(object sender, RoutedEventArgs e)

        {
            ImageHelpers.applyFillter("Sepia", ImageHelpers.bitmapFromBitmapImage(bitmap), canvas1);

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
            this.layersList[itemId].isChecked = flag;
            if (flag)
            {
                if (itemId == 0)
                {
                    ImageBrush ib =(ImageBrush) layersList[itemId].canvas.Background;
                    canvas1.Background = ib;
                }
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
                if(itemId == 0)
                {

                    BitmapImage newImg = new BitmapImage();
                    newImg.BeginInit(); 
                  
                    newImg.UriSource = new Uri(@"Resources\images\bigPng.png", UriKind.Relative);
                    newImg.DecodePixelWidth = (int)canvas1.ActualWidth;
                    newImg.DecodePixelHeight = (int)canvas1.ActualHeight;
                    newImg.EndInit();

                    ImageBrush ib = new ImageBrush();
                    ib.ImageSource = newImg;
                    this.canvas1.Background = ib;
                }
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

        private void eraserButton_Click(object sender, RoutedEventArgs e)
        {

        }

        

        private void selectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectionBox == null)
            {
                selectButton.Background = Brushes.Red;
                this.flag = "select";
            }
            else
            {
                this.flag = "none";
                selectButton.Background = Brushes.Transparent;
                selectionBox = null;

                var ancestList = canvas1.Children.Cast<UIElement>().ToList();

                foreach(UIElement item in ancestList)
                {
                    if(item.Uid == "0")
                    {
                        canvas1.Children.Remove(item);
                    }
                }

                var uilist = layersList[currentIdx].canvas.Children.Cast<UIElement>().ToList();


                foreach (UIElement item in uilist)
                {
                    if (item.Uid == "0")
                    {
                        layersList[currentIdx].canvas.Children.Remove(item);
                    }
                }



            }
        }

        

        private void canvas1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(flag == "crop")
            {
                Point begin = new Point((double)cropBox.GetValue(Canvas.LeftProperty), (double)cropBox.GetValue(Canvas.TopProperty));
                double w = cropBox.Width;double h = cropBox.Height;
                System.Drawing.Image newImage;
                for(int i = 0;i < canvas1.Children.Count; i++)
                {
                    if(canvas1.Children[i].Uid == "1")
                    {
                        canvas1.Children.Remove(canvas1.Children[i]);
                        break;
                    }
                }
                RenderTargetBitmap bmp = ImageHelpers.snipCanvas(canvas1,new System.Windows.Size((int)canvas1.ActualWidth,(int)canvas1.ActualHeight));

                newImage = ImageHelpers.BitmapImage2Bitmap(bmp);

                var imageFactory = new ImageFactory(false);


                imageFactory.Load(newImage).Crop(new System.Drawing.Rectangle((int)begin.X,(int) begin.Y, (int)w, (int)h));


                System.Drawing.Image tmp = imageFactory.Image;

                BitmapSource source = ImageHelpers.GetImageStream(tmp);

                ImageBrush ib = new ImageBrush();
                ib.ImageSource = source;
                canvas1.Background = ib;

                layersList[0].canvas.Background = ib;
                this.layersListView.ItemsSource = null;
                this.layersListView.ItemsSource = this.layersList;

                
            }
        }

        private void resizeButton_Click(object sender, RoutedEventArgs e)
        {
            choose = new chooseWindow();          
            choose.interpolateBtn.Click += new RoutedEventHandler(interpolate_Click);           
            choose.SRBtn.Click += new RoutedEventHandler(SR_Click);
            choose.Show();
        }
        private void interpolate_Click(object sender,RoutedEventArgs e)
        {
            choose.Close();
            resizeWindow = new Resize();
            resizeWindow.Show();
            resizeWindow.submitButton.Click += new RoutedEventHandler(submitButton_Click);
        }
        private void SR_Click(object sender, RoutedEventArgs e)
        {
            choose.Close();
            srWindow = new SRWindow();

            srWindow.submitSRButton.Click += new RoutedEventHandler(submitSR_Click);

            srWindow.Show();
        }
        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            resizeWindow.w = int.Parse(resizeWindow.widthBox.Text);
            resizeWindow.h = int.Parse(resizeWindow.heightBox.Text);
            var item = (ComboBoxItem)resizeWindow.comboBox.SelectedItem;
            resizeWindow.type = item.Content.ToString();

            RenderTargetBitmap bmpCopied = ImageHelpers.snipCanvas(canvas1, new System.Windows.Size((int)bitmap.Width, (int)bitmap.Height));
            System.Drawing.Bitmap resBitmap = ImageHelpers.interpolate(ImageHelpers.BitmapImage2Bitmap(bmpCopied), resizeWindow.type, resizeWindow.w, resizeWindow.h) ;

            this.bitmap = ImageHelpers.Bitmap2BitmapImage(resBitmap);
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = ImageHelpers.Bitmap2BitmapImage(resBitmap);

            canvas1.Children.Clear();
            canvas1.Background = ib;
            
            Canvas newCanvas = new Canvas();
            newCanvas.Background = ib;

            this.layersList = new List<Layer>();
            this.layersListView.ItemsSource = null;

            this.layersList.Add(new Layer(newCanvas, "Layer 0", true, 0));
            this.layersListView.ItemsSource = layersList;
            this.currentIdx = 0;
            this.idx++;

            resizeWindow.Close();
        }

        private void submitSR_Click(Object sender , RoutedEventArgs e)
        {
            bool offRes = (bool)srWindow.offlineBtn.IsChecked;
            var item = (ComboBoxItem)srWindow.cmbx.SelectedItem;
            int scaler = int.Parse(item.Content.ToString());

            if (offRes)
            {
                //offline SR
            }
            else
            {
                //online SR
            }
            srWindow.Close();

            RenderTargetBitmap bmpCopied = ImageHelpers.snipCanvas(canvas1, new System.Windows.Size((int)bitmap.Width, (int)bitmap.Height));
            System.Drawing.Bitmap bmp = ImageHelpers.BitmapImage2Bitmap(bmpCopied);
            System.Drawing.Bitmap resBitmap = ImageProcessor.PerformSR(scaler, bmp);
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = ImageHelpers.Bitmap2BitmapImage(resBitmap);
            canvas1.Children.Clear();
            canvas1.Background = ib;

            Canvas newCanvas = new Canvas();
            newCanvas.Background = ib;

            this.layersList = new List<Layer>();
            this.layersListView.ItemsSource = null;

            this.layersList.Add(new Layer(newCanvas, "Layer 0", true, 0));
            this.layersListView.ItemsSource = layersList;
            this.currentIdx = 0;
            this.idx++;

        }
        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            this.window.Close();
        }

        private void BLur_Click(object sender, RoutedEventArgs e)
        {
            ImageHelpers.applyFillter("Blur", ImageHelpers.bitmapFromBitmapImage(bitmap),canvas1);
        }

        private void MergeLayersButton_Click(object sender, RoutedEventArgs e)
        {
            bool first = true;
            int resIdx = -1;
            Layer res = new Layer();
            List<int> removedIdx = new List<int>();
           
            foreach (Layer layer in this.layersList)
            {

                if (layer.isChecked)
                {

                    if (first)
                    {
                        resIdx = layer.idx;
                        first = false;
                        res = layer.deepCopy();
                        if (layer.idx == 0)
                        {
                            ImageBrush ib = new ImageBrush();
                            ib.ImageSource = bitmap;
                            res.canvas.Background = ib;
                        }
                        else
                        {
                            BitmapImage bitmap2 = new BitmapImage();

                            var stream = File.OpenRead(@"C:\Users\NSEARATY\Desktop\png.png");
                            bitmap2.BeginInit();
                            bitmap2.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap2.StreamSource = stream;
                            bitmap2.EndInit();
                            stream.Close();
                            stream.Dispose();

                            ImageBrush ib = new ImageBrush();
                            ib.ImageSource = bitmap2;
                            res.canvas.Background = ib;
                        }
                    }
                    else if(!first)
                    {
                        var uilist = layersList[layer.idx].canvas.Children.Cast<UIElement>().ToList();
                        foreach (UIElement item in uilist)
                        {
                            string itemXaml = XamlWriter.Save(item);
                            StringReader stringReader = new StringReader(itemXaml);
                            XmlReader xmlReader = XmlReader.Create(stringReader);
                            UIElement newItem = (UIElement)XamlReader.Load(xmlReader);



                            res.canvas.Children.Add(newItem);
                        }
                        removedIdx.Add(layer.idx);
                    }

                }
            }
            for(int i = removedIdx.Count-1 ; i >= 0; i--)
            {
                this.layersList.RemoveAt(removedIdx[i]);
            }
            this.layersList[resIdx] = res;
            this.layersListView.ItemsSource = null;
            this.layersListView.ItemsSource = this.layersList;
        }

        private void DeleteLayerButton_Click(object sender, RoutedEventArgs e)
        {
            var uilist = layersList[this.currentIdx].canvas.Children.Cast<UIElement>().ToList();
            var mainList = canvas1.Children.Cast<UIElement>().ToList();

            List<string> removed = new List<string>();
            foreach(UIElement item in uilist)
            {
                foreach(UIElement mainItem in mainList)
                {
                    if (item.Uid == mainItem.Uid)
                        removed.Add(item.Uid);
                }
            }
            for (int j = removed.Count - 1; j >= 0; j--) {
                for (int i = 0; i < canvas1.Children.Count; i++)
                {
                    if (canvas1.Children[i].Uid == removed[j])
                    {
                        canvas1.Children.Remove(canvas1.Children[i]);
                        break;
                    }
            } }
            this.layersList.RemoveAt(currentIdx);
            this.layersListView.ItemsSource = null;
            this.layersListView.ItemsSource = layersList;
        }

        private void canvas1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double ScaleRate = 1.1;
            if (e.Delta > 0)
            {
                this.scaleTr.ScaleX *= ScaleRate;
                this.scaleTr.ScaleY *= ScaleRate;
            }
            else
            {
                this.scaleTr.ScaleX /= ScaleRate;
                this.scaleTr.ScaleY /= ScaleRate;
            }
        }

        private void profileButton_Click(object sender, RoutedEventArgs e)
        {
            if (signedIn)
            {
                //show profile
                ObservableCollection<myImages> images = new ObservableCollection<myImages>();
                try
                {
                    feedItems = Comunicator.GetFeedItems(user).Data;
                }
                catch (Exception e1)
                { //ok continue
                    MessageBox.Show("Failled to connect server");
                }
                int ind = 0;
                foreach (FeedItem item in feedItems)
                {
                    //show the feedItems on the listvIew profile
                    var stream = Comunicator.DownLoadOriginalImage(user, item);
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(stream);
                    BitmapImage tmp = ImageHelpers.Bitmap2BitmapImage(bmp);
                    Button orgBtn = new Button();
                    Button enhbtn = new Button();
                    orgBtn.Content = "Dowload Orginal";
                    enhbtn.Content = "Download Enhanced";
                    orgBtn.FontSize = 14;
                    enhbtn.FontSize = 14;

                    orgBtn.Click += new RoutedEventHandler(this.downloadOrginalButton_Click);
                    enhbtn.Click += new RoutedEventHandler(this.downloadEnhancedButton_Click);
                    orgBtn.Uid = ind + "";
                    enhbtn.Uid = ind + "";
                    images.Add(new myImages(tmp, item.originalImageURL, orgBtn, enhbtn));
                    ind++;

                }
                profile.imagesListView.ItemsSource = images;
                profile.Show();


            }
            else
            {
                
                //sign in up
                loginPage = new Login();
                signupPage = new Signup();
                loginPage.loginButton.Click += new RoutedEventHandler(loginButton_Click);
                loginPage.signupButton.Click += new RoutedEventHandler(moveSignupButton_Click);
                loginPage.Show();
                
            }
        }
        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = loginPage.emailBox.Text;
            string password = loginPage.passwordBox.Text;
            user = new User { Email = email, Password = password};
            ObservableCollection<myImages> images = new ObservableCollection<myImages>();
            try
            {
                user.Login();
                this.signedIn = true;
                loginPage.Close();
                try
                {
                    feedItems = Comunicator.GetFeedItems(user).Data;
                }
                catch (Exception e1)
                { //ok continue
                }
                int ind = 0;
                foreach (FeedItem item in feedItems)
                {
                    //show the feedItems on the listvIew profile
                    var stream = Comunicator.DownLoadOriginalImage(user, item);
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(stream);
                    BitmapImage tmp = ImageHelpers.Bitmap2BitmapImage(bmp);
                    Button orgBtn = new Button();
                    Button enhbtn = new Button();
                    orgBtn.Content = "Dowload Orginal";
                    enhbtn.Content = "Download Enhanced";
                    orgBtn.FontSize = 14;
                    enhbtn.FontSize = 14;

                    orgBtn.Click += new RoutedEventHandler(this.downloadOrginalButton_Click);
                    enhbtn.Click += new RoutedEventHandler(this.downloadEnhancedButton_Click);
                    orgBtn.Uid = ind+"";
                    enhbtn.Uid = ind + "";
                    images.Add(new myImages(tmp, item.originalImageURL,orgBtn,enhbtn));
                    ind++;
                   
                }
                profile.imagesListView.ItemsSource = images;
                profile.Show();                
                
            }
            catch (Exception e1)
            {
                //failed to login
                this.loginPage.passwordBox.Clear();
                MessageBox.Show("Wrong email or password\ntry to signup");
                Console.WriteLine(e1.Message);
                user = null;
            }
            
            // send login request
        }
        private void moveSignupButton_Click(object sender, RoutedEventArgs e)
        {
            loginPage.Close();
            signupPage.Show();

            signupPage.signUpButton.Click += new RoutedEventHandler(signupButton_Click);
        }
        private void signupButton_Click(object sender, RoutedEventArgs e)
        {
            string email = signupPage.emailBox.Text;
            string name = signupPage.nameBox.Text;
            string password = signupPage.passwordBox.Text;
            string cnfPassword = signupPage.cnfPasswordBox.Text;

            if (!password.Equals(cnfPassword))
            {
                this.signupPage.clearPassWord();
                MessageBox.Show("Password not matched");
            }
            else
            {
                try
                {
                    // send signUp request to server
                    user = new User { Email = email, Name = name, Password = password };
                    user.Signup();
                    user.Login();
                    this.signedIn = true;
                    this.signupPage.Close();
                }catch(Exception e1)
                {
                    this.signupPage.passwordBox.Clear();
                    this.signupPage.cnfPasswordBox.Clear();
                    //failled signup
                    MessageBox.Show("Failled to Signup");
                    user = null;
                }
            }
            
        }
        private void downloadOrginalButton_Click(object sender, RoutedEventArgs e)
        {
            String id = (sender as UIElement).Uid;
            FileStream res = Comunicator.DownLoadOriginalImage(user, feedItems[int.Parse(id)]);

            System.Drawing.Bitmap resBmp = new System.Drawing.Bitmap(res);
            BitmapImage bitmapImage = ImageHelpers.Bitmap2BitmapImage(resBmp);
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Image files (*.jpg, *.jpeg) | *.jpg; *.jpeg";
            if (saveDialog.ShowDialog() == true)
            {
                if (saveDialog.FileName == "")
                    return;
                JpegBitmapEncoder jpg = new JpegBitmapEncoder();
                jpg.Frames.Add(BitmapFrame.Create(bitmapImage));
                using (Stream stm = File.Create(saveDialog.FileName))
                {
                    jpg.Save(stm);
                }
            }
        }
        private void downloadEnhancedButton_Click(object sender, RoutedEventArgs e)
        {
            String id = (sender as UIElement).Uid;
            FileStream res = Comunicator.DownLoadEnhancedImage(user, feedItems[int.Parse(id)]);

            System.Drawing.Bitmap resBmp = new System.Drawing.Bitmap(res);
            BitmapImage bitmapImage = ImageHelpers.Bitmap2BitmapImage(resBmp);
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Image files (*.jpg, *.jpeg) | *.jpg; *.jpeg";
            if (saveDialog.ShowDialog() == true)
            {
                if (saveDialog.FileName == "")
                    return;
                JpegBitmapEncoder jpg = new JpegBitmapEncoder();
                jpg.Frames.Add(BitmapFrame.Create(bitmapImage));
                using (Stream stm = File.Create(saveDialog.FileName))
                {
                    jpg.Save(stm);
                }
            }

        }

        private void cutButton_Click(object sender, RoutedEventArgs e)
        {

        }

      
    }
}
