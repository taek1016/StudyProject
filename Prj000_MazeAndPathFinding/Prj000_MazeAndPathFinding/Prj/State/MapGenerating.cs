using Prj000_MazeAndPathFinding.Util;
using System;
using System.Diagnostics;

namespace Prj000_MazeAndPathFinding.Prj.State
{
    public class MapGenerating : StateBase
    {
        MapGenerator.MapGeneratorBase m_MapGenerator = null;

        public enum GeneratingMapState
        {
            SelectMapXSize,
            SelectMapYSize,
            GeneratingMap,
            WaitEnd
        }

        public GeneratingMapState MapGenerateState { get; set; } = GeneratingMapState.SelectMapXSize;
        
        Point m_MapSize = new Point();

        public MapGenerating(Process.Process process)
            : base(process)
        {
        }

        string m_Input;

        double m_CurrentDeltaTime = 0;
        double m_MapGenerateSpeed = 0.3;

        const int UPPER_WIDTH_LIMIT = 349 / 4;
        const int UPPER_HEIGHT_LIMIT = 101 - 20;

        public override void Update(double deltaTime)
        {
            switch (MapGenerateState)
            {
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
                            m_CurrentDeltaTime = 0;
                            m_MapGenerator.GenerateMap(deltaTime); 
                        }

                        if (m_MapGenerator.bGenerateEnded)
                        {
                            MapGenerateState = GeneratingMapState.WaitEnd;
                        }
                    }
                    break;

                case GeneratingMapState.WaitEnd:
                    m_Process.SetStateObj(Process.Process.ProcessState.PathFinding, m_Process);
                    break;
            }
        }

        public override void Render()
        {
            Console.SetCursorPosition(0, 0);

            switch (MapGenerateState)
            {
                case GeneratingMapState.SelectMapXSize:
                    Console.Write($"Map Width Size (Odd Number, Upper Limit {UPPER_WIDTH_LIMIT}) : {m_Input}");
                    break;

                case GeneratingMapState.SelectMapYSize:
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
            m_MapGenerator = MapGenerator.MapGeneratorBase.Create(m_Process, m_MapSize.X, m_MapSize.Y);
            Debug.Assert(m_MapGenerator != null, "Map Generator is not created!");
        }

        void RenderMap()
        {
            Console.ForegroundColor = ConsoleColor.White;

            Debug.Assert(m_MapGenerator != null, "Map Type is null!");

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