﻿using System;

namespace MultilevelLibrary
{
    public static class Utils
    {
        //индексы массива лабиринта
        public const int IndexAir = 0; //пустота
        public const int IndexWall = 1; //стена
        public const int IndexStairsP = 2; //пассивный блок лестницы
        public const int IndexStairs = 20; //отсчёт ступенек
        public const int IndexSafetyRoom = 30; //отсчёт безопасных комнат
        public const int IndexRadarRoom = 40; //отсчёт радарных комнат
        public const int IndexSaveRoom = 50; //отсчёт сохраняющих комнат
        public const int IndexLift = 60; //отсчёт лифтов
        public const int IndexRoof = 70; //отсчёт выходных будок
        public const int IndexCamera = 80; //отсчёт камер
        public const int IndexWindow = 90; //отсчёт окон
        public const int IndexFireTube = 100; //отсчёт пожарных шестов

        //индексы ключевой карты
        public const int IndexPlayer = 1; //игрок
        public const int IndexEnemy = 2; //враг
        public const int IndexKey = 3; //ключ
        public const int IndexBottle = 4; //энергетик

        //элемент является ведущим углом лестницы
        public static bool IsStairs(int item) => item >= IndexStairs && item < IndexStairs + 4;

        //элемент является безопасной комнатой
        public static bool IsSafetyRoom(int item) => item >= IndexSafetyRoom && item < IndexSafetyRoom + 4;

        //элемент является радарной комнатой
        public static bool IsRadarRoom(int item) => item >= IndexRadarRoom && item < IndexRadarRoom + 4;

        //элемент является сохраняющей комнатой
        public static bool IsSaveRoom(int item) => item >= IndexSaveRoom && item < IndexSaveRoom + 4;

        //элемент является лифтом
        public static bool IsLift(int item) => item >= IndexLift && item < IndexLift + 4;

        //элемент является выходной будкой
        public static bool IsRoof(int item) => item >= IndexRoof && item < IndexRoof + 8;

        //элемент является камерой
        public static bool IsCamera(int item) => item >= IndexCamera && item < IndexCamera + 4;

        //элемент является окном
        public static bool IsWindow(int item) => item >= IndexWindow && item < IndexWindow + 4;

        //элемент является пожарным шестов
        public static bool IsFireTube(int item) => item >= IndexFireTube && item < IndexFireTube + 4;

        //элемент является любой комнатой
        public static bool IsAnyRoom(int item) => IsSafetyRoom(item) || IsSaveRoom(item) || IsRadarRoom(item);

        //получить максимальный этаж для радарных комнат
        public static int GetRadarFloorMax(int count, bool liftExists) => (liftExists ? count : (int)Math.Ceiling(count * 2f / 3f)) - 1;

        //получить стили слоёв по вертикальным размерам помещения
        public static int[] GetLayerStyles(MultilevelMaze maze)
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
                    int floor = i / 2;
                    int style = floor < maze.CountInside - stylesOrder.Length
                        ? floor % Constants.STYLES_COUNT
                        : stylesOrder[floor % Constants.STYLES_COUNT];
                    layerStyles[i] = style;
                }
                else //задать стиль крыши и выходной будки
                    layerStyles[i] = Constants.ROOF_STYLE;
            }

            return layerStyles;
        }
    }
}
