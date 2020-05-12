using LORA.Parameters;
using LORA.Properties;
using LORA.Properties;
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
    /// Interaction logic for UIPowers.xaml
    /// 
    /// </summary>
    public partial class UIPowers : Window
    {
        MainWindow __MainWindow;
        public UIPowers(MainWindow _MainWindow) 
        {
            InitializeComponent();
            __MainWindow = _MainWindow;
            try
            {
                for (int i = 0; i <= PublicParamerters.ControlsRange; i++)
                {
                    if(i== PublicParamerters.ControlsRange)
                    {
                        double dc = Convert.ToDouble(i);
                        com_direction.Items.Add(dc);
                        com_energy.Items.Add(dc);
                        com_prependicular.Items.Add(dc);
                        com_transmision_distance.Items.Add(dc);
                    }
                    else
                    {
                        for (int j = 0; j <= 9; j++)
                        {
                            string str = i + "." + j;
                            double dc = Convert.ToDouble(str);
                            com_direction.Items.Add(dc);
                            com_energy.Items.Add(dc);
                            com_prependicular.Items.Add(dc);
                            com_transmision_distance.Items.Add(dc);

                        }
                    }
                }

               

                com_direction.Text = Settings.Default.DirectionDistCnt.ToString();
                com_energy.Text = Settings.Default.EnergyDistCnt.ToString();
                com_prependicular.Text = Settings.Default.PrepDistanceDistCnt.ToString();
                com_transmision_distance.Text = Settings.Default.TransDistanceDistCnt.ToString();
                com_keep_logs.Text = Settings.Default.KeepLogs.ToString();
                com_show_zone.Text = Settings.Default.ShowRoutigZone.ToString();

            }
            catch
            {
                MessageBox.Show("Error!!!.");
            }
        }


        
        private void btn_set_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Settings.Default.PrepDistanceDistCnt = Convert.ToDouble(com_prependicular.Text);
                Settings.Default.TransDistanceDistCnt = Convert.ToDouble(com_transmision_distance.Text);
                Settings.Default.EnergyDistCnt = Convert.ToDouble(com_energy.Text);
                Settings.Default.DirectionDistCnt = Convert.ToDouble(com_direction.Text);
                
                Settings.Default.KeepLogs = Convert.ToBoolean(com_keep_logs.Text);
                Settings.Default.ShowRoutigZone = Convert.ToBoolean(com_show_zone.Text);

                string PowersString = "γΦ=" + Settings.Default.EnergyDistCnt + ",γd=" + Settings.Default.TransDistanceDistCnt + ", γθ=" + Settings.Default.DirectionDistCnt + ", γψ=" + Settings.Default.PrepDistanceDistCnt;
                __MainWindow.lbl_hops_dis_network_info.Content = PublicParamerters.NetworkName + "," + PowersString;

                this.Close();
            }
            catch
            {
                MessageBox.Show("Error.");
            }


        }

       
    }
}
