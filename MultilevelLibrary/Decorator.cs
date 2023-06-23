using System.Collections.Generic;
using PrimitiveData3D;

namespace MultilevelLibrary
{
    static class Decorator
    {
        //набор ограничений размещения украшений
        private struct DecorMask
        {
            public bool Center { get; } //ставить на центр
            public bool Sides { get; } //ставить на стороны
            public bool Angles { get; } //ставить на углы
            public int Chance { get; } //шанс появления [0..10]

            public DecorMask(bool center, bool sides, bool angles, int chance)
            {
                Center = center;
                Sides = sides;
                Angles = angles;
                Chance = chance;
            }
        }

        //правила размещения украшений для каждого стиля
        private static readonly DecorMask[] decorMasks = new DecorMask[14]
        {
            new DecorMask(true, true, true, 5), // 1
            new DecorMask(true, true, true, 5), // 2
            new DecorMask(true, true, false, 3), // 3
            new DecorMask(false, true, true, 5), // 4
            new DecorMask(true, true, true, 5), // 5
            new DecorMask(true, true, true, 5), // 6
            new DecorMask(true, true, true, 5), // 7
            new DecorMask(false, true, false, 5), // 8
            new DecorMask(true, true, false, 3), // 9
            new DecorMask(true, true, true, 3), // 10
            new DecorMask(true, true, false, 3), // 11
            new DecorMask(true, false, false, 2), // 12
            new DecorMask(true, true, true, 5), // 13
            new DecorMask(true, true, true, 5) // 14
        };

        //украсить этаж
        public static void DecorateFloor(MultilevelMaze maze, int floor, int style)
        {
            DecorMask decorMask = decorMasks[style];

            //поиск годящихся мест
            for (int y = 0; y < maze.Height; y++)
                for (int x = 0; x < maze.Width; x++)
                {
                    Vector3P position = new Vector3P(x, y, floor) * 2 + 1; //текущая позиция
                    if (maze.Map.Get(position) == Utils.IndexAir &&
                        maze.KeyMap.Get(position / 2) == Utils.IndexAir && //должна быть свободной
                        maze.Map.Get(position + Vector3P.Down) == Utils.IndexWall) //и не над дыркой
                    {
                        //получить набор максимум 9 смещений
                        List<int> offsets = new List<int>();
                        for (int i = 0; i < 4; i++)
                            if (maze.IsWallExist(position, Vector3P.FromNumber(i)))
                            {
                                if (decorMask.Sides) offsets.Add(i * 2);
                                if (decorMask.Angles) offsets.Add(i * 2 + 1);
                            }
                        if (decorMask.Center) offsets.Add(8);

                        //если нашлась хотя бы одна позиция
                        if (offsets.Count > 0)
                        {
                            //установить украшение по смещению
                            int offsetIndex = offsets[r.Next(offsets.Count)];
                            if (offsetIndex != 8 || r.Next(10) < decorMask.Chance) //применить шанс только к центральной позиции
                            {
                                int decorIndex = offsetIndex == 8 ? 1 : r.Next(2, 6);
                                maze.DecorMap.Set(position / 2, offsetIndex * 100 + decorIndex);
                            }
                        }
                    }
                }
        }
    }
}
