using System;

namespace MultilevelLibrary
{
    public static class Utils
    {
        //индексы массива лабиринта
        public const int IndexAir = 0; //пустота
        public const int IndexWall = 1; //стена
        public const int IndexStairsP = 2; //пассивный блок лестницы
        public const int IndexHole = 3; //дырка в полу
        public const int IndexStairs = 20; //отсчёт ступенек
        public const int IndexSafetyRoom = 30; //отсчёт безопасных комнат
        public const int IndexLift = 60; //отсчёт лифтов
        public const int IndexRoof = 70; //отсчёт выходных будок
        public const int IndexWindow = 90; //отсчёт окон
        public const int IndexFireTube = 100; //отсчёт пожарных шестов

        //индексы ключевой карты
        public const int IndexPlayer = -1; //игрок

        //элемент является ведущим углом лестницы
        public static bool IsStairs(int item) => item >= IndexStairs && item < IndexStairs + 4;

        //элемент является безопасной комнатой
        public static bool IsSafetyRoom(int item) => item >= IndexSafetyRoom && item < IndexSafetyRoom + 4;

        //элемент является лифтом
        public static bool IsLift(int item) => item >= IndexLift && item < IndexLift + 4;

        //элемент является выходной будкой
        public static bool IsRoof(int item) => item >= IndexRoof && item < IndexRoof + 8;

        //элемент является окном
        public static bool IsWindow(int item) => item >= IndexWindow && item < IndexWindow + 4;

        //элемент является пожарным шестов
        public static bool IsFireTube(int item) => item >= IndexFireTube && item < IndexFireTube + 4;

        //получить стили слоёв по вертикальным размерам помещения
        public static int[] GetLayerStyles(MultilevelMaze maze, int seed, bool layersShuffled, bool layers9)
        {
            //вычислить порядок стилей слоёв
            int[] stylesOrder = new int[maze.CountInside % Constants.STYLES_COUNT];
            if (stylesOrder.Length > 0)
            {
                Array.Copy(Constants.STYLES_ORDER_INTERNAL, stylesOrder, stylesOrder.Length);
                Array.Sort(stylesOrder);
            }

            //вычислить стили слоёв
            int[] layerStyles = new int[maze.Count * 2 + 1];
            for (int i = 0; i <= maze.Count * 2; i++)
            {
                if (i < maze.CountInside * 2)
                {
                    //задать стиль этажа
                    if (layers9)
                        layerStyles[i] = 8;
                    else
                    {
                        int floor = i / 2;
                        int style = floor < maze.CountInside - stylesOrder.Length
                            ? floor % Constants.STYLES_COUNT
                            : stylesOrder[floor % Constants.STYLES_COUNT];
                        layerStyles[i] = style;
                    }
                }
                else //задать стиль крыши и выходной будки
                    layerStyles[i] = Constants.ROOF_STYLE;
            }

            //перемешать стили слоёв
            if (layersShuffled)
            {
                r.Init(seed);
                for (int i = 0; i < maze.CountInside; i++)
                {
                    int index1 = i * 2;
                    int index2 = index1 + 1;

                    int rnd1 = r.Next(i, maze.CountInside) * 2;
                    int rnd2 = rnd1 + 1;

                    int key1 = layerStyles[index1];
                    int key2 = layerStyles[index2];
                    layerStyles[index1] = layerStyles[rnd1];
                    layerStyles[index2] = layerStyles[rnd2];
                    layerStyles[rnd1] = key1;
                    layerStyles[rnd2] = key2;
                }
            }

            return layerStyles;
        }
    }
}
