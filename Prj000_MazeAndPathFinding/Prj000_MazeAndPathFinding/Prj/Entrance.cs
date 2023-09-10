using Prj000_MazeAndPathFinding.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj000_MazeAndPathFinding.Prj
{
    public class Entrance
    {
        Time m_Time = new Time();
        Process.Process m_Process = new Process.Process();

        int m_FramePerMinute = 60;

        public void Init()
        {
        }

        public void Run()
        {
            m_Time.StartDeltaTime();

            while (true)
            {
                if (m_Time.CanDoNextFrame(m_FramePerMinute))
                {
                    double deltaTime = m_Time.DeltaTime;

                    m_Process.Run(deltaTime);
                }
            }
        }
    }
}
