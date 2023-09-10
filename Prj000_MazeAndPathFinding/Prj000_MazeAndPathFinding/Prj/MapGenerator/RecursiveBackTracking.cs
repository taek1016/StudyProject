using Prj000_MazeAndPathFinding.Util;
using System;
using System.Collections.Generic;

namespace Prj000_MazeAndPathFinding.Prj.MapGenerator
{
    public class RecursiveBackTracking : MapGeneratorBase
    {
        enum GenerateState
        {
            SetStartingPoint,
            SetNextPoint,
            FindUnvisitedPoint,
            EndPosConnect,
            GenerateEnd
        }

        GenerateState m_State = GenerateState.SetStartingPoint;

        Point m_CurrentPosition = new Point();

        Stack<Point> m_PositionStack = new Stack<Point>();

        Random m_Random = new Random(DateTime.Now.Millisecond);
        enum FindWay
        {
            North,
            East,
            South,
            West,
            End
        }

        Point[] m_WayData = new Point[(int)FindWay.End];

        public RecursiveBackTracking(Util.MapData mapData)
            : base(mapData)
        {
            m_WayData[(int)FindWay.North] = new Point(0, -2);
            m_WayData[(int)FindWay.East] = new Point(2, 0);
            m_WayData[(int)FindWay.South] = new Point(-2, 0);
            m_WayData[(int)FindWay.West] = new Point(0, 2);

            m_MapData.AddMapInfo("Wall", '■', ConsoleColor.Gray);
            m_MapData.AddMapInfo("Empty", '※', ConsoleColor.White);
            m_MapData.AddMapInfo("Way", '□', ConsoleColor.Yellow);
            m_MapData.AddMapInfo("StartPoint", '□', ConsoleColor.Red);
            m_MapData.AddMapInfo("EndPoint", '□', ConsoleColor.Green);
            m_MapData.AddMapInfo("CurrentPoint", '□', ConsoleColor.Cyan);
        }

        public override void GenerateMap(double deltaTime)
        {
            switch (m_State)
            {
                case GenerateState.SetStartingPoint:
                    while (true)
                    {
                        int xPos = m_Random.Next() % m_MapData.WidthSize;
                        int yPos = m_Random.Next() % m_MapData.HeightSize;

                        if (m_MapData.Visited[yPos, xPos] == false)
                        {
                            m_CurrentPosition.X = xPos;
                            m_CurrentPosition.Y = yPos;

                            m_MapData.StartPoint = m_CurrentPosition.Copy();
                            SetMapVisited(m_CurrentPosition, m_MapData["StartPoint"]);

                            break;
                        }
                    }

                    while (true)
                    {
                        int xPos = m_Random.Next() % m_MapData.WidthSize;
                        int yPos = m_Random.Next() % m_MapData.HeightSize;

                        if (m_MapData.Visited[yPos, xPos] == false)
                        {
                            Point endPos = new Point(xPos, yPos);

                            m_MapData.EndPoint = endPos.Copy();
                            SetMapVisited(endPos, m_MapData["EndPoint"]);

                            break;
                        }
                    }

                    m_PositionStack.Push(m_CurrentPosition.Copy());

                    m_State = GenerateState.SetNextPoint;
                    break;

                case GenerateState.SetNextPoint:
                    {
                        int END_COUNT = (int)FindWay.End;

                        bool[] bVisited = GetVisited();

                        while (true)
                        {
                            int randomWay = m_Random.Next(END_COUNT);

                            Point pos = m_CurrentPosition + m_WayData[randomWay];

                            int heightLength = m_MapData.Map.GetLength(0);
                            int rowLength = m_MapData.Map.Length / heightLength;

                            if (0 >= pos.X || pos.X >= rowLength || 0 >= pos.Y || pos.Y >= heightLength)
                            {
                                continue;
                            }

                            if (m_MapData.Visited[pos.Y, pos.X] == false)
                            {
                                m_PositionStack.Push(m_CurrentPosition.Copy());

                                if (m_PositionStack.Count > 0)
                                {
                                    SetMapVisited(m_PositionStack.Peek(), m_MapData["Way"]);
                                }

                                m_CurrentPosition.X = pos.X;
                                m_CurrentPosition.Y = pos.Y;

                                SetMapVisited(m_CurrentPosition - (m_WayData[randomWay] / 2), m_MapData["Way"]);
                                SetMapVisited(m_CurrentPosition, m_MapData["CurrentPoint"]);

                                break;
                            }
                            else
                            {
                                bVisited[randomWay] = true;

                                bool bNeedToChange = true;
                                for (int i = 0; i < END_COUNT; ++i)
                                {
                                    if (!bVisited[i])
                                    {
                                        bNeedToChange = false;
                                        break;
                                    }
                                }

                                if (bNeedToChange)
                                {
                                    m_State = GenerateState.FindUnvisitedPoint;
                                    break;
                                }
                            }
                        }
                    }
                    break;

                case GenerateState.FindUnvisitedPoint:
                    {
                        SetMapVisited(m_CurrentPosition, m_MapData["Way"]);

                        if (m_PositionStack.Count == 0)
                        {
                            m_State = GenerateState.EndPosConnect;
                            break;
                        }

                        m_CurrentPosition = m_PositionStack.Pop();

                        bool[] bVisited = GetVisited();

                        bool bNeedBackTracking = true;

                        for (int i = 0; i < (int)FindWay.End; ++i)
                        {
                            if (!bVisited[i])
                            {
                                bNeedBackTracking = false;
                                break;
                            }
                        }

                        SetMapVisited(m_CurrentPosition, m_MapData["CurrentPoint"]);

                        if (!bNeedBackTracking)
                        {
                            m_State = GenerateState.SetNextPoint;
                        }
                    }
                    break;

                case GenerateState.EndPosConnect:
                    {
                        m_CurrentPosition = m_MapData.EndPoint;

                        bool[] bVisited = GetVisited();

                        while (true)
                        {
                            int way = m_Random.Next((int)FindWay.End);

                            if (bVisited[way])
                            {
                                continue;
                            }

                            m_CurrentPosition = m_CurrentPosition + (m_WayData[way] / 2);

                            SetMapVisited(m_CurrentPosition, m_MapData["Way"]);

                            m_State = GenerateState.GenerateEnd;

                            break;
                        }
                    }
                    break;

                case GenerateState.GenerateEnd:
                    SetMapVisited(m_CurrentPosition, m_MapData["Way"]);

                    EndMapGenerate();

                    break;
            }
        }

        private bool[] GetVisited()
        {
            int END_COUNT = (int)FindWay.End;
            bool[] bVisited = new bool[END_COUNT];

            int heightLength = m_MapData.Map.GetLength(0);
            int rowLength = m_MapData.Map.Length / heightLength;

            for (int i = 0; i < END_COUNT; ++i)
            {
                Point pos = m_CurrentPosition + m_WayData[i];

                if (0 >= pos.X || pos.X >= rowLength || 0 >= pos.Y || pos.Y >= heightLength)
                {
                    bVisited[i] = true;
                }
            }

            return bVisited;
        }
    }
}
