using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Media.Animation;

namespace animationDemo.ControlFolder
{
    /// <summary>
    /// controlMain.xaml 的交互逻辑
    /// </summary>
    public partial class controlMain : UserControl
    {
        #region Initialization
        public controlMain()
        {
            InitializeComponent();
            initTimeRefresh();
        }
        #endregion

        #region Fields
        //创建更新界面控件委托
        delegate void AsynUpdateTime();
        private Boolean ThreadEnable = true;
        public Boolean moveFinished = false;
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

        public void stopThread(){
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
            ColorAnimation ct = new ColorAnimation();
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
            parentWindow.removeControlMain();
        }

        private void ServoMotorButton_Click(object sender, RoutedEventArgs e)
        {
            exitControl();
            MainWindow parentWindow = ((Grid)this.Parent).Parent as MainWindow;
            parentWindow.showMotorControl();
        }

        private void RobotButton_Click(object sender, RoutedEventArgs e)
        {
            exitControl();
            MainWindow parentWindow = ((Grid)this.Parent).Parent as MainWindow;
            parentWindow.showRobotControl();
        }

        private void InspectButton_Click(object sender, RoutedEventArgs e)
        {
            exitControl();
            MainWindow parentWindow = ((Grid)this.Parent).Parent as MainWindow;
            parentWindow.showInspectControl();
        }

        private void PneumaticButton_Click(object sender, RoutedEventArgs e)
        {
            exitControl();
            MainWindow parentWindow = ((Grid)this.Parent).Parent as MainWindow;
            parentWindow.showPneumaticControl();
        }

        private void LogisticsButton_Click(object sender, RoutedEventArgs e)
        {
            exitControl();
            MainWindow parentWindow = ((Grid)this.Parent).Parent as MainWindow;
            parentWindow.showLogisticsControl();
        }

        private void IOButton_Click(object sender, RoutedEventArgs e)
        {
            exitControl();
            MainWindow parentWindow = ((Grid)this.Parent).Parent as MainWindow;
            parentWindow.showIOControl();
        }

        private void agvButton_Click(object sender, RoutedEventArgs e)
        {
            exitControl();
            MainWindow parentWindow = ((Grid)this.Parent).Parent as MainWindow;
            parentWindow.showRobotinoControl();
        }

        private void UAVButton_Click(object sender, RoutedEventArgs e)
        {
            exitControl();
            MainWindow parentWindow = ((Grid)this.Parent).Parent as MainWindow;
            parentWindow.showUAVControl();
        }
    }
}
