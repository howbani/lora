using LORA.Charts;
using LORA.db;
using LORA.Modules;
using LORA.Parameters;
using LORA.Properties;
using LORA.Forwarding;
using LORA.ui.conts;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using LORA.Properties;

namespace LORA.ui
{

    /// <summary>
    /// the date list here is for eech experment.
    /// </summary>
    public class ExpermentalData
    {
        // resulates:
        public int ExpID { get; set; }
        public double EnergyConsumed { get; set; } // average consumed anerg for the path.
        public double NumberOfHops { get; set; }
        public double TransmissionDelay { get; set; } // average delay for paths 
        public double RoutingDistance { get; set; } // average routing distance for a path
        public double TransmissionDistance { get; set; } // average transmission distance for each hop in the paths.
        public double RoutingDistanceEfficiency { get; set; }
        public double TransmissionDistanceEfficiency { get; set; }
        public double RoutingEfficiency { get; set; }
        public double RoutingBalancingEfficiency { get; set; }
        public double PathsSpread { get; set; }

        // Settings:
        public string NetName { get; set; }
        public int SourceID { get; set; }
        public int NumberOfPackets { get; set; } // per experment.
        public string ParametersSet { get; set; } // en,dis,dir,per
        public double ZoneWidth { get; set; } // Zone Width

    }
    public class Experment
    {

        // basics:
        public int ExpId { get; set; }
        public int SourceID { get; set; }
        public int SinkID { get; set; }
        public double NumberofPackets { get; set; }
        public double NumberofExperments { get; set; }
        public string NetworkName { get; set; }

        // controls:
        public double PrepDistanceDistCnt { set; get; }
        public double TransDistanceDistCnt { set; get; }
        public double EnergyDistCnt { set; get; }
        public double DirectionDistCnt { set; get; }

        // display:
        public bool IsRandomControl { get; set; }
        public string Title
        {
            get
            {
                if (IsRandomControl)
                {
                    string PowersString = "Exp:" + ExpId;
                    return PowersString;
                }
                else
                {
                    string PowersString = "Exp:" + ExpId + "[γΦ=" + EnergyDistCnt + ",γd=" + TransDistanceDistCnt + ", γθ=" + DirectionDistCnt + ", γψ=" + PrepDistanceDistCnt + "]";
                    return PowersString;
                }
            }
        }

        // hops:
        public List<KeyValuePair<int, double>> HopsDistributions = new List<KeyValuePair<int, double>>();
        
        // delay and hops
        public KeyValuePair<int, double> EnergyConsumed { get; set; } // total energy for this experment
        public KeyValuePair<int, double> Hops { get; set; } // total hops for this experment 
        public KeyValuePair<int, double> Delay { get; set; } // total delay for this expermeent 
        public KeyValuePair<int, double> RoutingDistance { get; set; } // total delay for this expermeent  

        // enegy Efficiency
        public KeyValuePair<int, double> AverageTransDistrancePerHop { get; set; } // average distance per hop.  
        public KeyValuePair<int, double> TransDistanceEfficiency { get; set; } // TransDistanceEfficiency  for apath. for hop.
        public KeyValuePair<int, double> RoutingDistanceEffiecncy { get; set; } // average distance per hop. 
        public KeyValuePair<int, double> RoutingEfficiency { get; set; } // RoutingEfficiency

        // balancing Efficiency:
        public KeyValuePair<int, double> RelaysCount { get; set; }
        public KeyValuePair<int, double> HopsPerRelay { get; set; }
        public KeyValuePair<int, double> PathsSpread { get; set; }


        public KeyValuePair<int, double> BalanceingEfficiency 
        {
            get
            {
                return new KeyValuePair<int, double>(ExpId, (RoutingDistanceEffiecncy.Value+PathsSpread.Value+ TransDistanceEfficiency.Value)/3);
            }
        }

    }
    /// <summary>
    /// Interaction logic for UiSetparamertes.xaml
    /// </summary>
    public partial class UiSetparamertes : Window
    {
        double numberOfExp;
        List<Experment> expermentsList = new List<Experment>();
        public List<ImportedSensor> InmportedSensors = new List<ImportedSensor>();
        MainWindow _mianWinsow;

