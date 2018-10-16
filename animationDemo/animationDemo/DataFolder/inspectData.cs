using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace animationDemo.DataFolder
{
    public class inspectData : INotifyPropertyChanged
    {
        /// <summary>
        /// 扫码枪获得的数据
        /// </summary>
        public String scannerInfo
        {
            get { return scannerData; }
            set
            {
                scannerData = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("scannerInfo"));
                }
            }
        }

        /// <summary>
        /// RFID获得的数据
        /// </summary>
        public String RFIDInfo
        {
            get { return RFIDData; }
            set
            {
                RFIDData = value;
                if (PropertyChanged != null)
                {
                    //属性变更事件的传出名称要与绑定名称相同
                    PropertyChanged(this, new PropertyChangedEventArgs("RFIDInfo"));
                }
            }
        }

        #region Fields
        private String scannerData;
        private String RFIDData;

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
