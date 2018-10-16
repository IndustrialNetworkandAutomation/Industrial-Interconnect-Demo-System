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
using animationDemo.DataFolder;

using System.Windows.Media.Animation;

using Opc.Ua;
using animationDemo.opcRealated.opcUA;

namespace animationDemo.DisplayFolder
{
    /// <summary>
    /// BeckhoffDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class BeckhoffDisplay : UserControl
    {
        public BeckhoffDisplay()
        {
            InitializeComponent();
            initData();
            bindingData();
        }

        public BeckhoffDisplay(opcUAServer curServer)
        {
            m_Server = curServer;
            InitializeComponent();
            initData(curServer);
            bindingData();
        }

        #region Fields
        List<dataGridData> beckhoffInputData = new List<dataGridData>();
        List<dataGridData> beckhoffOutputData = new List<dataGridData>();
        List<dataGridData> beckhoffOtherData = new List<dataGridData>();

        opcUAServer m_Server = new opcUAServer();
        #endregion

        #region initialize
        private void initData()
        {
            for (int i = 0; i < 16; i++)
            {
                dataGridData myData = new dataGridData();
                myData.varName = "I" + i;
                myData.varValue = 0;
                beckhoffInputData.Add(myData);
            }
            for (int i = 0; i < 16; i++)
            {
                dataGridData myData = new dataGridData();
                myData.varName = "O" + i;
                myData.varValue = 0;
                beckhoffOutputData.Add(myData);
            }
            
            dataGridData otherData = new dataGridData();
            otherData.varName = "AI1";
            otherData.varValue = 0;
            beckhoffOtherData.Add(otherData);

            otherData = new dataGridData();
            otherData.varName = "AO1";
            otherData.varValue = 0;
            beckhoffOtherData.Add(otherData);

            otherData = new dataGridData();
            otherData.varName = "电流";
            otherData.varValue = 0;
            beckhoffOtherData.Add(otherData);

            otherData = new dataGridData();
            otherData.varName = "气压";
            otherData.varValue = 0;
            beckhoffOtherData.Add(otherData);
        }

        private void initData(opcUAServer curServer)
        {
            Subscription in_Subscription = curServer.AddSubscription(100);
            object serverHandle = null;                     //初始化服务器句柄
            String nodeIDStr = "ns=2;s=curSimulation.IOGroup.Input0";
            NodeId nodeId = new NodeId(nodeIDStr);

            String iid = "data1";
            object item = iid;

            try
            {
                in_Subscription.AddDataMonitoredItem(nodeId, item, ClientApi_ValueChanged, 100, out serverHandle);

                dataGridData myData = new dataGridData();
                myData.IID = iid;
                myData.varName = "I0";
                myData.varValue = 0;
                beckhoffInputData.Add(myData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("添加项时出现异常，异常信息：" + ex);
            }

            serverHandle = null;                     //初始化服务器句柄
            nodeIDStr = "ns=2;s=curSimulation.IOGroup.Input1";
            nodeId = new NodeId(nodeIDStr);

            iid = "data2";
            item = iid;

            try
            {
                in_Subscription.AddDataMonitoredItem(nodeId, item, ClientApi_ValueChanged, 100, out serverHandle);

                dataGridData myData = new dataGridData();
                myData.IID = iid;
                myData.varName = "I1";
                myData.varValue = 0;
                beckhoffInputData.Add(myData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("添加项时出现异常，异常信息：" + ex);
            }

            for (int i = 2; i < 16; i++)
            {
                dataGridData myData = new dataGridData();
                myData.varName = "I" + i;
                myData.varValue = 0;
                beckhoffInputData.Add(myData);
            }
            for (int i = 0; i < 16; i++)
            {
                dataGridData myData = new dataGridData();
                myData.varName = "O" + i;
                myData.varValue = 0;
                beckhoffOutputData.Add(myData);
            }

            dataGridData otherData = new dataGridData();
            otherData.varName = "AI1";
            otherData.varValue = 0;
            beckhoffOtherData.Add(otherData);

            otherData = new dataGridData();
            otherData.varName = "AO1";
            otherData.varValue = 0;
            beckhoffOtherData.Add(otherData);

            otherData = new dataGridData();
            otherData.varName = "电流";
            otherData.varValue = 0;
            beckhoffOtherData.Add(otherData);

            otherData = new dataGridData();
            otherData.varName = "气压";
            otherData.varValue = 0;
            beckhoffOtherData.Add(otherData);
        }

        /// <summary>
        /// OPC UA值的响应函数
        /// </summary>
        /// <param name="clientHandle"></param>
        /// <param name="value"></param>
        private void ClientApi_ValueChanged(object clientHandle, DataValue value)
        {
            opcRealated.opcUA.opcUAServer curServer = m_Server;
            // 如果需要唤醒主线程的更新控件功能. 
            if (!this.CheckAccess())
            {
                // Asynchronous execution of the valueChanged delegate.
                this.Dispatcher.BeginInvoke(new valueChanged(ClientApi_ValueChanged), clientHandle, value);
                return;
            }

            String keyID = (String)clientHandle;

            dataGridData curData = beckhoffInputData.Find(c => c.IID.Equals(keyID));
            curData.varValue = double.Parse(Utils.Format("{0}", value.Value));
        }

        private void bindingData()
        {
            inputData.ItemsSource = beckhoffInputData;
            outputData.ItemsSource = beckhoffOutputData;
            othersData.ItemsSource = beckhoffOtherData;
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
            MainWindow parentWindow = ((Grid)this.Parent).Parent as MainWindow;
            parentWindow.removeBeckhoffDisplay();
        }
        #endregion
    }
}
