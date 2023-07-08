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

        double m_AccTime = 0;
        double m_ChangeTime = 3.0;
        bool m_bStateChangeCalled = false;

        #region CoreData
        Util.MapData m_MapData = new Util.MapData();

        public Util.MapData MapData { get => m_MapData; }

        public Util.PathData PathData { get; set; }
        #endregion

        ProcessState m_CurrentState = ProcessState.Init;
        ProcessState m_NextState = ProcessState.MapGenerating;
        public Process()
        {
            Console.CursorVisible = false;

            SetStateObj(m_CurrentState);

            m_AccTime = m_ChangeTime + 1;
        }

        ~Process()
        {
            Console.CursorVisible = true;
        }

        public void SetStateObj(ProcessState state)
        {
            Console.Clear();

            m_NextState = state;

            m_NextStateObj = State.StateBase.Create(m_NextState, this);
            m_bStateChangeCalled = true;
        }

        public void Run(double deltaTime)
        {
            if (m_bStateChangeCalled)
            {
                m_AccTime += deltaTime;
            }

            if (m_bStateChangeCalled && m_AccTime > m_ChangeTime && m_NextStateObj != null && m_NextStateObj != m_StateObj)
            {
                m_bStateChangeCalled = false;
                Console.Clear();
                m_StateObj = m_NextStateObj;
                m_AccTime = 0;
            }

            Debug.Assert(m_StateObj != null, "State Object is null!");

            m_StateObj?.Update(deltaTime);
            if (!m_bStateChangeCalled)
            {
                m_StateObj?.Render();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(0, 0);

                Console.Write($"{m_CurrentState} -> {m_NextState}");

                double accTime = m_AccTime;

                while (accTime > 0)
                {
                    Console.Write(".");
                    accTime -= 0.3;
                }
            }
        }
    }
}