        List<KeyValuePair<int, double>> EnergyKeyValues = new List<KeyValuePair<int, double>>();
        List<KeyValuePair<int, double>> DelayKeyValues = new List<KeyValuePair<int, double>>();
        List<KeyValuePair<int, double>> HopsKeyvals = new List<KeyValuePair<int, double>>();
        List<KeyValuePair<int, double>> RoutitngDistanceKeyVales = new List<KeyValuePair<int, double>>();

        List<KeyValuePair<int, double>> AverageTransDistrancePerHopList = new List<KeyValuePair<int, double>>();
        List<KeyValuePair<int, double>> TransDistanceEfficiencylist = new List<KeyValuePair<int, double>>();
        List<KeyValuePair<int, double>> RoutingDistanceEffiecncyList = new List<KeyValuePair<int, double>>();
        List<KeyValuePair<int, double>> RoutingEfficiencyList = new List<KeyValuePair<int, double>>();


        List<KeyValuePair<int, double>> RelaysCountList = new List<KeyValuePair<int, double>>();
        List<KeyValuePair<int, double>> HopsPerRelayList = new List<KeyValuePair<int, double>>();
        List<KeyValuePair<int, double>> PathsSpreadList = new List<KeyValuePair<int, double>>();


        List<KeyValuePair<int, double>> BalanceingEfficiencyList = new List<KeyValuePair<int, double>>();

        public UiSetparamertes(MainWindow mianWinsow) 
        {
            InitializeComponent();
            wrap_tab1.Height = SystemParameters.FullPrimaryScreenHeight - 2;
            wrap_tab1.Width = SystemParameters.FullPrimaryScreenWidth - 2;
            _mianWinsow = mianWinsow;

            // BASICS DATA
            laodNetworksNames();
        }

        public void laodNetworksNames()
        {
            foreach (string netName in NetworkTopolgy.ImportNetworkNamesAsStrings())
            {
                com_network_name.Items.Add(new ComboBoxItem() { Content = netName });
            }
        }


        /// <summary>
        /// get the names of networkss:
        /// </summary>
        /// <param name="Canvas_SensingFeild"></param>
        /// <param name="myNetWork"></param>
        public void LaodBasicsData()
        {


            // set the values of four powers:
            int conrange = PublicParamerters.ControlsRange;
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

           

            for (int i = 1; i <= 100; i++)
            {
                com_numberof_exp.Items.Add(new ComboBoxItem() { Content = i.ToString() });
            }

            // packets:
            for (int i=1;i<=10;i++)
            {
                comb_number_of_packets.Items.Add(new ComboBoxItem() { Content = i });
                comb_number_of_packets.Items.Add(new ComboBoxItem() { Content = i*10 });
                comb_number_of_packets.Items.Add(new ComboBoxItem() { Content = i * 100 });

            }

          

            // steps:
            for (int i = 0; i <= 1; i++)
            {
                if (i == 1)
                {
                    com_direction_steps.Items.Add(new ComboBoxItem() { Content = i });
                    com_energy_steps.Items.Add(new ComboBoxItem() { Content = i });
                    com_prependicular_steps.Items.Add(new ComboBoxItem() { Content = i });
                    com_transmision_distance_steps.Items.Add(new ComboBoxItem() { Content = i });

                    com_direction_steps.Items.Add(new ComboBoxItem() { Content = -1 * i });
                    com_energy_steps.Items.Add(new ComboBoxItem() { Content = -1 * i });
                    com_prependicular_steps.Items.Add(new ComboBoxItem() { Content = -1 * i });
                    com_transmision_distance_steps.Items.Add(new ComboBoxItem() { Content = -1 * i });
                }
                else
                {
                    for (int j = 0; j <= 9; j++)
                    {
                        string str = i + "." + j;
                        double dc = Convert.ToDouble(str);

                        com_direction_steps.Items.Add(new ComboBoxItem() { Content = dc });
                        com_energy_steps.Items.Add(new ComboBoxItem() { Content = dc });
                        com_prependicular_steps.Items.Add(new ComboBoxItem() { Content = dc });
                        com_transmision_distance_steps.Items.Add(new ComboBoxItem() { Content = dc });

                        if (j > 0)
                        {
                            com_direction_steps.Items.Add(new ComboBoxItem() { Content = -1 * dc });
                            com_energy_steps.Items.Add(new ComboBoxItem() { Content = -1 * dc });
                            com_prependicular_steps.Items.Add(new ComboBoxItem() { Content = -1 * dc });
                            com_transmision_distance_steps.Items.Add(new ComboBoxItem() { Content = -1 * dc });
                        }

                    }
                }
            }


            // set defuals:
            com_direction.Text = Settings.Default.DirectionDistCnt.ToString();
            com_energy.Text = Settings.Default.EnergyDistCnt.ToString();
            com_prependicular.Text = Settings.Default.PrepDistanceDistCnt.ToString();
            com_transmision_distance.Text = Settings.Default.TransDistanceDistCnt.ToString();
            
            com_numberof_exp.Text = "1";
            com_direction_steps.Text = "0";
            com_energy_steps.Text = "0";
            com_prependicular_steps.Text = "0";
            com_transmision_distance_steps.Text = "0";
            comb_number_of_packets.Text = "300";
        }


