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

            Point currentPos = m_VisitQueue.Peek();

            Point[] posData = { new Point(1, 0), new Point(0, 1), new Point(-1, 0), new Point(0, -1) };
            bool[] bSearched = new bool[(int)MapWay.WayEnd];

            bool bFullSearched = true;

            if (m_MapSearched.Count > 0 && m_MapSearched.Peek().Equals(m_MapPointer.EndPoint))
            {
                m_bEnded = true;
                return;
            }

            for (int i = 0; i < bSearched.Length; ++i)
            {
                Point tempPos = currentPos + posData[i];

                if (!m_MapVisited[tempPos.Y, tempPos.X])
                {
                    bFullSearched = false;
                    break;
                }
            }

            if (bFullSearched)
            {
                m_VisitQueue.Dequeue();
                var pos = m_MapSearched.Pop();
                m_VisitQueue.Enqueue(m_MapSearched.Peek());
                return;
            }

            while (true)
            {
                int way = m_WayGenerator.Next(0, (int)MapWay.WayEnd);

                Point nextPos = currentPos + posData[way];

                if (!m_MapVisited[nextPos.Y, nextPos.X])
                {
                    m_VisitQueue.Dequeue();
                    m_VisitQueue.Enqueue(nextPos);
                    m_MapVisited[nextPos.Y, nextPos.X] = true;
                    m_MapSearched.Push(nextPos);
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

            m_MapVisited = new bool[heightSize, widthSize];

            var wallInfo = m_MapPointer.Info["Wall"];

            for (int i = 0; i < heightSize; ++i)
            {
                for (int j = 0; j < widthSize; ++j)
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
            m_MapSearched.Push(startPos);

            m_MapVisited[startPos.Y, startPos.X] = true;
            #endregion
        }
    }
}
