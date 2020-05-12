using LORA.db;
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
    /// Interaction logic for UiDataExport.xaml
    /// </summary>
    public partial class UiDataExport : Window
    {
        public List<ExpermentalData> ExpList { get; set; }
        public string NetName { get; set; }
        public string ParametersSet { get; set; } // en,dis,dir,per

        public UiDataExport()
        {
            InitializeComponent();
            dg_date.MaxHeight = SystemParameters.FullPrimaryScreenHeight - 10;
        }

        private void btn_expor_Click(object sender, RoutedEventArgs e)
        {
            string TableName = NetName + "x" + ParametersSet + "x" + DateTime.Now.Hour + "" + DateTime.Now.Minute + "" + DateTime.Now.Second + "" + DateTime.Now.Millisecond + "";

            string DbName = "ExpData";
            string Password= "";
            bool isTableCreated = DbOperations.CreateTable(TableName, DbName, Password);
            if(!isTableCreated)
            {
                DbOperations.SaveData(TableName, DbName, Password, ExpList);
            }
        }
    }
}
