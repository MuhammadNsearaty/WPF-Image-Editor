using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Project2ImageEditor
{
    class myImages
    {
       public string orginalUrl { get; set; }

        public BitmapImage bmp { get; set; }

       public Button orgBtn { get; set; }
        public Button enhBtn { get; set; }
        public myImages(BitmapImage bmp,string org ,Button orgBtn,Button enhBtn)
        {
            this.orgBtn = orgBtn;
            this.enhBtn = enhBtn;
            this.bmp = bmp;
            this.orginalUrl = org;
        }
    }
}
