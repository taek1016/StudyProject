using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj000_MazeAndPathFinding.Prj.State
{
    internal class InitState : StateBase
    {
        string[] m_MapGeneratorType = null;

        enum MapGeneratorType
        {
            Recursive_Back_Tracking,
            Binary_Tree,
            SideWinder,
            Recursive_Division,
            Eller,
            Wilson,
            Hunt_and_Kill,
            Growing_Tree,
            Aldous_Broder,
            Prim,
            Kruskal
        }

        int m_Input = -1;

        public InitState(Process.Process process)
            : base(process)
        {
            m_MapGeneratorType = Enum.GetNames(typeof(MapGeneratorType));

            int mapGeneratorCount = m_MapGeneratorType.Length;

            for (int i = 0; i < mapGeneratorCount; ++i)
            {
                m_MapGeneratorType[i] = m_MapGeneratorType[i].Replace('_', ' ');
            }
        }

        public override void Update(double deltaTime)
        {
            if (Console.KeyAvailable)
            {
                m_Input = int.Parse(((char)Console.ReadKey(true).Key).ToString());

                if (0 <= m_Input && m_Input < m_MapGeneratorType.Length)
                {
                    m_Process.SetMapGenerator(m_MapGeneratorType[m_Input].Replace(' ', '_'));
                    m_Process.SetStateObj(Process.Process.ProcessState.MapGenerating, m_Process);
                }
            }
        }

        public override void Render()
        {
            Debug.Assert(m_MapGeneratorType != null, "Map Type is null!");

            Console.SetCursorPosition(0, 0);

            int mapGeneratorCount = m_MapGeneratorType.Length;
            for (int i = 0; i < mapGeneratorCount; ++i)
            {
                Console.WriteLine($"{i + 1}.\t{m_MapGeneratorType[i]}");
            }

            Console.WriteLine();

            Console.Write("Select\t: ");

            if (m_Input != -1)
            {
                Console.Write($"{m_MapGeneratorType[m_Input - 1]}");
            }
        }
    }
}
