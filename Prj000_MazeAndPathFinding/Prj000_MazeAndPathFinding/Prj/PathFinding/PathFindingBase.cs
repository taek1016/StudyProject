using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj000_MazeAndPathFinding.Prj.PathFinding
{
    public abstract class PathFindingBase
    {
        public static PathFindingBase Create(Process.Process process, State.PathFinding.PathFindingType pathFindingType)
        {
            PathFindingBase obj = null;

            switch (pathFindingType)
            {
                case State.PathFinding.PathFindingType.BFS:
                    obj = new BFS();
                    break;

                case State.PathFinding.PathFindingType.DFS:
                    break;

                case State.PathFinding.PathFindingType.Dijikstra:
                    break;

                case State.PathFinding.PathFindingType.AStar:
                    break;

                case State.PathFinding.PathFindingType.End:
                    Debug.Assert(false, "Type is end!");
                    break;
            }

            return obj;
        }

        protected Util.PathData m_OptimizedPathData = null;
        public Util.PathData Path { get => m_OptimizedPathData; protected set { m_OptimizedPathData = value; } }

        public abstract void UpdatePath(double deltaTime);

        public abstract bool IsUpdateEnded();

        public abstract void CopyPathToMap(Util.MapData mapData);
    }
}