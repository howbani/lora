using LORA.Computations;
using LORA.Energy;
using LORA.Logs;
using LORA.Forwarding;
using LORA.DataPacket;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LORA.Parameters;
using LORA.ui;
using LORA.Properties;
using LORA.Datatypes;
using LORA.MAC;
using System.Windows.Threading;

namespace LORA.Modules
{

    /// <summary>
    /// Interaction logic for Node.xaml
    /// </summary>
    public partial class Sensor : UserControl
    {
        private DispatcherTimer DeliveerPacketsInQueuTimer = new DispatcherTimer();// ashncrous swicher. 
        public MainWindow MainWindow { get; set; }

        public static double SR { get; set; } // the radios of SENSING range.
        public double SensingRangeRadius { get { return SR;  } }
        public static double CR { get; set; }  // the radios of COMUNICATION range. double OF SENSING RANGE
        public double ComunicationRangeRadius { get { return CR;  } }
        public double BatteryIntialEnergy; // jouls // value will not be changed
        private double _ResidualEnergy; //// jouls this value will be changed according to useage of battery

        public List<Datapacket> WaitingList = new List<Datapacket>();
        // 
        public double ResidualEnergy // jouls this value will be changed according to useage of battery
        {
           get { return _ResidualEnergy; }
          set
            {
                _ResidualEnergy = value;
                Prog_batteryCapacityNotation.Value = _ResidualEnergy;
            }
        } //@unit(JOULS);


       // public List<DataPacket.Datapacket> LoopedPackets = new List<Datapacket>(); // THE SINK WILL USE THIS ONE.
        public List<int> DutyCycleString = new List<int>(); // return the first letter of each state.
        public BoXMAC Mac { get; set; } // the mac protocol for the node.
        public SensorStatus CurrentSensorState { get; set; } // state of node.
       
        // the nodes are sordted acording to
        public List<Datapacket> PacketsList = new List<Datapacket>();// for source nodes, the generated. for the sink is the packets that recived.
        public List<SensorRoutingLog> Logs = new List<SensorRoutingLog>();
        // public List<RelativityRow> RelativityRows = new List<RelativityRow>(); // add them:

        // for intilized:
        public List<RoutingMetric> MyForwardersLongList = new List<RoutingMetric>();// all candidcats
        // forwarders: the forwarder will be selected from this list.
        public List<RoutingMetric> MyForwardersShortedList = new List<RoutingMetric>(); // shortedforwarders, the next hop is selected from this list.
        public List<Sensor> NeighboreNodes { get; set; } // my neighbors.
       
        public FirstOrderRadioModel EnergyModel = new FirstOrderRadioModel(); 
        public double RoutingDataLength; // @ UNIT bit. ==1KB
        public List<BatRange> BatRangesList = new List<Energy.BatRange>();



        public int NumberofPacketsGeneratedByMe = 0;

        /// <summary>
        /// 
        /// </summary>
        public void SwichToActive()
        {
            Mac.SwichToActive();

        }

        /// <summary>
        /// 
        /// </summary>
        private void SwichToSleep()
        {
            Mac.SwichToSleep();
        }