        // Select network:
        private void com_network_name_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _mianWinsow.ClearExperment();
            com_zone_width_control.Items.Clear(); 
            string networkName = (com_network_name.SelectedItem as ComboBoxItem).Content.ToString();
            PublicParamerters.NetworkName = networkName;

            // clear:
            InmportedSensors.Clear();
            comb_sink_node.Items.Clear();
            comb_source_node.Items.Clear();
            // add new:
            InmportedSensors = NetworkTopolgy.ImportNetwok(networkName);
            foreach(ImportedSensor sen in InmportedSensors)
            {
                comb_sink_node.Items.Add(new ComboBoxItem() { Content = sen.NodeID });
                comb_source_node.Items.Add(new ComboBoxItem() { Content = sen.NodeID });
            }
            PublicParamerters.SensingRangeRadius = InmportedSensors[0].R;

            comb_sink_node.Text = "0";

            for (int i = Convert.ToInt16(PublicParamerters.SensingRangeRadius / 2); i <= 3 * PublicParamerters.SensingRangeRadius; i++) { com_zone_width_control.Items.Add(new ComboBoxItem() { Content= i }); } 
            com_zone_width_control.Text = Settings.Default.ZoneWidth.ToString();
        }


        private void btn_deploy_Click(object sender, RoutedEventArgs e)
        {
            if (com_network_name.Text != "")
            {
                if (comb_sink_node.Text != "")
                {
                    if (comb_source_node.Text != "")
                    {
                        if (comb_sink_node.Text != comb_source_node.Text)
                        {
                            foreach (ImportedSensor imsensor in InmportedSensors)
                            {
                                Sensor node = new Sensor(imsensor.NodeID);
                                node.MainWindow = _mianWinsow;
                                Point p = new Point(imsensor.Pox, imsensor.Poy);
                                node.Position = p;
                                node.VisualizedRadius = imsensor.R;
                                _mianWinsow.myNetWork.Add(node);
                                _mianWinsow.Canvas_SensingFeild.Children.Add(node);
                                _mianWinsow.Hide();
                            }

                            // make the deplayments:
                            _mianWinsow.RandomDeplayment(Convert.ToInt16(comb_sink_node.Text));

                            lbl_sensingrange.Content = InmportedSensors[0].R;
                            lbl_density.Content = PublicParamerters.Density.ToString("0.00");
                            lbl_control_range.Content = PublicParamerters.ControlsRange;

                            LaodBasicsData();
                            // set defualts:
                        }
                    }
                }
            }
        }

