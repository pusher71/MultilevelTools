using PrimitiveData3D;

namespace MultilevelLibrary
{
    static class CliqueController
    {
        private static MultilevelMaze maze; //сам лабиринт
        private static MazeMap<bool> cliqueMap; //карта посещаемости

        //проинициализировать
        public static void Init(MultilevelMaze m)
        {
            maze = m;
            cliqueMap = new MazeMap<bool>(maze.Width, maze.Height, maze.Count);
        }

        //этаж имеет единую площадь
        public static bool IsCliqueOne(Vector3P start)
        {
            start /= 2;

            //сбросить карту посещаемости
            for (int y = 0; y < cliqueMap.Height; y++)
                for (int x = 0; x < cliqueMap.Width; x++)
                    cliqueMap.Set(x, y, start.Z, false);

            //построить карту посещаемости от заданной точки
            Go(start);

            //проверить, остались ли непосещённые стены
            bool single = true;
            for (int y = 0; y < cliqueMap.Height; y++)
                for (int x = 0; x < cliqueMap.Width; x++)
                {
                    Vector3P position = new Vector3P(x, y, start.Z);
                    if (maze.Map.Get(position * 2 + 1) == Utils.IndexWall && !cliqueMap.Get(position))
                        single = false;
                }
            return single;
        }

        //построить карту посещаемости
        private static void Go(Vector3P position)
        {
            if (!cliqueMap.Get(position))
            {
                cliqueMap.Set(position, true);
                for (int i = 0; i < 4; i++)
                    if (maze.IsWallWalkerCanGo(position * 2 + 1, Vector3P.FromNumber(i)))
                        Go(position + Vector3P.FromNumber(i));
            }
        }
    }
}
