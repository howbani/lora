using LORA.Parameters;
using LORA.Properties;
using LORA.ui;
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

namespace LORA.ExpermentsResults.Energy_consumptions
{
    /// <summary>
    /// Interaction logic for UISetParEnerConsum.xaml
    /// </summary>
    public partial class UISetParEnerConsum : Window
    {
        MainWindow _MainWindow;
        public UISetParEnerConsum(MainWindow __MainWindow_)
        {
            InitializeComponent();
            _MainWindow = __MainWindow_;

            for (int i = 0; i <= 1000; i = i + 10)
            {
                comb_simuTime.Items.Add(i);
               
            }
            comb_simuTime.Text = "300";

            comb_packet_rate.Items.Add("0.001");
            comb_packet_rate.Items.Add("0.01");
            comb_packet_rate.Items.Add("0.1");
            comb_packet_rate.Items.Add("0.5");
            for (int i = 1; i <= 5; i++)
            {
                comb_packet_rate.Items.Add(i);
            }

            comb_packet_rate.Text = "0.1";

            for(int i=5;i<=15;i++)
            {
                comb_startup.Items.Add(i);
            }
            comb_startup.Text = "10";

            for(int i=1;i<=5;i++)
            {
                comb_active.Items.Add(i);
                comb_sleep.Items.Add(i);
            }
            comb_active.Text = "1";
            comb_sleep.Text = "2";

           

            int conrange = 5;
            for (int i = 0; i <= conrange; i++)
            {
                if (i == conrange)
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

            // set defuals:
            com_direction.Text = Settings.Default.DirectionDistCnt.ToString();
            com_energy.Text = Settings.Default.EnergyDistCnt.ToString();
            com_prependicular.Text = Settings.Default.PrepDistanceDistCnt.ToString();
            com_transmision_distance.Text = Settings.Default.TransDistanceDistCnt.ToString();


            for(int i=1;i<=100;i++)
            {
                comb_update.Items.Add(i);
            }
            comb_update.Text = "5";

        }


        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {

            PublicParamerters.BatteryLosePerUpdate = Convert.ToInt16(comb_update.Text);
            Settings.Default.DrawPacketsLines = Convert.ToBoolean(chk_drawrouts.IsChecked);
            Settings.Default.KeepLogs= Convert.ToBoolean(chk_save_logs.IsChecked);
            Settings.Default.StopeWhenFirstNodeDeid = Convert.ToBoolean(chk_stope_when_first_node_deis.IsChecked);

            Settings.Default.PrepDistanceDistCnt = Convert.ToDouble(com_prependicular.Text);
            Settings.Default.TransDistanceDistCnt = Convert.ToDouble(com_transmision_distance.Text);
            Settings.Default.EnergyDistCnt = Convert.ToDouble(com_energy.Text);
            Settings.Default.DirectionDistCnt = Convert.ToDouble(com_direction.Text);

            if (Settings.Default.StopeWhenFirstNodeDeid==false)
            {
                double stime = Convert.ToDouble(comb_simuTime.Text);

                double packetRate = Convert.ToDouble(comb_packet_rate.Text);
                _MainWindow.stopSimlationWhen = Convert.ToInt32(stime);
                _MainWindow.RandomDeplayment(0);
                double numpackets = stime / packetRate;
                _MainWindow.GenerateUplinkPacketsRandomly(numpackets, packetRate);
                _MainWindow.PacketRate = "1 packet per " + packetRate + " s";

                
            }
            else if( Settings.Default.StopeWhenFirstNodeDeid == true)
            {
                // round. in each round
                if (check_rounds.IsChecked == true)
                {
                    double stime = 100000000;
                    _MainWindow.stopSimlationWhen = Convert.ToInt32(stime);
                    _MainWindow.RandomDeplayment(0);
                    _MainWindow.GeneratePacketsForRounds();
                    _MainWindow.PacketRate = " one round /15 s";
                }
                else
                {
                    int stime = 100000000;
                    double packper = Convert.ToDouble(comb_packet_rate.Text);
                    _MainWindow.stopSimlationWhen = stime;
                    _MainWindow.SendPackectPerSecond(packper);
                    _MainWindow.RandomDeplayment(0);
                }
            }

            this.Close();

        }

        private void comb_startup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object objval = comb_startup.SelectedItem as object;
            int va = Convert.ToInt16(objval);
            Parameters.PublicParamerters.MacStartUp = va;
        }

        private void comb_active_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object objval = comb_active.SelectedItem as object;
            int va = Convert.ToInt16(objval);
            PublicParamerters.Periods.ActivePeriod = va;
        }

        private void comb_sleep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object objval = comb_sleep.SelectedItem as object;
            int va = Convert.ToInt16(objval);
            PublicParamerters.Periods.SleepPeriod = va;
        }

        private void chk_stope_when_first_node_deis_Checked(object sender, RoutedEventArgs e)
        {
            comb_simuTime.IsEnabled = false;
        }

        private void chk_stope_when_first_node_deis_Unchecked(object sender, RoutedEventArgs e)
        {
            comb_simuTime.IsEnabled = true;
        }
    }
}
