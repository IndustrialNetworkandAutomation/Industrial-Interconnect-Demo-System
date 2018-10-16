using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace animationDemo.DataFolder
{
    public class pneumaticData : INotifyPropertyChanged
    {
        public pneumaticData()
        {
            curPosition = 50;
            pressure = 0.101;
        }

        /// <summary>
        /// 气压值
        /// </summary>
        public double airPressure
        {
            get { return pressure; }
            set
            {
                pressure = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("airPressure"));
                }
            }
        }

        /// <summary>
        /// 气缸位置
        /// </summary>
        public double curPosition
        {
            get { return position; }
            set
            {
                position = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("curPosition"));
                }
            }
        }

        private double pressure;
        private double position;

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
