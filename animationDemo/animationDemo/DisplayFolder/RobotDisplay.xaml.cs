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
using System.Windows.Media.Animation;

using animationDemo.DataFolder;

using URSDK.RobController.Communication;
using System.Threading;


namespace animationDemo.DisplayFolder
{
    /// <summary>
    /// RobotDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class RobotDisplay : UserControl
    {
        public RobotDisplay()
        {
            InitializeComponent();
            initData();
            bindingData();
        }

        #region Fields
        List<dataGridData> RobotInputData = new List<dataGridData>();
        List<dataGridData> RobotOutputData = new List<dataGridData>();
        List<dataGridData> RobotOtherData = new List<dataGridData>();

        private Boolean ThreadEnable = true;

        //Definition
        private DashBoard aDashBoard;        //Dashboard instance, port: 29999
        private RTClient aRTClient;          //RTClient instance, port: 30003
        private RTDE aRTDE;                  //RTDE instance, port: 30004
        #endregion

        #region initialize
        private void initData()
        {
            aDashBoard = new DashBoard("192.168.100.11");
            aRTClient = new RTClient("192.168.100.11");
            aRTDE = new RTDE(System.Environment.CurrentDirectory + "\\RTDEConfig.xml", "192.168.100.11");
            //RTDE用于采集数据
            aRTDE.startSync();
            if (!aRTDE.isSynchronizing)
                Thread.Sleep(10);
            

            for (int i = 0; i < 16; i++)
            {
                dataGridData myData = new dataGridData();
                myData.varName = "I" + i;
                myData.varValue = 0;
                RobotInputData.Add(myData);
            }
            for (int i = 0; i < 16; i++)
            {
                dataGridData myData = new dataGridData();
                myData.varName = "O" + i;
                myData.varValue = 0;
                RobotOutputData.Add(myData);
            }

            dataGridData otherData = new dataGridData();
            otherData.varName = "A1";
            otherData.varValue = 0;
            RobotOtherData.Add(otherData);

            otherData = new dataGridData();
            otherData.varName = "A2";
            otherData.varValue = 0;
            RobotOtherData.Add(otherData);

            otherData = new dataGridData();
            otherData.varName = "A3";
            otherData.varValue = 0;
            RobotOtherData.Add(otherData);

            otherData = new dataGridData();
            otherData.varName = "A4";
            otherData.varValue = 0;
            RobotOtherData.Add(otherData);

            otherData = new dataGridData();
            otherData.varName = "A5";
            otherData.varValue = 0;
            RobotOtherData.Add(otherData);

            otherData = new dataGridData();
            otherData.varName = "A6";
            otherData.varValue = 0;
            RobotOtherData.Add(otherData);

            Thread thread = new Thread(new ThreadStart(refreshRobotData));
            thread.Start();
        }

        private void refreshRobotData()
        {
            while (ThreadEnable)
            {
                refreshRobotDataInstance();
                Thread.Sleep(100);
            }
        }

        private void refreshRobotDataInstance()
        {
            if (aRTDE.isSynchronizing)
            {
                RTDEOutputObj OutObj = aRTDE.getOutputObj();

                dataGridData otherData = RobotOtherData.Find(x => x.varName == "A1");
                otherData.varValue = Math.Round(OutObj.actual_q.X / Math.PI * 180, 2);

                otherData = RobotOtherData.Find(x => x.varName == "A2");
                otherData.varValue = Math.Round(OutObj.actual_q.Y / Math.PI * 180, 2);

                otherData = RobotOtherData.Find(x => x.varName == "A3");
                otherData.varValue = Math.Round(OutObj.actual_q.Z / Math.PI * 180, 2);

                otherData = RobotOtherData.Find(x => x.varName == "A4");
                otherData.varValue = Math.Round(OutObj.actual_q.Rx / Math.PI * 180, 2);

                otherData = RobotOtherData.Find(x => x.varName == "A5");
                otherData.varValue = Math.Round(OutObj.actual_q.Ry / Math.PI * 180, 2);

                otherData = RobotOtherData.Find(x => x.varName == "A6");
                otherData.varValue = Math.Round(OutObj.actual_q.Rz / Math.PI * 180, 2);
            }
        }

        private void bindingData()
        {
            inputData.ItemsSource = RobotInputData;
            outputData.ItemsSource = RobotOutputData;
            othersData.ItemsSource = RobotOtherData;
        }
        #endregion

        #region UserDefine
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
            parentWindow.removeRobotDisplay();
        }
        #endregion
    }
}
