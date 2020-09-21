using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Project2ImageEditor
{
    class chooseBtns
    {
        public Button btn1 { get; set; }
        public Button btn2 { get; set; }

        public chooseBtns(Button btn1,Button btn2)
        {
            this.btn1 = btn1;
            this.btn2 = btn2;
        }

    }
}
