using Prj000_MazeAndPathFinding.Util;
using System;
using System.Collections.Generic;
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

        int m_StackIndex = 0;

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

                        Debug.Assert(m_PathFinding != null, "Path Finding Algorithm is null!");

                        m_CurrentState = State.GeneratePathData;
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

                            case ConsoleKey.RightArrow:
                                m_StackIndex++;
                                m_StackIndex %= m_PathFinding.Searched.Count;
                                break;

                            case ConsoleKey.LeftArrow:
                                m_StackIndex--;
                                m_StackIndex %= m_PathFinding.Searched.Count;
                                break;
                        }
                    }
                    Debug.Assert(m_PathFinding != null, "Map Generator is null!");

                    m_CurrentDeltaTime += deltaTime;

                    m_PathFinding.UpdatePath(deltaTime);

                    if (m_StackIndex < 0 && m_PathFinding.Searched.Count > 0)
                    {
                        m_StackIndex = 0;
                    }

                    if (m_CurrentDeltaTime >= m_PathFindingSpeed && m_PathFinding.IsUpdateEnded())
                    {
                        m_CurrentDeltaTime -= m_PathFindingSpeed;

                        m_CurrentState = State.SetPathDataToProcess;
                    }

                    break;

                case State.SetPathDataToProcess:
                    m_Process.PathData = m_PathFinding.Path;
                    //m_Process.SetStateObj(Process.Process.ProcessState.Run);

                    break;
            }
        }

        public override void SetRenderData(Renderer renderer)
        {
            Point pos = new Point(0, 0);

            switch (m_CurrentState)
            {
                case State.SelectPathType:
                    {
                        int pathLength = m_PathFindingNames.Length - 1;

                        for (int i = 0; i < pathLength; ++i)
                        {
                            renderer.SetMap($"{i + 1}. {m_PathFindingNames[i]}", pos);
                            pos.Y++;
                        }


                        renderer.SetMap("Select Path Finding Algorithm : ", pos);

                        if (m_PathFindingType != PathFindingType.End)
                        {
                            Console.Write($"{m_PathFindingNames[(int)m_PathFindingType]}");
                        }
                        pos.Y++;
                    }
                    break;

                case State.GeneratePathData:
                case State.SetPathDataToProcess:
                    {
                        Debug.Assert(m_PathFinding != null, "Map Type is null!");

                        var map = m_Process.MapData;

                        int heightSize = map.Map.GetLength(0);
                        int widthSize = map.Map.Length / heightSize;

                        renderer.SetSize(heightSize, widthSize);

                        for (int i = 0; i < heightSize; ++i)
                        {
                            for (int j = 0; j < widthSize; ++j)
                            {
                                renderer.SetMap(map.Map[i, j], new Point(j, i), map.Color[i, j]);
                            }
                        }

                        if (0 <= m_StackIndex && m_StackIndex < m_PathFinding.Searched.Count)
                        {
                            var startPos = map.StartPoint;
                            var endPos = map.EndPoint;
                            var pathSearched = m_PathFinding.Searched[m_StackIndex];
                            var pathArr = pathSearched.ToArray();

                            int pathCount = pathArr.Length;

                            for (int i = pathCount - 1; i >= 0; --i)
                            {
                                if (pathArr[i].Equals(startPos) || pathArr[i].Equals(endPos))
                                {
                                    continue;
                                }

                                renderer.SetMap('□', pathArr[i], ConsoleColor.Magenta);
                            }
                        }

                        pos.X = widthSize + 2;
                        pos.Y = 0;

                        var posInfo = map.Info["StartPoint"];
                        renderer.SetMap($"{posInfo.MapCharacter} : {posInfo.MapStr}", pos, posInfo.MapColor);
                        pos.Y++;

                        posInfo = map.Info["EndPoint"];
                        renderer.SetMap($"{posInfo.MapCharacter} : {posInfo.MapStr}", pos, posInfo.MapColor);
                        pos.Y++;
                        renderer.SetMap($"□ : Path Finding", pos, ConsoleColor.Magenta);
                        pos.Y++;
                        pos.Y++;

                        renderer.SetMap("↑ : Speed Up", pos, ConsoleColor.Yellow);
                        pos.Y++;
                        renderer.SetMap("↓ : Speed Down", pos, ConsoleColor.Yellow);
                        pos.Y++;
                        renderer.SetMap(string.Format("{0:F5} Sec : Current Speed", m_PathFindingSpeed), pos, ConsoleColor.Yellow);
                        pos.Y++;
                        pos.Y++;

                        if (m_PathFinding.Searched.Count > 0)
                        {
                            renderer.SetMap("→ : Next Path", pos, ConsoleColor.Yellow);
                            pos.Y++;
                            renderer.SetMap("← : Before Path", pos, ConsoleColor.Yellow);
                            pos.Y++;
                            renderer.SetMap($"{m_StackIndex} / {m_PathFinding.Searched.Count.ToString("000")} : Current Path Index / Path Count", pos, ConsoleColor.Yellow);
                            pos.Y++;
                        }

                        pos.X = 0;
                        pos.Y = heightSize + 1;
                        renderer.SetMap($"Path Finding Algorithm : {m_PathFindingType}", pos);
                    }
                    break;
            }
        }
    }
}
