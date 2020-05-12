using LORA.Modules;
using LORA.Parameters;
using LORA.ui;
using System;
using System.Collections.Generic;

namespace LORA.Computations
{
    public class Density
    {
       
        /// <summary>
        /// standared deviasion.
        /// </summary>
        /// <param name="net"></param>
        /// <returns></returns>
        public static double GetDensity(List<Sensor> Nodes)
        {

            double n = Nodes.Count;
            double SumOFNeibors = 0; 
            foreach(Sensor s in Nodes)
            {
                
                if (s.NeighboreNodes != null)
                {
                    SumOFNeibors += s.NeighboreNodes.Count;
                }
            }
           
            double AreaOfOneNode = Math.PI * Math.Pow(PublicParamerters.CommunicationRangeRadius, 2); // the area.
            double oneLayerNodes = (MainWindow.SensingFeildArea / AreaOfOneNode);// one layer means that the feild can be totally covered by one layer. and the other layers are redundants.
            // NumberofLayers= how many expected nodes are within the range of the node x. that is how many neighbors nodes of x. 
            double NumberofLayers = Nodes.Count / oneLayerNodes; // ONE LAYER=ExpectedNumberOFnODES; this means that in the range of each node, we have NumberofLayers nodes in.
            //DegreeDistrbution= how many neighbors nodes.
            double DegreeDistrbution = SumOFNeibors / n;
            // the average 
            double density = (NumberofLayers + DegreeDistrbution) / 2; 
            return density;
        }

    }
}
