using LORA.Modules;
using LORA.Forwarding;
using System.Collections.Generic;
using System.Data;

namespace LORA.DataPacket
{
    public class Matrix3D
    {
        public int PID { get; set; }
        public double[,] Matrix2D;
    }

     

    public class AdjecentMatrix
    {

        /// <summary>
        /// 2d.:Matrix3D
        /// </summary>
        /// <param name="martix"></param>
        /// <returns></returns>
        public List<Matrix3D> Convert2DMatrix(List<Sensor> N)
        {
            double[,,] martix = GetMatrix(N);
            List<Matrix3D> MatrixList = new List<DataPacket.Matrix3D>();
            for(int k=0;k< martix.GetLength(0); k++)
            {
                Matrix3D ma = new Matrix3D();
                ma.Matrix2D = new double[martix.GetLength(1), martix.GetLength(2)];
                ma.PID = k + 1; //
                for(int i=0;i< martix.GetLength(1);i++)
                {
                    for(int j=0;j< martix.GetLength(2);j++)
                    {
                        ma.Matrix2D[i, j] = martix[k, i, j];
                    }
                }
                MatrixList.Add(ma);
            }

            return MatrixList;
        }


        /// <summary>
        /// DataTable
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public List<DataTable> ConvertToTable(List<Sensor> N)
        {
            double[,,] martix = GetMatrix(N);
            List<DataTable> MatrixList = new List<DataTable>();
            for (int k = 0; k < martix.GetLength(0); k++)
            {
                DataTable dataTable = new DataTable();
                dataTable.TableName = "Path= " + (k + 1).ToString();
                for (int j = -1; j < martix.GetLength(1); j++)
                {
                    dataTable.Columns.Add(new DataColumn(j.ToString()));
                }

                for (int i = 0; i < martix.GetLength(1); i++)
                {
                    var newRow = dataTable.NewRow();
                    for (int j = -1; j < martix.GetLength(2); j++)
                    {
                        if (j == -1)
                        {
                            newRow[j.ToString()] = i;
                        }
                        else
                        {
                            newRow[j.ToString()] = martix[k, i, j];
                        }
                    }
                    dataTable.Rows.Add(newRow);
                }
                MatrixList.Add(dataTable);
            }

            return MatrixList;
        }
        /// <summary>
        /// 3d
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public double[,,] GetMatrix(List<Sensor> N)
        {
            int n = N.Count;
            double[,,] M = new double[Parameters.PublicParamerters.NumberofGeneratedPackets,n,n];
            List<RoutingMetric> L = new List<RoutingMetric>();
            foreach(Sensor s in N) { L.AddRange(s.MyForwardersShortedList); }

            for (int x = 0; x < L.Count; x++)
            {
                RoutingMetric row = L[x];
                int k = System.Convert.ToInt32(row.PID)-1; // the path is starting from the 1.
                int i = row.SenderID;
                int j = row.ForwarderID;
                M[k, i, j] = row.LinkEstimationNormalized;
            }

            return M;
        }
    }
}
