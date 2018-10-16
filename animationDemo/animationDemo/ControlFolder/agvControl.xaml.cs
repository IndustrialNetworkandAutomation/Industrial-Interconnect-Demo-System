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

using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

using System.Windows.Media.Animation;
using System.Threading;

namespace animationDemo.ControlFolder
{
    /// <summary>
    /// agvControl.xaml 的交互逻辑
    /// </summary>
    public partial class agvControl : UserControl
    {
        public agvControl()
        {
            InitializeComponent();
            initTimeRefresh();

            xValue.DataContext = MainWindow.myAgvData;
            yValue.DataContext = MainWindow.myAgvData;
            phiValue.DataContext = MainWindow.myAgvData;
            vValue.DataContext = MainWindow.myAgvData;

            initData();
        }

        #region Fields
        //创建更新界面控件委托
        delegate void AsynUpdateTime();
        private Boolean ThreadEnable = true;
        //MQTT代理地址
        String MQTT_BROKER_ADDRESS = "192.168.43.129";
        //MQTT代理端口号
        int brokerPort = 61613;
        String userName = "admin";
        String password = "password";
        String clientId = "mqtt4IIS";
        double curMoveSpeed = 0;
        double curRotateSpeed = 0;
        String curMove = "";
        MqttClient client;
        #endregion

        private void initData()
        {
            string[] topicX = { "robotinoX" };
            string[] topicY = { "robotinoY" };
            string[] topicPhi = { "robotinoPhi" };
            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE };
            try
            {
                client = new MqttClient(MQTT_BROKER_ADDRESS, brokerPort, false, null, null, MqttSslProtocols.None);

                client.MqttMsgPublishReceived += new MqttClient.MqttMsgPublishEventHandler(messageReceive);

                //连接Broker
                client.Connect(clientId, userName, password);

                client.Subscribe(topicX, qosLevels);
                client.Subscribe(topicY, qosLevels);
                client.Subscribe(topicPhi, qosLevels);
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接失败，请查看后台日志！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine("连接失败！错误信息:" + ex);
            }
        }

        static void messageReceive(object sender, MqttMsgPublishEventArgs e)
        {
            string msg = "Topic:" + e.Topic + "   Message:" + System.Text.Encoding.Default.GetString(e.Message);
            Console.WriteLine(msg);
            if (e.Topic.Equals("robotinoX"))
            {
                String xData = System.Text.Encoding.Default.GetString(e.Message);
                MainWindow.myAgvData.AgvX = double.Parse(xData);
            }
            else if (e.Topic.Equals("robotinoY"))
            {
                String yData = System.Text.Encoding.Default.GetString(e.Message);
                MainWindow.myAgvData.AgvY = double.Parse(yData);
            }
            else if (e.Topic.Equals("robotinoPhi"))
            {
                String phiData = System.Text.Encoding.Default.GetString(e.Message);
                MainWindow.myAgvData.AgvPhi = double.Parse(phiData);
            }
        }

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

        public void stopThread()
        {
            ThreadEnable = false;
        }

        private void speedupButton_Click(object sender, RoutedEventArgs e)
        {
            if (curMove.Equals("clockwise") || curMove.Equals("anticlock"))
            {
                curRotateSpeed = curRotateSpeed + 0.5;
                client.Publish("RobotinoControl", Encoding.UTF8.GetBytes(curMove + ";" + curRotateSpeed), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            else
            {
                curMoveSpeed = curMoveSpeed + 0.1;
                client.Publish("RobotinoControl", Encoding.UTF8.GetBytes(curMove + ";" + curMoveSpeed), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
        }

        private void forwardButton_Click(object sender, RoutedEventArgs e)
        {
            curMoveSpeed = 0.1;
            curMove = "forward";
            client.Publish("RobotinoControl", Encoding.UTF8.GetBytes("forward;" + curMoveSpeed), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        private void speedDownButton_Click(object sender, RoutedEventArgs e)
        {
            if ((curMove.Equals("clockwise") || curMove.Equals("anticlock")) && curRotateSpeed >= 0)
            {
                curRotateSpeed = curRotateSpeed - 0.5;
                client.Publish("RobotinoControl", Encoding.UTF8.GetBytes(curMove + ";" + curRotateSpeed), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            if ((curMove.Equals("forward") || curMove.Equals("backword") || curMove.Equals("left") || curMove.Equals("right")) && curMoveSpeed >= 0)
            {
                curMoveSpeed = curMoveSpeed - 0.1;
                client.Publish("RobotinoControl", Encoding.UTF8.GetBytes(curMove + ";" + curMoveSpeed), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
        }

        private void leftButton_Click(object sender, RoutedEventArgs e)
        {
            curMoveSpeed = 0.1;
            curMove = "left";
            client.Publish("RobotinoControl", Encoding.UTF8.GetBytes("left;" + curMoveSpeed), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            client.Publish("RobotinoControl", Encoding.UTF8.GetBytes("stop"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            curMoveSpeed = 0;
            curRotateSpeed = 0;
        }

        private void rightButton_Click(object sender, RoutedEventArgs e)
        {
            curMoveSpeed = 0.1;
            curMove = "left";
            client.Publish("RobotinoControl", Encoding.UTF8.GetBytes("right;" + curMoveSpeed), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        private void clockwiseButton_Click(object sender, RoutedEventArgs e)
        {
            curRotateSpeed = 1;
            curMove = "clockwise";
            client.Publish("RobotinoControl", Encoding.UTF8.GetBytes("clockwise;" + curRotateSpeed), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        private void backwardButton_Click(object sender, RoutedEventArgs e)
        {
            curMoveSpeed = 0.1;
            curMove = "backward";
            client.Publish("RobotinoControl", Encoding.UTF8.GetBytes("backward;" + curMoveSpeed), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        private void antiClockButton_Click(object sender, RoutedEventArgs e)
        {
            curRotateSpeed = 1;
            curMove = "anticlock";
            client.Publish("RobotinoControl", Encoding.UTF8.GetBytes("anticlock;" + curRotateSpeed), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
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
            client.Disconnect();
            MainWindow parentWindow = ((Grid)this.Parent).Parent as MainWindow;
            parentWindow.removeAGVControl();
        }
    }
}
