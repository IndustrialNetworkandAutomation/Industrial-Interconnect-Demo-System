using System;
using System.Collections.Generic;
using System.Threading;
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

using URSDK.RobController.Communication;

namespace animationDemo.ControlFolder
{
    /// <summary>
    /// robotControl.xaml 的交互逻辑
    /// </summary>
    public partial class robotControl : UserControl
    {
        public robotControl()
        {
            InitializeComponent();
            initTimeRefresh();
            a1Value.DataContext = MainWindow.myRobotData;
            a2Value.DataContext = MainWindow.myRobotData;
            a3Value.DataContext = MainWindow.myRobotData;
            a4Value.DataContext = MainWindow.myRobotData;
            a5Value.DataContext = MainWindow.myRobotData;
            a6Value.DataContext = MainWindow.myRobotData;
            pawProgress.DataContext = MainWindow.myRobotData;
            fixtureProgress.DataContext = MainWindow.myRobotData;

            //利用机器人模块测试MQTT正常运转
            //connectMQTTServer();

            //String testDir = System.Environment.CurrentDirectory + "\\RTDEConfig.xml";
            initURRobot();
        }

        #region Fields
        //创建更新界面控件委托
        delegate void AsynUpdateTime();
        private Boolean ThreadEnable = true;
        public Boolean moveFinished = false;
        private String selectedAxis = "";

        //Definition
        private DashBoard aDashBoard;        //Dashboard instance, port: 29999
        private RTClient aRTClient;          //RTClient instance, port: 30003
        private RTDE aRTDE;                  //RTDE instance, port: 30004
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

        private void initURRobot()
        {
            aDashBoard = new DashBoard("192.168.100.11");
            aRTClient = new RTClient("192.168.100.11");
            aRTDE = new RTDE(System.Environment.CurrentDirectory + "\\RTDEConfig.xml", "192.168.100.11");
            //RTDE用于采集数据
            aRTDE.startSync();
            if (!aRTDE.isSynchronizing)
                Thread.Sleep(10);
            //RTClient用来控制
            aRTClient.startRTClient();
            while (aRTClient.Status != RTClientStatus.Syncing)
                Thread.Sleep(10);
            Thread thread = new Thread(new ThreadStart(refreshRobotData));
            thread.Start();
        }

        private void connectMQTTServer()
        {
            String MQTT_BROKER_ADDRESS = "192.168.43.129";
            int brokerPort = 61613;
            String clientId = "mqtt4IIS";
            String userName = "admin";
            String password = "password";

            string[] topic = {"robotinoPhi"};
            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE};
            try
            {
                MqttClient client = new MqttClient(MQTT_BROKER_ADDRESS, brokerPort, false, null, null, MqttSslProtocols.None);

                client.MqttMsgPublishReceived += new MqttClient.MqttMsgPublishEventHandler(messageReceive);

                //连接Broker
                client.Connect(clientId, userName, password);

                client.Subscribe(topic, qosLevels);
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
        }

        private void refreshTime()
        {
            while (ThreadEnable)
            {
                refreshTimeInstance();
                Thread.Sleep(1000);
            }
        }

        private void refreshRobotData()
        {
            while (ThreadEnable)
            {
                refreshRobotDataInstance();
                Thread.Sleep(100);
            }
        }

        public void stopThread()
        {
            ThreadEnable = false;
        }

