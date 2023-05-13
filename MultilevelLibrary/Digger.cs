using System.Collections.Generic;
using PrimitiveData3D;

namespace MultilevelLibrary
{
    static class Digger
    {
        private static MultilevelMaze maze;
        private static List<Vector3P> walls;
        private static List<Vector3P> airs;

        public static void Dig(MultilevelMaze m, int z)
        {
            maze = m;

            //создать списки стен и пустот
            walls = maze.GetWallPositions(z / 2);
            airs = new List<Vector3P>();

            //прокопать стартовую позицию
            Vector3P startPos = walls[r.Next(walls.Count)];
            maze.Map.Set(startPos, Utils.IndexAir);
            walls.Remove(startPos);
            airs.Add(startPos);

            //прокопать остальную площадь
            while (walls.Count > 0)
                DigRandom(airs[r.Next(airs.Count)], Constants.DIGGER_ENERGY);
        }

        //копать по случайному направлению
        private static void DigRandom(Vector3P position, int energy)
        {
            if (energy > 0 && GetDigDirections(position, out List<Vector3P> dirs)) //если нашлись доступные направления
            {
                Vector3P direction = dirs[r.Next(dirs.Count)];

                //прокопать стену и соседнюю позицию
                position += direction;
                maze.Map.Set(position, Utils.IndexAir);
                position += direction;
                maze.Map.Set(position, Utils.IndexAir);
                walls.Remove(position);
                airs.Add(position);

                DigRandom(position, energy - 1);
            }
        }

        //получить список доступных направлений для копания
        private static bool GetDigDirections(Vector3P position, out List<Vector3P> dirs)
        {
            dirs = new List<Vector3P>();
            for (int i = 0; i < 4; i++)
            {
                Vector3P direction = Vector3P.FromNumber(i);
                if (maze.IsWallWalkerCanGo(position, direction))
                    dirs.Add(direction);
            }
            return dirs.Count > 0;
        }
    }
}
