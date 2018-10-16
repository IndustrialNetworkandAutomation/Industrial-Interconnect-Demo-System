using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace animationDemo.DataFolder
{
    public class robotData : INotifyPropertyChanged
    {
        public robotData()
        {
            a1 = 0;
            a2 = 0;
            a3 = 0;
            a4 = 0;
            a5 = 0;
            a6 = 0;

            fixture = 0;
            paw = 0;
        }

        /// <summary>
        /// A1转角值
        /// </summary>
        public double a1Value
        {
            get { return a1; }
            set
            {
                a1 = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("a1Value"));
                }
            }
        }

        /// <summary>
        /// A2转角值
        /// </summary>
        public double a2Value
        {
            get { return a2; }
            set
            {
                a2 = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("a2Value"));
                }
            }
        }

        /// <summary>
        /// A3转角值
        /// </summary>
        public double a3Value
        {
            get { return a3; }
            set
            {
                a3 = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("a3Value"));
                }
            }
        }

        /// <summary>
        /// A4转角值
        /// </summary>
        public double a4Value
        {
            get { return a4; }
            set
            {
                a4 = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("a4Value"));
                }
            }
        }

        /// <summary>
        /// A3转角值
        /// </summary>
        public double a5Value
        {
            get { return a5; }
            set
            {
                a5 = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("a5Value"));
                }
            }
        }

        /// <summary>
        /// A4转角值
        /// </summary>
        public double a6Value
        {
            get { return a6; }
            set
            {
                a6 = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("a6Value"));
                }
            }
        }

        /// <summary>
        /// A1转角值,弧度
        /// </summary>
        public double raw1Value
        {
            get { return raw1; }
            set
            {
                raw1 = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("raw1Value"));
                }
            }
        }

        /// <summary>
        /// A2转角值，弧度
        /// </summary>
        public double raw2Value
        {
            get { return raw2; }
            set
            {
                raw2 = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("raw2Value"));
                }
            }
        }

        /// <summary>
        /// A3转角值，弧度
        /// </summary>
        public double raw3Value
        {
            get { return raw3; }
            set
            {
                raw3 = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("raw3Value"));
                }
            }
        }

        /// <summary>
        /// A4转角值，弧度
        /// </summary>
        public double raw4Value
        {
            get { return raw4; }
            set
            {
                raw4 = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("raw4Value"));
                }
            }
        }

        /// <summary>
        /// A5转角值，弧度
        /// </summary>
        public double raw5Value
        {
            get { return raw5; }
            set
            {
                raw5 = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("raw5Value"));
                }
            }
        }

        /// <summary>
        /// A6转角值
        /// </summary>
        public double raw6Value
        {
            get { return raw6; }
            set
            {
                raw6 = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("raw6Value"));
                }
            }
        }

        /// <summary>
        /// 夹具转角值
        /// </summary>
        public double fixtureProgress
        {
            get { return fixture; }
            set
            {
                fixture = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("fixtureProgress"));
                }
            }
        }

        /// <summary>
        /// 抓手转角值
        /// </summary>
        public double pawProgress
        {
            get { return paw; }
            set
            {
                paw = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("pawProgress"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private double a1;
        private double a2;
        private double a3;
        private double a4;
        private double a5;
        private double a6;

        private double raw1;
        private double raw2;
        private double raw3;
        private double raw4;
        private double raw5;
        private double raw6;

        private double fixture;
        private double paw;
    }
}
