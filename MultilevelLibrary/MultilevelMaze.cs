using System;
using System.Collections.Generic;
using PrimitiveData3D;

namespace MultilevelLibrary
{
    [Serializable]
    public class MultilevelMaze
    {
        public int Width { get; }  //длина
        public int Height { get; }  //ширина
        public int Count { get; }  //количество этажей
        public int CountInside => Count - 1; //количество внутренних этажей

        public MazeMap<int> Map { get; private set; } //карта лабиринта и элементов
        public MazeMap<int> KeyMap { get; private set; } //карта ключевых объектов
        public MazeMap<int> DecorMap { get; private set; } //карта украшений

        public LogicPos PlayerPosition { get; set; } //позиция игрока
        public int KeyCount { get; set; } //количество ключей

        public MultilevelMaze(int width, int height, int count)
        {
            Width = width;
            Height = height;
            Count = count;
            Map = new MazeMap<int>(width * 2 + 1, height * 2 + 1, count * 2 + 1);
            KeyMap = new MazeMap<int>(width, height, count);
            DecorMap = new MazeMap<int>(width, height, count);
        }

        //конструктор копирования
        public MultilevelMaze Clone()
        {
            MultilevelMaze copy = new MultilevelMaze(Width, Height, Count)
            {
                Map = Map.Clone(),
                KeyMap = KeyMap.Clone(),
                DecorMap = DecorMap.Clone(),
                PlayerPosition = PlayerPosition,
                KeyCount = KeyCount
            };
            return copy;
        }

        //установить на горизонтальный слой
        public void SetRectangle(Vector3P p1, Vector3P p2, int item, RectangleType ft)
        {
            Vector3P pStart = new Vector3P(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Min(p1.Z, p2.Z));
            Vector3P pEnd = new Vector3P(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y), Math.Max(p1.Z, p2.Z));
            for (int z = pStart.Z; z <= pEnd.Z; z++)
                for (int y = pStart.Y; y <= pEnd.Y; y++)
                    for (int x = pStart.X; x <= pEnd.X; x++)
                        if (ft == RectangleType.FILL || x == pStart.X || x == pEnd.X || y == pStart.Y || y == pEnd.Y)
                            Map.Set(x, y, z, item);
        }

        //получить список пустых позиций с условием
        internal List<LogicPos> GetEmptyPositions(EmptyCondition ec, bool shuffle)
        {
            List<LogicPos> list = new List<LogicPos>();
            for (int i = 0; i < CountInside; i++)
                list.AddRange(GetEmptyPositions(ec, i, false));

            if (shuffle)
                Shuffler<LogicPos>.ShuffleList(list);

            return list;
        }

        //получить список пустых позиций с этажа с условием
        internal List<LogicPos> GetEmptyPositions(EmptyCondition ec, int floor, bool shuffle)
        {
            List<LogicPos> list = new List<LogicPos>();
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    Vector3P position = new Vector3P(x, y, floor) * 2 + 1;
                    if (CheckPositionByEmptyType(position, ec, out List<Vector3P> dirs))
                        list.Add(new LogicPos(position, dirs[r.Next(dirs.Count)]));
                }

            if (shuffle)
                Shuffler<LogicPos>.ShuffleList(list);

            return list;
        }

        //проверить позицию на условие
        private bool CheckPositionByEmptyType(Vector3P position, EmptyCondition ec, out List<Vector3P> dirs)
        {
            if (Map.Get(position) == Utils.IndexAir && KeyMap.Get(position / 2) == Utils.IndexAir)
            {
                if (ec == EmptyCondition.DEADEND)
                    return IsDeadend(position, out dirs);
                else if (ec == EmptyCondition.ABOVE_BORDER)
                    return IsAboveBorder(position, out dirs, 2);
                else //EmptyCondition.EMPTY
                {
                    dirs = new List<Vector3P>() { Vector3P.Zero };
                    return true;
                }
            }
            else
            {
                dirs = new List<Vector3P>();
                return false;
            }
        }

        //данная позиция находится возле стены (возвращается список направлений к стенам)
        private bool IsAboveWall(Vector3P position, out List<Vector3P> dirs) =>
            GetWallsAboveCount(position, out dirs, out _) > 0;

        //данная позиция является тупиком (возвращается свободное направление)
        private bool IsDeadend(Vector3P position, out List<Vector3P> dirs) =>
            GetWallsAboveCount(position, out _, out dirs) == 3;

        //данная позиция находится возле границы (возвращается список направлений к границам)
        internal bool IsAboveBorder(Vector3P position, out List<Vector3P> dirs, int dist)
        {
            dirs = new List<Vector3P>();
            for (int i = 0; i < 4; i++)
            {
                Vector3P direction = Vector3P.FromNumber(i);
                if (IsBorder(position, direction, dist) && (dist < 2 || Map.Get(position + direction) == Utils.IndexWall))
                    dirs.Add(direction);
            }
            return dirs.Count > 0;
        }

        //получить количество стен рядом (возвращается список направлений к стенам и список свободных направлений)
        private int GetWallsAboveCount(Vector3P position, out List<Vector3P> dirsWall, out List<Vector3P> dirsEmpty)
        {
            dirsWall = new List<Vector3P>();
            dirsEmpty = new List<Vector3P>();
            for (int i = 0; i < 4; i++)
            {
                Vector3P direction = Vector3P.FromNumber(i);
                if (IsWallExist(position, direction))
                    dirsWall.Add(direction);
                else
                    dirsEmpty.Add(direction);
            }
            return dirsWall.Count;
        }

        //имеется стена по направлению
        internal bool IsWallExist(Vector3P position, Vector3P direction) =>
            !IsBorder(position, direction, 1) &&
            Map.Get(position + direction) == Utils.IndexWall;

        //граница карты находится по данному направлению
        internal bool IsBorder(Vector3P position, Vector3P direction, int dist = 2) =>
            IsOutOfRange(position + direction * dist);

        //позиция находится за пределами карты
        private bool IsOutOfRange(Vector3P position) =>
            position.X < 0 ||
            position.Y < 0 ||
            position.Z < 0 ||
            position.X > Width * 2 ||
            position.Y > Height * 2 ||
            position.Z > Count * 2;

        //получить список застроенных позиций
        internal List<Vector3P> GetWallPositions(int floor)
        {
            List<Vector3P> list = new List<Vector3P>();
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    Vector3P position = new Vector3P(x, y, floor) * 2 + 1;
                    if (Map.Get(position) == Utils.IndexWall)
                        list.Add(position);
                }
            return list;
        }

        //данная позиция является одиночной колонной
        internal bool IsSinglePillar(Vector3P position) => !IsAboveWall(position, out _);

        //может ли кто-либо пойти по данному направлению
        internal bool IsEverybodyCanGo(Vector3P position, Vector3P direction) =>
            !IsBorder(position, direction) &&
            Map.Get(position + direction) != Utils.IndexWall &&
            Map.Get(position + direction * 2) != Utils.IndexWall &&
            !Utils.IsRoof(Map.Get(position + direction * 2));

        //может ли настенный обходитель пойти по данному направлению
        internal bool IsWallWalkerCanGo(Vector3P position, Vector3P direction) =>
            !IsBorder(position, direction) &&
            Map.Get(position + direction * 2) == Utils.IndexWall;
    }
}
