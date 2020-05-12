using LORA.Modules;
using LORA.ui;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace LORA.db
{
    /// <summary>
    /// Interaction logic for NetwokImport.xaml
    /// </summary>
    public partial class NetwokImport : UserControl
    {
        public MainWindow MainWindow { set; get; }
        public List<ImportedSensor> ImportedSensorSensors = new List<ImportedSensor>();

        public UiImportTopology UiImportTopology { get; set; }
        public NetwokImport()
        {
            InitializeComponent();
        }

        private void brn_import_Click(object sender, RoutedEventArgs e)
        {
            NetworkTopolgy.ImportNetwok(this);
            Parameters.PublicParamerters.NetworkName = lbl_network_name.Content.ToString();
            Parameters.PublicParamerters.SensingRangeRadius = ImportedSensorSensors[0].R;
            // now add them to feild.

            foreach (ImportedSensor imsensor in ImportedSensorSensors)
            {
                Sensor node = new Sensor(imsensor.NodeID);
                node.MainWindow = MainWindow;
                Point p = new Point(imsensor.Pox, imsensor.Poy);
                node.Position = p;
                node.VisualizedRadius = imsensor.R;
                MainWindow.myNetWork.Add(node);
                MainWindow.Canvas_SensingFeild.Children.Add(node);
               
            }
           

            try
            {
                UiImportTopology.Close();
            }
            catch
            {

            }
            

           

        }
    }
}
