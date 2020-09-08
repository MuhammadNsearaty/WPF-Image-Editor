using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Project2ImageEditor
{
    class Layers
    {
        public ObservableCollection<Image> imagesList = new ObservableCollection<Image>();
        public ObservableCollection<System.Windows.Controls.Label> labelsList = new ObservableCollection<System.Windows.Controls.Label>();
        public ObservableCollection<System.Windows.Controls.CheckBox> checksList = new ObservableCollection<System.Windows.Controls.CheckBox>();
        public void addLayer(RenderTargetBitmap bmp,String str,Boolean isChecked)
        {
            Image im = new Image();
            im.Source = bmp;
            this.imagesList.Add(im);

            Label lb = new Label();
            lb.Content = str;
            this.labelsList.Add(lb);

            CheckBox chk = new CheckBox();
            chk.IsChecked = isChecked;
            this.checksList.Add(chk);

            //RenderTargetBitmap bmp= ImageHelpers.snipCanvas(canvas, bitmap, ImageViewr);
            //Image image = new Image();
            
            //image.Source = bmp;
            //image.Width = 130;
            //image.Height = 100;
            //layersList.Add(image);
        }
        public void removeLayer(int index)
        {
            //layersList.RemoveAt(index);
        }
    }
}
