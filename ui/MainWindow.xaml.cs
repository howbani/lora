using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using LORA.Modules;
using LORA.db;
using LORA.Computations;
using LORA.Coverage;
using LORA.Parameters;
using LORA.Charts;
using LORA.DataPacket;
using LORA.Properties;
using System.Windows.Media;
using LORA.Logs;
using LORA.ui.conts;
using System.Data;
using LORA.ExpermentsResults;
using System.Windows.Threading;
using LORA.Forwarding;
using LORA.ExpermentsResults.Energy_consumptions;
using LORA.ExpermentsResults.Lifetime;

namespace LORA.ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string PacketRate { get; set; }
        public Int32 stopSimlationWhen = 1000000000; // s by defult.
        public DispatcherTimer TimerCounter = new DispatcherTimer();
        public DispatcherTimer RandomSelectSourceNodesTimer = new DispatcherTimer();

        public static double Swith;// sensing feild width.
        public static double Sheigh;// sensing feild height.

        /// <summary>
        /// the area of sensing feild.
        /// </summary>
        public static double SensingFeildArea
        {
            get
            {
                double w = Swith;
                double h = Sheigh;

                if (PublicParamerters.NetworkName != null)
                {
                    string x1 = PublicParamerters.NetworkName;
                    string[] x1s = x1.Split('_');
                    if (x1s.Length >= 2)
                    {
                        if (x1s[0] == "area")
                        {
                            if (x1s[1] != null)
                            {
                                //300m2
                                string x2 = x1s[1];
                                string[] x2s = x2.Split('m');
                                if (x2s[0] != null)
                                {
                                    w = Convert.ToDouble(x2s[0]);
                                    h = w;
                                }
                            }
                        }
                    }
                }
                return w * h;
            }
        }

        public List<Sensor> myNetWork = new List<Sensor>();

        bool isCoverageSelected = false;


        public MainWindow()
        {
            InitializeComponent();
            // sensing feild
            Swith = Canvas_SensingFeild.Width - 218;
            Sheigh = Canvas_SensingFeild.Height - 218;

            // battery levels colors:
            FillColors();
            RandomSelectSourceNodesTimer.Tick += RandomSelectNodes_Tick;
            TimerCounter.Interval = TimeSpan.FromSeconds(1);
            TimerCounter.Tick += TimerCounter_Tick;

            /*
            UIProbabilityofMultipleReceiversinSubzone x = new UIProbabilityofMultipleReceiversinSubzone();
            x.Topmost = true;
            x.Show();*/

          
            

        }

        private void TimerCounter_Tick(object sender, EventArgs e)
        {
            //
            if (PublicParamerters.SimulationTime <= stopSimlationWhen + PublicParamerters.MacStartUp)
            {
                PublicParamerters.SimulationTime += 1;
                Title = "LORA:" + PublicParamerters.SimulationTime.ToString();
            }
            else
            {
                TimerCounter.Stop();
                RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(0);
                RandomSelectSourceNodesTimer.Stop();
                top_menu.IsEnabled = true;
            }
        }

        private void RandomSelectNodes_Tick(object sender, EventArgs e)
        {
            // start sending after the nodes are intilized all.
            if (PublicParamerters.SimulationTime > PublicParamerters.MacStartUp)
            {
                int index = 1 + Convert.ToInt16(UnformRandomNumberGenerator.GetUniform(PublicParamerters.NumberofNodes - 2));
                if (index != PublicParamerters.SinkNode.ID)
                {
                    myNetWork[index].GeneratePacketAndSent();
                }
            }
        }

        private void FillColors()
        {

            // POWER LEVEL:
            lvl_0.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col0));
            lvl_1_9.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col1_9));
            lvl_10_19.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col10_19));
            lvl_20_29.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col20_29));
            lvl_30_39.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col30_39));
            lvl_40_49.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col40_49));
            lvl_50_59.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col50_59));
            lvl_60_69.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col60_69));
            lvl_70_79.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col70_79));
            lvl_80_89.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col80_89));
            lvl_90_100.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col90_100));

            // MAC fuctions:
            lbl_node_state_check.Fill = NodeStateColoring.ActiveColor;
            lbl_node_state_sleep.Fill = NodeStateColoring.SleepColor;
        }


        private void BtnFile(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            switch (Header)
            {
                case "_Multiple Nodes":
                    {
                        UiAddNodes ui = new UiAddNodes();
                        ui.MainWindow = this;
                        ui.Show();
                        break;
                    }

                case "_Export Topology":
                    {
                        UiExportTopology top = new UiExportTopology(myNetWork);
                        top.Show();
                        break;
                    }

                case "_Import Topology":
                    {
                        UiImportTopology top = new UiImportTopology(this);
                        top.Show();
                        break;
                    }
            }

        }

        int rounds = 0;


        public void DisplaySimulationParameters(int rootNodeId, string deblpaymentMethod)
        {
            PublicParamerters.SinkNode = myNetWork[rootNodeId];
            PublicParamerters.SinkNode.Ellipse_center.Width = 16;
            PublicParamerters.SinkNode.Ellipse_center.Height = 16;
            PublicParamerters.SinkNode.Ellipse_center.Fill = Brushes.OrangeRed;
            PublicParamerters.SinkNode.Ellipse_MAC.Fill = Brushes.OrangeRed;

            PublicParamerters.SinkNode.lbl_Sensing_ID.Foreground = Brushes.Blue;
            PublicParamerters.SinkNode.lbl_Sensing_ID.FontWeight = FontWeights.Bold;
            lbl_sink_id.Content = rootNodeId;
            lbl_coverage.Content = deblpaymentMethod;
            lbl_network_size.Content = myNetWork.Count;
            lbl_sensing_range.Content = PublicParamerters.SinkNode.VisualizedRadius;
            lbl_communication_range.Content = (PublicParamerters.SinkNode.VisualizedRadius * 2);
            lbl_Transmitter_Electronics.Content = PublicParamerters.E_elec;
            lbl_fes.Content = PublicParamerters.Efs;
            lbl_Transmit_Amplifier.Content = PublicParamerters.Emp;
            lbl_data_length_control.Content = PublicParamerters.ControlDataLength;
            lbl_data_length_routing.Content = PublicParamerters.RoutingDataLength;
            lbl_density.Content = PublicParamerters.Density;
            lbl_control_range.Content = PublicParamerters.ControlsRange;
            lbl_zone_width.Content = PublicParamerters.CandidateZoneWidth.ToString();
            Settings.Default.IsIntialized = true;
            // start sending date.
            lbl_expected_num_forwarders.Content = PublicParamerters.ExpectedNumberOfFowarders;

            // ehtmal
            lbl_pr_emprysubzone.Content = ProbsAndStatistics.PrNoNodeWithinSubZone;
            lbl_pr_zone_have_ExF.Content = ProbsAndStatistics.ProThereAreExfNodesWithinTheSubzone;
            lbl_pr_at_least_one_insubzone.Content = ProbsAndStatistics.ProAtLeastOneAndAtMostExf;

            TimerCounter.Start(); // START count the running time.


        }

        public void HideSimulationParameters()
        {
            menSimuTim.IsEnabled = true;
            stopSimlationWhen = 1000000;

            rounds = 0;
            lbl_rounds.Content = "0";
            PublicParamerters.SinkNode = null;
            lbl_sink_id.Content = "nil";
            lbl_coverage.Content = "nil";
            lbl_network_size.Content = "unknown";
            lbl_sensing_range.Content = "unknown";
            lbl_communication_range.Content = "unknown";
            lbl_Transmitter_Electronics.Content = "unknown";
            lbl_fes.Content = "unknown";
            lbl_Transmit_Amplifier.Content = "unknown";
            lbl_data_length_control.Content = "unknown";
            lbl_data_length_routing.Content = "unknown";
            lbl_density.Content = "0";
            lbl_control_range.Content = "0";
            lbl_zone_width.Content = "0";
            Settings.Default.IsIntialized = false;
            Settings.Default.ZoneWidth = 0;
            lbl_expected_num_forwarders.Content = "0";

            lbl_pr_emprysubzone.Content = "0";
            lbl_pr_zone_have_ExF.Content = "0";
            lbl_pr_at_least_one_insubzone.Content = "0";

            //
            RandomSelectSourceNodesTimer.Stop();
            TimerCounter.Stop();

        }



        private void EngageMacProcol()
        {
            ForwardersSelection x = new ForwardersSelection();
            x.IntializeForwarders(myNetWork);

            foreach (Sensor sen in myNetWork)
            {
                sen.Mac = new MAC.BoXMAC(sen);
                sen.BatRangesList = PublicParamerters.getRanges();
            }
        }


        public void RandomDeplayment(int sinkIndex)
        {
            PublicParamerters.NumberofNodes = myNetWork.Count;
            int rootNodeId = sinkIndex;
            PublicParamerters.SinkNode = myNetWork[rootNodeId];
            GetOverlappingNodes overlappingNodesFinder = new GetOverlappingNodes(myNetWork);
            overlappingNodesFinder.GetOverlappingForAllNodes();

            string PowersString = "γΦ=" + Settings.Default.EnergyDistCnt + ",γd=" + Settings.Default.TransDistanceDistCnt + ", γθ=" + Settings.Default.DirectionDistCnt + ", γψ=" + Settings.Default.PrepDistanceDistCnt;
            lbl_hops_dis_network_info.Content = PublicParamerters.NetworkName + "," + PowersString;
            isCoverageSelected = true;

            PublicParamerters.SensingFeildArea = SensingFeildArea;

            PublicParamerters.Density = Density.GetDensity(myNetWork);
            Settings.Default.ZoneWidth = PublicParamerters.CandidateZoneWidth;
            DisplaySimulationParameters(rootNodeId, "Random");

            EngageMacProcol();


        }

        public void GridCoverag2(int sinkIndex)
        {
            PublicParamerters.NumberofNodes = myNetWork.Count;
            int rootNodeId = sinkIndex;
            PublicParamerters.SinkNode = myNetWork[rootNodeId];
            GridCoverage GridCoverage = new GridCoverage();
            GridCoverage.GridCoverage2(Canvas_SensingFeild, myNetWork, Convert.ToInt16((Sensor.SR * 2)));
            GetOverlappingNodes overlappingNodesFinder = new GetOverlappingNodes(myNetWork);
            overlappingNodesFinder.GetOverlappingForAllNodes();

            string PowersString = "γΦ=" + Settings.Default.EnergyDistCnt + ",γd=" + Settings.Default.TransDistanceDistCnt + ", γθ=" + Settings.Default.DirectionDistCnt + ", γψ=" + Settings.Default.PrepDistanceDistCnt;
            lbl_hops_dis_network_info.Content = PublicParamerters.NetworkName + "," + PowersString;
            isCoverageSelected = true;
            PublicParamerters.Density = Density.GetDensity(myNetWork);
            Settings.Default.ZoneWidth = PublicParamerters.CandidateZoneWidth;
            DisplaySimulationParameters(rootNodeId, "grid_coverag_2");

            EngageMacProcol();
        }

        public void GridCoverag1(int sinkIndex)
        {
            PublicParamerters.NumberofNodes = myNetWork.Count;
            int rootNodeId = sinkIndex;
            PublicParamerters.SinkNode = myNetWork[rootNodeId];
            GridCoverage GridCoverage = new Coverage.GridCoverage();
            GridCoverage.GridCoverage1(Canvas_SensingFeild, myNetWork, Convert.ToInt16((Sensor.SR * 2) * 0.7));
            GetOverlappingNodes overlappingNodesFinder = new GetOverlappingNodes(myNetWork);
            overlappingNodesFinder.GetOverlappingForAllNodes();

            string PowersString = "γΦ=" + Settings.Default.EnergyDistCnt + ",γd=" + Settings.Default.TransDistanceDistCnt + ", γθ=" + Settings.Default.DirectionDistCnt + ", γψ=" + Settings.Default.PrepDistanceDistCnt;
            lbl_hops_dis_network_info.Content = PublicParamerters.NetworkName + "," + PowersString;
            isCoverageSelected = true;
            PublicParamerters.Density = Density.GetDensity(myNetWork);
            Settings.Default.ZoneWidth = PublicParamerters.CandidateZoneWidth;
            DisplaySimulationParameters(rootNodeId, "grid_coverag_1");

            EngageMacProcol();
        }

        public void ZigzagCoverage(int sinkIndex)
        {
            int rootNodeId = sinkIndex;
            PublicParamerters.NumberofNodes = myNetWork.Count;
            PublicParamerters.SinkNode = myNetWork[rootNodeId];
            ZizageCoverage zig = new ZizageCoverage();
            zig.coverage(Canvas_SensingFeild, myNetWork, Convert.ToInt16(2 * Sensor.SR));
            GetOverlappingNodes overlappingNodesFinder = new GetOverlappingNodes(myNetWork);
            overlappingNodesFinder.GetOverlappingForAllNodes();

            string PowersString = "γΦ=" + Settings.Default.EnergyDistCnt + ",γd=" + Settings.Default.TransDistanceDistCnt + ", γθ=" + Settings.Default.DirectionDistCnt + ", γψ=" + Settings.Default.PrepDistanceDistCnt;
            lbl_hops_dis_network_info.Content = PublicParamerters.NetworkName + "," + PowersString;
            isCoverageSelected = true;
            PublicParamerters.Density = Density.GetDensity(myNetWork);
            Settings.Default.ZoneWidth = PublicParamerters.CandidateZoneWidth;
            DisplaySimulationParameters(rootNodeId, "Zigzag");

            EngageMacProcol();
        }


        private void Coverage_Click(object sender, RoutedEventArgs e)
        {
            if (myNetWork.Count > 0)
            {
                MenuItem item = sender as MenuItem;
                string Header = item.Name.ToString();
                switch (Header)
                {
                    case "grid_coverag_1":
                        if (myNetWork.Count > 1)
                        {
                            GridCoverag1(0);
                        }
                        break;
                    case "grid_coverag_2":
                        if (myNetWork.Count > 1)
                        {
                            GridCoverag2(0);
                        }
                        break;
                    case "zigzag_coverage":
                        if (myNetWork.Count > 1)
                        {
                            ZigzagCoverage(0);
                        }
                        break;

                    case "btn_Random":
                        {
                            RandomDeplayment(0);
                        }

                        break;
                }
            }
            else
            {
                MessageBox.Show("Please imort the nodes from Db.");
            }
        }

        private void base_station_position_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            switch (Header)
            {
                case "_Top":

                    break;
                case "_Bottom":

                    break;
                case "_Right":

                    break;
                case "_Left":

                    break;
            }
        }
        private void Display_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Name.ToString();
            switch (Header)
            {
                case "_show_id":
                    foreach (Sensor sensro in myNetWork)
                    {
                        if (sensro.lbl_Sensing_ID.Visibility == Visibility.Hidden)
                        {
                            sensro.lbl_Sensing_ID.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            sensro.lbl_Sensing_ID.Visibility = Visibility.Hidden;
                        }
                    }
                    break;

                case "_show_rang":
                    foreach (Sensor sensro in myNetWork)
                    {
                        if (sensro.Ellipse_Sensing_range.Visibility == Visibility.Hidden)
                        {
                            sensro.Ellipse_Sensing_range.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            sensro.Ellipse_Sensing_range.Visibility = Visibility.Hidden;
                        }

                        if (sensro.Ellipse_Communication_range.Visibility == Visibility.Hidden)
                        {
                            sensro.Ellipse_Communication_range.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            sensro.Ellipse_Communication_range.Visibility = Visibility.Hidden;
                        }
                    }
                    break;

                case "_show_sen_range":
                    foreach (Sensor sensro in myNetWork)
                    {
                        if (sensro.Ellipse_Sensing_range.Visibility == Visibility.Hidden)
                        {
                            sensro.Ellipse_Sensing_range.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            sensro.Ellipse_Sensing_range.Visibility = Visibility.Hidden;
                        }


                    }
                    break;
                case "_show_com_range":
                    foreach (Sensor sensro in myNetWork)
                    {
                        if (sensro.Ellipse_Communication_range.Visibility == Visibility.Hidden)
                        {
                            sensro.Ellipse_Communication_range.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            sensro.Ellipse_Communication_range.Visibility = Visibility.Hidden;
                        }
                    }
                    break;
                //_show_device
                case "_show_device":
                    foreach (Sensor sensro in myNetWork)
                    {
                        if (sensro.Ellipse_center.Visibility == Visibility.Hidden)
                        {
                            sensro.Ellipse_center.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            sensro.Ellipse_center.Visibility = Visibility.Hidden;
                        }
                    }
                    break;
                case "_show_battrey":
                    foreach (Sensor sensro in myNetWork)
                    {
                        if (sensro.Prog_batteryCapacityNotation.Visibility == Visibility.Hidden)
                        {
                            sensro.Prog_batteryCapacityNotation.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            sensro.Prog_batteryCapacityNotation.Visibility = Visibility.Hidden;
                        }
                    }
                    break;
            }
        }

        private void btn_other_Menu(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            switch (Header)
            {

                //
                case "_Show Dead Node":
                    {
                        if (myNetWork.Count > 0)
                        {
                            if (PublicParamerters.DeadNodeList.Count > 0)
                            {
                                UiNetworkLifetimeReport xx = new UiNetworkLifetimeReport();
                                xx.Title = "LORA Lifetime report";
                                xx.dg_grid.ItemsSource = PublicParamerters.DeadNodeList;
                                xx.Show();
                            }
                            else
                                MessageBox.Show("No Dead node.");
                        }
                        else
                        {
                            MessageBox.Show("No Network is selected.");
                        }
                    }
                    break;

                case "_Show Resultes":
                    {
                        if (myNetWork.Count > 0)
                        {
                            ExpReport xx = new ExpReport(this);
                            xx.Show();
                        }
                    }
                    break;
                case "_Draw Tree":

                    break;
                case "_Print Info":
                    UIshowSensorsLocations uIlocations = new UIshowSensorsLocations(myNetWork);
                    uIlocations.Show();
                    break;
                case "_Entir Network Routing Log":
                    UiRoutingDetailsLong routingLogs = new ui.UiRoutingDetailsLong(myNetWork);
                    routingLogs.Show();
                    break;
                case "_Log For Each Sensor":

                    break;
                //_Relatives:
                case "_Node Forwarding Probability Distributions":
                    {
                        UiShowLists windsow = new UiShowLists();
                        windsow.Title = "Forwarding Probability Distributions For Each Node";
                        foreach (Sensor source in myNetWork)
                        {
                            if (source.MyForwardersShortedList.Count > 0)
                            {
                                ListControl List = new conts.ListControl();
                                List.lbl_title.Content = "Node:" + source.ID;
                                List.dg_date.ItemsSource = source.MyForwardersShortedList;
                                windsow.stack_items.Children.Add(List);
                            }
                        }
                        windsow.Show();
                        break;
                    }
                //
                case "_Expermental Results":
                    UIExpermentResults xxxiu = new UIExpermentResults();
                    xxxiu.Show();
                    break;
                case "_Probability Matrix":
                    {
                        UiShowLists windsow = new UiShowLists();
                        windsow.Title = "Matrix";
                        AdjecentMatrix mat = new AdjecentMatrix();
                        List<DataTable> Tables = mat.ConvertToTable(myNetWork);
                        foreach (DataTable table in Tables)
                        {
                            ListControl List = new conts.ListControl();
                            List.lbl_title.Content = table.TableName;
                            List.dg_date.ItemsSource = table.DefaultView;
                            windsow.stack_items.Children.Add(List);
                        }
                        windsow.Show();
                    }
                    break;
                //
                case "_Packets Paths":
                    UiRecievedPackertsBySink packsInsinkList = new UiRecievedPackertsBySink();
                    packsInsinkList.Show();

                    break;
                //
                case "_Random Numbers":

                    List<KeyValuePair<int, double>> rands = new List<KeyValuePair<int, double>>();
                    int index = 0;
                    foreach (Sensor sen in myNetWork)
                    {
                        foreach (SensorRoutingLog log in sen.Logs)
                        {
                            if (log.IsSend)
                            {
                                index++;
                                rands.Add(new KeyValuePair<int, double>(index, log.ForwardingRandomNumber));
                            }
                        }
                    }

                    UiRandomNumberGeneration wndsow = new ui.UiRandomNumberGeneration();
                    wndsow.chart_x.DataContext = rands;
                    wndsow.Show();

                    break;
                case "_Nodes Load":
                    {
                        SegmaManager sgManager = new SegmaManager();
                        Sensor sink = PublicParamerters.SinkNode;
                        List<string> Paths = new List<string>();
                        if (sink != null)
                        {
                            foreach (Datapacket pck in sink.PacketsList)
                            {
                                Paths.Add(pck.Path);
                            }

                        }

                        sgManager.Filter(Paths);
                        UiShowLists windsow = new UiShowLists();
                        windsow.Title = "Nodes Load";
                        SegmaCollection collectionx = sgManager.GetCollection;
                        foreach (SegmaSource source in collectionx.GetSourcesList)
                        {
                            source.NumberofPacketsGeneratedByMe = myNetWork[source.SourceID].NumberofPacketsGeneratedByMe;
                            ListControl List = new conts.ListControl();
                            List.lbl_title.Content = "Source:" + source.SourceID + " Pks:" + source.NumberofPacketsGeneratedByMe + " Relays:" + source.RelaysCount + " Hops:" + source.HopsSum + " Mean:" + source.Mean + " Variance:" + source.Veriance + " E:" + source.PathsSpread;
                            List.dg_date.ItemsSource = source.GetRelayNodes;
                            windsow.stack_items.Children.Add(List);
                        }
                        windsow.Show();
                        break;
                    }
                //_Distintc Paths
                case "_Distintc Paths":
                    {
                        UiShowLists windsow = new UiShowLists();
                        windsow.Title = "Distinct Paths for each Source";
                        DisPathConter dip = new DisPathConter();
                        List<ClassfyPathsPerSource> classfy = dip.ClassyfyDistinctPathsPerSources();
                        foreach (ClassfyPathsPerSource source in classfy)
                        {
                            ListControl List = new conts.ListControl();
                            List.lbl_title.Content = "Source:" + source.SourceID;
                            List.dg_date.ItemsSource = source.DistinctPathsForThisSource;
                            windsow.stack_items.Children.Add(List);
                        }
                        windsow.Show();
                        break;
                    }
            }
        }


        private void Btn_Send_packetsFromEachNode(object sender, RoutedEventArgs e)
        {
            if (isCoverageSelected)
            {
                // not random:
                MenuItem slected = sender as MenuItem;
                int pktsNumber = Convert.ToInt16(slected.Header.ToString().Split('_')[1]);
                rounds += pktsNumber;
                lbl_rounds.Content = rounds;

                for (int i = 1; i <= pktsNumber; i++)
                {
                    foreach (Sensor sen in myNetWork)
                    {
                        if (sen.ID != PublicParamerters.SinkNode.ID)
                        {
                            sen.GeneratePacketAndSent();
                           // sen.GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                        }
                    }
                }

                /*
                foreach (Sensor sen in myNetWork)
                {
                    if (sen.ID != PublicParamerters.SinkNode.ID)
                    {
                        for (int i = 1; i <= pktsNumber; i++)
                        {
                            sen.GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                        }
                    }
                }*/

            }
            else
            {
                MessageBox.Show("Please selete the coverage.Coverage->Random");
            }
        }

        private void BuildTheTree(object sender, RoutedEventArgs e)
        {

        }

        private void tconrol_charts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((tconrol_charts.SelectedItem as TabItem).Name == "tab_Path_Efficiency")
            {
                List<List<KeyValuePair<int, double>>> Vals = PathEfficiencyChart.BuildPathEfficiencyAndDelayChart();
                if (Vals.Count == 2)
                {
                    col_Path_Efficiency.DataContext = Vals[0];
                    col_Delay.DataContext = Vals[1];
                }
                col_EnergyConsumptionForEachNode.DataContext = EnergyConsumptionForEachNode.BuildEnergyConsumptionForEachNodeChart(myNetWork);
            }
            else if ((tconrol_charts.SelectedItem as TabItem).Name == "tab_packets_chart")
            {
                List<List<KeyValuePair<int, double>>> Vals = PathEnergyChart.BuildChartPackets();
                if (Vals.Count == 2)
                {
                    col_packs_hops.DataContext = Vals[0];
                    //   col_packs_energy.DataContext= Vals[1];
                }
            }
            //

            else if ((tconrol_charts.SelectedItem as TabItem).Name == "tab_hops_distrubitions")
            {
                List<List<KeyValuePair<int, double>>> xxx = Distrubtions.FindDistubtions();

                List<KeyValuePair<int, double>> hops = xxx[0];
                List<KeyValuePair<int, double>> energy = xxx[1];

                List<KeyValuePair<int, double>> delay = xxx[2];

                cols_hops_ditrubtions.DataContext = hops;
                cols_energy_distribution.DataContext = energy;

                cols_delay_distribution.DataContext = delay;
            }
        }

        public void ClearExperment()
        {
            try
            {
                top_menu.IsEnabled = true;
                PublicParamerters.SimulationTime = 0;
                Canvas_SensingFeild.Children.Clear();
                if (myNetWork != null)
                    myNetWork.Clear();

                isCoverageSelected = false;


                HideSimulationParameters();
                isOpendtab_location = false;
                col_Path_Efficiency.DataContext = null;
                col_Delay.DataContext = null;
                col_EnergyConsumptionForEachNode.DataContext = null;


                cols_hops_ditrubtions.DataContext = null;
                lbl_hops_dis_network_info.Content = "";
                lbl_hops_dis_network_info.Content = "";
                cols_hops_ditrubtions.DataContext = null;
                cols_energy_distribution.DataContext = null;
                cols_delay_distribution.DataContext = null;

                PublicParamerters.NumberofGeneratedPackets = 0;
                PublicParamerters.IsNetworkDied = false;
                PublicParamerters.Density = 0;
                PublicParamerters.NetworkName = "";
                PublicParamerters.DeadNodeList.Clear();
                PublicParamerters.NOP = 0;
                PublicParamerters.NOS = 0;
                PublicParamerters.Rounds = 0;
                PublicParamerters.NumberofGeneratedPackets = 0;
                PublicParamerters.SinkNode = null;

                

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }


        private void ben_clear_click(object sender, RoutedEventArgs e)
        {
            TimerCounter.Stop();
            RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(0);
            RandomSelectSourceNodesTimer.Stop();

            Settings.Default.IsIntialized = false;

            ClearExperment();

        }

        bool isOpendtab_location = false;

        public object NetworkLifeTime { get; private set; }

        private void tab_network_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((tab_network.SelectedItem as TabItem).Name == "tab_location")
            {
                if (isOpendtab_location == false)
                {
                    if (isCoverageSelected)
                    {
                        col_network.DataContext = NodesLocationsScatter.BuildNodesLocationsScatterNodeChart(myNetWork);
                        col_Neighbors.DataContext = NodesLocationsScatter.GetNieborsDist(myNetWork);
                        isOpendtab_location = true;
                    }
                }
            }

        }

        private void lbl_show_grid_line_x_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (col_network_X_Gird.ShowGridLines == false) col_network_X_Gird.ShowGridLines = true;
            else col_network_X_Gird.ShowGridLines = false;
        }

        private void lbl_show_grid_line_y_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (col_network_Y_Gird.ShowGridLines == false) col_network_Y_Gird.ShowGridLines = true;
            else col_network_Y_Gird.ShowGridLines = false;
        }



        private void setDisributaions_Click(object sender, RoutedEventArgs e)
        {
            if (myNetWork.Count > 0)
            {
                if (isCoverageSelected)
                {
                    UIPowers cc = new ui.UIPowers(this);
                    cc.Show();
                }
                else
                {
                    MessageBox.Show("plz select coverage. Coverage->Random");
                }
            }
            else
            {
                MessageBox.Show("please select a network: File>importe");
            }
        }


        private void _set_paramertes_Click(object sender, RoutedEventArgs e)
        {
            /*
            ben_clear_click(sender, e);

            UiMultipleExperments setpa = new UiMultipleExperments(this);
            this.WindowState = WindowState.Minimized;
            setpa.Show();*/

        }



        private void btn_chek_lifetime_Click(object sender, RoutedEventArgs e)
        {
            if (isCoverageSelected)
            {
                this.WindowState = WindowState.Minimized;
                for (int i = 0; ; i++)
                {
                    rounds++;
                    lbl_rounds.Content = rounds;
                    if (!PublicParamerters.IsNetworkDied)
                    {
                        foreach (Sensor sen in myNetWork)
                        {
                            if (sen.ID != PublicParamerters.SinkNode.ID)
                            {
                                sen.GeneratePacketAndSent();
                              //  sen.GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt,
                              //  Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Please selete the coverage. Coverage->Random");
            }
        }

        private void btn_lifetime_s1_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.IsIntialized == false)
            {
                RandomDeplayment(0);
                UiComputeLifeTime lifewin = new UiComputeLifeTime(this);
                lifewin.Show();
                lifewin.Owner = this;
                top_menu.IsEnabled = false;
                Settings.Default.IsIntialized = true;
            }
            else
            {
                MessageBox.Show("File->clear and try agian.");
            }

           
        }


        

        /// <summary>
        /// _Randomly Select Nodes With Distance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnCon_RandomlySelectNodesWithDistance_Click(object sender, RoutedEventArgs e)
        {
            if (isCoverageSelected)
            {
                if (PublicParamerters.SinkNode.PacketsList.Count == 0)
                {
                    ui.UiSelectNodesWidthDistance win = new UiSelectNodesWidthDistance(this);
                    win.Show();
                }
                else
                {
                    MessageBox.Show("Please clear first: File->Clear!");
                }
            }
            else
            {
                MessageBox.Show("Please selected the Coverage.Coverage->Random");
            }

        }

        public void SendPackectPerSecond(double s)
        {
            RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(s);
            RandomSelectSourceNodesTimer.Start();
            PacketRate = "1 packet per " + s + " s";
        }

        double xPackets = 0;
        public void GenerateUplinkPacketsRandomly(double numofPackets, double packetRate)
        {
            xPackets = 0;
            PublicParamerters.NumberofGeneratedPackets = 0;

            xPackets = numofPackets;
            RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(packetRate);
            RandomSelectSourceNodesTimer.Tick += RandomSelectSourceNodesTimer_Tick;
            RandomSelectSourceNodesTimer.Start();
            
        }

        private void RandomSelectSourceNodesTimer_Tick(object sender, EventArgs e)
        {
            if (PublicParamerters.NumberofGeneratedPackets < xPackets)
            {
                int index = 1 + Convert.ToInt16(UnformRandomNumberGenerator.GetUniform(PublicParamerters.NumberofNodes - 2));
                myNetWork[index].GeneratePacketAndSent();

                Console.WriteLine("Target" + xPackets + ". Generated:" + PublicParamerters.NumberofGeneratedPackets.ToString());
            }
            else
            {
                RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(0);
                RandomSelectSourceNodesTimer.Stop();
                xPackets = 0;
                Console.WriteLine("Finised");
            }
        }



        // rounds:------------------
        public void GeneratePacketsForRounds()
        {
            PublicParamerters.Rounds = 0;
            RandomSelectSourceNodesTimer.Interval = TimeSpan.FromSeconds(10);
            RandomSelectSourceNodesTimer.Tick += RoundsGenerateTimer;
            RandomSelectSourceNodesTimer.Start();
            
        }

        private void RoundsGenerateTimer(object sender, EventArgs e) 
        {
            if (!PublicParamerters.IsNetworkDied)
            {
                PublicParamerters.Rounds++;
                foreach (Sensor sen in myNetWork)
                    sen.GeneratePacketAndSent();
                Console.WriteLine("Round:" + PublicParamerters.Rounds);
                Dispatcher.Invoke(() => lbl_rounds.Content = PublicParamerters.Rounds);
            }
            else
            {
                Console.WriteLine("network is dead");
            }
        }

        //-------


        private void btn_select_sources_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            if (Settings.Default.IsIntialized)
            {
                switch (Header)
                {

                    case "1pck/0.1s":
                        SendPackectPerSecond(0.1);
                        break;

                    case "1pck/1s":
                        SendPackectPerSecond(1);
                        break;
                    case "1pck/2s":
                        SendPackectPerSecond(2);
                        break;
                    case "1pck/4s":
                        SendPackectPerSecond(4);
                        break;
                    case "1pck/6s":
                        SendPackectPerSecond(6);
                        break;
                    case "1pck/8s":
                        SendPackectPerSecond(8);
                        break;
                    case "1pck/10s":
                        SendPackectPerSecond(10);
                        break;
                    case "1pck/0s(Stop)":
                        SendPackectPerSecond(0);
                        break;
                }
            }
            else
            {
                MessageBox.Show("Please select Coverage->Random. then continue.");
            }
        }

        private void btn_simTime_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            if (Settings.Default.IsIntialized)
            {
                stopSimlationWhen = Convert.ToInt32(Header.ToString());
                menSimuTim.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Please select Coverage->Random. then continue.");
            }
        }

        private void Btn_comuputeEnergyCon_withinTime_Click(object sender, RoutedEventArgs e)
        {

            if (Settings.Default.IsIntialized)
            {
                MessageBox.Show("File->clear and try agian.");
            }
            else
            {
                PacketRate = "";
                stopSimlationWhen = 0;
                UISetParEnerConsum con = new UISetParEnerConsum(this);
                con.Owner = this;
                con.Show();
                top_menu.IsEnabled = false;
            }
        }
    }
}



           
