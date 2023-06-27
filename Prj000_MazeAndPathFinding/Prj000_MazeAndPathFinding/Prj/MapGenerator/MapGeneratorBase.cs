using Prj000_MazeAndPathFinding.Prj.Util;
using Prj000_MazeAndPathFinding.Util;
using System;
using System.Diagnostics;

namespace Prj000_MazeAndPathFinding.Prj.MapGenerator
{
    public abstract class MapGeneratorBase
    {
        #region Create
        public static MapGeneratorBase Create(Process.Process process, int widthSize, int heightSize)
        {
            MapGeneratorBase mapAlgorithm = null;

            string mapAlgorithmName = process.MapAlgorithmName;
            var mapData = process.MapData;

            switch (mapAlgorithmName)
            {
                case "Recursive_Back_Tracking":
                    mapAlgorithm = new RecursiveBackTracking(mapData);
                    break;

                default:
                    Debug.Assert(false, $"{mapAlgorithmName} is not exist!");
                    break;
            }

            mapData.CreateMap(widthSize, heightSize);

            Debug.Assert(mapAlgorithm != null, "Map Algorithm is null!");

            return mapAlgorithm;
        }
        #endregion

        protected Util.MapData m_MapData = null;

        public char[,] MapData { get => m_MapData.Map; }
        public bool[,] Visited { get => m_MapData.Visited; }
        public ConsoleColor[,] MapColor { get => m_MapData.Color; }

        public bool bGenerateEnded { get; private set; } = false;

        public abstract void GenerateMap(double deltaTime);

        protected MapGeneratorBase(Util.MapData mapData)
        {
            m_MapData = mapData;
        }

        protected void SetMapVisited(Point pos, SingleMapInfo info)
        {
            int xPos = pos.X;
            int yPos = pos.Y;

            m_MapData.Map[xPos, yPos] = info.MapCharacter;

            if (m_MapData.StartPoint.Equals(pos))
            {
                m_MapData.Color[xPos, yPos] = m_MapData["StartPoint"].MapColor;
            }
            else if (m_MapData.EndPoint.Equals(pos))
            {
                m_MapData.Color[xPos, yPos] = m_MapData["EndPoint"].MapColor;
            }
            else
            {
                m_MapData.Color[xPos, yPos] = info.MapColor;
            }

            m_MapData.Visited[xPos, yPos] = true;
        }

        protected void EndMapGenerate()
        {
            bGenerateEnded = true;
        }
    }
}
