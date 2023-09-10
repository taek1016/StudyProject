using Prj000_MazeAndPathFinding.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Prj000_MazeAndPathFinding.Prj.Util
{
    public class SingleMapInfo
    {
        public string MapStr { get; private set; }
        public char MapCharacter { get; private set; }
        public ConsoleColor MapColor { get; private set; }

        public SingleMapInfo(string mapStr, char mapCharacter, ConsoleColor color)
        {
            MapStr = mapStr;
            MapCharacter = mapCharacter;
            MapColor = color;
        }

        public SingleMapInfo Clone()
        {
            SingleMapInfo mapInfo = new SingleMapInfo(MapStr, MapCharacter, MapColor);

            Debug.Assert(mapInfo != null, "SingleMapInfo Clone is null!");

            return mapInfo;
        }
    }

    #region MapInfo
    public class MapInfo
    {
        internal MapInfo()
        {
            m_MapInfo = new Dictionary<string, SingleMapInfo>();
        }

        internal MapInfo(MapInfo other)
        {
            m_MapInfo = new Dictionary<string, SingleMapInfo>();

            foreach (var item in other.m_MapInfo)
            {
                m_MapInfo.Add(item.Key, item.Value.Clone());
            }
        }

        public void Init()
        {
            m_MapInfo.Clear();
        }

        public void Add(string key, char mapCharacter, ConsoleColor color)
        {
            m_MapInfo.Add(key, new SingleMapInfo(key, mapCharacter, color));
        }
        
        public SingleMapInfo this[string key]
        {
            get
            {
                return m_MapInfo[key];
            }
        }

        Dictionary<string, SingleMapInfo> m_MapInfo = null;
    }
    #endregion

    public class MapData
    {
        public MapData()
        {
            m_Info = new MapInfo();
        }

        // PathFinding하면서 Map 정보 추가하기 위해서 필요해짐
        public MapData(MapData other)
        {
            m_Info = new MapInfo(other.m_Info);

            CreateMap(other.m_WidthSize, other.m_HeightSize);

            for (int i = 0; i < m_WidthSize; ++i)
            {
                for (int j = 0; j < m_HeightSize; ++j)
                {
                    m_MapData[i, j] = other.m_MapData[i, j];
                    m_Color[i, j] = other.m_Color[i, j];
                    m_Visited[i, j] = other.m_Visited[i, j];
                }
            }

            m_StartPoint = other.m_StartPoint.Copy();
            m_EndPoint = other.m_EndPoint.Copy();
        }

        private MapInfo m_Info = null;

        public MapInfo Info { get => m_Info; }

        int m_WidthSize = -1;
        public int WidthSize { get => m_WidthSize; }

        int m_HeightSize = -1;
        public int HeightSize { get => m_HeightSize; }

        protected char[,] m_MapData = null;
        public char[,] Map { get => m_MapData; }

        protected ConsoleColor[,] m_Color = null;
        public ConsoleColor[,] Color { get => m_Color; set => m_Color = value; }

        protected bool[,] m_Visited = null;
        public bool[,] Visited { get => m_Visited; set => m_Visited = value; }

        protected Point m_StartPoint = new Point();
        public Point StartPoint { get => m_StartPoint; set => m_StartPoint = value; }

        protected Point m_EndPoint = new Point();
        public Point EndPoint { get => m_EndPoint; set => m_EndPoint = value; }

        public void CreateMap(int widthSize, int heightSize)
        {
            m_StartPoint.X = -1;
            m_StartPoint.Y = -1;

            m_EndPoint.X = -1;
            m_EndPoint.Y = -1;

            m_WidthSize = widthSize;
            m_HeightSize = heightSize;

            m_MapData = new char[m_HeightSize, m_WidthSize];
            m_Color = new ConsoleColor[m_HeightSize, m_WidthSize];
            m_Visited = new bool[m_HeightSize, m_WidthSize];

            var wallInfo = m_Info["Wall"];
            Debug.Assert(wallInfo != null, "Wall is not defined");

            var emptyInfo = m_Info["Empty"];
            Debug.Assert(emptyInfo != null, "Empty is not defined");

            Debug.Assert(m_Info["StartPoint"] != null, "StartPoint is not defined");
            Debug.Assert(m_Info["EndPoint"] != null, "EndPoint is not defined");

            for (int i = 0; i < heightSize; ++i)
            {
                for (int j = 0; j < widthSize; ++j)
                {
                    if (i == 0 || j == 0 || i == widthSize - 1 || j == heightSize - 1)
                    {
                        m_MapData[i, j] = wallInfo.MapCharacter;
                        m_Visited[i, j] = true;
                        m_Color[i, j] = wallInfo.MapColor;
                    }
                    else if (i % 2 == 0 || j % 2 == 0)
                    {
                        m_MapData[i, j] = wallInfo.MapCharacter;
                        m_Visited[i, j] = true;
                        m_Color[i, j] = wallInfo.MapColor;
                    }
                    else
                    {
                        m_MapData[i, j] = emptyInfo.MapCharacter;
                        m_Visited[i, j] = false;
                        m_Color[i, j] = emptyInfo.MapColor;
                    }
                }
            }
        }

        public SingleMapInfo this[string key]
        {
            get
            {
                return m_Info[key];
            }
        }

        public void AddMapInfo(string key, char mapCharacter, ConsoleColor color)
        {
            m_Info.Add(key, mapCharacter, color);
        }

        public char GetWallInfo() 
        { 
            return m_Info["Wall"].MapCharacter; 
        }
    }
}