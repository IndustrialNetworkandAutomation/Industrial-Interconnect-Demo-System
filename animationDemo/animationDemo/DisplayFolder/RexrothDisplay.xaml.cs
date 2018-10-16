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
    /// RexrothDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class RexrothDisplay : UserControl
    {
        public RexrothDisplay()
        {
            InitializeComponent();
            initData();
            bindingData();
        }

        #region Fields
        List<dataGridData> RexrothInputData = new List<dataGridData>();
        List<dataGridData> RexrothOutputData = new List<dataGridData>();
        List<dataGridData> RexrothOtherData = new List<dataGridData>();
        #endregion


        #region initialize
        private void initData()
        {
            for (int i = 0; i < 16; i++)
            {
                dataGridData myData = new dataGridData();
                myData.varName = "I" + i;
                myData.varValue = 0;
                RexrothInputData.Add(myData);
            }
            for (int i = 0; i < 16; i++)
            {
                dataGridData myData = new dataGridData();
                myData.varName = "O" + i;
                myData.varValue = 0;
                RexrothOutputData.Add(myData);
            }

            dataGridData otherData = new dataGridData();
            otherData.varName = "AI1";
            otherData.varValue = 0;
            RexrothOtherData.Add(otherData);

            otherData = new dataGridData();
            otherData.varName = "AI2";
            otherData.varValue = 0;
            RexrothOtherData.Add(otherData);

            otherData = new dataGridData();
            otherData.varName = "AO1";
            otherData.varValue = 0;
            RexrothOtherData.Add(otherData);

            otherData = new dataGridData();
            otherData.varName = "AO2";
            otherData.varValue = 0;
            RexrothOtherData.Add(otherData);

            otherData = new dataGridData();
            otherData.varName = "X电机";
            otherData.varValue = 0;
            RexrothOtherData.Add(otherData);

            otherData = new dataGridData();
            otherData.varName = "Y电机";
            otherData.varValue = 0;
            RexrothOtherData.Add(otherData);
        }

        private void bindingData()
        {
            inputData.ItemsSource = RexrothInputData;
            outputData.ItemsSource = RexrothOutputData;
            othersData.ItemsSource = RexrothOtherData;
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
            parentWindow.removeRexrothDisplay();
        }
        #endregion
    }
}
