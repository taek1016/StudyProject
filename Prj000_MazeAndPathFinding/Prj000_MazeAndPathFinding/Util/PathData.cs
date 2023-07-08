using Prj000_MazeAndPathFinding.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj000_MazeAndPathFinding.Prj.Util
{
    public class PathData
    {
        int m_CurrentIndex = -1;
        List<Point> m_PathData = new List<Point>();

        public Point CurrentPos
        {
            get
            {
                return m_PathData[m_CurrentIndex];
            }
        }

        public void Init()
        {
            m_CurrentIndex = 0;
            m_PathData.Clear();
        }

        public void AddPath()
        {

        }

        public void SetPathData(PathData other)
        {
            Debug.Assert(other != null, "Other Object is null!");

            Init();

            var otherPathData = other.m_PathData;

            Debug.Assert(otherPathData != null, "Other Path Data is null!");

            int otherSize = otherPathData.Count;
            Debug.Assert(otherSize != 0, "Path Must Set!");

            for (int i = 0; i < otherSize; ++i)
            {
                m_PathData.Add(otherPathData[i]);
            }

            other.Init();
        }
    }
}
