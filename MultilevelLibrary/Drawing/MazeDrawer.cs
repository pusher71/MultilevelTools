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
            int itemOverlap = maze.Map.Get(position);
            int itemFloorUnder = position.Z >= 2 ? maze.Map.Get(position + Vector3P.Down * 2) : Utils.IndexAir;
            int itemKey = Config.DrawKeyLayer && position.IsUneven() ? maze.KeyMap.Get(position / 2) : Utils.IndexAir;
            int itemFloorDirNumber = itemFloor % 10;

            //засекретить тип комнаты
            if (!Config.DrawSafetyRoomsType)
            {
                if (Utils.IsSaveRoom(itemFloor)) itemFloor = itemFloor - Utils.IndexSaveRoom + Utils.IndexSafetyRoom;
                else if (Utils.IsRadarRoom(itemFloor)) itemFloor = itemFloor - Utils.IndexRadarRoom + Utils.IndexSafetyRoom;
            }

            //пол на перекрытии имеется
            if (Config.DrawOverlaps && itemOverlap == 1)
                FillRectangle(-1, rect);

            //основной слой
            if (itemFloor != Utils.IndexAir)
            {
                bool onStairs = Utils.IsStairs(itemFloor);
                bool aboveStairs = Utils.IsStairs(itemFloorUnder);
                bool isSolidColor = Config.IsSolidColor(itemFloor);
                bool isTexture = Config.IsTexture(itemFloor, aboveStairs);

                if (isSolidColor) //сплошной цвет
                {
                    int colorIndex = (Config.IsArrow(itemFloor) || Config.IsArrowCorner(itemFloor)) ? itemFloor / 10 : itemFloor;
                    FillRectangle(colorIndex, rect);
                    if (Config.IsArrow(itemFloor))
                        DrawArrow(itemFloorDirNumber, point);
                    else if (Config.IsArrowCorner(itemFloor))
                        DrawArrowCorner(itemFloorDirNumber, point);
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
                    else if (Utils.IsSaveRoom(itemFloor))
                        DrawSaveRoom(itemFloorDirNumber, point);
                    else if (Utils.IsRadarRoom(itemFloor))
                        DrawRadarRoom(itemFloorDirNumber, point);
                    else if (Utils.IsLift(itemFloor))
                        DrawLift(itemFloorDirNumber, point);
                    else if (Utils.IsRoof(itemFloor))
                        DrawRoof(itemFloorDirNumber, point);
                    else if (Utils.IsCamera(itemFloor))
                        DrawCamera(itemFloorDirNumber, point);
                    else if (Utils.IsWindow(itemFloor))
                        DrawWindow(itemFloorDirNumber, point);
                    else if (Utils.IsFireTube(itemFloor))
                        DrawFireTube(itemFloorDirNumber, point);
                }

                //нарисовать энергетик в комнате
                if (Utils.IsAnyRoom(itemFloor) && itemKey == Utils.IndexBottle)
                {
                    if (Config.ShiftBottleTexture)
                        point = ShiftPoint(point, Vector3P.FromNumber(itemFloorDirNumber) * 17);
                    DrawBottleInRoom(point);
                }
            }

            //ключевой слой
            if (itemKey != Utils.IndexAir && itemKey != Utils.IndexBottle)
            {
                DrawKey(itemKey, point);
                if (itemKey == Utils.IndexPlayer)
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
        protected abstract void DrawArrowCorner(int dirNumber, DrawingPoint point); //нарисовать угловую стрелку
        protected abstract void DrawStairs(int dirNumber, DrawingPoint point); //нарисовать лестницу
        protected abstract void DrawSafetyRoom(int dirNumber, DrawingPoint point); //нарисовать безопасную комнату
        protected abstract void DrawSaveRoom(int dirNumber, DrawingPoint point); //нарисовать сохраняющую комнату
        protected abstract void DrawRadarRoom(int dirNumber, DrawingPoint point); //нарисовать радарную комнату
        protected abstract void DrawLift(int dirNumber, DrawingPoint point); //нарисовать лифт
        protected abstract void DrawRoof(int dirNumber, DrawingPoint point); //нарисовать выходную будку
        protected abstract void DrawCamera(int dirNumber, DrawingPoint point); //нарисовать камеру
        protected abstract void DrawWindow(int dirNumber, DrawingPoint point); //нарисовать окно
        protected abstract void DrawFireTube(int dirNumber, DrawingPoint point); //нарисовать пожарный шест
        protected abstract void DrawBottleInRoom(DrawingPoint point); //нарисовать энергетик в комнате
        protected abstract void DrawKey(int itemKey, DrawingPoint point); //нарисовать ключевой объект
        protected abstract void DrawDoorEntrance(int dirNumber, DrawingPoint point); //нарисовать входную дверь
        protected abstract void DrawDecoration(int decorIndex, DrawingRectangle rect); //нарисовать украшение
    }
}
