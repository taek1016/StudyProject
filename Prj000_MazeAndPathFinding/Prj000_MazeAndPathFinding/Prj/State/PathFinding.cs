using System;
using System.Diagnostics;

namespace Prj000_MazeAndPathFinding.Prj.State
{
    public class PathFinding : StateBase
    {
        public enum PathFindingType
        {
            BFS,
            DFS,
            Dijikstra,
            AStar,
            End
        }

        string[] m_PathFindingNames = null;

        PathFindingType m_PathFindingType = PathFindingType.End;

        enum State
        {
            SelectPathType,
            GeneratePathData,
            SetPathDataToProcess,
            End
        }

        State m_CurrentState = State.SelectPathType;

        Prj.PathFinding.PathFindingBase m_PathFinding = null;

        Util.MapData MapData { get; }
        public PathFinding(Process.Process process)
            : base(process)
        {
            MapData = process.MapData;

            m_PathFindingNames = Enum.GetNames(typeof(PathFindingType));
        }

        double m_CurrentDeltaTime = 0;
        double m_PathFindingSpeed = 0.3;

        public override void Update(double deltaTime)
        {
            switch (m_CurrentState)
            {
                case State.SelectPathType:
                    if (Console.KeyAvailable)
                    {
                        var readKey = Console.ReadKey(true);

                        int input;

                        if (!int.TryParse(readKey.KeyChar.ToString(), out input))
                        {
                            return;
                        }

                        if (0 > input || input >= (int)PathFindingType.End)
                        {
                            return;
                        }

                        m_PathFindingType = (PathFindingType)(input - 1);

                        m_PathFinding = Prj.PathFinding.PathFindingBase.Create(m_Process, m_PathFindingType);

                        Debug.Assert(false, "Path Finding Algorithm is null!");

                        m_CurrentState = State.GeneratePathData;

                        Console.Clear();
                    }
                    break;

                case State.GeneratePathData:
                    if (Console.KeyAvailable)
                    {
                        var temp = Console.ReadKey(true);

                        switch (temp.Key)
                        {
                            case ConsoleKey.PageUp:
                            case ConsoleKey.UpArrow:
                                m_PathFindingSpeed -= m_PathFindingSpeed * 0.1;
                                break;

                            case ConsoleKey.PageDown:
                            case ConsoleKey.DownArrow:
                                m_PathFindingSpeed += m_PathFindingSpeed * 0.1;
                                break;
                        }
                    }
                    Debug.Assert(m_PathFinding != null, "Map Generator is null!");

                    m_CurrentDeltaTime += deltaTime;

                    m_PathFinding.UpdatePath(deltaTime);

                    if (m_CurrentDeltaTime >= m_PathFindingSpeed && !m_PathFinding.IsUpdateEnded())
                    {
                        m_CurrentDeltaTime -= m_PathFindingSpeed;

                        m_CurrentState = State.SetPathDataToProcess;
                    }

                    break;

                case State.SetPathDataToProcess:
                    m_Process.PathData = m_PathFinding.Path;
                    m_Process.SetStateObj(Process.Process.ProcessState.Run);

                    break;
            }
        }

        public override void Render()
        {
            Console.SetCursorPosition(0, 0);

            Console.ForegroundColor = ConsoleColor.White;

            switch (m_CurrentState)
            {
                case State.SelectPathType:
                    {
                        int pathLength = m_PathFindingNames.Length - 1;

                        for (int i = 0; i < pathLength; ++i)
                        {
                            Console.WriteLine($"{i + 1}. {m_PathFindingNames[i]}");
                        }

                        Console.Write("Select Path Finding Algorithm : ");

                        if (m_PathFindingType != PathFindingType.End)
                        {
                            Console.Write($"{m_PathFindingNames[(int)m_PathFindingType]}");
                        }
                    }
                    break;

                case State.GeneratePathData:
                case State.SetPathDataToProcess:
                    {
                        var map = new Util.MapData(MapData);

                        m_PathFinding.CopyPathToMap(map);

                        RenderMap(map);

                        map.RenderMapInfo();
                    }
                    break;
            }
        }

        void RenderMap(Util.MapData map)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Debug.Assert(m_PathFinding != null, "Map Type is null!");

            Console.WriteLine($"Path Finding Algorithm : {m_PathFindingType}");
            Console.WriteLine();

            int widthSize = map.Map.GetLength(0);
            int heightSize = map.Map.Length / widthSize;

            for (int i = 0; i < widthSize; ++i)
            {
                for (int j = 0; j < heightSize; ++j)
                {
                    Console.ForegroundColor = map.Color[i, j];
                    Console.Write(map.Map[i, j]);
                }
                Console.WriteLine();
            }

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("↑ : Speed Up, ↓ : Speed Down, Current Speed : {0:F5} Sec", m_PathFindingSpeed);
        }
    }
}
