using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace animationDemo.ControlFolder
{
    /// <summary>
    /// uavControl.xaml 的交互逻辑
    /// </summary>
    public partial class uavControl : UserControl
    {
        public uavControl()
        {
            InitializeComponent();
            initTimeRefresh();

            batteryValue.DataContext = MainWindow.myUavData;
            vValue.DataContext = MainWindow.myUavData;

            initConnect();

        }

        #region Fields
        //创建更新界面控件委托
        delegate void AsynUpdateTime();
        private Boolean ThreadEnable = true;

        private UdpClient local;
        private static IPEndPoint epServer;

        private Boolean ifConnect = false;
        #endregion

        private void initConnect()
        {
            epServer = new IPEndPoint(IPAddress.Parse("192.168.10.1"), 8889);
            local = new UdpClient(9001);    //绑定本机IP和端口，9001

            byte[] data = new byte[1024];

            String sendStr = "command";

            data = Encoding.ASCII.GetBytes(sendStr);

            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(data, data.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(initReceiveCallback), null);

            //initSpeedQuest();
            initBatteryQuest();
        }

        /// <summary>
        /// 开启速度更新线程
        /// </summary>
        //private void initSpeedQuest()
        //{
        //    Thread thread = new Thread(new ThreadStart(quest4Speed));
        //    thread.Start();
        //}

        /// <summary>
        /// 开启电量更新线程
        /// </summary>
        private void initBatteryQuest()
        {
            Thread thread = new Thread(new ThreadStart(quest4Battery));
            thread.Start();
        }

        //private void quest4Speed()
        //{
        //    while (ThreadEnable)
        //    {
        //        speedQuestInstance();
        //        Thread.Sleep(1000);
        //    }
        //}

        private void quest4Battery()
        {
            while (ThreadEnable)
            {
                batteryQuestInstance();
                Thread.Sleep(1000);
            }
        }

        //private void speedQuestInstance()
        //{
        //    byte[] sendData = Encoding.ASCII.GetBytes("speed?");
        //    //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
        //    local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
        //    //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
        //    local.BeginReceive(new AsyncCallback(speedReceiveCallback), null);
        //}

        private void batteryQuestInstance()
        {
            byte[] sendData = Encoding.ASCII.GetBytes("battery?");
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(batteryReceiveCallback), null);
        }

        //private void speedReceiveCallback(IAsyncResult iar)
        //{
        //    //byte[] receiveData = myClient.EndReceive(iar, ref targetIpep);
        //    byte[] receiveData = local.EndReceive(iar, ref epServer);
        //    String speed = Encoding.ASCII.GetString(receiveData);
        //    try
        //    {
        //        MainWindow.myUavData.velocity = double.Parse(speed);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Write(ex);
        //    }
            
        //}

        private void batteryReceiveCallback(IAsyncResult iar)
        {
            //byte[] receiveData = myClient.EndReceive(iar, ref targetIpep);
            byte[] receiveData = local.EndReceive(iar, ref epServer);
            String battery = Encoding.ASCII.GetString(receiveData);

            try
            {
                MainWindow.myUavData.battery = double.Parse(battery);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            
        }

        private void initReceiveCallback(IAsyncResult iar)
        {
            //byte[] receiveData = myClient.EndReceive(iar, ref targetIpep);
            byte[] receiveData = local.EndReceive(iar, ref epServer);
            String connectSign = Encoding.ASCII.GetString(receiveData);
            if (connectSign == "OK")
            {
                ifConnect = true;
            }
            else
            {
                //MessageBox.Show("连接异常！请确认是否连接无人机Wifi！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SendCallback(IAsyncResult iar)
        {
            int sendCount = local.EndSend(iar);
            if (sendCount == 0)
            {
                Console.WriteLine("Send a message failure...");
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
            //client.Disconnect();
            MainWindow parentWindow = ((Grid)this.Parent).Parent as MainWindow;
            parentWindow.removeUAVControl();
        }

        private void takeoffButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] sendData = Encoding.ASCII.GetBytes("takeoff");
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(ReceiveCallback), null);

            MainWindow.myUavData.velocity = 100;
        }

        private void ReceiveCallback(IAsyncResult iar)
        {
            //byte[] receiveData = myClient.EndReceive(iar, ref targetIpep);
            byte[] receiveData = local.EndReceive(iar, ref epServer);
            Console.WriteLine(Encoding.ASCII.GetString(receiveData));
        }

        private void forwardButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] sendData = Encoding.ASCII.GetBytes("forward 200");
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void landButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] sendData = Encoding.ASCII.GetBytes("land");
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void leftButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] sendData = Encoding.ASCII.GetBytes("left 200");
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void rightButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] sendData = Encoding.ASCII.GetBytes("right 200");
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void backwardButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] sendData = Encoding.ASCII.GetBytes("back 200");
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void moveUpButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] sendData = Encoding.ASCII.GetBytes("up 100");
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void moveDownClockButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] sendData = Encoding.ASCII.GetBytes("down 100");
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void rollForwardButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] sendData = Encoding.ASCII.GetBytes("flip f");
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void rollLeftButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] sendData = Encoding.ASCII.GetBytes("flip l");
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void rollRightButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] sendData = Encoding.ASCII.GetBytes("flip r");
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void rollBackwardButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] sendData = Encoding.ASCII.GetBytes("flip b");
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void clockButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] sendData = Encoding.ASCII.GetBytes("cw 90");
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void antiClockButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] sendData = Encoding.ASCII.GetBytes("ccw 90");
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void speedUpButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.myUavData.velocity = MainWindow.myUavData.velocity + 10;
            String questStr = "speed " + MainWindow.myUavData.velocity;

            byte[] sendData = Encoding.ASCII.GetBytes(questStr);
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void speedDownButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.myUavData.velocity = MainWindow.myUavData.velocity - 10;
            String questStr = "speed " + MainWindow.myUavData.velocity;

            byte[] sendData = Encoding.ASCII.GetBytes(questStr);
            //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送
            local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
            //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收
            local.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }
    }
}
