using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace animationDemo.DataFolder
{
    public class motorPos : INotifyPropertyChanged
    {

        public motorPos()
        {
            X_Value = 0;
            Y_Value = 0;
        }

        public String IID
        {
            get;
            set;
        }

        /// <summary>
        /// X值
        /// </summary>
        public double XValue
        {
            get { return X_Value; }
            set
            {
                X_Value = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("XValue"));
                }
            }
        }

        /// <summary>
        /// Y值
        /// </summary>
        public double YValue
        {
            get { return Y_Value; }
            set
            {
                Y_Value = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("YValue"));
                }
            }
        }

        private double X_Value;
        private double Y_Value;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
