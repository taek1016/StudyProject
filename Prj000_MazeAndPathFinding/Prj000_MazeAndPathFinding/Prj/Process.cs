using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj000_MazeAndPathFinding.Prj.Process
{
    public class Process
    {
        #region RunState
        public enum ProcessState
        {
            Init,
            MapGenerating,
            PathFinding,
            Run,
            WaitKey,
        }
        #endregion

        #region StateBase
        State.StateBase m_StateObj = null;
        State.StateBase m_NextStateObj = null;
        #endregion

        public string MapAlgorithmName { get; private set; }

        double m_AccTime = 0;
        double m_ChangeTime = 3.0;
        bool m_bStateChangeCalled = false;

        public Process()
        {
            Console.CursorVisible = false;

            SetStateObj(ProcessState.Init, this);

            m_AccTime = m_ChangeTime + 1;
        }

        ~Process()
        {
            Console.CursorVisible = true;
        }

        public void SetStateObj(ProcessState state, Process obj)
        {
            m_NextStateObj = State.StateBase.Create(state, obj);
            m_bStateChangeCalled = true;
        }

        public void SetMapGenerator(string generatorName)
        {
            MapAlgorithmName = generatorName;
        }

        public void Run(double deltaTime)
        {
            if (m_bStateChangeCalled)
            {
                m_AccTime += deltaTime;
            }

            if (m_bStateChangeCalled && m_AccTime > m_ChangeTime && m_NextStateObj != null && m_NextStateObj != m_StateObj)
            {
                m_StateObj = m_NextStateObj;
                m_AccTime = 0;
            }

            Debug.Assert(m_StateObj != null, "State Object is null!");

            m_StateObj?.Update(deltaTime);
            m_StateObj?.Render();
        }
    }
}
