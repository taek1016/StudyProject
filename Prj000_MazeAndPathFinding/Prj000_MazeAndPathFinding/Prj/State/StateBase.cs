using System.Diagnostics;

namespace Prj000_MazeAndPathFinding.Prj.State
{
    public abstract class StateBase
    {
        public static StateBase Create(Process.Process.ProcessState state, Process.Process process)
        {
            StateBase stateObj = null;
            switch (state)
            {
                case Process.Process.ProcessState.Init:
                    stateObj = new InitState(process);
                    break;

                case Process.Process.ProcessState.MapGenerating:
                    stateObj = new MapGenerating(process);
                    break;

                case Process.Process.ProcessState.PathFinding:
                    stateObj = new PathFinding(process);
                    break;

                case Process.Process.ProcessState.Run:
                    break;

                case Process.Process.ProcessState.WaitKey:
                    break;
            }

            Debug.Assert(stateObj != null, string.Format($"{state} not created!"));

            return stateObj;
        }

        protected Process.Process m_Process = null;

        public StateBase(Process.Process process)
        {
            m_Process = process;

            Debug.Assert(m_Process != null, $"Process object is null");
        }

        public abstract void Update(double deltaTime);

        public abstract void SetRenderData(Renderer renderer);
    }
}