        private void refreshRobotDataInstance()
        {
            if (aRTDE.isSynchronizing)
            {
                RTDEOutputObj OutObj = aRTDE.getOutputObj();
                //textBox7.Text = "Actual tcp pose: "+OutObj.actual_TCP_pose.ToString()+"\n";
                //textBox7.Text =textBox7.Text+" Elbow position: "+ OutObj.elbow_position.ToString();
                Console.WriteLine(OutObj.actual_q.ToString());
                MainWindow.myRobotData.a1Value = Math.Round(OutObj.actual_q.X / Math.PI * 180, 2);
                MainWindow.myRobotData.a2Value = Math.Round(OutObj.actual_q.Y / Math.PI * 180, 2);
                MainWindow.myRobotData.a3Value = Math.Round(OutObj.actual_q.Z / Math.PI *180, 2);
                MainWindow.myRobotData.a4Value = Math.Round(OutObj.actual_q.Rx / Math.PI * 180, 2);
                MainWindow.myRobotData.a5Value = Math.Round(OutObj.actual_q.Ry / Math.PI * 180, 2);
                MainWindow.myRobotData.a6Value = Math.Round(OutObj.actual_q.Rz / Math.PI * 180, 2);

                MainWindow.myRobotData.raw1Value = OutObj.actual_q.X;
                MainWindow.myRobotData.raw2Value = OutObj.actual_q.Y;
                MainWindow.myRobotData.raw3Value = OutObj.actual_q.Z;
                MainWindow.myRobotData.raw4Value = OutObj.actual_q.Rx;
                MainWindow.myRobotData.raw5Value = OutObj.actual_q.Ry;
                MainWindow.myRobotData.raw6Value = OutObj.actual_q.Rz;
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

        private void a1AxisButton_Click(object sender, RoutedEventArgs e)
        {
            a1AxisButton.Background = Brushes.Red;
            a2AxisButton.Background = Brushes.Gainsboro;
            a3AxisButton.Background = Brushes.Gainsboro;
            a4AxisButton.Background = Brushes.Gainsboro;
            a5AxisButton.Background = Brushes.Gainsboro;
            a6AxisButton.Background = Brushes.Gainsboro;

            fixtureButton.Background = Brushes.Gainsboro;
            pawButton.Background = Brushes.Gainsboro;

            selectedAxis = "A1";
        }

        private void a2AxisButton_Click(object sender, RoutedEventArgs e)
        {
            a1AxisButton.Background = Brushes.Gainsboro;
            a2AxisButton.Background = Brushes.Red;
            a3AxisButton.Background = Brushes.Gainsboro;
            a4AxisButton.Background = Brushes.Gainsboro;
            a5AxisButton.Background = Brushes.Gainsboro;
            a6AxisButton.Background = Brushes.Gainsboro;

            fixtureButton.Background = Brushes.Gainsboro;
            pawButton.Background = Brushes.Gainsboro;

            selectedAxis = "A2";
        }

        private void a3AxisButton_Click(object sender, RoutedEventArgs e)
        {
            a1AxisButton.Background = Brushes.Gainsboro;
            a2AxisButton.Background = Brushes.Gainsboro;
            a3AxisButton.Background = Brushes.Red;
            a4AxisButton.Background = Brushes.Gainsboro;
            a5AxisButton.Background = Brushes.Gainsboro;
            a6AxisButton.Background = Brushes.Gainsboro;

            fixtureButton.Background = Brushes.Gainsboro;
            pawButton.Background = Brushes.Gainsboro;

            selectedAxis = "A3";
        }

        private void a4AxisButton_Click(object sender, RoutedEventArgs e)
        {
            a1AxisButton.Background = Brushes.Gainsboro;
            a2AxisButton.Background = Brushes.Gainsboro;
            a3AxisButton.Background = Brushes.Gainsboro;
            a4AxisButton.Background = Brushes.Red;
            a5AxisButton.Background = Brushes.Gainsboro;
            a6AxisButton.Background = Brushes.Gainsboro;

            fixtureButton.Background = Brushes.Gainsboro;
            pawButton.Background = Brushes.Gainsboro;

            selectedAxis = "A4";
        }

        private void a5AxisButton_Click(object sender, RoutedEventArgs e)
        {
            a1AxisButton.Background = Brushes.Gainsboro;
            a2AxisButton.Background = Brushes.Gainsboro;
            a3AxisButton.Background = Brushes.Gainsboro;
            a4AxisButton.Background = Brushes.Gainsboro;
            a5AxisButton.Background = Brushes.Red;
            a6AxisButton.Background = Brushes.Gainsboro;

            fixtureButton.Background = Brushes.Gainsboro;
            pawButton.Background = Brushes.Gainsboro;

            selectedAxis = "A5";
        }

        private void a6AxisButton_Click(object sender, RoutedEventArgs e)
        {
            a1AxisButton.Background = Brushes.Gainsboro;
            a2AxisButton.Background = Brushes.Gainsboro;
            a3AxisButton.Background = Brushes.Gainsboro;
            a4AxisButton.Background = Brushes.Gainsboro;
            a5AxisButton.Background = Brushes.Gainsboro;
            a6AxisButton.Background = Brushes.Red;

            fixtureButton.Background = Brushes.Gainsboro;
            pawButton.Background = Brushes.Gainsboro;

            selectedAxis = "A6";
        }

        private void fixtureButton_Click(object sender, RoutedEventArgs e)
        {
            a1AxisButton.Background = Brushes.Gainsboro;
            a2AxisButton.Background = Brushes.Gainsboro;
            a3AxisButton.Background = Brushes.Gainsboro;
            a4AxisButton.Background = Brushes.Gainsboro;
            a5AxisButton.Background = Brushes.Gainsboro;
            a6AxisButton.Background = Brushes.Gainsboro;

            fixtureButton.Background = Brushes.Red;
            pawButton.Background = Brushes.Gainsboro;

            selectedAxis = "F";
        }

        private void pawButton_Click(object sender, RoutedEventArgs e)
        {
            a1AxisButton.Background = Brushes.Gainsboro;
            a2AxisButton.Background = Brushes.Gainsboro;
            a3AxisButton.Background = Brushes.Gainsboro;
            a4AxisButton.Background = Brushes.Gainsboro;
            a5AxisButton.Background = Brushes.Gainsboro;
            a6AxisButton.Background = Brushes.Gainsboro;

            fixtureButton.Background = Brushes.Gainsboro;
            pawButton.Background = Brushes.Red;

            selectedAxis = "P";
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAxis == "")
            {
                MessageBox.Show("请先选择需要进行的操作！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                switch (selectedAxis)
                {
                    case "A1":
                        //MainWindow.myRobotData.a1Value = MainWindow.myRobotData.a1Value + 0.1;
                        String actionStr = "movej([d2r(" + (MainWindow.myRobotData.a1Value + 10) + "),"
                            + "d2r(" + MainWindow.myRobotData.a2Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a3Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a4Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a5Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a6Value + ")"
                            + "], a = 0.1, v = 0.1)";
                        aRTClient.sendScript(actionStr);
                        break;
                    case "A2":
                        //MainWindow.myRobotData.a2Value = MainWindow.myRobotData.a2Value + 0.1;
                        actionStr = "movej([d2r(" + MainWindow.myRobotData.a1Value + "),"
                            + "d2r(" + (MainWindow.myRobotData.a2Value + 10) + "),"
                            + "d2r(" + MainWindow.myRobotData.a3Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a4Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a5Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a6Value + ")"
                            + "], a = 0.1, v = 0.1)";
                        aRTClient.sendScript(actionStr);
                        break;
                    case "A3":
                        //MainWindow.myRobotData.a3Value = MainWindow.myRobotData.a3Value + 0.1;
                        actionStr = "movej([d2r(" + MainWindow.myRobotData.a1Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a2Value + "),"
                            + "d2r(" + (MainWindow.myRobotData.a3Value + 10) + "),"
                            + "d2r(" + MainWindow.myRobotData.a4Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a5Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a6Value + ")"
                            + "], a = 0.1, v = 0.1)";
                        aRTClient.sendScript(actionStr);
                        break;
                    case "A4":
                        //MainWindow.myRobotData.a4Value = MainWindow.myRobotData.a4Value + 0.1;
                        actionStr = "movej([d2r(" + MainWindow.myRobotData.a1Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a2Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a3Value + "),"
                            + "d2r(" + (MainWindow.myRobotData.a4Value + 10) + "),"
                            + "d2r(" + MainWindow.myRobotData.a5Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a6Value + ")"
                            + "], a = 0.1, v = 0.1)";
                        aRTClient.sendScript(actionStr);
                        break;
                    case "A5":
                        //MainWindow.myRobotData.a5Value = MainWindow.myRobotData.a5Value + 0.1;
                        actionStr = "movej([d2r(" + MainWindow.myRobotData.a1Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a2Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a3Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a4Value + "),"
                            + "d2r(" + (MainWindow.myRobotData.a5Value + 10) + "),"
                            + "d2r(" + MainWindow.myRobotData.a6Value + ")"
                            + "], a = 0.1, v = 0.1)";
                        aRTClient.sendScript(actionStr);
                        break;
                    case "A6":
                        //MainWindow.myRobotData.a6Value = MainWindow.myRobotData.a6Value + 0.1;
                        actionStr = "movej([d2r(" + MainWindow.myRobotData.a1Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a2Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a3Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a4Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a5Value + "),"
                            + "d2r(" + (MainWindow.myRobotData.a6Value + 10) + ")"
                            + "], a = 0.1, v = 0.1)";
                        aRTClient.sendScript(actionStr);
                        break;
                    case "F":
                        if (MainWindow.myRobotData.fixtureProgress < 100)
                        {
                            MainWindow.myRobotData.fixtureProgress = MainWindow.myRobotData.fixtureProgress + 1;
                        }
                        else
                        {
                            MessageBox.Show("已经张开至最大值", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        break;
                    case "P":
                        if (MainWindow.myRobotData.pawProgress < 100)
                        {
                            MainWindow.myRobotData.pawProgress = MainWindow.myRobotData.pawProgress + 1;
                        }
                        else
                        {
                            MessageBox.Show("已经张开至最大值", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        break;
                }

            }
        }

        private void reduceButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAxis == "")
            {
                MessageBox.Show("请先选择需要进行的操作！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                switch (selectedAxis)
                {
                    case "A1":
                        //MainWindow.myRobotData.a1Value = MainWindow.myRobotData.a1Value - 0.1;
                        String actionStr = "movej([d2r(" + (MainWindow.myRobotData.a1Value - 10) + "),"
                            + "d2r(" + MainWindow.myRobotData.a2Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a3Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a4Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a5Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a6Value + ")"
                            + "], a = 0.1, v = 0.1)";
                        aRTClient.sendScript(actionStr);
                        break;
                    case "A2":
                        //MainWindow.myRobotData.a2Value = MainWindow.myRobotData.a2Value - 0.1;
                        actionStr = "movej([d2r(" + MainWindow.myRobotData.a1Value + "),"
                            + "d2r(" + (MainWindow.myRobotData.a2Value - 10) + "),"
                            + "d2r(" + MainWindow.myRobotData.a3Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a4Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a5Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a6Value + ")"
                            + "], a = 0.1, v = 0.1)";
                        aRTClient.sendScript(actionStr);
                        break;
                    case "A3":
                        //MainWindow.myRobotData.a3Value = MainWindow.myRobotData.a3Value - 0.1;
                        actionStr = "movej([d2r(" + MainWindow.myRobotData.a1Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a2Value + "),"
                            + "d2r(" + (MainWindow.myRobotData.a3Value - 10) + "),"
                            + "d2r(" + MainWindow.myRobotData.a4Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a5Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a6Value + ")"
                            + "], a = 0.1, v = 0.1)";
                        aRTClient.sendScript(actionStr);
                        break;
                    case "A4":
                        //MainWindow.myRobotData.a4Value = MainWindow.myRobotData.a4Value - 0.1;
                        actionStr = "movej([d2r(" + MainWindow.myRobotData.a1Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a2Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a3Value + "),"
                            + "d2r(" + (MainWindow.myRobotData.a4Value - 10) + "),"
                            + "d2r(" + MainWindow.myRobotData.a5Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a6Value + ")"
                            + "], a = 0.1, v = 0.1)";
                        aRTClient.sendScript(actionStr);
                        break;
                    case "A5":
                        //MainWindow.myRobotData.a5Value = MainWindow.myRobotData.a5Value - 0.1;
                        actionStr = "movej([d2r(" + MainWindow.myRobotData.a1Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a2Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a3Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a4Value + "),"
                            + "d2r(" + (MainWindow.myRobotData.a5Value - 10) + "),"
                            + "d2r(" + MainWindow.myRobotData.a6Value + ")"
                            + "], a = 0.1, v = 0.1)";
                        aRTClient.sendScript(actionStr);
                        break;
                    case "A6":
                        //MainWindow.myRobotData.a6Value = MainWindow.myRobotData.a6Value - 0.1;
                        actionStr = "movej([d2r(" + MainWindow.myRobotData.a1Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a2Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a3Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a4Value + "),"
                            + "d2r(" + MainWindow.myRobotData.a5Value + "),"
                            + "d2r(" + (MainWindow.myRobotData.a6Value - 10) + ")"
                            + "], a = 0.1, v = 0.1)";
                        aRTClient.sendScript(actionStr);
                        break;
                    case "F":
                        if (MainWindow.myRobotData.fixtureProgress > 0)
                        {
                            MainWindow.myRobotData.fixtureProgress = MainWindow.myRobotData.fixtureProgress - 1;
                        }
                        else
                        {
                            MessageBox.Show("角度已达到最小值", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        
                        break;
                    case "P":
                        if (MainWindow.myRobotData.pawProgress > 0)
                        {
                            MainWindow.myRobotData.pawProgress = MainWindow.myRobotData.pawProgress - 1;
                        }
                        else
                        {
                            MessageBox.Show("角度已达到最小值", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        
                        break;
                }
            }
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
            aRTDE.stopSync();
            MainWindow parentWindow = ((Grid)this.Parent).Parent as MainWindow;
            parentWindow.removeRobotControl();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            aDashBoard.loadProgram("test.urp");
            aDashBoard.play();
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            aDashBoard.stop();
        }
    }
}
