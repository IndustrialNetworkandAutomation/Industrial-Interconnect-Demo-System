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
    /// logisticsControl.xaml 的交互逻辑
    /// </summary>
    public partial class logisticsControl : UserControl
    {
        public logisticsControl()
        {
            InitializeComponent();
            initTimeRefresh();
            trayPosition.DataContext = MainWindow.myLogisticData;
            doorPosition.DataContext = MainWindow.myLogisticData;
        }

        #region Fields
        //创建更新界面控件委托
        delegate void AsynUpdateTime();
        private Boolean ThreadEnable = true;
        public Boolean moveFinished = false;
        private String selectedSystem = "";
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
            parentWindow.removeLogisticsControl();
        }

        private void No1SystemButton_Click(object sender, RoutedEventArgs e)
        {
            No1SystemButton.BorderBrush = Brushes.Yellow;
            No2SystemButton.BorderBrush = Brushes.Black;
            selectedSystem = "1";
        }

        private void No2SystemButton_Click(object sender, RoutedEventArgs e)
        {
            No1SystemButton.BorderBrush = Brushes.Black;
            No2SystemButton.BorderBrush = Brushes.Yellow;
            selectedSystem = "2";
        }

        private void powerOnButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSystem == "")
            {
                MessageBox.Show("请先选择需要操作的系统！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                switch (selectedSystem)
                {
                    case "1":
                        No1SystemButton.Background = Brushes.Green;
                        break;
                    case "2":
                        No2SystemButton.Background = Brushes.Green;
                        break;
                }
            }
        }

        private void powerOffButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSystem == "")
            {
                MessageBox.Show("请先选择需要操作的系统！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                switch (selectedSystem)
                {
                    case "1":
                        No1SystemButton.Background = Brushes.Red;
                        break;
                    case "2":
                        No2SystemButton.Background = Brushes.Red;
                        break;
                }
            }
        }

        private void up1Button_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.myLogisticData.trayPosition < 100)
            {
                MainWindow.myLogisticData.trayPosition = MainWindow.myLogisticData.trayPosition + 1;
            }
            else
            {
                MessageBox.Show("达到最高点", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void down1Button_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.myLogisticData.trayPosition > 0)
            {
                MainWindow.myLogisticData.trayPosition = MainWindow.myLogisticData.trayPosition - 1;
            }
            else
            {
                MessageBox.Show("达到最低点", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void up2Button_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.myLogisticData.doorPosition < 100)
            {
                MainWindow.myLogisticData.doorPosition = MainWindow.myLogisticData.doorPosition + 1;
            }
            else
            {
                MessageBox.Show("达到最高点", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void down2Button_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.myLogisticData.doorPosition > 0)
            {
                MainWindow.myLogisticData.doorPosition = MainWindow.myLogisticData.doorPosition - 1;
            }
            else
            {
                MessageBox.Show("达到最低点", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
