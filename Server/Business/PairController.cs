using System.Collections.Generic;
using System.Linq;

namespace Server.Business
{
    public class PairController
    {
        private static List<PairRobotAndUI> _pairList;
        public static List<PairRobotAndUI> PairList 
        { 
            get 
            {
                return _pairList;
            }
        }
        public static bool Pair(string source, string destination)
        {
            if(_pairList == null)
            {
                _pairList = new List<PairRobotAndUI>();
            }
            if (FindPair(source, destination)==null)
            {
                _pairList.Add(new PairRobotAndUI(){
                    SourceId = source,
                    DestinationId = destination
                });
                return true;
            }
            return false;
        }

        public static PairRobotAndUI FindPair(string source, string destination)
        {
            if(_pairList!=null)
            {
                return _pairList.FirstOrDefault(w=>(w.DestinationId == destination && w.SourceId == source));
            }
            else
            {
                return null;
            }
        }

        public static PairRobotAndUI FindPairByDestination(string destination)
        {
            if(_pairList!=null)
            {
                return _pairList.FirstOrDefault(w=>(w.DestinationId == destination));
            }
            else
            {
                return null;
            }
        }

        public static PairRobotAndUI FindPairBySource(string source)
        {
            if(_pairList!=null)
            {
                return _pairList.FirstOrDefault(w=>(w.SourceId == source));
            }
            else
            {
                return null;
            }
        }

        public static bool UnPair(PairRobotAndUI pairItem)
        {
            if (_pairList != null)
            {
                return _pairList.Remove(pairItem);
            }
            else 
            {
                return false;
            }
        }

        public static string UnPairByConnectionId(string connId)
        {
            var pairSource = FindPairBySource(connId);
            var r1 = UnPair(pairSource);
            var pairDestionation = FindPairByDestination(connId);
            var r2 = UnPair(pairDestionation);
            if (r1)
            {
                return pairSource.DestinationId;
            }
            else if (r2)
            {
                return pairDestionation.SourceId;
            }
            else 
            {
                return null;
            }
        }
    }
}