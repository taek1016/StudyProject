using System;
using System.Diagnostics;

namespace Prj000_MazeAndPathFinding.Prj.State
{
    internal class InitState : StateBase
    {


        //int m_Input = -1;

        bool m_bInitCalled = false;
        double m_AccTime = 0;

        public InitState(Process.Process process)
            : base(process)
        {
            
        }

        public override void Update(double deltaTime)
        {
            m_AccTime += deltaTime;
            if (!m_bInitCalled)
            {
                m_bInitCalled = true;
                m_Process.SetStateObj(Process.Process.ProcessState.MapGenerating);
            }
        }

        public override void SetRenderData(Renderer renderer)
        {
            string data = $"{Process.Process.ProcessState.Init} -> {Process.Process.ProcessState.MapGenerating}";

            var pos = new Prj000_MazeAndPathFinding.Util.Point();

            renderer.SetMap(data, pos);

            pos.X += data.Length;

            double accTime = m_AccTime;

            string dots = "";
            while (accTime > 0)
            {
                dots += ".";
                accTime -= 0.3;
            }

            renderer.SetMap(dots, pos);
        }
    }
}
