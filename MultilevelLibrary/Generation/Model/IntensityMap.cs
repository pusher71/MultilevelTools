using System.Collections.Generic;
using PrimitiveData3D;

namespace MultilevelLibrary.Generation.Model
{
    class IntensityMap : MazeMap<int>
    {
        public IntensityMap(MultilevelMaze maze) : base(maze.Width, maze.Height, maze.Count) { }

        //распространить интенсивность по карте
        public void AddIntensity(GraphNode node, int intensity, bool temperatureMode = false)
        {
            //если значение интенсивности на карте меньше текущего
            if (Get(node.Position) < intensity)
            {
                //обновить значение интенсивности на карте
                Set(node.Position, intensity);

                //продолжить обновление на соседних вершинах
                List<Vector3P> dirs = node.GetDirections();
                for (int i = 0; i < dirs.Count; i++)
                {
                    GraphNode neighbour = node.GetNeighbour(dirs[i]);
                    if (!temperatureMode || node.IsCycledSafety || !neighbour.IsCycledSafety)
                        AddIntensity(neighbour, intensity - 1);
                }
            }
        }
    }
}
