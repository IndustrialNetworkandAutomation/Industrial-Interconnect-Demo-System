using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace animationDemo.DataFolder
{
    public class IOData : INotifyPropertyChanged
    {
        public IOData()
        {
            voltageVal = 0;
        }

        public double voltage
        {
            get { return voltageVal; }
            set
            {
                voltageVal = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("voltage"));
                }
            }
        }

        private double voltageVal;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
