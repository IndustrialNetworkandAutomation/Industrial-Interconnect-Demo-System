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
    /// PFDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class PFDisplay : UserControl
    {
        public PFDisplay()
        {
            InitializeComponent();
            initData();
            bindingData();
        }

        #region Fields
        List<dataGridData> PFInputData = new List<dataGridData>();
        List<dataGridData> PFOutputData = new List<dataGridData>();
        List<dataGridData> PFOtherData = new List<dataGridData>();
        #endregion

        #region initialize
        private void initData()
        {
            for (int i = 0; i < 6; i++)
            {
                dataGridData myData = new dataGridData();
                String explainName = "";
                switch (i)
                {
                    case 0:
                        explainName = "绿按钮";
                        break;
                    case 1:
                        explainName = "红按钮";
                        break;
                    case 2:
                        explainName = "急停按钮";
                        break;
                    case 3:
                        explainName = "气缸左位";
                        break;
                    case 4:
                        explainName = "气缸右位";
                        break;
                    case 5:
                        explainName = "";
                        break;
                    default:
                        explainName = "";
                        break;
                }

                myData.varName = "I" + i + "/" + explainName;
                myData.varValue = 0;
                PFInputData.Add(myData);
            }
            for (int i = 0; i < 6; i++)
            {
                dataGridData myData = new dataGridData();
                String explainName = "";
                switch (i)
                {
                    case 0:
                        explainName = "红灯";
                        break;
                    case 1:
                        explainName = "绿灯";
                        break;
                    case 2:
                        explainName = "黄灯";
                        break;
                    case 3:
                        explainName = "急停灯";
                        break;
                    case 4:
                        explainName = "气缸左移";
                        break;
                    case 5:
                        explainName = "气缸右移";
                        break;
                    default:
                        explainName = "";
                        break;
                }
                myData.varName = "O" + i + "/" + explainName;
                myData.varValue = 0;
                PFOutputData.Add(myData);
            }

        }

        private void bindingData()
        {
            inputData.ItemsSource = PFInputData;
            outputData.ItemsSource = PFOutputData;
            othersData.ItemsSource = PFOtherData;
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
            parentWindow.removePFDisplay();
        }
        #endregion
    }
}
