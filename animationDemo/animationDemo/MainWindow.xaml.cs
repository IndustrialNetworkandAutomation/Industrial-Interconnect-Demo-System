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

using Opc.Ua;
using animationDemo.opcRealated.opcUA;


namespace animationDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region initialization
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Fields
        private Boolean ifConfig = false;
        static public DataFolder.motorPos myMotorPos = new DataFolder.motorPos();
        static public DataFolder.robotData myRobotData = new DataFolder.robotData();
        static public DataFolder.inspectData myInspectData = new DataFolder.inspectData();
        static public DataFolder.pneumaticData myPneumaticData = new DataFolder.pneumaticData();
        static public DataFolder.logisticData myLogisticData = new DataFolder.logisticData();
        static public DataFolder.IOData myIOData = new DataFolder.IOData();
        static public DataFolder.robotinoData myAgvData = new DataFolder.robotinoData();
        static public DataFolder.uavData myUavData = new DataFolder.uavData();
        //当前主显示器下的OPC UA服务器
        private opcRealated.opcUA.opcUAServer m_Server = new opcRealated.opcUA.opcUAServer();
        #endregion

        #region EventHandler
        private void beckhoffButton_Click(object sender, RoutedEventArgs e)
        {
            animationMove();
            if (ifConfig)               //由于已经修改，故与动画中的逻辑相反
            {
                //showControlMain();
                showBeckhoffDisplay();
            }
            else
            {
                destroyAll();
            }
        }

        private void rexrothButton_Click(object sender, RoutedEventArgs e)
        {
            animationMove();
            if (ifConfig)               //由于已经修改，故与动画中的逻辑相反
            {
                showRexrothDisplay();
            }
            else
            {
                destroyAll();
            }
        }

        private void PFButton_Click(object sender, RoutedEventArgs e)
        {
            animationMove();
            if (ifConfig)               //由于已经修改，故与动画中的逻辑相反
            {
                showPFDisplay();
            }
            else
            {
                destroyAll();
            }
        }

        
        private void SiemenseButton_Click(object sender, RoutedEventArgs e)
        {
            animationMove();
            if (ifConfig)               //由于已经修改，故与动画中的逻辑相反
            {
                showSiemensDisplay();
            }
            else
            {
                destroyAll();
            }
        }

        private void UniversalButton_Click(object sender, RoutedEventArgs e)
        {
            animationMove();
            if (ifConfig)               //由于已经修改，故与动画中的逻辑相反
            {
                //showControlMain();
                ShowRobotDisplay();
            }
            else
            {
                destroyAll();
            }
        }

        private void ControlButton_Click(object sender, RoutedEventArgs e)
        {
            animationMove();
            if (ifConfig)               //由于已经修改，故与动画中的逻辑相反
            {
                showControlMain();
            }
            else
            {
                destroyAll();
            }
            
        }

        private void showControlMain()
        {
            ControlFolder.controlMain newControlMain = new ControlFolder.controlMain();

            newControlMain.Name = "ControlMain";

            newControlMain.Width = 1000;
            newControlMain.Height = 660;

            newControlMain.HorizontalAlignment = HorizontalAlignment.Center;
            newControlMain.VerticalAlignment = VerticalAlignment.Center;


            Grid1.Children.Add(newControlMain);
            Grid1.RegisterName(newControlMain.Name, newControlMain);
            newControlMain.SetValue(Grid.RowSpanProperty, 2);
        }

        private void showBeckhoffDisplay()
        {
            opcRealated.opcUA.opcUAServer curServer = connectBeckhoff();
            m_Server = curServer;
            DisplayFolder.BeckhoffDisplay newBeckhoffDisplay;

            if (curServer != null)
            {
                newBeckhoffDisplay = new DisplayFolder.BeckhoffDisplay(curServer);
                newBeckhoffDisplay.Name = "BeckhoffDisplay";

                newBeckhoffDisplay.Width = 900;
                newBeckhoffDisplay.Height = 660;

                newBeckhoffDisplay.HorizontalAlignment = HorizontalAlignment.Center;
                newBeckhoffDisplay.VerticalAlignment = VerticalAlignment.Center;
                Grid1.Children.Add(newBeckhoffDisplay);
                Grid1.RegisterName(newBeckhoffDisplay.Name, newBeckhoffDisplay);
                newBeckhoffDisplay.SetValue(Grid.RowSpanProperty, 2);
            }
            else
            {
                MessageBox.Show("无法连接OPC UA服务器", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                newBeckhoffDisplay = new DisplayFolder.BeckhoffDisplay();
                newBeckhoffDisplay.Name = "BeckhoffDisplay";

                newBeckhoffDisplay.Width = 900;
                newBeckhoffDisplay.Height = 660;

                newBeckhoffDisplay.HorizontalAlignment = HorizontalAlignment.Center;
                newBeckhoffDisplay.VerticalAlignment = VerticalAlignment.Center;
                Grid1.Children.Add(newBeckhoffDisplay);
                Grid1.RegisterName(newBeckhoffDisplay.Name, newBeckhoffDisplay);
                newBeckhoffDisplay.SetValue(Grid.RowSpanProperty, 2);
            }
        }

        private opcRealated.opcUA.opcUAServer connectBeckhoff()
        {
            String beckhoffIP = "127.0.0.1";        //倍福设备的IP地址
            String beckhoffPort = "49320";          //倍福设备的端口号

            opcRealated.opcUA.opcUAServer uaServer = new opcRealated.opcUA.opcUAServer();
            uaServer.CertificateEvent += new certificateValidation(m_Server_CertificateEvent);
            String connectVar = "opc.tcp://" + beckhoffIP + ":" + beckhoffPort;

            uaServer.Connect(connectVar);

            return uaServer;
        }

        void m_Server_CertificateEvent(CertificateValidator validator, CertificateValidationEventArgs e)
        {
            if (e.Accept == false)
            {
                e.Accept = true;
            }
        }

        private void ShowRobotDisplay()
        {
            DisplayFolder.RobotDisplay newRobotDisplay = new DisplayFolder.RobotDisplay();

            newRobotDisplay.Name = "RobotDisplay";

            newRobotDisplay.Width = 900;
            newRobotDisplay.Height = 660;

            newRobotDisplay.HorizontalAlignment = HorizontalAlignment.Center;
            newRobotDisplay.VerticalAlignment = VerticalAlignment.Center;


            Grid1.Children.Add(newRobotDisplay);
            Grid1.RegisterName(newRobotDisplay.Name, newRobotDisplay);
            newRobotDisplay.SetValue(Grid.RowSpanProperty, 2);
        }

        private void showSiemensDisplay()
        {
            DisplayFolder.SiemensDisplay newSiemensDisplay = new DisplayFolder.SiemensDisplay();

            newSiemensDisplay.Name = "SiemensDisplay";

            newSiemensDisplay.Width = 900;
            newSiemensDisplay.Height = 660;

            newSiemensDisplay.HorizontalAlignment = HorizontalAlignment.Center;
            newSiemensDisplay.VerticalAlignment = VerticalAlignment.Center;


            Grid1.Children.Add(newSiemensDisplay);
            Grid1.RegisterName(newSiemensDisplay.Name, newSiemensDisplay);
            newSiemensDisplay.SetValue(Grid.RowSpanProperty, 2);
        }

        private void showPFDisplay()
        {
            DisplayFolder.PFDisplay newPFDisplay = new DisplayFolder.PFDisplay();

            newPFDisplay.Name = "PFDisplay";

            newPFDisplay.Width = 900;
            newPFDisplay.Height = 660;

            newPFDisplay.HorizontalAlignment = HorizontalAlignment.Center;
            newPFDisplay.VerticalAlignment = VerticalAlignment.Center;


            Grid1.Children.Add(newPFDisplay);
            Grid1.RegisterName(newPFDisplay.Name, newPFDisplay);
            newPFDisplay.SetValue(Grid.RowSpanProperty, 2);
        }

        private void showRexrothDisplay()
        {
            DisplayFolder.RexrothDisplay newRexrothDisplay = new DisplayFolder.RexrothDisplay();

            newRexrothDisplay.Name = "RexrothDisplay";

            newRexrothDisplay.Width = 900;
            newRexrothDisplay.Height = 660;

            newRexrothDisplay.HorizontalAlignment = HorizontalAlignment.Center;
            newRexrothDisplay.VerticalAlignment = VerticalAlignment.Center;


            Grid1.Children.Add(newRexrothDisplay);
            Grid1.RegisterName(newRexrothDisplay.Name, newRexrothDisplay);
            newRexrothDisplay.SetValue(Grid.RowSpanProperty, 2);
        }

        public void showMotorControl()
        {
            ControlFolder.motorControl newMotorControl = new ControlFolder.motorControl();

            newMotorControl.Name = "MotorControl";

            newMotorControl.Width = 800;
            newMotorControl.Height = 660;

            newMotorControl.HorizontalAlignment = HorizontalAlignment.Center;
            newMotorControl.VerticalAlignment = VerticalAlignment.Center;


            Grid1.Children.Add(newMotorControl);
            Grid1.RegisterName(newMotorControl.Name, newMotorControl);
            newMotorControl.SetValue(Grid.RowSpanProperty, 2);
        }

        public void showInspectControl()
        {
            ControlFolder.inspectControl newInspectControl = new ControlFolder.inspectControl();

            newInspectControl.Name = "InspectControl";

            newInspectControl.Width = 800;
            newInspectControl.Height = 660;

            newInspectControl.HorizontalAlignment = HorizontalAlignment.Center;
            newInspectControl.VerticalAlignment = VerticalAlignment.Center;


            Grid1.Children.Add(newInspectControl);
            Grid1.RegisterName(newInspectControl.Name, newInspectControl);
            newInspectControl.SetValue(Grid.RowSpanProperty, 2);
        }

        public void showRobotControl()
        {
            ControlFolder.robotControl newRobotControl = new ControlFolder.robotControl();

            newRobotControl.Name = "RobotControl";

            newRobotControl.Width = 800;
            newRobotControl.Height = 660;

            newRobotControl.HorizontalAlignment = HorizontalAlignment.Center;
            newRobotControl.VerticalAlignment = VerticalAlignment.Center;


            Grid1.Children.Add(newRobotControl);
            Grid1.RegisterName(newRobotControl.Name, newRobotControl);
            newRobotControl.SetValue(Grid.RowSpanProperty, 2);
        }

        public void showPneumaticControl()
        {
            ControlFolder.pneumaticControl newPneumaticControl = new ControlFolder.pneumaticControl();

            newPneumaticControl.Name = "PneumaticControl";

            newPneumaticControl.Width = 800;
            newPneumaticControl.Height = 660;

            newPneumaticControl.HorizontalAlignment = HorizontalAlignment.Center;
            newPneumaticControl.VerticalAlignment = VerticalAlignment.Center;


            Grid1.Children.Add(newPneumaticControl);
            Grid1.RegisterName(newPneumaticControl.Name, newPneumaticControl);
            newPneumaticControl.SetValue(Grid.RowSpanProperty, 2);
        }

        public void showLogisticsControl()
        {
            ControlFolder.logisticsControl newLogisticsControl = new ControlFolder.logisticsControl();

            newLogisticsControl.Name = "LogisticsControl";

            newLogisticsControl.Width = 800;
            newLogisticsControl.Height = 660;

            newLogisticsControl.HorizontalAlignment = HorizontalAlignment.Center;
            newLogisticsControl.VerticalAlignment = VerticalAlignment.Center;


            Grid1.Children.Add(newLogisticsControl);
            Grid1.RegisterName(newLogisticsControl.Name, newLogisticsControl);
            newLogisticsControl.SetValue(Grid.RowSpanProperty, 2);
        }

        public void showIOControl()
        {
            ControlFolder.IOControl newIOControl = new ControlFolder.IOControl();

            newIOControl.Name = "IOControl";

            newIOControl.Width = 800;
            newIOControl.Height = 660;

            newIOControl.HorizontalAlignment = HorizontalAlignment.Center;
            newIOControl.VerticalAlignment = VerticalAlignment.Center;


            Grid1.Children.Add(newIOControl);
            Grid1.RegisterName(newIOControl.Name, newIOControl);
            newIOControl.SetValue(Grid.RowSpanProperty, 2);
        }

        public void showRobotinoControl()
        {
            ControlFolder.agvControl newAGVControl = new ControlFolder.agvControl();

            newAGVControl.Name = "AGVControl";

            newAGVControl.Width = 900;
            newAGVControl.Height = 660;

            newAGVControl.HorizontalAlignment = HorizontalAlignment.Center;
            newAGVControl.VerticalAlignment = VerticalAlignment.Center;


            Grid1.Children.Add(newAGVControl);
            Grid1.RegisterName(newAGVControl.Name, newAGVControl);
            newAGVControl.SetValue(Grid.RowSpanProperty, 2);
        }

        public void showUAVControl()
        {
            ControlFolder.uavControl newUAVControl = new ControlFolder.uavControl();

            newUAVControl.Name = "UAVControl";

            newUAVControl.Width = 900;
            newUAVControl.Height = 660;

            newUAVControl.HorizontalAlignment = HorizontalAlignment.Center;
            newUAVControl.VerticalAlignment = VerticalAlignment.Center;


            Grid1.Children.Add(newUAVControl);
            Grid1.RegisterName(newUAVControl.Name, newUAVControl);
            newUAVControl.SetValue(Grid.RowSpanProperty, 2);
        }

        private void destroyAll()
        {
            m_Server.Disconnect();
            destroyControlMain();
            destroyBeckhoffDisplay();
            destroyPFDisplay();
            destroyRexrothDisplay();
            destroyMotorControl();
            destroySiemensDisplay();
            destroyRobotDisplay();
            destroyRobotControl();
            destroyInspectControl();
            destroyPneumaticControl();
            destroyLogisticsControl();
            destroyIOControl();
            destroyRobotinoControl();
            destroyUAVControl();
        }

        private void destroyControlMain()
        {
            ControlFolder.controlMain ControlMain2Del = Grid1.FindName("ControlMain") as ControlFolder.controlMain;
            if (ControlMain2Del != null)
            {
                ControlMain2Del.exitControl();
            }
        }

        private void destroyBeckhoffDisplay()
        {
            DisplayFolder.BeckhoffDisplay BeckhoffDisplay2Del = Grid1.FindName("BeckhoffDisplay") as DisplayFolder.BeckhoffDisplay;
            if (BeckhoffDisplay2Del != null)
            {
                BeckhoffDisplay2Del.exitControl();
            }
        }

        private void destroyPFDisplay()
        {
            DisplayFolder.PFDisplay PFDisplay2Del = Grid1.FindName("PFDisplay") as DisplayFolder.PFDisplay;
            if (PFDisplay2Del != null)
            {
                PFDisplay2Del.exitControl();
            }
        }

        private void destroySiemensDisplay()
        {
            DisplayFolder.SiemensDisplay SiemensDisplay2Del = Grid1.FindName("SiemensDisplay") as DisplayFolder.SiemensDisplay;
            if (SiemensDisplay2Del != null)
            {
                SiemensDisplay2Del.exitControl();
            }
        }

        private void destroyRobotDisplay()
        {
            DisplayFolder.RobotDisplay RobotDisplay2Del = Grid1.FindName("RobotDisplay") as DisplayFolder.RobotDisplay;
            if (RobotDisplay2Del != null)
            {
                RobotDisplay2Del.exitControl();
            }
        }

        private void destroyRexrothDisplay()
        {
            DisplayFolder.RexrothDisplay RexrothDisplay2Del = Grid1.FindName("RexrothDisplay") as DisplayFolder.RexrothDisplay;
            if (RexrothDisplay2Del != null)
            {
                RexrothDisplay2Del.exitControl();
            }
        }

        private void destroyMotorControl()
        {
            ControlFolder.motorControl MotorControl2Del = Grid1.FindName("MotorControl") as ControlFolder.motorControl;
            if (MotorControl2Del != null)
            {
                MotorControl2Del.exitControl();
            }
        }

        private void destroyInspectControl()
        {
            ControlFolder.inspectControl InspectControl2Del = Grid1.FindName("InspectControl") as ControlFolder.inspectControl;
            if (InspectControl2Del != null)
            {
                InspectControl2Del.exitControl();
            }
        }

        private void destroyRobotControl()
        {
            ControlFolder.robotControl RobotControl2Del = Grid1.FindName("RobotControl") as ControlFolder.robotControl;
            if (RobotControl2Del != null)
            {
                RobotControl2Del.exitControl();
            }
        }

        private void destroyPneumaticControl()
        {
            ControlFolder.pneumaticControl PneumaticControl2Del = Grid1.FindName("PneumaticControl") as ControlFolder.pneumaticControl;
            if (PneumaticControl2Del != null)
            {
                PneumaticControl2Del.exitControl();
            }
        }

        private void destroyLogisticsControl()
        {
            ControlFolder.logisticsControl LogisticsControl2Del = Grid1.FindName("LogisticsControl") as ControlFolder.logisticsControl;
            if (LogisticsControl2Del != null)
            {
                LogisticsControl2Del.exitControl();
            }
        }

        private void destroyIOControl()
        {
            ControlFolder.IOControl IOControl2Del = Grid1.FindName("IOControl") as ControlFolder.IOControl;
            if (IOControl2Del != null)
            {
                IOControl2Del.exitControl();
            }
        }

        private void destroyRobotinoControl()
        {
            ControlFolder.agvControl AGVControl2Del = Grid1.FindName("AGVControl") as ControlFolder.agvControl;
            if (AGVControl2Del != null)
            {
                AGVControl2Del.exitControl();
            }
        }

        private void destroyUAVControl()
        {
            ControlFolder.uavControl UAVControl2Del = Grid1.FindName("UAVControl") as ControlFolder.uavControl;
            if (UAVControl2Del != null)
            {
                UAVControl2Del.exitControl();
            }
        }

        public void removeControlMain()
        {
            ControlFolder.controlMain ControlMain2Del = Grid1.FindName("ControlMain") as ControlFolder.controlMain;
            ControlMain2Del.stopThread();
            Grid1.Children.Remove(ControlMain2Del);
            Grid1.UnregisterName(ControlMain2Del.Name);
        }

        public void removeMotorControl()
        {
            ControlFolder.motorControl MotorControl2Del = Grid1.FindName("MotorControl") as ControlFolder.motorControl;
            MotorControl2Del.stopThread();
            Grid1.Children.Remove(MotorControl2Del);
            Grid1.UnregisterName(MotorControl2Del.Name);
        }

        public void removeInspectControl()
        {
            ControlFolder.inspectControl InspectControl2Del = Grid1.FindName("InspectControl") as ControlFolder.inspectControl;
            InspectControl2Del.stopThread();
            Grid1.Children.Remove(InspectControl2Del);
            Grid1.UnregisterName(InspectControl2Del.Name);
        }

        public void removeRobotControl()
        {
            ControlFolder.robotControl RobotControl2Del = Grid1.FindName("RobotControl") as ControlFolder.robotControl;
            RobotControl2Del.stopThread();
            Grid1.Children.Remove(RobotControl2Del);
            Grid1.UnregisterName(RobotControl2Del.Name);
        }

        public void removePneumaticControl()
        {
            ControlFolder.pneumaticControl PneumaticControl2Del = Grid1.FindName("PneumaticControl") as ControlFolder.pneumaticControl;
            PneumaticControl2Del.stopThread();
            Grid1.Children.Remove(PneumaticControl2Del);
            Grid1.UnregisterName(PneumaticControl2Del.Name);
        }

        public void removeLogisticsControl()
        {
            ControlFolder.logisticsControl LogisticsControl2Del = Grid1.FindName("LogisticsControl") as ControlFolder.logisticsControl;
            LogisticsControl2Del.stopThread();
            Grid1.Children.Remove(LogisticsControl2Del);
            Grid1.UnregisterName(LogisticsControl2Del.Name);
        }

        public void removeIOControl()
        {
            ControlFolder.IOControl IOControl2Del = Grid1.FindName("IOControl") as ControlFolder.IOControl;
            IOControl2Del.stopThread();
            Grid1.Children.Remove(IOControl2Del);
            Grid1.UnregisterName(IOControl2Del.Name);
        }

        public void removeAGVControl()
        {
            ControlFolder.agvControl AGVControl2Del = Grid1.FindName("AGVControl") as ControlFolder.agvControl;
            AGVControl2Del.stopThread();
            Grid1.Children.Remove(AGVControl2Del);
            Grid1.UnregisterName(AGVControl2Del.Name);
        }

        public void removeUAVControl()
        {
            ControlFolder.uavControl UAVControl2Del = Grid1.FindName("UAVControl") as ControlFolder.uavControl;
            UAVControl2Del.stopThread();
            Grid1.Children.Remove(UAVControl2Del);
            Grid1.UnregisterName(UAVControl2Del.Name);
        }

        public void removeBeckhoffDisplay()
        {
            DisplayFolder.BeckhoffDisplay BeckhoffDisplay2Del = Grid1.FindName("BeckhoffDisplay") as DisplayFolder.BeckhoffDisplay;
            //BeckhoffDisplay2Del.stopThread();
            Grid1.Children.Remove(BeckhoffDisplay2Del);
            Grid1.UnregisterName(BeckhoffDisplay2Del.Name);
        }

        public void removeRobotDisplay()
        {
            DisplayFolder.RobotDisplay RobotDisplay2Del = Grid1.FindName("RobotDisplay") as DisplayFolder.RobotDisplay;
            //BeckhoffDisplay2Del.stopThread();
            Grid1.Children.Remove(RobotDisplay2Del);
            Grid1.UnregisterName(RobotDisplay2Del.Name);
        }

        public void removeSiemensDisplay()
        {
            DisplayFolder.SiemensDisplay SiemensDisplay2Del = Grid1.FindName("SiemensDisplay") as DisplayFolder.SiemensDisplay;
            //BeckhoffDisplay2Del.stopThread();
            Grid1.Children.Remove(SiemensDisplay2Del);
            Grid1.UnregisterName(SiemensDisplay2Del.Name);
        }

        public void removePFDisplay()
        {
            DisplayFolder.PFDisplay PFDisplay2Del = Grid1.FindName("PFDisplay") as DisplayFolder.PFDisplay;
            //BeckhoffDisplay2Del.stopThread();
            Grid1.Children.Remove(PFDisplay2Del);
            Grid1.UnregisterName(PFDisplay2Del.Name);
        }

        public void removeRexrothDisplay()
        {
            DisplayFolder.RexrothDisplay RexrothDisplay2Del = Grid1.FindName("RexrothDisplay") as DisplayFolder.RexrothDisplay;
            //BeckhoffDisplay2Del.stopThread();
            Grid1.Children.Remove(RexrothDisplay2Del);
            Grid1.UnregisterName(RexrothDisplay2Del.Name);
        }


        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
        #endregion

        #region userMethod
        private void animationMove()
        {
            Point zoomCenter = new Point(SystemParameters.WorkArea.Width / 2, SystemParameters.WorkArea.Height / 2);

            if (!ifConfig)
            {
                Storyboard story = Resources["initAfterClick"] as Storyboard;
                DoubleAnimation moveRexrothAnimationX = (DoubleAnimation)story.Children[0];
                moveRexrothAnimationX.From = 0;
                moveRexrothAnimationX.To = 2000;

                DoubleAnimation moveRexrothAnimationY = (DoubleAnimation)story.Children[5];
                moveRexrothAnimationY.From = 0;
                moveRexrothAnimationY.To = -1200;

                DoubleAnimation zoomRexrothAnimationX = (DoubleAnimation)story.Children[10];
                zoomRexrothAnimationX.From = 1;
                zoomRexrothAnimationX.To = 0.3;

                DoubleAnimation zoomRexrothAnimationY = (DoubleAnimation)story.Children[11];
                zoomRexrothAnimationY.From = 1;
                zoomRexrothAnimationY.To = 0.3;

                zoomRexroth.CenterX = 286;
                zoomRexroth.CenterY = 300;

                DoubleAnimation moveBeckhoffAnimationX = (DoubleAnimation)story.Children[1];
                moveBeckhoffAnimationX.From = 0;
                moveBeckhoffAnimationX.To = 2000;

                DoubleAnimation moveBeckhoffAnimationY = (DoubleAnimation)story.Children[6];
                moveBeckhoffAnimationY.From = 0;
                moveBeckhoffAnimationY.To = -1200;

                DoubleAnimation zoomBeckhoffAnimationX = (DoubleAnimation)story.Children[12];
                zoomBeckhoffAnimationX.From = 1;
                zoomBeckhoffAnimationX.To = 0.3;

                DoubleAnimation zoomBeckhoffAnimationY = (DoubleAnimation)story.Children[13];
                zoomBeckhoffAnimationY.From = 1;
                zoomBeckhoffAnimationY.To = 0.3;

                zoomBeckhoff.CenterX = 0;
                zoomBeckhoff.CenterY = 300;

                DoubleAnimation movePFAnimationX = (DoubleAnimation)story.Children[2];
                movePFAnimationX.From = 0;
                movePFAnimationX.To = 2000;

                DoubleAnimation movePFAnimationY = (DoubleAnimation)story.Children[7];
                movePFAnimationY.From = 0;
                movePFAnimationY.To = -1200;

                DoubleAnimation zoomPFAnimationnX = (DoubleAnimation)story.Children[14];
                zoomPFAnimationnX.From = 1;
                zoomPFAnimationnX.To = 0.3;

                DoubleAnimation zoomPFAnimationnY = (DoubleAnimation)story.Children[15];
                zoomPFAnimationnY.From = 1;
                zoomPFAnimationnY.To = 0.3;

                zoomPFButton.CenterX = 177;
                zoomPFButton.CenterY = 0;

                DoubleAnimation moveSiemenseAnimationX = (DoubleAnimation)story.Children[3];
                moveSiemenseAnimationX.From = 0;
                moveSiemenseAnimationX.To = 2000;

                DoubleAnimation moveSiemenseAnimationY = (DoubleAnimation)story.Children[8];
                moveSiemenseAnimationY.From = 0;
                moveSiemenseAnimationY.To = -1200;

                DoubleAnimation zoomSiemenseAnimationX = (DoubleAnimation)story.Children[16];
                zoomSiemenseAnimationX.From = 1;
                zoomSiemenseAnimationX.To = 0.3;

                DoubleAnimation zoomSiemenseAnimationY = (DoubleAnimation)story.Children[17];
                zoomSiemenseAnimationY.From = 1;
                zoomSiemenseAnimationY.To = 0.3;

                zoomSiemenseButton.CenterX = 283;
                zoomSiemenseButton.CenterY = 92.706;

                DoubleAnimation moveUniversalAnimationX = (DoubleAnimation)story.Children[4];
                moveUniversalAnimationX.From = 0;
                moveUniversalAnimationX.To = 2000;

                DoubleAnimation moveUniversalAnimationY = (DoubleAnimation)story.Children[9];
                moveUniversalAnimationY.From = 0;
                moveUniversalAnimationY.To = -1200;

                DoubleAnimation zoomUniversalAnimationX = (DoubleAnimation)story.Children[18];
                zoomUniversalAnimationX.From = 1;
                zoomUniversalAnimationX.To = 0.3;

                DoubleAnimation zoomUniversalAnimationY = (DoubleAnimation)story.Children[19];
                zoomUniversalAnimationY.From = 1;
                zoomUniversalAnimationY.To = 0.3;

                zoomUniversalButton.CenterX = 0;
                zoomUniversalButton.CenterY = 92.706;

                DoubleAnimation moveControlAnimationX = (DoubleAnimation)story.Children[20];
                moveControlAnimationX.From = 0;
                moveControlAnimationX.To = 2000;

                DoubleAnimation moveControlAnimationY = (DoubleAnimation)story.Children[21];
                moveControlAnimationY.From = 0;
                moveControlAnimationY.To = -1200;

                DoubleAnimation zoomControlAnimationX = (DoubleAnimation)story.Children[22];
                zoomControlAnimationX.From = 1;
                zoomControlAnimationX.To = 0.3;

                DoubleAnimation zoomControlAnimationY = (DoubleAnimation)story.Children[23];
                zoomControlAnimationY.From = 1;
                zoomControlAnimationY.To = 0.3;

                zoomControlButton.CenterX = 50;
                zoomControlButton.CenterY = 50;

                story.AutoReverse = false;
                story.Begin();
                ifConfig = true;
            }
            else
            {
                Storyboard story = Resources["initAfterClick"] as Storyboard;
                DoubleAnimation moveRexrothAnimationX = (DoubleAnimation)story.Children[0];
                moveRexrothAnimationX.From = 2000;
                moveRexrothAnimationX.To = 0;

                DoubleAnimation moveRexrothAnimationY = (DoubleAnimation)story.Children[5];
                moveRexrothAnimationY.From = -1200;
                moveRexrothAnimationY.To = 0;

                DoubleAnimation zoomRexrothAnimationX = (DoubleAnimation)story.Children[10];
                zoomRexrothAnimationX.From = 0.3;
                zoomRexrothAnimationX.To = 1;

                DoubleAnimation zoomRexrothAnimationY = (DoubleAnimation)story.Children[11];
                zoomRexrothAnimationY.From = 0.3;
                zoomRexrothAnimationY.To = 1;

                zoomRexroth.CenterX = 286;
                zoomRexroth.CenterY = 300;

                DoubleAnimation moveBeckhoffAnimationX = (DoubleAnimation)story.Children[1];
                moveBeckhoffAnimationX.From = 2000;
                moveBeckhoffAnimationX.To = 0;

                DoubleAnimation moveBeckhoffAnimationY = (DoubleAnimation)story.Children[6];
                moveBeckhoffAnimationY.From = -1200;
                moveBeckhoffAnimationY.To = 0;

                DoubleAnimation zoomBeckhoffAnimationX = (DoubleAnimation)story.Children[12];
                zoomBeckhoffAnimationX.From = 0.3;
                zoomBeckhoffAnimationX.To = 1;

                DoubleAnimation zoomBeckhoffAnimationY = (DoubleAnimation)story.Children[13];
                zoomBeckhoffAnimationY.From = 0.3;
                zoomBeckhoffAnimationY.To = 1;

                zoomBeckhoff.CenterX = 0;
                zoomBeckhoff.CenterY = 300;

                DoubleAnimation movePFAnimationX = (DoubleAnimation)story.Children[2];
                movePFAnimationX.From = 2000;
                movePFAnimationX.To = 0;

                DoubleAnimation movePFAnimationY = (DoubleAnimation)story.Children[7];
                movePFAnimationY.From = -1200;
                movePFAnimationY.To = 0;

                DoubleAnimation zoomPFAnimationnX = (DoubleAnimation)story.Children[14];
                zoomPFAnimationnX.From = 0.3;
                zoomPFAnimationnX.To = 1;

                DoubleAnimation zoomPFAnimationnY = (DoubleAnimation)story.Children[15];
                zoomPFAnimationnY.From = 0.3;
                zoomPFAnimationnY.To = 1;

                zoomPFButton.CenterX = 177;
                zoomPFButton.CenterY = 0;

                DoubleAnimation moveSiemenseAnimationX = (DoubleAnimation)story.Children[3];
                moveSiemenseAnimationX.From = 2000;
                moveSiemenseAnimationX.To = 0;

                DoubleAnimation moveSiemenseAnimationY = (DoubleAnimation)story.Children[8];
                moveSiemenseAnimationY.From = -1200;
                moveSiemenseAnimationY.To = 0;

                DoubleAnimation zoomSiemenseAnimationX = (DoubleAnimation)story.Children[16];
                zoomSiemenseAnimationX.From = 0.3;
                zoomSiemenseAnimationX.To = 1;

                DoubleAnimation zoomSiemenseAnimationY = (DoubleAnimation)story.Children[17];
                zoomSiemenseAnimationY.From = 0.3;
                zoomSiemenseAnimationY.To = 1;

                zoomSiemenseButton.CenterX = 283;
                zoomSiemenseButton.CenterY = 92.706;

                DoubleAnimation moveUniversalAnimationX = (DoubleAnimation)story.Children[4];
                moveUniversalAnimationX.From = 2000;
                moveUniversalAnimationX.To = 0;

                DoubleAnimation moveUniversalAnimationY = (DoubleAnimation)story.Children[9];
                moveUniversalAnimationY.From = -1200;
                moveUniversalAnimationY.To = 0;

                DoubleAnimation zoomUniversalAnimationX = (DoubleAnimation)story.Children[18];
                zoomUniversalAnimationX.From = 0.3;
                zoomUniversalAnimationX.To = 1;

                DoubleAnimation zoomUniversalAnimationY = (DoubleAnimation)story.Children[19];
                zoomUniversalAnimationY.From = 0.3;
                zoomUniversalAnimationY.To = 1;

                zoomUniversalButton.CenterX = 0;
                zoomUniversalButton.CenterY = 92.706;

                DoubleAnimation moveControlAnimationX = (DoubleAnimation)story.Children[20];
                moveControlAnimationX.From = 2000;
                moveControlAnimationX.To = 0;

                DoubleAnimation moveControlAnimationY = (DoubleAnimation)story.Children[21];
                moveControlAnimationY.From = -1200;
                moveControlAnimationY.To = 0;

                DoubleAnimation zoomControlAnimationX = (DoubleAnimation)story.Children[22];
                zoomControlAnimationX.From = 0.3;
                zoomControlAnimationX.To = 1;

                DoubleAnimation zoomControlAnimationY = (DoubleAnimation)story.Children[23];
                zoomControlAnimationY.From = 0.3;
                zoomControlAnimationY.To = 1;

                zoomControlButton.CenterX = 50;
                zoomControlButton.CenterY = 50;

                story.AutoReverse = false;
                story.Begin();
                ifConfig = false;
            }
        }
        #endregion

        
    }
}
