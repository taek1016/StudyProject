using Prj000_MazeAndPathFinding.Prj.Util;
using Prj000_MazeAndPathFinding.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj000_MazeAndPathFinding.Prj.PathFinding
{
    public struct PathData
    {
        Point Point;
        ConsoleColor color;

    }
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
                    obj = new DFS();
                    break;

                case State.PathFinding.PathFindingType.Dijikstra:
                    break;

                case State.PathFinding.PathFindingType.AStar:
                    break;

                case State.PathFinding.PathFindingType.End:
                    Debug.Assert(false, "Type is end!");
                    break;
            }

            obj.InitData(process.MapData);

            return obj;
        }

        protected Util.PathData m_OptimizedPathData = null;
        public Util.PathData Path { get => m_OptimizedPathData; protected set { m_OptimizedPathData = value; } }

        protected abstract void InitData(MapData mapData);

        public abstract void UpdatePath(double deltaTime);

        public abstract bool IsUpdateEnded();

    }
}