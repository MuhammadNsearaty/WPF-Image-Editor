using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Project2ImageEditor
{
    [Serializable]
    class Layer
    {
        public Canvas canvas { get; set; }
        public RenderTargetBitmap bmp { get; set; }

        public String str { get; set; }
        public Boolean isChecked { get; set; }


        public int idx {get;set;}
        public Layer()
        {
            this.canvas = new Canvas();
            this.canvas.Height = 80;
            this.canvas.Width = 80;
        }
        public Layer(RenderTargetBitmap bmp, String str ,Boolean isChecked, int idx)
        {
            this.bmp = bmp;
            this.str = str;
            this.isChecked = isChecked;
            this.idx = idx;
        }
        public Layer(Canvas canvas, String str, Boolean isChecked,int idx)
        {
            this.canvas = canvas;
            this.canvas.Height = 80;
            this.canvas.Width = 80;
            this.str = str;
            this.isChecked = isChecked;
            this.idx = idx;
        }

        public Layer deepCopy()
        {
            Layer res = new Layer();
            res.str = this.str;
            res.idx = this.idx;
            res.isChecked = this.isChecked;
            var uilist = this.canvas.Children.Cast<System.Windows.UIElement>().ToList();

            foreach(System.Windows.UIElement item in uilist)
            {
                res.canvas.Children.Add(ImageHelpers.CloneXaml(item));
            }

            res.canvas.Background = this.canvas.Background;
            return res;
        }

       
    }
}
