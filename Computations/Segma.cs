using LORA.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LORA.Computations
{
    /// <summary>
    ///  how many times the node ID is repeated.
    ///  for example, we send 300 packets in difrrent time, from the source to the sink. node i is a relay and repeated 20 times ( included 20 time as a relay node).
    ///  
    /// </summary>
    public class SegmaRelayNode
    {
        public int RID { get; set; } // RELAY id
        public int Count { get; set; }
}
    /* 000000000000000000000000000000000000000000000000000000000000000000000000000000000*/
    public class SegmaSource
    {

        public int SourceID { get; set; }
        public int SinkID { get; set; } 
        private List<SegmaRelayNode> SegmaLst = new List<SegmaRelayNode>();

        public void Add(int RID)
        {
            if (RID != SourceID)
            {
                if (SinkID != RID)
                {
                    HopsSum++;
                    bool isFound = false;
                    foreach (SegmaRelayNode s in SegmaLst)
                    {
                        if (s.RID == RID) {  isFound = true; s.Count += 1; break; }
                    }
                    if (!isFound) { SegmaLst.Add(new SegmaRelayNode() { RID = RID, Count = 1 }); }
                }
            }
        }

        public List<SegmaRelayNode> GetRelayNodes
        {
            get
            {
                return SegmaLst;
            }
        }

        public double NumberofPacketsGeneratedByMe
        {
            get; set;
        }

        /// <summary>
        /// how many relayes on the path.
        /// r
        /// </summary>
        public double RelaysCount { get { return SegmaLst.Count;  } }
        /// <summary>
        /// the number of hops on the relayes.
        /// c:
        /// </summary>
        public double HopsSum
        {
            get;set;
        }

        /// <summary>
        /// Mean: hops/relay:
        /// how many packets relayed over the a relay node (average)
        /// multiplicity (that is, number of occurrences)
        /// c Miu
        /// </summary>
        public double Mean
        {
            get
            {
                return HopsSum / RelaysCount;
            }
        }

        /// <summary>
        /// Path Spread / NumberofPacketsGeneratedByMe
        /// </summary>
        public double PathsSpread
        {
            get
            {
               return 100 - (Mean * Veriance / NumberofPacketsGeneratedByMe);
            }
        }

      /// <summary>
      /// 
      /// </summary>
        public double Veriance
        {
            get
            {
                double m = Mean;
                double var = 0;
                foreach (SegmaRelayNode s in SegmaLst)
                {
                    var+= (s.Count - m) * (s.Count - m);
                }
                double su = (var / SegmaLst.Count);
                if (var == 0) return 0;
                else return Math.Log(PublicParamerters.ControlsRange) + Math.Log(su);
            }
        }




    }

    /*)))))))))))))))))))))))))))))))))))))))))))))))*/
    public class SegmaCollection
    {
        private List<SegmaSource> Sources = new List<SegmaSource>(); // list for each source.

        public void addSegmaList(SegmaSource newglist)
        {
            bool isFound = false;
            foreach (SegmaSource oldList in Sources)
            {
                if (oldList.SourceID == newglist.SourceID)
                {
                    isFound = true;
                    // merge seglist into sList.
                    foreach (SegmaRelayNode sn in newglist.GetRelayNodes)
                    {
                        oldList.Add(sn.RID);
                    }
                    //
                    break;
                }
            }

            if (!isFound) { Sources.Add(newglist); }
        }

        public List<SegmaSource> GetSourcesList { get { return Sources; } }
    }




    public class SegmaManager
    {
        SegmaCollection collection = new SegmaCollection();
        //pathStr= 8>28>23>27>9>19>2>0
        public void Filter(List<string> pathStrList) 
        {
            foreach (string pathStr in pathStrList)
            {
                if (pathStr != null)
                {
                    if (pathStr.Trim() != "")
                    {
                        SegmaSource segPath = new Computations.SegmaSource();
                        int Source = -1;
                        int sink = -1;
                        string[] nodesIDs = pathStr.Split('>');
                        if (nodesIDs.Length >= 2)
                        {
                            Source = Convert.ToInt16(nodesIDs[0]);
                            sink = Convert.ToInt16(nodesIDs[nodesIDs.Length - 1]);
                            segPath.SourceID = Source;
                            segPath.SinkID = sink;
                            foreach (string id in nodesIDs)
                            {
                                segPath.Add(Convert.ToInt16(id));
                            }
                        }
                        // add to collections:
                        if(segPath.SinkID!=-1 && segPath.SourceID!=-1)
                        collection.addSegmaList(segPath);
                    }
                }
            }
        }

        public SegmaCollection GetCollection
        {
            get
            {

                return collection;
            }
        }

        
    }



}
