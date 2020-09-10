﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Project2ImageEditor
{
    class Layer
    {
        public Canvas canvas { get; set; }
        public RenderTargetBitmap bmp { get; set; }

        public String str { get; set; }
        public Boolean isChecked { get; set; }


        public int idx {get;set;}
        public Layer()
        {
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

       
    }
}
