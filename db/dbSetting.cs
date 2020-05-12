using LORA.ui;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Windows;

namespace LORA.db
{
  
	public class DbSetting  
	{

        public static string DATABASENAME = "db";
        public static string DATABASEPASSWORD = "_12_LG1705504004";
        public static string ConnectionString = ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + DATABASENAME + ".accdb;Jet OLEDB:Database Password=" + DATABASEPASSWORD + ";";

	}

    /// <summary>
    ///  NOT PUBLIC
    /// </summary>
    class ConnectDb
    {
        public static string Connect(string DATABASENAME, string DATABASEPASSWORD)
        {
            string ConnectionString = ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + DATABASENAME + ".accdb;Jet OLEDB:Database Password=" + DATABASEPASSWORD + ";";
            return ConnectionString;
        }
    }



    public static class DbOperations
    {

        /// <summary>
        /// save the data
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="DATABASENAME"></param>
        /// <param name="DATABASEPASSWORD"></param>
        /// <param name="ListData"></param>
        /// <returns></returns>
        public static bool SaveData(string TableName, string DATABASENAME, string DATABASEPASSWORD, List<ExpermentalData> ExpList)
        {
            string conStr = ConnectDb.Connect(DATABASENAME, DATABASEPASSWORD);
            int re = 0;
            OleDbConnection connection = null;
            OleDbCommand command = null;
            try
            {
                connection = new OleDbConnection();
                connection.ConnectionString = conStr;
                connection.Open();
                // save:
                foreach (ExpermentalData exp in ExpList)
                {
                    string[] cols = new string[]
                           {
                        "[ExpID]", //1
                        "[EnergyConsumed]",//2
                        "[NumberOfHops]",//3
                        "[TransmissionDelay]",//4
                        "[RoutingDistance]",//5
                        "[TransmissionDistance]",//6
                        "[RoutingDistanceEfficiency]",//7
                        "[TransmissionDistanceEfficiency]",//8
                        "[RoutingEfficiency]",//9
                        "[RoutingBalancingEfficiency]",//10
                        "[PathsSpread]",
                        "[SourceID]",
                        "[NumberOfPackets]",
                        "[ParametersSet]",
                        "[NetName]",
                        "[ZoneWidth]"
                           };
                    string[] vals = new string[]
                    {
                        exp.ExpID.ToString(), //1
                        exp.EnergyConsumed.ToString(),//2
                        exp.NumberOfHops.ToString(),//3
                        exp.TransmissionDelay.ToString(),//4
                        exp.RoutingDistance.ToString(),//5
                        exp.TransmissionDistance.ToString(),//6
                        exp.RoutingDistanceEfficiency.ToString(),//7
                        exp.TransmissionDistanceEfficiency.ToString(),//8
                        exp.RoutingEfficiency.ToString(),//9
                        exp.RoutingBalancingEfficiency.ToString(),//10
                        exp.PathsSpread.ToString(),//11
                        exp.SourceID.ToString(),
                        exp.NumberOfPackets.ToString(),
                        exp.ParametersSet.ToString(),
                        exp.NetName.ToString(),
                        exp.ZoneWidth.ToString()
                    };
                    string sql = SqlOperations.InsertInTo.Insert(TableName, cols, vals);
                    command = new OleDbCommand();
                    command.Connection = connection;
                    command.CommandText = sql;
                    re += command.ExecuteNonQuery();
                }
            }
            catch (Exception expen) { MessageBox.Show(expen.Message); }
            finally
            {
                if (connection != null) { connection.Close(); connection.Dispose(); }
                if (command != null) { command.Dispose(); }
            }



            if(re== ExpList.Count)
            {
                MessageBox.Show("The Expermental Results are saved");
                return true;
            }
            else
            {
                MessageBox.Show("Error ONLY" + re.ToString() + " Record/s Are Saved.");
                return false;
            }
        }

        /// <summary>
        /// RETURN FALSE is the table is created. createn true if the table is not created.
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="DATABASENAME"></param>
        /// <param name="DATABASEPASSWORD"></param>
        /// <returns></returns>
        public static bool CreateTable(string TableName,string DATABASENAME,string DATABASEPASSWORD)
        {
            bool re = false;
            string ConStr = ConnectDb.Connect(DATABASENAME, DATABASEPASSWORD);
            OleDbConnection con = new OleDbConnection(ConStr);
            con.Open();
            try
            {
                DataTable dataTable = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                    new object[] { null, null, TableName, "TABLE" });
                if (dataTable.Rows.Count > 0)
                {
                    re = true;// true. the table is created BEFORE!
                }
                else
                {
                    /*
                      public string NetName { get; set; }
        public int SourceID { get; set; }
        public int NumberOfPackets { get; set; } // per experment.
        public string ParametersSet { get; set; } // en,dis,dir,per*/

                                                  // Dim cmd As New OleDb.OleDbCommand(, con)
        OleDbCommand cmd = new OleDbCommand("CREATE TABLE [" + TableName + "] " +
                        "([ExpID] Text(120) PRIMARY KEY," +
                        "[ParametersSet] Text(120)," +
                        "[NetName] Text(120)," +
                        "[ZoneWidth] Text(120)," +
                        "[EnergyConsumed] Text(120)," +
                        "[NumberOfHops] Text(120)," +
                        "[TransmissionDelay] Text(120)," +
                        "[RoutingDistance] Text(120)," +
                        "[TransmissionDistance] Text(120)," +
                        "[RoutingDistanceEfficiency] Text(120)," +
                        "[TransmissionDistanceEfficiency] Text(120)," +
                        "[RoutingEfficiency] Text(120)," +
                        "[RoutingBalancingEfficiency] Text(120)," +
                        "[PathsSpread] Text(120)," +
                        "[SourceID] Text(120)," +
                        "[NumberOfPackets] Text(120)" +
                        ")", con);
                    if (cmd.ExecuteNonQuery() > 0)
                        re = true;
                    con.Close();
                }
            }
            catch { con.Close(); }
            return re;
        }
    


        
    
    }

}
