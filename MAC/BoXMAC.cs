using LORA.Computations;
using LORA.Datatypes;
using LORA.Forwarding;
using LORA.Modules;
using LORA.Parameters;
using System;
using System.Windows.Shapes;
using System.Windows.Threading;
using static LORA.Parameters.PublicParamerters;
using System.Windows.Media;

namespace LORA.MAC
{

    /// <summary>
    /// implementation of BoxMAC.
    /// Ammar Hawbani.
    /// </summary>
    public class BoXMAC: Shape
    {
        /// <summary>
        /// in sec.
        /// </summary>
      

        private Sensor Node; // the node which will builin with this MAC protocl

        // this timer to swich on the sensor, when to start. after swiching on this sensor, this timer will be stoped.
        private DispatcherTimer SwichOnTimer = new DispatcherTimer();// ashncrous swicher.

        // the timer to swich between the sleep and active states.
        private DispatcherTimer ActiveSleepTimer = new DispatcherTimer();

       
        private int ActiveCounter = 0;
        private int SleepCounter = 0;

        protected override Geometry DefiningGeometry
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// intilize the MAC
        /// </summary>
        /// <param name="_Node"></param>
        public BoXMAC(Sensor _Node)
        {
            Node = _Node;
            if (Node != null)
            {
                if (Node.ID != PublicParamerters.SinkNode.ID)
                {
                    double xpasn = 1 + UnformRandomNumberGenerator.GetUniformSleepSec(MacStartUp);
                    // the swich on timer.
                    SwichOnTimer.Interval = TimeSpan.FromSeconds(xpasn);
                    SwichOnTimer.Start();
                    SwichOnTimer.Tick += ASwichOnTimer_Tick;
                    ActiveCounter = 0;
                    // active/sleep timer:
                    ActiveSleepTimer.Interval =TimeSpan.FromSeconds(1);// the intervale is one second.
                    ActiveSleepTimer.Tick += ActiveSleepTimer_Tick; ;
                    SleepCounter = 0;

                    // intialized:
                    Node.CurrentSensorState = SensorStatus.intalized;
                    Node.Ellipse_MAC.Fill = NodeStateColoring.IntializeColor;
                }
            }
            else
            {
                // the
                PublicParamerters.SinkNode.CurrentSensorState = SensorStatus.Active;
            }
        }


        private void ActiveSleepTimer_Tick(object sender, EventArgs e)
        {
            if (Node.CurrentSensorState == SensorStatus.Active)
            {
                ActiveCounter = ActiveCounter + 1;
                if (ActiveCounter == 1)
                {
                    Action x = () => Node.Ellipse_MAC.Fill = NodeStateColoring.ActiveColor;
                    Dispatcher.Invoke(x);

                  //  Node.DutyCycleString.Add(PublicParamerters.SimulationTime);
                }
                else if (ActiveCounter >= Periods.ActivePeriod)
                {
                    ActiveCounter = 0;
                    SleepCounter = 0;
                    Node.CurrentSensorState = SensorStatus.Sleep;
                }
            }
            else if (Node.CurrentSensorState == SensorStatus.Sleep)
            {
                SleepCounter = SleepCounter + 1;
                if (SleepCounter == 1)
                {
                    Action x = () => Node.Ellipse_MAC.Fill = NodeStateColoring.SleepColor;
                    Dispatcher.Invoke(x);
                 //   Node.DutyCycleString.Add(PublicParamerters.SimulationTime);
                }
                else if (SleepCounter >= Periods.SleepPeriod)
                {
                    ActiveCounter = 0;
                    SleepCounter = 0;
                    Node.CurrentSensorState = SensorStatus.Active;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SwichToActive()
        {
            if (Node.ID != PublicParamerters.SinkNode.ID)
            {
                if (Node.CurrentSensorState ==  SensorStatus.Sleep)
                {
                    Dispatcher.Invoke(() => Node.CurrentSensorState =  SensorStatus.Active, DispatcherPriority.Send);
                    Dispatcher.Invoke(() => SleepCounter = 0, DispatcherPriority.Send);
                    Dispatcher.Invoke(() => ActiveCounter = 0, DispatcherPriority.Send);
                }
                else
                {
                    ActiveCounter = 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SwichToSleep()
        {
            if (Node.ID != PublicParamerters.SinkNode.ID)
            {
                if (Node.CurrentSensorState ==  SensorStatus.Active)
                {
                    Dispatcher.Invoke(() => Node.CurrentSensorState =  SensorStatus.Sleep, DispatcherPriority.Send);
                    Dispatcher.Invoke(() => SleepCounter = 0, DispatcherPriority.Send);
                    Dispatcher.Invoke(() => ActiveCounter = 0, DispatcherPriority.Send); ;
                }
                else
                {
                    SleepCounter = 0;
                }
            }
        }

        /// <summary>
        /// run the timer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ASwichOnTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => ActiveSleepTimer.Start(), DispatcherPriority.Send);
            Dispatcher.Invoke(() => Node.CurrentSensorState = SensorStatus.Active, DispatcherPriority.Send);
            Dispatcher.Invoke(() => Node.Ellipse_MAC.Fill = NodeStateColoring.ActiveColor, DispatcherPriority.Send);
            Dispatcher.Invoke(() => SwichOnTimer.Interval = TimeSpan.FromSeconds(0), DispatcherPriority.Send);
            Dispatcher.Invoke(() => SwichOnTimer.Stop(), DispatcherPriority.Send);// stop me
        }


    }
}