        public int ID { get; set; }
        public Sensor(int nodeID)
        {
            InitializeComponent();
            RoutingDataLength = PublicParamerters.RoutingDataLength;
            BatteryIntialEnergy = PublicParamerters.BatteryIntialEnergy; // the value will not be change
            
            ResidualEnergy = BatteryIntialEnergy;// joules. intializing.
            Prog_batteryCapacityNotation.Value = BatteryIntialEnergy;
            Prog_batteryCapacityNotation.Maximum = BatteryIntialEnergy;
            lbl_Sensing_ID.Content = nodeID;
            ID = nodeID;
            DeliveerPacketsInQueuTimer.Interval = PublicParamerters.SenderWaitingTime;
            DeliveerPacketsInQueuTimer.Tick += DeliveerPacketsInQueuTimer_Tick;
            //:
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Prog_batteryCapacityNotation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           
            double val = ResidualEnergyPercentage;
            if (val <=0)
            {
                /*
                // dead certificate:
                ExpermentsResults.Lifetime.DeadNodesRecord recod = new ExpermentsResults.Lifetime.DeadNodesRecord();
                recod.DeadAfterPackets = PublicParamerters.NumberofGeneratedPackets;
                recod.DeadOrder = PublicParamerters.DeadNodeList.Count + 1;
                recod.Rounds = PublicParamerters.Rounds + 1;
                recod.DeadNodeID = ID;
                recod.NOS = PublicParamerters.NOS;
                recod.NOP = PublicParamerters.NOP;
                recod.RoutingZone = Settings.Default.ZoneWidth;
                PublicParamerters.DeadNodeList.Add(recod);*/
                PublicParamerters.IsNetworkDied = true;

                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col0));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col0));
            }
            if (val >= 1 && val <= 9)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col1_9));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col1_9));
            }

            if (val >= 10 && val <= 19)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col10_19));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col10_19));
            }

            if (val >= 20 && val <= 29)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col20_29));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col20_29));
            }

            // full:
            if (val >= 30 && val <= 39)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col30_39));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col30_39));
            }
            // full:
            if (val >= 40 && val <= 49)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col40_49));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col40_49));
            }
            // full:
            if (val >= 50 && val <= 59)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col50_59));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col50_59));
            }
            // full:
            if (val >= 60 && val <= 69)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col60_69));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col60_69));
            }
            // full:
            if (val >= 70 && val <= 79)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col70_79));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col70_79));
            }
            // full:
            if (val >= 80 && val <= 89)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col80_89));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col80_89));
            }
            // full:
            if (val >= 90 && val <= 100)
            {
                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col90_100));
                Ellipse_center.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col90_100));
            }




            // update the battery distrubtion.
            int per = Convert.ToInt16(val);
            if (per > PublicParamerters.BatteryLosePerUpdate)
            {
                int indx = per / PublicParamerters.BatteryLosePerUpdate;
                if (indx >= 1)
                {
                    if (BatRangesList.Count > 0)
                    {
                        BatRange x = BatRangesList[indx - 1];
                        if (per >= x.Rang[0] && per <= x.Rang[1])
                        {
                            if (x.isUpdated == false)
                            {
                                x.isUpdated = true;
                                MainWindow.RandomSelectSourceNodesTimer.Stop();
                                ForwardersSelection select = new ForwardersSelection();
                                select.UpdateMyForwardersAccordingtoBattryPower(this);
                                MainWindow.RandomSelectSourceNodesTimer.Start();
                            }
                        }
                    }
                }
            }
        }

        public double ResidualEnergyPercentage
        {
            get { return (ResidualEnergy / BatteryIntialEnergy) * 100; }
        }
        /// <summary>
        /// visualized sensing range and comuinication range
        /// </summary>
        public double VisualizedRadius
        {
            get { return Ellipse_Sensing_range.Width/2; }
            set
            {
                // sensing range:
                Ellipse_Sensing_range.Height = value * 2; // heigh= sen rad*2;
                Ellipse_Sensing_range.Width = value * 2; // Width= sen rad*2;
                SR = VisualizedRadius;
                CR = SR * 2; // comunication rad= sensing rad *2;

                // device:
                Device_Sensor.Width = value * 4; // device = sen rad*4;
                Device_Sensor.Height = value * 4;
                // communication range
                Ellipse_Communication_range.Height= value * 4; // com rang= sen rad *4;
                Ellipse_Communication_range.Width = value * 4;

                // battery:
                Prog_batteryCapacityNotation.Width = 8;
                Prog_batteryCapacityNotation.Height = 2;
            }
        }

        /// <summary>
        /// Real postion of object.
        /// </summary>
        public Point Position
        {
            get
            {
                double x = Device_Sensor.Margin.Left;
                double y = Device_Sensor.Margin.Top;
                Point p = new Point(x, y);
                return p;
            }
            set
            {
                Point p = value;
                Device_Sensor.Margin = new Thickness(p.X, p.Y, 0, 0);
            }
        }

        /// <summary>
        /// center location of node.
        /// </summary>
        public Point CenterLocation 
        {
            get
            {
                double x = Device_Sensor.Margin.Left;
                double y = Device_Sensor.Margin.Top;
                Point p = new Point(x + CR, y + CR);
                return p;
            }
        }

       

        /***********************END ZONE***********************/


        bool StartMove = false;
        private void Device_Sensor_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point P = e.GetPosition(MainWindow.Canvas_SensingFeild);
                P.X = P.X - CR;
                P.Y = P.Y - CR;
                this.Position = P;
                StartMove = true;
            }
        }

        private void Device_Sensor_MouseMove(object sender, MouseEventArgs e)
        {
            if (StartMove)
            {
                System.Windows.Point P = e.GetPosition(MainWindow.Canvas_SensingFeild);
                P.X = P.X - CR;
                P.Y = P.Y - CR;
                this.Position = P;
            }
        }

        private void Device_Sensor_MouseUp(object sender, MouseButtonEventArgs e)
        {
            StartMove = false;
        }

        //////////////////////////////Zone//////////////////////////
        /// <summary>
        /// The zone is defined after selecting the the sink.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public List<Point> MyRoutingZone
        {
            get
            {
                if (PublicParamerters.SinkNode != null)
                {
                    if (this != null)
                    {
                        // no need to calculate for sink.
                        if (ID != PublicParamerters.SinkNode.ID)
                        {
                            Point ps, pb;
                            ps = CenterLocation;// the source Location
                            pb = PublicParamerters.SinkNode.CenterLocation;// sink location
                            List<Point> reL = new List<Point>();

                            double xs = ps.X;
                            double xb = pb.X;
                            double ys = ps.Y;
                            double yb = pb.Y;
                            double Halfw = Settings.Default.ZoneWidth / 2;
                           
                            double DistanceBSS = Operations.DistanceBetweenTwoPoints(ps, pb);
                            double DelateY = Halfw * ((xb - xs) / DistanceBSS);
                            double DeltaX = Halfw * ((ys - yb) / DistanceBSS);

                            Point p3 = new Point(xs + DeltaX, ys + DelateY);
                            Point p4 = new Point(xs - DeltaX, ys - DelateY);

                            Point p5 = new Point(xb + DeltaX, yb + DelateY);
                            Point p6 = new Point(xb - DeltaX, yb - DelateY);

                            reL.Add(p3);
                            reL.Add(p4);
                            reL.Add(p5);
                            reL.Add(p6);

                            return reL;
                        }
                        else return null;
                    }
                    else return null;
                }
                else return null;
            }
        }



        public bool isZoneDrawn = false;
        public List<MyLine> MyZoneLines = new List<MyLine>();
        public void DrawMyZone()
        {
            if (isZoneDrawn ==false)
            {
                List<Point> zone = MyRoutingZone;
                if (zone != null)
                {
                    if (zone.Count == 4)
                    {
                        Point p3 = zone[0];
                        Point p4 = zone[1];
                        Point p5 = zone[2];
                        Point p6 = zone[3];

                        MyLine l34 = new MyLine(p3, p4, MainWindow.Canvas_SensingFeild);
                        MyLine l56 = new MyLine(p5, p6, MainWindow.Canvas_SensingFeild);
                        MyLine l35 = new MyLine(p3, p5, MainWindow.Canvas_SensingFeild);
                        MyLine l46 = new MyLine(p4, p6, MainWindow.Canvas_SensingFeild);

                        isZoneDrawn = true;
                        MyZoneLines.Add(l34);
                        MyZoneLines.Add(l56);
                        MyZoneLines.Add(l35);
                        MyZoneLines.Add(l46);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            UndrawMyZone();
            Ellipse_Communication_range.Visibility = Visibility.Hidden;
        }

        public void UndrawMyZone()
        {
            if (isZoneDrawn == true)
            {
                if (MyZoneLines.Count > 0)
                {
                    foreach (MyLine myl in MyZoneLines)
                    {
                        MainWindow.Canvas_SensingFeild.Children.Remove(myl.GetMyPath());
                    }
                    MyZoneLines.Clear();
                    isZoneDrawn = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
          DrawMyZone();
          Ellipse_Communication_range.Visibility = Visibility.Visible;
        }

        

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
        }

        public void Dispose()
        {
            PacketsList.Clear();
            Logs.Clear();
        }

        public int ComputeMaxHops
        {
            get
            {
                double  DIS= Operations.DistanceBetweenTwoSensors(PublicParamerters.SinkNode, this);
                return Convert.ToInt16(Math.Ceiling((Math.Sqrt(PublicParamerters.Density) * (DIS / ComunicationRangeRadius))));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void GeneratePacketAndSent()
        {

            List<RoutingMetric> Forwarders = MyForwardersShortedList;
            if (Forwarders != null)
            {
                ForwardersCoordination coordDinator = new ForwardersCoordination();
                RoutingMetric nextHop = coordDinator.SelectedForwarder(this, this);
                if (nextHop != null)
                {
                    NumberofPacketsGeneratedByMe++;
                    PublicParamerters.NumberofGeneratedPackets++;// public
                    Datapacket datap = new Datapacket(this, nextHop.PotentialForwarder);
                    datap.SourceNode = this;
                    datap.SourceNodeID = ID;
                    datap.Path = this.ID.ToString();
                    datap.DistanceFromSourceToSink = Operations.DistanceBetweenTwoSensors(PublicParamerters.SinkNode, this);
                    datap.PacektSequentID = PublicParamerters.NumberofGeneratedPackets;
                    datap.MaxHops = ComputeMaxHops;
                    this.SwichToActive();
                    SendData(this, nextHop.PotentialForwarder, datap);// send the data:
                    this.lbl_Sensing_ID.Foreground = Brushes.Red;
                    
                    if (Settings.Default.KeepLogs)
                    {
                        // HistoricalRelativityRows.AddRange(Forwarders);
                    }
                    else
                    {
                        Dispose();
                    }
                }
                else
                {
                    // has no forwarders then let him try:
                    PublicParamerters.NumberofGeneratedPackets++;// public
                    NumberofPacketsGeneratedByMe++;
                    Datapacket datap = new Datapacket(); // packet with no sender.
                    datap.SourceNode = this;
                    datap.MaxHops = ComputeMaxHops;
                    datap.SourceNodeID = ID;
                    datap.Path = this.ID.ToString();
                    datap.DistanceFromSourceToSink = Operations.DistanceBetweenTwoSensors(PublicParamerters.SinkNode, this);
                    datap.PacektSequentID = PublicParamerters.NumberofGeneratedPackets;
                    SwichToActive();
                    this.lbl_Sensing_ID.Foreground = Brushes.Red;
                    RelayThePacket(this, datap);
                }
            }
            else
            {
                if (ID != PublicParamerters.SinkNode.ID)
                    MessageBox.Show("Node:" + ID + "Has No Forwarders");
            }
        }



        public void RelayThePacket(Sensor RelayNode, Datapacket datap)
        {
            List<RoutingMetric> GetMyForw = MyForwardersShortedList;
            ForwardersCoordination coordinator = new ForwardersCoordination();
            RoutingMetric nextHop = coordinator.SelectedForwarder(RelayNode, datap.SourceNode);

            if (nextHop != null) // not the sink
            {
                // no loop: 
                Datapacket forwardPacket;
                forwardPacket = new Datapacket(RelayNode, nextHop.PotentialForwarder);
                forwardPacket.MaxHops = datap.MaxHops;
                forwardPacket.PacketWaitingTimes = datap.PacketWaitingTimes;
                forwardPacket.SourceNodeID = datap.SourceNodeID;
                forwardPacket.DistanceFromSourceToSink = datap.DistanceFromSourceToSink;
                forwardPacket.RoutingDistance = datap.RoutingDistance;
                forwardPacket.Path = datap.Path;
                forwardPacket.Hops = datap.Hops;
                forwardPacket.UsedEnergy_Joule = datap.UsedEnergy_Joule;
                forwardPacket.Delay = datap.Delay;
                forwardPacket.SourceNode = datap.SourceNode;
                forwardPacket.PacektSequentID = datap.PacektSequentID;

                if (IsThisPacketInTheWaitingList(RelayNode, datap))
                {
                    WaitingList.Remove(datap);
                }
                RelayNode.SendData(RelayNode, nextHop.PotentialForwarder, forwardPacket);
            }
            else
            {
                // wait:
                // waiting time.
                // the either all the nodes are sleep
                if (GetMyForw.Count > 0)
                {
                    datap.PacketWaitingTimes = datap.PacketWaitingTimes + 1;
                    if (!IsThisPacketInTheWaitingList(RelayNode, datap)) { WaitingList.Add(datap); }
                    RelayNode.DeliveerPacketsInQueuTimer.Start();
                }
                else
                {
                   // MessageBox.Show("This Node has no forwarders!");
                }
            }
        }

        private void DeliveerPacketsInQueuTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                List<Datapacket> AllWaiting = new List<Datapacket>();
                AllWaiting.AddRange(WaitingList);
                List<Datapacket> shouldbeRemoved = new List<Datapacket>();

                // try 20 times to re-sent.
                foreach (Datapacket datap in AllWaiting)
                {
                    if (datap.PacketWaitingTimes <= 20)
                    {
                        RelayThePacket(this, datap);
                    }
                    else
                    {
                        // tried many times and not delived
                        shouldbeRemoved.Add(datap);
                        datap.IsDelivered = false;
                        PublicParamerters.SinkNode.PacketsList.Add(datap);
                    }
                }

                foreach (Datapacket pak in shouldbeRemoved)
                {
                    WaitingList.Remove(pak);
                    PublicParamerters.SinkNode.PacketsList.Add(pak);
                }

                shouldbeRemoved.Clear();

                if (WaitingList.Count == 0) { DeliveerPacketsInQueuTimer.Stop(); }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }


        }

        /// <summary>
        /// check if the packet is within the WaitingList of RelayNode.
        /// </summary>
        /// <param name="RelayNode"></param>
        /// <param name="datap"></param>
        /// <returns></returns>
        public bool IsThisPacketInTheWaitingList(Sensor RelayNode, Datapacket datap)
        {
            List<Datapacket> COPY = new List<Datapacket>();
            COPY.AddRange(RelayNode.WaitingList);
            foreach (Datapacket pack in COPY)
            {
                if (pack.PacektSequentID == datap.PacektSequentID) { return true; }
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="Reciver"></param>
        /// <param name="datap"></param>
        public void SendData(Sensor sender, Sensor Reciver, Datapacket datap)
        {
            if (Reciver != null || sender != null)
            {
                if (Reciver.ID != sender.ID)
                {
                    if (sender.ResidualEnergy > 0)
                    {
                        sender.SwichToActive();
                        Reciver.SwichToActive();

                        SensorRoutingLog log = new SensorRoutingLog();
                        log.IsSend = true;
                        log.NodeID = this.ID;
                        log.Operation = "To:" + Reciver.ID;
                        log.Time = DateTime.Now;
                        log.Distance_M = Operations.DistanceBetweenTwoSensors(sender, Reciver);
                        log.UsedEnergy_Nanojoule = EnergyModel.Transmit(RoutingDataLength, log.Distance_M);
                        //
                        // set the remain battery Energy:
                        double remainEnergy = ResidualEnergy - log.UsedEnergy_Joule;
                        sender.ResidualEnergy = remainEnergy;
                        log.RemaimBatteryEnergy_Joule = ResidualEnergy;
                        log.PID = datap.PacektSequentID;
                        //
                        // add the path:457430817
                        datap.UsedEnergy_Joule += log.UsedEnergy_Joule;
                        PublicParamerters.TotalEnergyConsumptionJoule += log.UsedEnergy_Joule;
                        //
                        if (Settings.Default.KeepLogs)
                        {
                            log.RelaySequence = datap.Hops + 1;
                            sender.Logs.Add(log); // keep logs for each node.
                        }
                        else
                        {
                            Dispose();
                        }

                        if(Settings.Default.DrawPacketsLines)
                        {
                            MainWindow.Canvas_SensingFeild.Children.Add(datap); // add the lines to the boad.
                        }

                        // ToSensor ReceiveData
                        Reciver.ReceiveData(sender, Reciver, datap);

                    }
                    else
                    {
                       
                        Ellipse_Sensing_range.Fill = Brushes.Brown; // die out node.   // MessageBox.Show("DeadNODE!");
                    }
                }
            }
            else
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reciver"></param>
        /// <param name="datap"></param>
        public void ReceiveData(Sensor sender, Sensor reciver, Datapacket datap)
        {
            if (reciver != null || sender != null)
            {
                if (reciver.ID != PublicParamerters.SinkNode.ID)
                {
                    if (reciver.ID != sender.ID)
                    {
                        if (reciver.ResidualEnergy > 0)
                        {
                            SensorRoutingLog log = new SensorRoutingLog();
                            log.IsReceive = true;
                            log.NodeID = this.ID;
                            log.Operation = "From:" + sender.ID;
                            log.Time = DateTime.Now;
                            log.Distance_M = Operations.DistanceBetweenTwoSensors(reciver, sender);
                            log.UsedEnergy_Nanojoule = EnergyModel.Receive(RoutingDataLength);
                            log.PID = datap.PacektSequentID;

                            // set the remain battery Energy:
                            if (reciver.ID != PublicParamerters.SinkNode.ID)
                            {
                                double remainEnergy = ResidualEnergy - log.UsedEnergy_Joule;
                                reciver.ResidualEnergy = remainEnergy;
                                log.RemaimBatteryEnergy_Joule = ResidualEnergy;
                                PublicParamerters.TotalEnergyConsumptionJoule += log.UsedEnergy_Joule;
                            }

                            // routing distance:
                            datap.Path += ">" + reciver.ID;
                            datap.RoutingDistance += log.Distance_M;
                            datap.Hops += 1;
                            datap.UsedEnergy_Joule += log.UsedEnergy_Joule;
                            datap.Delay += DelayModel.DelayModel.Delay(sender, reciver);
                          

                            if (Settings.Default.KeepLogs)
                            {
                                reciver.Logs.Add(log); // keep logs for each node.
                               
                            }
                            else
                            {
                                Dispose();
                            }

                            if (datap.Hops > datap.MaxHops)
                            {
                                datap.IsDelivered = false;
                                PublicParamerters.SinkNode.PacketsList.Add(datap);
                                PublicParamerters.NumberofDropedPacket += 1;
                            }
                            else
                            {
                                // relay:
                                RelayThePacket(reciver, datap);// relay the packet:
                            }
                        }
                        else
                        {
                            if (Settings.Default.StopeWhenFirstNodeDeid)
                            {
                                MainWindow.TimerCounter.Stop();
                                MainWindow.RandomSelectSourceNodesTimer.Stop();
                                MainWindow.stopSimlationWhen = PublicParamerters.SimulationTime;
                                /*ExpReport win = new ExpReport(MainWindow);
                                win.Show();*/
                                MainWindow.top_menu.IsEnabled = true;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error, The sender and reciver are the same!");
                    }
                }
                else
                {
                    // recive the packet by the sink:
                    SensorRoutingLog log = new SensorRoutingLog();
                    log.IsReceive = true;
                    log.NodeID = this.ID;
                    log.Operation = "From:" + sender.ID;
                    log.Time = DateTime.Now;
                    log.Distance_M = Operations.DistanceBetweenTwoSensors(reciver, sender);
                    log.UsedEnergy_Nanojoule = EnergyModel.Receive(RoutingDataLength);
                    log.PID = datap.PacektSequentID;
                    // set the remain battery Energy:
                    if (reciver.ID != PublicParamerters.SinkNode.ID)
                    {
                        double remainEnergy = ResidualEnergy - log.UsedEnergy_Joule;
                        reciver.ResidualEnergy = remainEnergy;
                        log.RemaimBatteryEnergy_Joule = ResidualEnergy;
                        PublicParamerters.TotalEnergyConsumptionJoule += log.UsedEnergy_Joule;
                    }

                    // routing distance:
                    datap.Path += ">" + reciver.ID;
                    datap.RoutingDistance += log.Distance_M;
                    datap.Hops += 1;
                    datap.UsedEnergy_Joule += log.UsedEnergy_Joule;
                    datap.Delay += DelayModel.DelayModel.Delay(sender, reciver);
                    datap.IsDelivered = true;
                  
                    if (Settings.Default.KeepLogs)
                    {
                        datap.IsDelivered = true;
                        PublicParamerters.NumberofDeliveredPacket += 1;
                        PublicParamerters.SinkNode.PacketsList.Add(datap);
                        PublicParamerters.RoutingDistanceEffeciency += datap.RoutingDistanceEfficiency;
                        PublicParamerters.TotalNumberOfHope += datap.Hops;
                        PublicParamerters.TransmissionDistanceEff += datap.TransDistanceEfficiency;
                        PublicParamerters.TotalWaitingTime += datap.PacketWaitingTimes;
                    }
                    else
                    {
                        PublicParamerters.NumberofDeliveredPacket += 1;
                        PublicParamerters.RoutingDistanceEffeciency += datap.RoutingDistanceEfficiency;
                        PublicParamerters.TotalNumberOfHope += datap.Hops;
                        PublicParamerters.TransmissionDistanceEff += datap.TransDistanceEfficiency;
                        PublicParamerters.TotalWaitingTime += datap.PacketWaitingTimes;
                        Dispose();
                    }
                }
            }
            else
            {
                MessageBox.Show("Error. Reciver or Sender is Null.");
            }
        }

        public void GeneratePacketAndSent(int numOfPackets)
        {
            for (int j = 1; j <= numOfPackets; j++)
            {

                GeneratePacketAndSent();
            }
        }

      
        /// <summary>
        ///  select this node as a source and let it 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btn_send_packet_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Label lbl_title = sender as Label;
            switch(lbl_title.Name)
            {
                case "btn_send_1_packet":
                    {
                        GeneratePacketAndSent();
                        // GeneratePacketAndSent(false,Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                        break;
                    }
                case "btn_send_10_packet":
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            GeneratePacketAndSent();
                            //GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                        }
                        break;
                    }

                case "btn_send_100_packet":
                    {
                        for (int j = 1; j <= 100; j++)
                        {
                            //  GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                            GeneratePacketAndSent();
                        }
                        break;
                    }

                case "btn_send_300_packet":
                    {
                        for (int j = 1; j <= 300; j++)
                        {
                            GeneratePacketAndSent();
                        }
                        break;
                    }

                case "btn_send_1000_packet":
                    {
                        for (int j = 1; j <= 1000; j++)
                        {
                            // GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                            GeneratePacketAndSent();
                        }
                        break;
                    }

                case "btn_send_5000_packet":
                    {
                        for (int j = 1; j <= 5000; j++)
                        {
                            GeneratePacketAndSent();
                           // GeneratePacketAndSent(false, Settings.Default.EnergyDistCnt, Settings.Default.TransDistanceDistCnt, Settings.Default.DirectionDistCnt, Settings.Default.PrepDistanceDistCnt);
                        }
                        break;
                    }

                   
            }
        }

       

        private void btn_show_relativyt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MyForwardersShortedList.Count > 0)
            {
                UiShowRelativityForAnode re = new ui.UiShowRelativityForAnode();
                re.dg_relative_shortlist.ItemsSource = MyForwardersShortedList;
                re.dg_relative_longlist.ItemsSource = MyForwardersLongList;
                re.Show();
            }
        }

        private void lbl_MouseEnter(object sender, MouseEventArgs e)
        {
            // battery tool tib
            //ToolTip = new Label() { Content = "ExNIZ:" + expectedNuminZone.ToString() };

            
        }

        private void btn_show_routing_log_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(Logs.Count>0)
            {
                UiShowRelativityForAnode re = new ui.UiShowRelativityForAnode();
                re.dg_relative_shortlist.ItemsSource = Logs;
                re.Show();
            }
        }

        private void btn_draw_random_numbers_MouseDown(object sender, MouseButtonEventArgs e)
        {
            List<KeyValuePair<int, double>> rands = new List<KeyValuePair<int, double>>();
            int index = 0;
            foreach (SensorRoutingLog log in Logs )
            {
                if(log.IsSend)
                {
                    index++;
                    rands.Add(new KeyValuePair<int, double>(index, log.ForwardingRandomNumber));
                }
            }
            UiRandomNumberGeneration wndsow = new ui.UiRandomNumberGeneration();
            wndsow.chart_x.DataContext = rands;
            wndsow.Show();
        }

        private void Ellipse_center_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Settings.Default.IsIntialized)
            {
                double zoneLen = Operations.DistanceBetweenTwoSensors(this, PublicParamerters.SinkNode);
                double expectedNuminZone = PublicParamerters.ExpectedNumberoNodesInZone(zoneLen);
                ToolTip = new Label() { Content = "ExNIZ:" + expectedNuminZone.ToString() };
            }
        }

        private void btn_show_my_duytcycling_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new UIOverlappedIntervals(this).Show();
        }
    }
}
