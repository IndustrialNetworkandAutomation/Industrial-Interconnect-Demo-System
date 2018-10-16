using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Media.Animation;

namespace animationDemo.ControlFolder
{
    /// <summary>
    /// inspectControl.xaml 的交互逻辑
    /// </summary>
    public partial class inspectControl : UserControl
    {
        public inspectControl()
        {
            InitializeComponent();
            initTimeRefresh();
            scannerBox.DataContext = MainWindow.myInspectData;
            RFIDBox.DataContext = MainWindow.myInspectData;
        }

        #region Fields
        //创建更新界面控件委托
        delegate void AsynUpdateTime();
        private Boolean ThreadEnable = true;
        public Boolean moveFinished = false;
        private String selectedType = "";
        #endregion

        /// <summary>
        /// 开启时间更新线程
        /// </summary>
        private void initTimeRefresh()
        {
            DateTime curTime = DateTime.Now;
            timeLabel.Content = "时间：" + curTime.ToLongDateString() + curTime.ToLongTimeString();
            Thread thread = new Thread(new ThreadStart(refreshTime));
            thread.Start();
        }

        private void refreshTime()
        {
            while (ThreadEnable)
            {
                refreshTimeInstance();
                Thread.Sleep(1000);
            }
        }

        public void stopThread()
        {
            ThreadEnable = false;
        }

        private void refreshTimeInstance()
        {
            if (!this.CheckAccess())
            {
                // Asynchronous execution of the valueChanged delegate.
                this.Dispatcher.BeginInvoke(new AsynUpdateTime(refreshTimeInstance));
                return;
            }
            DateTime curTime = DateTime.Now;
            timeLabel.Content = "时间：" + curTime.ToLongDateString() + curTime.ToLongTimeString();
        }

        public void exitControl()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.Completed += new EventHandler(moveCompleted);
            Duration duration = new Duration(TimeSpan.FromSeconds(0.5));
            //this.RenderTransform = ct;
            animation.To = 0.3;
            animation.Duration = duration;
            this.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void moveCompleted(object sender, EventArgs e)
        {
            MainWindow parentWindow = ((Grid)this.Parent).Parent as MainWindow;
            parentWindow.removeInspectControl();
        }

        private void scannerButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.myInspectData.scannerInfo = "产品编号：8638492651";
            scannerButton.Background = Brushes.Red;
            RFIDButton.Background = Brushes.Gainsboro;
            selectedType = "Scanner";
        }

        private void RFIDButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.myInspectData.RFIDInfo = "物料编号：2630846233";
            scannerButton.Background = Brushes.Gainsboro;
            RFIDButton.Background = Brushes.Red;
            selectedType = "RFID";
        }
    }
}
