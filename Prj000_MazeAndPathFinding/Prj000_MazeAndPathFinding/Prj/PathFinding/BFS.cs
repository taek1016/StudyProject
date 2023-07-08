using Prj000_MazeAndPathFinding.Prj.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj000_MazeAndPathFinding.Prj.PathFinding
{
    public class BFS : PathFindingBase
    {
        internal BFS() : base()
        {

        }

        List<Util.PathData> m_MapObject = new List<Util.PathData>();

        private bool m_bEnded = false;

        public override bool IsUpdateEnded()
        {
            return m_bEnded;
        }

        public override void UpdatePath(double deltaTime)
        {
        }

        public override void CopyPathToMap(MapData mapData)
        {
            throw new NotImplementedException();
        }
    }
}
