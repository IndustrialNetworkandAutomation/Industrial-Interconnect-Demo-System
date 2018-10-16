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

namespace animationDemo.DisplayFolder
{
    /// <summary>
    /// SiemensDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class SiemensDisplay : UserControl
    {
        public SiemensDisplay()
        {
            InitializeComponent();
            initData();
            bindingData();
        }

        #region Fields
        List<dataGridData> SiemensInputData = new List<dataGridData>();
        List<dataGridData> SiemensOutputData = new List<dataGridData>();
        List<dataGridData> SiemensOtherData = new List<dataGridData>();
        #endregion

        #region initialize
        private void initData()
        {
            for (int i = 0; i < 16; i++)
            {
                dataGridData myData = new dataGridData();
                myData.varName = "I" + i;
                myData.varValue = 0;
                SiemensInputData.Add(myData);
            }
            for (int i = 0; i < 16; i++)
            {
                dataGridData myData = new dataGridData();
                myData.varName = "O" + i;
                myData.varValue = 0;
                SiemensOutputData.Add(myData);
            }

            //dataGridData otherData = new dataGridData();
            //otherData.varName = "AI1";
            //otherData.varValue = 0;
            //SiemensOtherData.Add(otherData);

            //otherData = new dataGridData();
            //otherData.varName = "AO1";
            //otherData.varValue = 0;
            //SiemensOtherData.Add(otherData);

            //otherData = new dataGridData();
            //otherData.varName = "电流";
            //otherData.varValue = 0;
            //SiemensOtherData.Add(otherData);

            //otherData = new dataGridData();
            //otherData.varName = "气压";
            //otherData.varValue = 0;
            //SiemensOtherData.Add(otherData);
        }

        private void bindingData()
        {
            inputData.ItemsSource = SiemensInputData;
            outputData.ItemsSource = SiemensOutputData;
            othersData.ItemsSource = SiemensOtherData;
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
            parentWindow.removeSiemensDisplay();
        }
        #endregion
    }
}