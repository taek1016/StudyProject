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
    public class DFS : PathFindingBase
    {
        enum MapWay
        {
            East,
            South,
            West,
            North,
            WayEnd
        }
        internal DFS() : base()
        {

        }

        Stack<Point> m_MapPath = new Stack<Point>();

        private bool m_bEnded = false;

        Queue<Point> m_VisitQueue = new Queue<Point>();
        bool[,] m_MapVisited = null;

        MapData m_MapPointer = null;
        Random m_WayGenerator = new Random();

        public override bool IsUpdateEnded()
        {
            return m_bEnded;
        }

        public override void UpdatePath(double deltaTime)
        {
            Debug.Assert(m_MapPointer != null, "Map Pointer is null!");

            Point currentPos = m_VisitQueue.Dequeue();

            Point[] posData = { new Point(1, 0), new Point(0, 1), new Point(-1, 0), new Point(0, -1) };
            bool[] bSearched = new bool[(int)MapWay.WayEnd];

            while (true)
            {
                int way = m_WayGenerator.Next(0, (int)MapWay.WayEnd);

                bool bFullSearched = true;

                for (int i = 0; i < bSearched.Length; ++i)
                {
                    Point tempPos = currentPos + posData[i];

                    if (!m_MapVisited[tempPos.X, tempPos.Y])
                    {
                        bFullSearched = false;
                        break;
                    }
                }

                if (m_MapPath.Peek().Equals(m_MapPointer.EndPoint))
                {
                    m_bEnded = true;
                    break;
                }

                if (bFullSearched)
                {
                    m_MapPath.Pop();
                    break;
                }

                Point nextPos = currentPos + posData[way];

                if (!m_MapVisited[nextPos.X, nextPos.Y])
                {
                    m_VisitQueue.Enqueue(nextPos);
                    m_MapVisited[nextPos.X, nextPos.Y] = true;
                    m_MapPath.Push(nextPos);
                    break;
                }
            }
        }

        protected override void InitData(MapData mapData)
        {
            #region Data
            m_MapPointer = mapData;

            int widthSize = mapData.WidthSize;
            int heightSize = mapData.HeightSize;

            m_MapVisited = new bool[widthSize, heightSize];

            var wallInfo = m_MapPointer.Info["Wall"];

            for (int i = 0; i < widthSize; ++i)
            {
                for (int j = 0; j < heightSize; ++j)
                {
                    if (m_MapPointer.Map[i, j] == wallInfo.MapCharacter)
                    {
                        m_MapVisited[i, j] = true;
                    }
                }
            }
            #endregion

            #region QueueSet
            Point startPos = mapData.StartPoint;
            m_VisitQueue.Enqueue(startPos);
            m_MapPath.Push(startPos);

            m_MapVisited[startPos.X, startPos.Y] = true;
            #endregion
        }
    }
}
