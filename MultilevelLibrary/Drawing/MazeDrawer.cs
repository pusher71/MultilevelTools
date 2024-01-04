using PrimitiveData3D;

namespace MultilevelLibrary.Drawing
{
    public abstract class MazeDrawer
    {
        private readonly MultilevelMaze maze;
        protected MazeDrawerConfig Config { get; }
        protected int Count => maze.Count;

        public MazeDrawer(MultilevelMaze maze, MazeDrawerConfig config)
        {
            this.maze = maze;
            Config = config;
        }

        protected int FloorImageWidth => maze.Width * Config.Step + Config.Weight; //ширина картинки этажа
        protected int FloorImageHeight => maze.Height * Config.Step + Config.Weight; //высота картинки этажа

        //отрисовать этаж
        public void DrawFloor(int floor)
        {
            if (Config.DrawRoofContour && floor == maze.CountInside)
                DrawContour(Utils.IndexWall);
            for (int y = 0; y <= maze.Height * 2; y++)
                for (int x = 0; x <= maze.Width * 2; x++)
                    DrawCell(new Vector3P(x, y, floor * 2 + 1));
        }

        //отрисовать заданную позицию
        private void DrawCell(Vector3P position)
        {
            //определить чётности рёберных звеньев
            int positionW = position.X / 2 * Config.Step + (position.X % 2 != 0 ? Config.Weight : 0);
            int positionH = position.Y / 2 * Config.Step + (position.Y % 2 != 0 ? Config.Weight : 0);
            int lengthW = position.X % 2 == 0 ? Config.Weight : Config.Step - Config.Weight;
            int lengthH = position.Y % 2 == 0 ? Config.Weight : Config.Step - Config.Weight;
            DrawingRectangle rect = new DrawingRectangle(positionW, positionH, lengthW, lengthH);
            DrawingPoint point = new DrawingPoint(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

            //получить элементы
            int itemFloor = maze.Map.Get(position);
            int itemOverlap = maze.Map.Get(position + Vector3P.Down);
            int itemFloorUnder = position.Z >= 2 ? maze.Map.Get(position + Vector3P.Down * 2) : Utils.IndexAir;
            int itemKey = Config.DrawKeyLayer && position.IsUneven() ? maze.KeyMap.Get(position / 2) : Utils.IndexAir;
            int itemFloorDirNumber = itemFloor % 10;

            //дырка в полу
            if (itemOverlap == Utils.IndexHole)
                DrawHole(point);

            //основной слой
            if (itemFloor != Utils.IndexAir)
            {
                bool onStairs = Utils.IsStairs(itemFloor);
                bool aboveStairs = Utils.IsStairs(itemFloorUnder);
                bool isSolidColor = Config.IsSolidColor(itemFloor);
                bool isTexture = Config.IsTexture(itemFloor, aboveStairs);

                if (isSolidColor) //сплошной цвет
                {
                    int colorIndex = Config.IsArrow(itemFloor) ? itemFloor / 10 : itemFloor;
                    FillRectangle(colorIndex, rect);
                    if (Config.IsArrow(itemFloor))
                        DrawArrow(itemFloorDirNumber, point);
                }
                else if (isTexture) //текстура
                {
                    if (onStairs || aboveStairs)
                    {
                        //нарисовать лестницу
                        int itemDirNumber = (aboveStairs ? itemFloorUnder : itemFloor) % 10;
                        point = ShiftPoint(point, Vector3P.FromNumber(itemDirNumber) * 30);
                        DrawStairs(itemDirNumber, point);
                    }
                    else if (Utils.IsSafetyRoom(itemFloor))
                        DrawSafetyRoom(itemFloorDirNumber, point);
                    else if (Utils.IsLift(itemFloor))
                        DrawLift(itemFloorDirNumber, point);
                    else if (Utils.IsRoof(itemFloor))
                        DrawRoof(itemFloorDirNumber, point);
                    else if (Utils.IsWindow(itemFloor))
                        DrawWindow(itemFloorDirNumber, point);
                    else if (Utils.IsFireTube(itemFloor))
                        DrawFireTube(itemFloorDirNumber, point);
                }

                //нарисовать цветные замок и ключ в комнате
                if (Utils.IsSafetyRoom(itemFloor) || Utils.IsRoof(itemFloor))
                {
                    if (Utils.IsRoof(itemFloor))
                        itemFloorDirNumber = (itemFloorDirNumber / 2 + 2) % 4;

                    Vector3P offsetDirection = Vector3P.FromNumber(itemFloorDirNumber);
                    DrawingPoint pointShifted = Config.ShiftLocksKeysColor ? ShiftPoint(point, offsetDirection * 3) : point;

                    int keyColorIndex = itemKey % 100 - 1;
                    int lockColorIndex = itemKey / 100 - 1;
                    if (keyColorIndex >= 0)
                        DrawKeyColorInRoom(itemFloorDirNumber, keyColorIndex, pointShifted);
                    if (lockColorIndex >= 0)
                        DrawLockColorInRoom(itemFloorDirNumber, lockColorIndex, pointShifted);
                }
            }

            //ключевой слой
            if (itemKey == Utils.IndexPlayer)
            {
                DrawPlayer(point);
                DrawDoorEntrance(maze.PlayerPosition.Direction.Number, point);
            }

            //слой украшений
            if (Config.DrawDecorations && position.IsUneven())
            {
                int decorIndex = maze.DecorMap.Get(position / 2);
                if (decorIndex != 0)
                    DrawDecoration(decorIndex, rect);
            }
        }

        //сместить точку отрисовывания текстуры
        private DrawingPoint ShiftPoint(DrawingPoint point, Vector3P offset)
        {
            point.X += offset.X;
            point.Y += offset.Y;
            return point;
        }

        //перевести точку в прямоугольник по размерам текстуры
        protected DrawingRectangle ConvertPointToRectangle(DrawingPoint point, int textureWidth, int textureHeight) =>
            new DrawingRectangle(point.X - textureWidth / 2, point.Y - textureHeight / 2, textureWidth, textureHeight);

        protected abstract void DrawContour(int colorIndex); //нарисовать прямоугольник по контуру
        protected abstract void FillRectangle(int colorIndex, DrawingRectangle rect); //залить прямоугольник
        protected abstract void DrawArrow(int dirNumber, DrawingPoint point); //нарисовать стрелку
        protected abstract void DrawStairs(int dirNumber, DrawingPoint point); //нарисовать лестницу
        protected abstract void DrawSafetyRoom(int dirNumber, DrawingPoint point); //нарисовать безопасную комнату
        protected abstract void DrawLift(int dirNumber, DrawingPoint point); //нарисовать лифт
        protected abstract void DrawRoof(int dirNumber, DrawingPoint point); //нарисовать выходную будку
        protected abstract void DrawWindow(int dirNumber, DrawingPoint point); //нарисовать окно
        protected abstract void DrawFireTube(int dirNumber, DrawingPoint point); //нарисовать пожарный шест
        protected abstract void DrawLockColorInRoom(int dirNumber, int colorIndex, DrawingPoint point); //нарисовать цветной замок в комнате
        protected abstract void DrawKeyColorInRoom(int dirNumber, int colorIndex, DrawingPoint point); //нарисовать цветной ключ в комнате
        protected abstract void DrawHole(DrawingPoint point); //нарисовать дырку в полу
        protected abstract void DrawPlayer(DrawingPoint point); //нарисовать игрока
        protected abstract void DrawDoorEntrance(int dirNumber, DrawingPoint point); //нарисовать входную дверь
        protected abstract void DrawDecoration(int decorIndex, DrawingRectangle rect); //нарисовать украшение
    }
}
