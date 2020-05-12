using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LORA.Computations
{
   public class BatteryLevelColoring
    {
        public static string
               col90_100 = "#FF61F01F",
               col80_89 = "#FF1FF0D3",
               col70_79 = "#FF741FF0",
               col60_69 = "#FF1F9AF0",
               col50_59 = "#FFFF0197",
               col40_49 = "#FFB67DCB",
               col30_39 = "#FFF0E50E",
               col20_29 = "#FFF0A80E",
               col10_19 = "#FFF0740E",
               col1_9 = "#FFF02C0E",
               col0 = "#FF1D1C1C";
    }
     
    /// <summary>
    /// colors 
    /// </summary>
    public class NodeStateColoring
    {
        public static SolidColorBrush ActiveColor = Brushes.Coral;
        public static SolidColorBrush SleepColor = Brushes.SeaGreen;
        public static SolidColorBrush IntializeColor = Brushes.LightBlue; 
    }
}
