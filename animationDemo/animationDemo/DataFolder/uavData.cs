using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace animationDemo.DataFolder
{
    public class uavData : INotifyPropertyChanged
    {
        #region Fields
        public event PropertyChangedEventHandler PropertyChanged;
        private double batteryVal;
        private double speed;
        #endregion

        #region Construction
        public uavData()
        {
            batteryVal = 0;
            speed = 0;
        }
        #endregion

        public double battery
        {
            get { return batteryVal; }
            set
            {
                batteryVal = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("battery"));
                }
            }
        }

        public double velocity
        {
            get { return speed; }
            set
            {
                speed = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("velocity"));
                }
            }
        }
    }
}
