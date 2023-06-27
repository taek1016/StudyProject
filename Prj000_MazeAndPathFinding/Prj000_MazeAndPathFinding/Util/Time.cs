using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj000_MazeAndPathFinding.Util
{
    public class Time
    {
        Stopwatch m_StopWatch = new Stopwatch();
        Stopwatch m_TotalTime = new Stopwatch();
        public double DeltaTime
        {
            get
            {
                double deltaTime = m_StopWatch.Elapsed.TotalSeconds;

                m_StopWatch.Restart();

                return deltaTime;
            }
        }

        public void StartDeltaTime()
        {
            m_StopWatch.Start();
        }

        public bool CanDoNextFrame(int frame)
        {
            double deltaTimePerFrame = 1.0f / frame;
            double currentDeltaTime = m_StopWatch.Elapsed.TotalSeconds;

            if (currentDeltaTime >= deltaTimePerFrame)
            {
                m_StopWatch.Stop();

                return true;
            }

            return false;
        }
    }
}
