using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj000_MazeAndPathFinding.Prj.MapGenerator
{
    public abstract class MapGeneratorBase
    {
        public static MapGeneratorBase Create(string algorithmName)
        {
            MapGeneratorBase mapAlgorithm = null;

            switch (algorithmName)
            {

                default:
                    Debug.Assert(false, $"{algorithmName} is not exist!");
                    break;
            }

            Debug.Assert(false, "Map Algorithm is null!");

            return mapAlgorithm;
        }
        public MapGeneratorBase(Process.Process process)
        {

        }


    }
}
