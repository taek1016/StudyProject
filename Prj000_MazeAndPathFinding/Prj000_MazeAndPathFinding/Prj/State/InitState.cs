using System;
using System.Diagnostics;

namespace Prj000_MazeAndPathFinding.Prj.State
{
    internal class InitState : StateBase
    {


        //int m_Input = -1;

        bool m_bInitCalled = false;

        public InitState(Process.Process process)
            : base(process)
        {
            
        }

        public override void Update(double deltaTime)
        {
            if (!m_bInitCalled)
            {
                m_bInitCalled = true;
                m_Process.SetStateObj(Process.Process.ProcessState.MapGenerating);
            }
        }

        public override void Render()
        {
            
        }
    }
}
