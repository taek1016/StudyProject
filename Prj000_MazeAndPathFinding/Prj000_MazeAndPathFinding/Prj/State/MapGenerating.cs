using Prj000_MazeAndPathFinding.Util;
using System;
using System.Diagnostics;

namespace Prj000_MazeAndPathFinding.Prj.State
{
    public class MapGenerating : StateBase
    {
        string[] m_MapGeneratorType = null;

        public enum MapGeneratorType
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
            Kruskal,
            End
        }
        MapGeneratorType m_SelectMapType = MapGeneratorType.End;

        MapGenerator.MapGeneratorBase m_MapGenerator = null;

        public enum GeneratingMapState
        {
            SelectMapType,
            SelectMapXSize,
            SelectMapYSize,
            GeneratingMap,
            WaitEnd
        }

        public GeneratingMapState MapGenerateState { get; set; } = GeneratingMapState.SelectMapType;

        Point m_MapSize = new Point();

        public MapGenerating(Process.Process process)
            : base(process)
        {
            m_MapGeneratorType = Enum.GetNames(typeof(MapGeneratorType));

            int mapGeneratorCount = m_MapGeneratorType.Length - 1;

            for (int i = 0; i < mapGeneratorCount; ++i)
            {
                m_MapGeneratorType[i] = m_MapGeneratorType[i].Replace('_', ' ');
            }
        }

        string m_Input;

        double m_CurrentDeltaTime = 0;
        double m_MapGenerateSpeed = 0.3;

        const int UPPER_WIDTH_LIMIT = 349 / 4;      // Console
        const int UPPER_HEIGHT_LIMIT = 101 - 20;    // 

        bool m_bStateChanged = false;
        public override void Update(double deltaTime)
        {
            switch (MapGenerateState)
            {
                case GeneratingMapState.SelectMapType:
                    {
                        if (Console.KeyAvailable)
                        {
                            var readKey = Console.ReadKey(true);

                            int input;

                            if (!int.TryParse(readKey.KeyChar.ToString(), out input))
                            {
                                return;
                            }

                            if (0 > input || input >= (int)MapGeneratorType.End)
                            {
                                return;
                            }

                            m_SelectMapType = (MapGeneratorType)(input - 1);

                            MapGenerateState = GeneratingMapState.SelectMapXSize;
                            Console.Clear();
                        }
                    }
                    break;

                case GeneratingMapState.SelectMapXSize:
                    {
                        if (Console.KeyAvailable)
                        {
                            var temp = Console.ReadKey(true);

                            if (char.IsNumber(temp.KeyChar))
                            {
                                m_Input += temp.KeyChar;
                            }

                            if (temp.Key == ConsoleKey.Backspace && m_Input.Length > 0)
                            {
                                m_Input = m_Input.Substring(0, m_Input.Length - 1);
                                Console.Clear();
                                break;
                            }
                            else if (temp.Key != ConsoleKey.Enter)
                            {
                                break;
                            }

                            int input = -1;
                            if (!int.TryParse(m_Input, out input))
                            {
                                break;
                            }

                            if (0 > input || input % 2 == 0 || input >= UPPER_WIDTH_LIMIT) // 349는 SetWindowSize에서 제한하는 크기
                            {
                                Console.Clear();
                                m_Input = "";
                                break;
                            }

                            m_MapSize.X = input;

                            MapGenerateState = GeneratingMapState.SelectMapYSize;
                            m_Input = "";
                        }
                    }
                    break;

                case GeneratingMapState.SelectMapYSize:
                    {
                        if (Console.KeyAvailable)
                        {
                            var temp = Console.ReadKey(true);

                            if (char.IsNumber(temp.KeyChar))
                            {
                                m_Input += temp.KeyChar;
                            }

                            if (temp.Key == ConsoleKey.Backspace && m_Input.Length > 0)
                            {
                                m_Input = m_Input.Substring(0, m_Input.Length - 1);
                                Console.Clear();
                                break;
                            }
                            else if (temp.Key != ConsoleKey.Enter)
                            {
                                break;
                            }

                            int input = -1;
                            if (!int.TryParse(m_Input, out input))
                            {
                                break;
                            }

                            if (0 > input || input % 2 == 0 || input >= UPPER_HEIGHT_LIMIT)
                            {
                                Console.Clear();
                                m_Input = "";
                                break;
                            }

                            m_MapSize.Y = input;

                            CreateMapGenerator();

                            Console.SetWindowSize(m_MapSize.X * 4, m_MapSize.Y + 20);

                            MapGenerateState = GeneratingMapState.GeneratingMap;
                            m_Input = "";
                        }
                    }
                    break;

                case GeneratingMapState.GeneratingMap:
                    {
                        if (Console.KeyAvailable)
                        {
                            var temp = Console.ReadKey(true);

                            switch (temp.Key)
                            {
                                case ConsoleKey.PageUp:
                                case ConsoleKey.UpArrow:
                                    m_MapGenerateSpeed -= m_MapGenerateSpeed * 0.1;
                                    break;

                                case ConsoleKey.PageDown:
                                case ConsoleKey.DownArrow:
                                    m_MapGenerateSpeed += m_MapGenerateSpeed * 0.1;
                                    break;
                            }
                        }
                        Debug.Assert(m_MapGenerator != null, "Map Generator is null!");

                        m_CurrentDeltaTime += deltaTime;

                        if (m_CurrentDeltaTime >= m_MapGenerateSpeed)
                        {
                            m_CurrentDeltaTime -= m_MapGenerateSpeed;
                            m_MapGenerator.GenerateMap(deltaTime);
                        }

                        if (m_MapGenerator.bGenerateEnded)
                        {
                            MapGenerateState = GeneratingMapState.WaitEnd;
                        }
                    }
                    break;

                case GeneratingMapState.WaitEnd:
                    if (!m_bStateChanged)
                    {
                        m_bStateChanged = true;

                        m_Process.SetStateObj(Process.Process.ProcessState.PathFinding); 
                    }
                    break;
            }
        }

