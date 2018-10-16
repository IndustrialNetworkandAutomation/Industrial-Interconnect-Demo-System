using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace animationDemo.DataFolder
{
    class dataGridData : INotifyPropertyChanged
    {
        public String IID
        {
            get;
            set;
        }

        /// <summary>
        /// X值
        /// </summary>
        public String varName
        {
            get { return myName; }
            set
            {
                myName = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("varName"));
                }
            }
        }

        /// <summary>
        /// Y值
        /// </summary>
        public double varValue
        {
            get { return myValue; }
            set
            {
                myValue = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("varValue"));
                }
            }
        }

        private String myName;
        private double myValue;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
