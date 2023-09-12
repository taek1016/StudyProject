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
        GeneratingMapState m_NextMapState;
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

        double m_CurrentMapChangeTime = 0;
        bool m_bStateChanged = false;
        public override void Update(double deltaTime)
        {
            if (m_bStateChanged)
            {
                m_CurrentMapChangeTime += deltaTime;
            }

            if (m_bStateChanged && m_CurrentMapChangeTime >= 1.0)
            {
                m_CurrentMapChangeTime = 0;

                m_bStateChanged = false;

                m_Process.RequestClearMap();

                MapGenerateState = m_NextMapState;
            }

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

                            m_NextMapState = GeneratingMapState.SelectMapXSize;

                            m_bStateChanged = true;
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
                                m_Input = "";
                                break;
                            }

                            m_MapSize.X = input;

                            m_NextMapState = GeneratingMapState.SelectMapYSize;

                            m_bStateChanged = true;

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
                                m_Input = "";
                                break;
                            }

                            m_MapSize.Y = input;

                            CreateMapGenerator();

                            m_NextMapState = GeneratingMapState.GeneratingMap;

                            m_bStateChanged = true;

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
                            m_NextMapState = GeneratingMapState.WaitEnd;

                            m_bStateChanged = true;
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

        public override void SetRenderData(Renderer renderer)
        {
            Point pos = new Point(0, 0);

            switch (MapGenerateState)
            {
                case GeneratingMapState.SelectMapType:
                    {
                        Debug.Assert(m_MapGeneratorType != null, "Map Type is null!");

                        int mapGeneratorCount = m_MapGeneratorType.Length - 1;
                        for (int i = 0; i < mapGeneratorCount; ++i)
                        {
                            renderer.SetMap($"{i + 1}.\t{m_MapGeneratorType[i]}", pos);
                            pos.Y++;
                        }

                        pos.Y++;

                        string temp = "Select\t: ";
                        renderer.SetMap(temp, pos);
                        pos.X = temp.Length + 1;

                        if (m_SelectMapType != MapGeneratorType.End)
                        {
                            renderer.SetMap($"{m_MapGeneratorType[(int)m_SelectMapType]}", pos);
                        }
                        pos.Y++;
                    }
                    break;

                case GeneratingMapState.SelectMapXSize:
                    {
                        renderer.SetMap($"Map Type : {m_SelectMapType}", pos);

                        pos.Y++;

                        renderer.SetMap($"Map Width Size (Odd Number, Upper Limit {UPPER_WIDTH_LIMIT}) : {m_Input}", pos);
                    }
                    break;

                case GeneratingMapState.SelectMapYSize:
                    {
                        renderer.SetMap($"Map Type : {m_SelectMapType}", pos);
                        pos.Y++;
                        renderer.SetMap($"Map Width Size (Odd Number, Upper Limit {UPPER_WIDTH_LIMIT}) : {m_MapSize.X}", pos);
                        pos.Y++;
                        renderer.SetMap($"Map Width Height (Odd Number, Upper Limit {UPPER_HEIGHT_LIMIT}) : {m_Input}", pos);
                    }
                    break;

                case GeneratingMapState.GeneratingMap:
                case GeneratingMapState.WaitEnd:
                    {
                        Debug.Assert(m_MapGenerator != null, "Map Type is null!");

                        int heightSize = m_MapGenerator.MapData.GetLength(0);
                        int widthSize = m_MapGenerator.MapData.Length / heightSize;

                        renderer.SetSize(heightSize, widthSize);

                        for (int i = 0; i < heightSize; ++i)
                        {
                            for (int j = 0; j < widthSize; ++j)
                            {
                                renderer.SetMap(m_MapGenerator.MapData[i, j], new Point(j, i), m_MapGenerator.MapColor[i, j]);
                            }
                        }

                        pos.Y = heightSize + 1;
                        renderer.SetMap($"Map Type : {m_SelectMapType}", pos);
                        pos.Y++;
                        renderer.SetMap($"Map Width Size (Odd Number, Upper Limit {UPPER_WIDTH_LIMIT}) : {m_MapSize.X}", pos);
                        pos.Y++;
                        renderer.SetMap($"Map Width Height (Odd Number, Upper Limit {UPPER_HEIGHT_LIMIT}) : {m_MapSize.Y}", pos);
                        pos.Y++;

                        pos.Y = 0;
                        pos.X = widthSize + 2;

                        var posInfo = m_MapGenerator.Info["StartPoint"];

                        renderer.SetMap($"{posInfo.MapCharacter} : {posInfo.MapStr}", pos, posInfo.MapColor);
                        pos.Y++;
                        posInfo = m_MapGenerator.Info["EndPoint"];
                        renderer.SetMap($"{posInfo.MapCharacter} : {posInfo.MapStr}", pos, posInfo.MapColor);
                        pos.Y++;
                        pos.Y++;

                        renderer.SetMap("↑ : Speed Up", pos, ConsoleColor.Yellow);
                        pos.Y++;
                        renderer.SetMap("↓ : Speed Down", pos, ConsoleColor.Yellow);
                        pos.Y++;
                        renderer.SetMap(string.Format("{0:F5} Sec : Current Speed", m_MapGenerateSpeed), pos, ConsoleColor.Yellow);
                    }
                    break;
            }
        }

        void CreateMapGenerator()
        {
            m_MapGenerator = MapGenerator.MapGeneratorBase.Create(m_Process, (MapGeneratorType)m_SelectMapType, m_MapSize.X, m_MapSize.Y);
            Debug.Assert(m_MapGenerator != null, "Map Generator is not created!");
        }
    }
}