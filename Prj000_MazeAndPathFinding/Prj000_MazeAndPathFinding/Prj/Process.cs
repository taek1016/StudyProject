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

        Renderer m_Renderer = new Renderer();

        public Process()
        {
            m_Renderer.Init();

            SetStateObj(m_CurrentState);

            m_AccTime = m_ChangeTime + 1;
        }

        ~Process()
        {
            m_Renderer.Destroy();
        }

        public void SetStateObj(ProcessState state)
        {
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
                m_StateObj = m_NextStateObj;
                m_AccTime = 0;

                m_Renderer.ClearMap();
            }

            Debug.Assert(m_StateObj != null, "State Object is null!");

            m_StateObj?.Update(deltaTime);

            m_StateObj?.SetRenderData(m_Renderer);
            m_Renderer.Render();
        }

        public void RequestClearMap()
        {
            m_Renderer.ClearMap();
        }
    }
}