        private void comb_sink_node_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string selectedSink = (comb_sink_node.SelectedItem as ComboBoxItem).Content.ToString();
                string selectedSource = (comb_source_node.SelectedItem as ComboBoxItem).Content.ToString();
                if (selectedSink == selectedSource)
                {
                    MessageBox.Show("Source and sink should be diffrent nodes");
                }
            }
            catch { }
        }

        private void comb_source_node_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string selectedSink = (comb_sink_node.SelectedItem as ComboBoxItem).Content.ToString();
                string selectedSource = (comb_source_node.SelectedItem as ComboBoxItem).Content.ToString();
                if (selectedSink == selectedSource)
                {
                    MessageBox.Show("Source and sink should be diffrent nodes");
                }
            }
            catch { }
        }



        /// <summary>
        /// change the Hij
        /// </summary>
        public List<Experment> DoExperment()
        {

            List<Experment> re = new List<ui.Experment>();
            Sensor source = _mianWinsow.myNetWork[Convert.ToInt16(comb_source_node.Text)];
            Sensor sink = _mianWinsow.myNetWork[Convert.ToInt16(comb_sink_node.Text)];

            int expermentsNumber = Convert.ToInt16(com_numberof_exp.Text);
          
            for (int i = 0; i < expermentsNumber; i ++)
            {

                // random:
                if (check_random.IsChecked == true)
                {
                    double PowerHijSteps = Convert.ToDouble(com_prependicular.Text) + (i * Convert.ToDouble(com_prependicular_steps.Text));
                    double PowerNijSteps = Convert.ToDouble(com_transmision_distance.Text) + (i * Convert.ToDouble(com_transmision_distance_steps.Text));
                    double PowerRjSteps = Convert.ToDouble(com_energy.Text) + (i * Convert.ToDouble(com_energy_steps.Text));
                    double PowerThetaijSteps = Convert.ToDouble(com_direction.Text) + (i * Convert.ToDouble(com_direction_steps.Text));

                    Settings.Default.KeepLogs = false;// don't keep logs for nodes, to save momery.
                    Settings.Default.PrepDistanceDistCnt = PowerHijSteps;
                    Settings.Default.TransDistanceDistCnt = PowerNijSteps;
                    Settings.Default.EnergyDistCnt = PowerRjSteps;
                    Settings.Default.DirectionDistCnt = PowerThetaijSteps;


                    // start: send data:
                    Sensor sourceNode = _mianWinsow.myNetWork[Convert.ToInt16(comb_source_node.Text)];
                    List<double> Controls = RandomControls.Generate4Controls();
                    sourceNode.GeneratePacketAndSent(Convert.ToInt16(comb_number_of_packets.Text));

                }
                else
                {
                    double PowerHijSteps = Convert.ToDouble(com_prependicular.Text) + (i * Convert.ToDouble(com_prependicular_steps.Text));
                    double PowerNijSteps = Convert.ToDouble(com_transmision_distance.Text) + (i * Convert.ToDouble(com_transmision_distance_steps.Text));
                    double PowerRjSteps = Convert.ToDouble(com_energy.Text) + (i * Convert.ToDouble(com_energy_steps.Text));
                    double PowerThetaijSteps = Convert.ToDouble(com_direction.Text) + (i * Convert.ToDouble(com_direction_steps.Text));

                    Settings.Default.KeepLogs = false;// don't keep logs for nodes, to save momery.
                    Settings.Default.PrepDistanceDistCnt = PowerHijSteps;
                    Settings.Default.TransDistanceDistCnt = PowerNijSteps;
                    Settings.Default.EnergyDistCnt = PowerRjSteps;
                    Settings.Default.DirectionDistCnt = PowerThetaijSteps;


                    // start: send data:
                    Sensor sourceNode = _mianWinsow.myNetWork[Convert.ToInt16(comb_source_node.Text)];
                    sourceNode.GeneratePacketAndSent(Convert.ToInt16(comb_number_of_packets.Text));
                }
                

                // get the distrubtions:
                List<KeyValuePair<int, double>> dis = Distrubtions.FindDistrubtionsForHops();
                ExpermentResults result = Distrubtions.GetEnergyDelayForAnExperment();

                Experment xp = new Experment();
                xp.IsRandomControl = (bool)check_random.IsChecked;
                xp.NumberofExperments = Convert.ToDouble(com_numberof_exp.Text);
                xp.NumberofPackets = Convert.ToDouble(comb_number_of_packets.Text);
                xp.PrepDistanceDistCnt = Settings.Default.PrepDistanceDistCnt;
                xp.TransDistanceDistCnt = Settings.Default.TransDistanceDistCnt;
                xp.EnergyDistCnt = Settings.Default.EnergyDistCnt;
                xp.DirectionDistCnt = Settings.Default.DirectionDistCnt;
                xp.SinkID = Convert.ToInt16(comb_sink_node.Text);
                xp.SourceID = Convert.ToInt16(comb_source_node.Text);
                xp.HopsDistributions.AddRange(dis);
                xp.ExpId = i + 1;

                // delay and hops
                xp.EnergyConsumed = new KeyValuePair<int, double>(xp.ExpId, (result.EnergyConsumtion / xp.NumberofPackets));
                xp.Delay = new KeyValuePair<int, double>(xp.ExpId, (result.Delay / xp.NumberofPackets));
                xp.Hops = new KeyValuePair<int, double>(xp.ExpId, result.Hops/ xp.NumberofPackets);
                xp.RoutingDistance = new KeyValuePair<int, double>(xp.ExpId, result.RoutingDistance/ xp.NumberofPackets);

                //: energy effecien
                xp.AverageTransDistrancePerHop = new KeyValuePair<int, double>(xp.ExpId, result.AverageTransDistrancePerHop / xp.NumberofPackets);
                xp.TransDistanceEfficiency = new KeyValuePair<int, double>(xp.ExpId, result.TransDistanceEfficiency/ xp.NumberofPackets);
                xp.RoutingEfficiency = new KeyValuePair<int, double>(xp.ExpId, result.RoutingEfficiency / xp.NumberofPackets);
                xp.RoutingDistanceEffiecncy= new KeyValuePair<int, double>(xp.ExpId, result.RoutingDistanceEfficiency / xp.NumberofPackets);

                // blancing: averags. don't divide by pkg number.
                xp.PathsSpread = new KeyValuePair<int, double>(xp.ExpId, result.PathsSpread);
                xp.HopsPerRelay= new KeyValuePair<int, double>(xp.ExpId, result.HopsPerRelay);
                xp.RelaysCount= new KeyValuePair<int, double>(xp.ExpId, result.RelaysCount);

                re.Add(xp);

                // clear.
                sink.PacketsList.Clear(); 
                source.NumberofPacketsGeneratedByMe = 0;

                // rechange sensors: reintialized the batteries of the nodes.
                ReChargeTheNodes();




            }

            return re;
        }

        /// <summary>
        /// after each experment the nodes are recharged.
        /// that is the nodes will start with a new battery.
        /// </summary>
       public void ReChargeTheNodes()
        {
            foreach(Sensor sen in _mianWinsow.myNetWork)
            {
                sen.ResidualEnergy = PublicParamerters.BatteryIntialEnergy;
            }
        }


        private void btn_simulate_Click(object sender, RoutedEventArgs e)
        {
            if (com_network_name.Text != "")
            {
                if (comb_sink_node.Text != "")
                {
                    if (comb_source_node.Text != "")
                    {
                        if (comb_sink_node.Text != comb_source_node.Text)
                        {

                            this.WindowState = WindowState.Minimized;
                            // reset:
                            Settings.Default.PrepDistanceDistCnt = Convert.ToDouble(com_prependicular.Text);
                            Settings.Default.TransDistanceDistCnt = Convert.ToDouble(com_transmision_distance.Text);
                            Settings.Default.EnergyDistCnt = Convert.ToDouble(com_energy.Text);
                            Settings.Default.DirectionDistCnt = Convert.ToDouble(com_direction.Text);
                           

                            expermentsList = DoExperment();
                            Draw_TAB1();
                        }
                        else
                        {
                            MessageBox.Show("Please selet diffrent Source/sink node");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please selet the Source node");
                    }
                }
                else
                {
                    MessageBox.Show("Please selet the sink node");
                }

            }
            else
            {
                MessageBox.Show("Please selet the network");
            }


        }

       
        /// <summary>
        /// 
        /// </summary>
        public void Draw_TAB1()
        {
            
            chart_hops.Series.Clear();
            chart_hops.Width = 700;
            chart_hops.Height = 226;

            numberOfExp = Convert.ToDouble(com_numberof_exp.Text);

            
            // clear:
            EnergyKeyValues.Clear();
            DelayKeyValues.Clear();
            HopsKeyvals.Clear();
            RoutitngDistanceKeyVales.Clear();

            AverageTransDistrancePerHopList.Clear();
            TransDistanceEfficiencylist.Clear();
            RoutingDistanceEffiecncyList.Clear();
            RoutingEfficiencyList.Clear();


            RelaysCountList.Clear();
            HopsPerRelayList.Clear();
            PathsSpreadList.Clear();


            BalanceingEfficiencyList.Clear();
            //


            double sumEnergy = 0;
            double sumRoutingDistance = 0;
            double sumHops = 0;
            double sumDelay = 0; 
            foreach (Experment exp in expermentsList)
            {

                sumEnergy += exp.EnergyConsumed.Value;
                sumRoutingDistance += exp.RoutingDistance.Value;
                sumHops += exp.Hops.Value;
                sumDelay += exp.Delay.Value;

                LineSeries LineHops = new LineSeries();
                LineHops.Title = exp.Title; // hops dist
                LineHops.DependentValuePath = "Value";
                LineHops.IndependentValuePath = "Key";
                LineHops.ItemsSource = exp.HopsDistributions;

                chart_hops.Series.Add(LineHops); // hops dis per exp *
                EnergyKeyValues.Add(exp.EnergyConsumed); // energy per exp *
                DelayKeyValues.Add(exp.Delay); // delay per exp *
                HopsKeyvals.Add(exp.Hops); // hops per exp *
                RoutitngDistanceKeyVales.Add(exp.RoutingDistance); // routing distance per exp *


                //: fig2:
                AverageTransDistrancePerHopList.Add(exp.AverageTransDistrancePerHop); // averag trans distance per exp *
                TransDistanceEfficiencylist.Add(exp.TransDistanceEfficiency); // trans effeicny per exp. *
                RoutingDistanceEffiecncyList.Add(exp.RoutingDistanceEffiecncy); // routing distance eff *
                RoutingEfficiencyList.Add(exp.RoutingEfficiency); // routing effecian.

                BalanceingEfficiencyList.Add(exp.BalanceingEfficiency); // blancing eff.
                //
                RelaysCountList.Add(exp.RelaysCount);
                HopsPerRelayList.Add(exp.HopsPerRelay);
                PathsSpreadList.Add(exp.PathsSpread); // path spread


            }

            cols_delay_distribution.DataContext = DelayKeyValues;
            cols_energy_distribution.DataContext = EnergyKeyValues;
            cols_hops.DataContext = HopsKeyvals;
            col_routingDistance.DataContext = RoutitngDistanceKeyVales;

            cols_hops.Title = (sumHops / numberOfExp).ToString("0.00");
            cols_delay_distribution.Title= (sumDelay / numberOfExp).ToString("0.00");
            col_routingDistance.Title = (sumRoutingDistance / numberOfExp).ToString("0.00");
            cols_energy_distribution.Title = (sumEnergy / numberOfExp).ToString("0.00");


            //: Fig2:
            col_averageroutingdistance.DataContext = AverageTransDistrancePerHopList;
            col_TransmissionDistanceEfficiency.DataContext = TransDistanceEfficiencylist;
            col_Routingefficiency.DataContext = RoutingEfficiencyList;
            col_routingdistanceefficiency.DataContext = RoutingDistanceEffiecncyList;

            // col_RDE, col_RE ,col_TDE
            col_RE.DataContext = RoutingEfficiencyList;
            col_RDE.DataContext = RoutingDistanceEffiecncyList;
            col_TDE.DataContext= TransDistanceEfficiencylist;

            //
            cols_Balanceing.DataContext = PathsSpreadList;
            cols_RelaysCount.DataContext = RelaysCountList;
            col_HopsPerRelay.DataContext = HopsPerRelayList;


            tab3_col_blancing.DataContext = PathsSpreadList;
            tab3_col_blancing_eff.DataContext = BalanceingEfficiencyList;
            tab3_col_routing_distance_eff.DataContext = RoutingDistanceEffiecncyList;
            tab3_col_transDistance_eff.DataContext = TransDistanceEfficiencylist;


           
        }

       


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _mianWinsow.Show();
        }

        private void check_random_Checked(object sender, RoutedEventArgs e)
        {
            stack_controls.Visibility = Visibility.Collapsed;
        }

        private void check_random_Unchecked(object sender, RoutedEventArgs e)
        {
            stack_controls.Visibility = Visibility.Visible;
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void btn_show_reslts_Click(object sender, RoutedEventArgs e)
        {
            UiShowLists winsData = new ui.UiShowLists();

            // HopsKeyvals
            ListControl NumberOFHopsList = new ListControl();
            NumberOFHopsList.lbl_title.Content = "Average Number of Hops Per Path";
            NumberOFHopsList.dg_date.ItemsSource = HopsKeyvals;
            winsData.stack_items.Children.Add(NumberOFHopsList);
            
            // DelayKeyValues
            ListControl DelayDataList = new ListControl();
            DelayDataList.lbl_title.Content = "Average Delay Per Path ms 10^5";
            DelayDataList.dg_date.ItemsSource = DelayKeyValues;
            winsData.stack_items.Children.Add(DelayDataList);

            //EnergyKeyValues
            ListControl EnergyDataList = new ListControl();
            EnergyDataList.lbl_title.Content = "Average Energy Consumption J*10^5";
            EnergyDataList.dg_date.ItemsSource = EnergyKeyValues;
            winsData.stack_items.Children.Add(EnergyDataList);

            // RoutitngDistanceKeyVales 
            ListControl RoutingDistanceDataList = new ListControl();
            RoutingDistanceDataList.lbl_title.Content = "Average Routing Distance(m)";
            RoutingDistanceDataList.dg_date.ItemsSource = RoutitngDistanceKeyVales;
            winsData.stack_items.Children.Add(RoutingDistanceDataList);



            // AverageTransDistrancePerHopList
            ListControl AverageTransDistrancDataList = new ListControl();
            AverageTransDistrancDataList.lbl_title.Content = "Average Transmission Distance(m)";
            AverageTransDistrancDataList.dg_date.ItemsSource = AverageTransDistrancePerHopList;
            winsData.stack_items.Children.Add(AverageTransDistrancDataList);

            //RoutingDistanceEffiecncyList
            ListControl RoutingDistanceEffiecncyDataList = new ListControl();
            RoutingDistanceEffiecncyDataList.lbl_title.Content = "Routing Distance Efficienc%";
            RoutingDistanceEffiecncyDataList.dg_date.ItemsSource = RoutingDistanceEffiecncyList;
            winsData.stack_items.Children.Add(RoutingDistanceEffiecncyDataList);


            //TransDistanceEfficiencylist
            ListControl TransDistanceEfficiencyDataList = new ListControl();
            TransDistanceEfficiencyDataList.lbl_title.Content = "Transmission Distance Efficiency%";
            TransDistanceEfficiencyDataList.dg_date.ItemsSource = TransDistanceEfficiencylist;
            winsData.stack_items.Children.Add(TransDistanceEfficiencyDataList);

            //RoutingEfficiencyList 
            ListControl RoutingEfficiencyDataList = new ListControl();
            RoutingEfficiencyDataList.lbl_title.Content = "Routing Efficiency%";
            RoutingEfficiencyDataList.dg_date.ItemsSource = RoutingEfficiencyList;
            winsData.stack_items.Children.Add(RoutingEfficiencyDataList);



            //BalanceingEfficiencyList
            ListControl BalanceingEfficiencyDataList = new ListControl();
            BalanceingEfficiencyDataList.lbl_title.Content = "Balancing Efficiency %";
            BalanceingEfficiencyDataList.dg_date.ItemsSource = BalanceingEfficiencyList;
            winsData.stack_items.Children.Add(BalanceingEfficiencyDataList);


            //PathsSpreadList 
            ListControl PathsSpreadDataList = new ListControl();
            PathsSpreadDataList.lbl_title.Content = "Paths Spread (PS) %";
            PathsSpreadDataList.dg_date.ItemsSource = PathsSpreadList;
            winsData.stack_items.Children.Add(PathsSpreadDataList);

            winsData.Show();

        }


        private void btn_show_comhansiveResults_Click(object sender, RoutedEventArgs e)
        {
            ExpermentalData averageAll = new ui.ExpermentalData(); 
            List<ExpermentalData> ListData = new List<ui.ExpermentalData>();
            for (int i = 0; i < numberOfExp; i++)
            {
                ExpermentalData Dat = new ExpermentalData();
                Dat.ExpID = i + 1;
                Dat.PathsSpread = PathsSpreadList[i].Value;
                Dat.RoutingBalancingEfficiency = BalanceingEfficiencyList[i].Value;
                Dat.RoutingEfficiency = RoutingEfficiencyList[i].Value;
                Dat.TransmissionDistanceEfficiency = TransDistanceEfficiencylist[i].Value;
                Dat.TransmissionDistance = AverageTransDistrancePerHopList[i].Value;
                Dat.RoutingDistance = RoutitngDistanceKeyVales[i].Value;
                Dat.EnergyConsumed = EnergyKeyValues[i].Value;
                Dat.TransmissionDelay = DelayKeyValues[i].Value;
                Dat.NumberOfHops = HopsKeyvals[i].Value;
                Dat.RoutingDistanceEfficiency = RoutingDistanceEffiecncyList[i].Value;

                //average
                averageAll.PathsSpread += PathsSpreadList[i].Value;
                averageAll.RoutingBalancingEfficiency += BalanceingEfficiencyList[i].Value;
                averageAll.RoutingEfficiency += RoutingEfficiencyList[i].Value;
                averageAll.TransmissionDistanceEfficiency += TransDistanceEfficiencylist[i].Value;
                averageAll.TransmissionDistance += AverageTransDistrancePerHopList[i].Value;
                averageAll.RoutingDistance += RoutitngDistanceKeyVales[i].Value;
                averageAll.EnergyConsumed += EnergyKeyValues[i].Value;
                averageAll.TransmissionDelay += DelayKeyValues[i].Value;
                averageAll.NumberOfHops += HopsKeyvals[i].Value;
                averageAll.RoutingDistanceEfficiency += RoutingDistanceEffiecncyList[i].Value;

                Dat.ZoneWidth = Settings.Default.ZoneWidth;
                Dat.NetName = PublicParamerters.NetworkName;
                Dat.SourceID = Convert.ToInt16(comb_source_node.Text);
                Dat.NumberOfPackets = Convert.ToInt16(comb_number_of_packets.Text);
                Dat.ParametersSet = Settings.Default.EnergyDistCnt + "_"
                    + Settings.Default.TransDistanceDistCnt + "_"
                    + Settings.Default.DirectionDistCnt + "_"
                    + Settings.Default.PrepDistanceDistCnt;

                //
                ListData.Add(Dat);
            }

            if (ListData.Count > 0)
            {
                averageAll.ExpID = ListData.Count + 1;
                averageAll.ZoneWidth = Settings.Default.ZoneWidth;
                averageAll.NetName = PublicParamerters.NetworkName;
                averageAll.SourceID = Convert.ToInt16(comb_source_node.Text);
                averageAll.NumberOfPackets = Convert.ToInt16(comb_number_of_packets.Text);
                averageAll.ParametersSet = Settings.Default.EnergyDistCnt + "_"
                    + Settings.Default.TransDistanceDistCnt + "_"
                    + Settings.Default.DirectionDistCnt + "_"
                    + Settings.Default.PrepDistanceDistCnt;
                // divied:
                averageAll.PathsSpread = averageAll.PathsSpread / numberOfExp;
                averageAll.RoutingBalancingEfficiency = averageAll.RoutingBalancingEfficiency / numberOfExp;
                averageAll.RoutingEfficiency = averageAll.RoutingEfficiency / numberOfExp;
                averageAll.TransmissionDistanceEfficiency = averageAll.TransmissionDistanceEfficiency / numberOfExp;
                averageAll.TransmissionDistance = averageAll.TransmissionDistance / numberOfExp;
                averageAll.RoutingDistance = averageAll.RoutingDistance / numberOfExp;
                averageAll.EnergyConsumed = averageAll.EnergyConsumed / numberOfExp;
                averageAll.TransmissionDelay = averageAll.TransmissionDelay / numberOfExp;
                averageAll.NumberOfHops = averageAll.NumberOfHops / numberOfExp;
                averageAll.RoutingDistanceEfficiency = averageAll.RoutingDistanceEfficiency / numberOfExp;


                ListData.Add(averageAll);

                UiDataExport dbExpert = new UiDataExport();
                dbExpert.ExpList = ListData;
                dbExpert.NetName = ListData[0].NetName;
                dbExpert.ParametersSet = ListData[0].ParametersSet;
                dbExpert.Title = "Expermantal Data For Each Experment";
                dbExpert.dg_date.ItemsSource = ListData;
                dbExpert.Show();
            }
        }

       
        private void com_zone_width_control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
