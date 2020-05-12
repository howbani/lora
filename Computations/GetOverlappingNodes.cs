using System.Collections.Generic;
using LORA.Modules;
using System.Windows;

namespace LORA.Computations
{
    public class GetOverlappingNodes
    {
       private List<Sensor> Network;
       public GetOverlappingNodes(List<Sensor> _NetworkNodes)
       {
           Network=_NetworkNodes; 
       }





        /// <summary>
        /// only those nodes which follow within the range of i.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private void GetOverlappingNodesForAnode(Sensor i)
        {
             // intializin:
                i.NeighboreNodes = null;
                // get the overlapping nodes:
                List<Sensor> Nnodes = new List<Sensor>();
                if (Network != null)
                {
                    if (Network.Count > 0)
                    {
                        foreach (Sensor node in Network)
                        {
                            if (i.ID != node.ID)
                            {
                                bool isOverlapped = Operations.isInMyComunicationRange(i, node);
                                if (isOverlapped)
                                {
                                    Nnodes.Add(node);
                                }
                            }
                        }
                    }
                }
                if (Nnodes.Count > 0)
                {
                    i.NeighboreNodes = Nnodes;
                }
            
        }

       /// <summary>
       /// for all nodes inside the newtwork find the overllapping nodes.
       /// </summary>
       public void GetOverlappingForAllNodes()
       {
           foreach(Sensor node in Network)
           {
               GetOverlappingNodesForAnode(node);
           }
       }









    }
}
