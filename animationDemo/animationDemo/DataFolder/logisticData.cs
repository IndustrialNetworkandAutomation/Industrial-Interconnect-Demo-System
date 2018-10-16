using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace animationDemo.DataFolder
{
    public class logisticData : INotifyPropertyChanged
    {
        public logisticData()
        {
            workpiecePos = 50;
            doorPos = 50;
        }

        /// <summary>
        /// 工件托盘位置
        /// </summary>
        public double trayPosition
        {
            get { return workpiecePos; }
            set
            {
                workpiecePos = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("trayPosition"));
                }
            }
        }

        /// <summary>
        /// 侧门位置
        /// </summary>
        public double doorPosition
        {
            get { return doorPos; }
            set
            {
                doorPos = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("doorPosition"));
                }
            }
        }

        private double workpiecePos;
        private double doorPos;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
