using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj000_MazeAndPathFinding.Prj.State
{
    public class MapGenerating : StateBase
    {
        MapGenerator.MapGeneratorBase m_MapGenerator = null;

        public MapGenerating(Process.Process process)
            : base(process)
        {
            m_MapGenerator = MapGenerator.MapGeneratorBase.Create(process.MapAlgorithmName);
            Debug.Assert(m_MapGenerator != null, "Map Generator is not created!");
        }

        public override void Update(double deltaTime)
        {
            Debug.Assert(m_MapGenerator != null, "Map Generator is null!");
        }

        public override void Render()
        {

        }
    }
}