        public override void Render()
        {
            Console.SetCursorPosition(0, 0);

            switch (MapGenerateState)
            {
                case GeneratingMapState.SelectMapType:
                    Debug.Assert(m_MapGeneratorType != null, "Map Type is null!");

                    int mapGeneratorCount = m_MapGeneratorType.Length - 1;
                    for (int i = 0; i < mapGeneratorCount; ++i)
                    {
                        Console.WriteLine($"{i + 1}.\t{m_MapGeneratorType[i]}");
                    }

                    Console.WriteLine();

                    Console.Write("Select\t: ");

                    if (m_SelectMapType != MapGeneratorType.End)
                    {
                        Console.Write($"{m_MapGeneratorType[(int)m_SelectMapType]}");
                    }
                    break;

                case GeneratingMapState.SelectMapXSize:
                    Console.WriteLine($"Map Type : {m_SelectMapType}");
                    Console.Write($"Map Width Size (Odd Number, Upper Limit {UPPER_WIDTH_LIMIT}) : {m_Input}");
                    break;

                case GeneratingMapState.SelectMapYSize:
                    Console.WriteLine($"Map Type : {m_SelectMapType}");
                    Console.WriteLine($"Map Width Size (Odd Number, Upper Limit {UPPER_WIDTH_LIMIT}) : {m_MapSize.X}");
                    Console.Write($"Map Width Height (Odd Number, Upper Limit {UPPER_HEIGHT_LIMIT}) : {m_Input}");
                    break;

                case GeneratingMapState.GeneratingMap:
                    RenderMap();

                    Console.WriteLine();

                    m_Process.MapData.RenderMapInfo();

                    break;

                case GeneratingMapState.WaitEnd:
                    RenderMap();

                    Console.WriteLine();

                    m_Process.MapData.RenderMapInfo();

                    break;
            }

        }

        void CreateMapGenerator()
        {
            m_MapGenerator = MapGenerator.MapGeneratorBase.Create(m_Process, (MapGeneratorType)m_SelectMapType, m_MapSize.X, m_MapSize.Y);
            Debug.Assert(m_MapGenerator != null, "Map Generator is not created!");
        }

        void RenderMap()
        {
            Console.ForegroundColor = ConsoleColor.White;

            Debug.Assert(m_MapGenerator != null, "Map Type is null!");

            Console.WriteLine($"Map Type : {m_SelectMapType}");
            Console.WriteLine($"Map Width Size (Odd Number, Upper Limit {UPPER_WIDTH_LIMIT}) : {m_MapSize.X}");
            Console.WriteLine($"Map Width Height (Odd Number, Upper Limit {UPPER_HEIGHT_LIMIT}) : {m_MapSize.Y}");
            Console.WriteLine();

            int widthSize = m_MapGenerator.MapData.GetLength(0);
            int heightSize = m_MapGenerator.MapData.Length / widthSize;

            for (int i = 0; i < widthSize; ++i)
            {
                for (int j = 0; j < heightSize; ++j)
                {
                    Console.ForegroundColor = m_MapGenerator.MapColor[i, j];
                    Console.Write(m_MapGenerator.MapData[i, j]);
                }
                Console.WriteLine();
            }

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("↑ : Speed Up, ↓ : Speed Down, Current Speed : {0:F5} Sec", m_MapGenerateSpeed);
        }
    }
}