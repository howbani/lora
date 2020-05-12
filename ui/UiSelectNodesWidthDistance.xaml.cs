using LORA.ExpermentsResults;
using LORA.Parameters;
using LORA.ui.conts;
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
using System.Windows.Shapes;

namespace LORA.ui
{
    /// <summary>
    /// Interaction logic for UiSelectNodesWidthDistance.xaml
    /// </summary>
    public partial class UiSelectNodesWidthDistance : Window
    {
        MainWindow mianWind;
        public UiSelectNodesWidthDistance(MainWindow _mianWind) 
        {
            InitializeComponent();
            mianWind = _mianWind;

            for(int i=50;i<=250;i++)
            {
                com_distance.Items.Add(i);
            }

            for (int i = 1; i < mianWind.myNetWork.Count; i++)
            {
                com_nos.Items.Add(new ComboBoxItem() { Content = i.ToString() });
                com_nop.Items.Add(new ComboBoxItem() { Content = i.ToString() });
            }
        }

        private void btn_compute_Click(object sender, RoutedEventArgs e)
        {
            PublicParamerters.SinkNode.PacketsList.Clear();
            List<EnergyConsmption2> lis = new List<EnergyConsmption2>();
            int NOS = Convert.ToInt16(com_nos.Text);
            int NOP = Convert.ToInt16(com_nop.Text);
            double dist = Convert.ToDouble(com_distance.Text);
            DoEnergyConsumptionExpermentRandomWithDistance x = new ExpermentsResults.DoEnergyConsumptionExpermentRandomWithDistance(mianWind.myNetWork);
            lis.Add(x.DoExperment(dist, NOS, NOP));

            UiShowLists win = new UiShowLists();
            win.Title = "Randomly selected " + NOS + " Sources each one sends " + NOP + " Packets";
            ListControl ContlList = new ListControl();
            ContlList.lbl_title.Content = win.Title;
            ContlList.dg_date.ItemsSource = lis;
            win.stack_items.Children.Add(ContlList);
            win.Show();
        }
    }
}
