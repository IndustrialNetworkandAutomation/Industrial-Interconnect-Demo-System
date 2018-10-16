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
using Opc.Ua;
using animationDemo.opcRealated.opcUA;

namespace animationDemo.ControlFolder
{
    /// <summary>
    /// motorControl.xaml 的交互逻辑
    /// </summary>
    public partial class motorControl : UserControl
    {
        #region initialization
        public motorControl()
        {
            m_Server = connectServer();
            InitializeComponent();
            initTimeRefresh();
            initData();
            xValue.DataContext = MainWindow.myMotorPos;
            yValue.DataContext = MainWindow.myMotorPos;
        }

        public motorControl(opcUAServer curServer)
        {
            m_Server = curServer;
            InitializeComponent();
            initTimeRefresh();
            initData();
            xValue.DataContext = MainWindow.myMotorPos;
            yValue.DataContext = MainWindow.myMotorPos;
        }
        #endregion

        #region Fields
        //创建更新界面控件委托
        delegate void AsynUpdateTime();
        private Boolean ThreadEnable = true;
        public Boolean moveFinished = false;
        private String selectedAxis = "";

        opcUAServer m_Server = new opcUAServer();

        private DataValue m_currentXValue;
        private DataValue m_currentYValue;
        #endregion

        private opcUAServer connectServer()
        {
            String serverIP = "127.0.0.1";        //设备的IP地址
            String serverPort = "49320";          //倍福设备的端口号

            opcUAServer uaServer = new opcRealated.opcUA.opcUAServer();
            uaServer.CertificateEvent += new certificateValidation(m_Server_CertificateEvent);
            String connectVar = "opc.tcp://" + serverIP + ":" + serverPort;
            try
            {
                uaServer.Connect(connectVar);
            }
            catch (Exception ex)
            {
                MessageBox.Show("服务器连接异常，错误原因：" + ex, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine("服务器连接异常，错误原因：" + ex);
            }
            return uaServer;
        }

        void m_Server_CertificateEvent(CertificateValidator validator, CertificateValidationEventArgs e)
        {
            if (e.Accept == false)
            {
                e.Accept = true;
            }
        }

        private void initData()
        {
            //下一步增加数据连接，保证能够对OPC UA服务器进行修改

            Subscription in_Subscription = m_Server.AddSubscription(100);
            object serverHandle = null;                     //初始化服务器句柄
            String nodeIDXStr = "ns=2;s=Channel1.Device1.Tag1";
            NodeId nodeIdX = new NodeId(nodeIDXStr);

            String nodeIDYStr = "ns=2;s=Channel1.Device1.Tag2";
            NodeId nodeIdY = new NodeId(nodeIDYStr);

            String iidX = "motorXPosition";
            object itemX = iidX;

            String iidY = "motorYPosition";
            object itemY = iidY;

            try
            {
                in_Subscription.AddDataMonitoredItem(nodeIdX, itemX, ClientApi_ValueChanged, 100, out serverHandle);
                in_Subscription.AddDataMonitoredItem(nodeIdY, itemY, ClientApi_ValueChanged, 100, out serverHandle);

                MainWindow.myMotorPos.XValue = 0;
                MainWindow.myMotorPos.YValue = 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine("添加项时出现异常，异常信息：" + ex);
            }
        }

        /// <summary>
        /// OPC UA值的响应函数
        /// </summary>
        /// <param name="clientHandle"></param>
        /// <param name="value"></param>
        private void ClientApi_ValueChanged(object clientHandle, DataValue value)
        {
            // 如果需要唤醒主线程的更新控件功能. 
            if (!this.CheckAccess())
            {
                // Asynchronous execution of the valueChanged delegate.
                this.Dispatcher.BeginInvoke(new valueChanged(ClientApi_ValueChanged), clientHandle, value);
                return;
            }

            String keyID = (String)clientHandle;

            if (keyID.Contains("motorX"))
            {
                MainWindow.myMotorPos.XValue = double.Parse(Utils.Format("{0}", value.Value));
                m_currentXValue = value;
            }
            else if (keyID.Contains("motorY"))
            {
                MainWindow.myMotorPos.YValue = double.Parse(Utils.Format("{0}", value.Value));
                m_currentYValue = value;
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

        private void xAxisButton_Click(object sender, RoutedEventArgs e)
        {
            xAxisButton.Background = Brushes.Red;
            yAxisButton.Background = Brushes.Gainsboro;
            selectedAxis = "X";
        }

        private void yAxisButton_Click(object sender, RoutedEventArgs e)
        {
            yAxisButton.Background = Brushes.Red;
            xAxisButton.Background = Brushes.Gainsboro;
            selectedAxis = "Y";
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
                    case "X":
                        WriteValuePlus("X");
                        break;
                    case "Y":
                        //MainWindow.myMotorPos.YValue = MainWindow.myMotorPos.YValue + 1;
                        WriteValuePlus("Y");
                        break;
                }

            }
        }

        private void WriteValuePlus(String valueSign)
        {
            try
            {
                // 声明需要写入的数据
                NodeIdCollection nodesToWrite = new NodeIdCollection(1);
                DataValueCollection values = new DataValueCollection(1);
                StatusCodeCollection results = null;

                Variant variant = new Variant();
                DataValue value = new DataValue();

                String sNodeId = "";
                if (valueSign.Equals("X"))
                {
                    String sValue = (MainWindow.myMotorPos.XValue + 1).ToString();
                    //variant = new Variant(MainWindow.myMotorPos.YValue + 1);
                    variant = new Variant(Convert.ChangeType(sValue, m_currentXValue.Value.GetType()));
                    value = new DataValue(variant);
                    values.Add(value);
                    sNodeId = "ns=2;s=Channel1.Device1.Tag1";
                }
                else if (valueSign.Equals("Y"))
                {
                    //variant = new Variant(MainWindow.myMotorPos.YValue + 1);
                    String sValue = (MainWindow.myMotorPos.YValue + 1).ToString();
                    Type testType = m_currentYValue.Value.GetType();
                    variant = new Variant(Convert.ChangeType(sValue, m_currentYValue.Value.GetType()));
                    value = new DataValue(variant);
                    values.Add(value);
                    sNodeId = "ns=2;s=Channel1.Device1.Tag2";
                }
                
                NodeId nodeId = new NodeId(sNodeId);
                nodesToWrite.Add(nodeId);

                // 调用API方法
                m_Server.WriteValues(
                    nodesToWrite,
                    values,
                    out results);
            }
            catch (Exception ex)
            {
                MessageBox.Show("写入数据失败！错误原因：" + ex, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    case "X":
                        WriteValueMinus("X");
                        break;
                    case "Y":
                        WriteValueMinus("Y");
                        break;
                }

            }
        }

        private void WriteValueMinus(String valueSign)
        {
            try
            {
                // 声明需要写入的数据
                NodeIdCollection nodesToWrite = new NodeIdCollection(1);
                DataValueCollection values = new DataValueCollection(1);
                StatusCodeCollection results = null;

                Variant variant = new Variant();
                DataValue value = new DataValue();

                String sNodeId = "";
                if (valueSign.Equals("X"))
                {
                    String sValue = (MainWindow.myMotorPos.XValue - 1).ToString();
                    variant = new Variant(Convert.ChangeType(sValue, m_currentXValue.Value.GetType()));
                    value = new DataValue(variant);
                    values.Add(value);
                    sNodeId = "ns=2;s=Channel1.Device1.Tag1";
                }
                else if (valueSign.Equals("Y"))
                {
                    String sValue = (MainWindow.myMotorPos.YValue - 1).ToString();
                    variant = new Variant(Convert.ChangeType(sValue, m_currentYValue.Value.GetType()));
                    value = new DataValue(variant);
                    values.Add(value);
                    sNodeId = "ns=2;s=Channel1.Device1.Tag2";
                }

                NodeId nodeId = new NodeId(sNodeId);
                nodesToWrite.Add(nodeId);

                // 调用API方法
                m_Server.WriteValues(
                    nodesToWrite,
                    values,
                    out results);
            }
            catch (Exception ex)
            {
                MessageBox.Show("写入数据失败！错误原因：" + ex, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
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
            MainWindow parentWindow = ((Grid)this.Parent).Parent as MainWindow;
            parentWindow.removeMotorControl();
        }
    }
}
