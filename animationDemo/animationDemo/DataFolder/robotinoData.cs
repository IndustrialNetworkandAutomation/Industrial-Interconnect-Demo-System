using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace animationDemo.DataFolder
{
    public class robotinoData : INotifyPropertyChanged
    {
        #region Fields
        public event PropertyChangedEventHandler PropertyChanged;
        private double robotinoX;
        private double robotinoY;
        private double robotinoPhi;
        private double speed;
        #endregion

        #region Construction
        public robotinoData()
        {
            robotinoX = 0;
            robotinoY = 0;
            robotinoPhi = 0;
            speed = 0;
        }
        #endregion

        public double AgvX
        {
            get { return robotinoX; }
            set
            {
                robotinoX = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("AgvX"));
                }
            }
        }

        public double AgvY
        {
            get { return robotinoY; }
            set
            {
                robotinoY = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("AgvY"));
                }
            }
        }

        public double AgvPhi
        {
            get { return robotinoPhi; }
            set
            {
                robotinoPhi = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("AgvPhi"));
                }
            }
        }

        public double AgvSpeed
        {
            get { return speed; }
            set
            {
                speed = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("AgvSpeed"));
                }
            }
        }
    }
}
