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
    /// IOControl.xaml 的交互逻辑
    /// </summary>
    public partial class IOControl : UserControl
    {
        public IOControl()
        {
            InitializeComponent();
            initTimeRefresh();
            voltageBox.DataContext = MainWindow.myIOData;
        }

        #region Fields
        private Boolean light1Sign = false;
        private Boolean light2Sign = false;
        private Boolean light3Sign = false;

        private Boolean relay1Sign = false;
        private Boolean relay2Sign = false;
        private Boolean relay3Sign = false;

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
            parentWindow.removeIOControl();
        }

        private void Light1_Click(object sender, RoutedEventArgs e)
        {
            if(light1Sign){
                Light1.Background = Brushes.Gainsboro;
                light1Sign = false;
            }
            else
            {
                Light1.Background = Brushes.Gold;
                light1Sign = true;
            }
        }

        private void Light2_Click(object sender, RoutedEventArgs e)
        {
            if (light2Sign)
            {
                Light2.Background = Brushes.Gainsboro;
                light2Sign = false;
            }
            else
            {
                Light2.Background = Brushes.Gold;
                light2Sign = true;
            }
        }

        private void Light3_Click(object sender, RoutedEventArgs e)
        {
            if (light3Sign)
            {
                Light3.Background = Brushes.Gainsboro;
                light3Sign = false;
            }
            else
            {
                Light3.Background = Brushes.Gold;
                light3Sign = true;
            }
        }

        private void Relay1_Click(object sender, RoutedEventArgs e)
        {
            if (relay1Sign)
            {
                Relay1.Background = Brushes.Gainsboro;
                relay1Sign = false;
            }
            else
            {
                Relay1.Background = Brushes.Red;
                relay1Sign = true;
            }
        }

        private void Relay2_Click(object sender, RoutedEventArgs e)
        {
            if (relay2Sign)
            {
                Relay2.Background = Brushes.Gainsboro;
                relay2Sign = false;
            }
            else
            {
                Relay2.Background = Brushes.Red;
                relay2Sign = true;
            }
        }

        private void Relay3_Click(object sender, RoutedEventArgs e)
        {
            if (relay3Sign)
            {
                Relay3.Background = Brushes.Gainsboro;
                relay3Sign = false;
            }
            else
            {
                Relay3.Background = Brushes.Red;
                relay3Sign = true;
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if(MainWindow.myIOData.voltage < 10)
            {
                MainWindow.myIOData.voltage = MainWindow.myIOData.voltage + 0.1;
            }
            else
            {
                MessageBox.Show("已达到最高电压值！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void reduceButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.myIOData.voltage > 0)
            {
                MainWindow.myIOData.voltage = MainWindow.myIOData.voltage - 0.1;
            }
            else
            {
                MessageBox.Show("电压已达到0！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
