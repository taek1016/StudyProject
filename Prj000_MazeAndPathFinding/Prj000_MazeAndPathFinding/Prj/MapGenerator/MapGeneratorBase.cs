using Prj000_MazeAndPathFinding.Prj.Util;
using Prj000_MazeAndPathFinding.Util;
using System;
using System.Diagnostics;
using static Prj000_MazeAndPathFinding.Prj.State.MapGenerating;

namespace Prj000_MazeAndPathFinding.Prj.MapGenerator
{
    public abstract class MapGeneratorBase
    {
        #region Create
        public static MapGeneratorBase Create(Process.Process process, MapGeneratorType mapGenerateType, int widthSize, int heightSize)
        {
            MapGeneratorBase mapAlgorithm = null;

            var mapData = process.MapData;

            switch (mapGenerateType)
            {
                case MapGeneratorType.Recursive_Back_Tracking:
                    mapAlgorithm = new RecursiveBackTracking(mapData);
                    break;

                case MapGeneratorType.Binary_Tree:
                    break;

                case MapGeneratorType.SideWinder:
                    break;

                case MapGeneratorType.Recursive_Division:
                    break;

                case MapGeneratorType.Eller:
                    break;

                case MapGeneratorType.Wilson:
                    break;

                case MapGeneratorType.Hunt_and_Kill:
                    break;

                case MapGeneratorType.Growing_Tree:
                    break;

                case MapGeneratorType.Aldous_Broder:
                    break;

                case MapGeneratorType.Prim:
                    break;

                case MapGeneratorType.Kruskal:
                    break;

                default:
                    Debug.Assert(false, $"{mapGenerateType} is not exist!");
                    break;
            }

            mapData.CreateMap(widthSize, heightSize);

            Debug.Assert(mapAlgorithm != null, "Map Algorithm is null!");

            return mapAlgorithm;
        }
        #endregion

        protected MapData m_MapData = null;

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
