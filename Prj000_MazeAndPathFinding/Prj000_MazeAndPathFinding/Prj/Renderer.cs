using Prj000_MazeAndPathFinding.Util;
using System;
using System.Collections.Generic;

namespace Prj000_MazeAndPathFinding.Prj
{
    public class Renderer
    {
        #region MapData
        List<List<char>> m_RenderMap = new List<List<char>>();
        List<List<ConsoleColor>> m_Color = new List<List<ConsoleColor>>();

        ConsoleColor m_CurrentColor;
        #endregion

        #region MapInfo
        private struct MapInfo
        {
            public string Msg;
            public ConsoleColor Color;
            public string Name;
        }

        List<MapInfo> m_Info = new List<MapInfo>();
        #endregion
        public void Init()
        {
            Console.CursorVisible = false;
        }

        public void SetSize(int heightSize, int widthSize)
        {
            m_RenderMap.Clear();
            m_Color.Clear();

            m_RenderMap = new List<List<char>>();
            m_Color = new List<List<ConsoleColor>>();

            for (int i = 0; i < heightSize; ++i)
            {
                m_RenderMap.Add(new List<char>());
                m_Color.Add(new List<ConsoleColor>());

                for (int j = 0; j < widthSize; ++j)
                {
                    m_RenderMap[i].Add(' ');
                    m_Color[i].Add(ConsoleColor.Black);
                }
            }
        }

        public void SetMap(char map, Point point, ConsoleColor color = ConsoleColor.White)
        {
            m_RenderMap[point.Y][point.X] = map;
            m_Color[point.Y][point.X] = color;
        }

        public void SetMap(string data, Point pos, ConsoleColor color = ConsoleColor.White)
        {
            int length = data.Length;

            for (int i = 0; i < length; ++i)
            {
                int currentHeight = pos.Y;

                while (m_RenderMap.Count <= currentHeight)
                {
                    m_RenderMap.Add(new List<char>());
                    m_Color.Add(new List<ConsoleColor>());
                }

                int currentHeightWidth = m_RenderMap[pos.Y].Count;
                if (currentHeightWidth <= pos.X + i)
                {
                    for (int j = currentHeightWidth; j < pos.X; ++j)
                    {
                        m_RenderMap[currentHeight].Add(' ');
                        m_Color[currentHeight].Add(ConsoleColor.Black);
                    }

                    m_RenderMap[currentHeight].Add(data[i]);
                    m_Color[currentHeight].Add(color);
                }
                else
                {
                    m_RenderMap[pos.Y][pos.X + i] = data[i];
                    m_Color[pos.Y][pos.X + i] = color;
                }
            }
        }

        public void Destroy()
        {
            Console.CursorVisible = true;

            Console.ForegroundColor = ConsoleColor.White;

            m_RenderMap.Clear();
            m_Color.Clear();
        }

        public void ClearMap()
        {
            Console.Clear();

            m_Info.Clear();

            m_CurrentColor = ConsoleColor.Black;

            m_RenderMap.Clear();
            m_Color.Clear();
        }

        public void Render()
        {
            Console.SetCursorPosition(0, 0);

            int heightSize = m_RenderMap.Count;
            
            for (int i = 0; i < heightSize; ++i)
            {
                int widthSize = m_RenderMap[i].Count;

                for (int j = 0; j < widthSize; ++j)
                {
                    if (m_CurrentColor != m_Color[i][j])
                    {
                        m_CurrentColor = m_Color[i][j];
                        Console.ForegroundColor = m_CurrentColor;
                    }

                    Console.Write(m_RenderMap[i][j]);
                }

                Console.WriteLine();
            }

            int infoSize = m_Info.Count;
            for (int i = 0; i < infoSize; ++i)
            {
                int widthPos = m_RenderMap.Count;

                Console.SetCursorPosition(widthPos + 2, i + 1);

                Console.Write($"{m_Info[i].Name} : ");

                Console.ForegroundColor = m_Info[i].Color;

                Console.Write(m_Info[i].Msg);

                Console.ForegroundColor = ConsoleColor.White;
            }

            int top = m_RenderMap.Count > 0 ? m_RenderMap.Count : 0;
            int left = m_RenderMap.Count > 0 ? m_RenderMap[top - 1].Count : 0;

            Console.SetCursorPosition(left, top);
        }
    }
}
