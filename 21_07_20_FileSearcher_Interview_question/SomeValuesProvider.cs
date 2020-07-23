using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace _21_07_20_FileSearcher_Interview_question
{
    class SomeValuesProvider
    {
        public double ThisWidth
        {
            get => SystemParameters.PrimaryScreenWidth / 2;
        }
        public double ThisHeight
        {
            get => SystemParameters.WorkArea.Height - 100;
        }

        public double TexTBox1Width
        {
            get => this.ThisWidth - this.ThisWidth / 5;
        }
        public double DefaultControlWidth
        {
            get => this.ThisWidth - this.ThisWidth / 10;
        }
        public double DefaultControlHeight
        {
            get => this.ThisHeight - this.ThisHeight / 3;
        }
        public double MaxControlheight
        {
            get => this.ThisHeight - this.ThisHeight / 3;
        }
    }
}
