using System;
using System.Drawing;
using MultilevelLibrary;
using MultilevelLibrary.Drawing;

namespace MultilevelViewer
{
    public class BitmapMazeDrawer : MazeDrawer
    {
        //цвета позиций
        private readonly Color[] colors20 = new Color[]
        {
            Color.White, //пустота
            Color.Black, //стена
            Color.Yellow, //лестница
            Color.Blue, //безопасная комната
            Color.Red, //радарная комната
            Color.Lime, //сохраняющая комната
            Color.Aqua, //лифт
            Color.Orange //выходная будка
        };
        private readonly Color[] colors50 = new Color[]
        {
            Color.Black, //пустота
            Color.White, //стена
            Color.FromArgb(48, 48, 48), //лестница
            Color.LightBlue, //безопасная комната
            Color.Red, //радарная комната
            Color.Yellow, //сохраняющая комната
            Color.Aqua, //лифт
            Color.Lime //выходная будка
        };
        private Color[] colors;

        //цвет пола в межэтажном перекрытии
        private readonly Color colorOverlap20 = Color.LightGray;
        private readonly Color colorOverlap50 = Color.Black;
        private Color colorOverlap;

        //текстуры 20 стрелок
        private readonly Bitmap[] texturesArrow20 = new Bitmap[]
        {
            Properties.Resources.arrow20_up,
            Properties.Resources.arrow20_right,
            Properties.Resources.arrow20_down,
            Properties.Resources.arrow20_left
        };

        //текстуры 20 угловых стрелок
        private readonly Bitmap[] texturesArrowCorner20 = new Bitmap[]
        {
            Properties.Resources.arrow20_corner_up_ccw,
            Properties.Resources.arrow20_corner_up_cw,
            Properties.Resources.arrow20_corner_right_ccw,
            Properties.Resources.arrow20_corner_right_cw,
            Properties.Resources.arrow20_corner_down_ccw,
            Properties.Resources.arrow20_corner_down_cw,
            Properties.Resources.arrow20_corner_left_ccw,
            Properties.Resources.arrow20_corner_left_cw
        };

        //текстуры 50 лестниц
        private readonly Bitmap[] texturesStairs50 = new Bitmap[]
        {
            Properties.Resources.stairs50_up,
            Properties.Resources.stairs50_right,
            Properties.Resources.stairs50_down,
            Properties.Resources.stairs50_left
        };

        //текстуры 50 безопасных комнат
        private readonly Bitmap[] texturesSafetyRoom50 = new Bitmap[]
        {
            Properties.Resources.safety_room50_up,
            Properties.Resources.safety_room50_right,
            Properties.Resources.safety_room50_down,
            Properties.Resources.safety_room50_left
        };

        //текстуры 50 сохраняющих комнат
        private readonly Bitmap[] texturesSaveRoom50 = new Bitmap[]
        {
            Properties.Resources.save_room50_up,
            Properties.Resources.save_room50_right,
            Properties.Resources.save_room50_down,
            Properties.Resources.save_room50_left
        };

        //текстуры 50 радарных комнат
        private readonly Bitmap[] texturesRadarRoom50 = new Bitmap[]
        {
            Properties.Resources.radar_room50_up,
            Properties.Resources.radar_room50_right,
            Properties.Resources.radar_room50_down,
            Properties.Resources.radar_room50_left
        };

        //текстуры 50 лифтов
        private readonly Bitmap[] texturesLift50 = new Bitmap[]
        {
            Properties.Resources.lift50_up,
            Properties.Resources.lift50_right,
            Properties.Resources.lift50_down,
            Properties.Resources.lift50_left
        };

        //текстуры 50 крыш
        private readonly Bitmap[] texturesRoof50 = new Bitmap[]
        {
            Properties.Resources.roof50_up_ccw,
            Properties.Resources.roof50_up_cw,
            Properties.Resources.roof50_right_ccw,
            Properties.Resources.roof50_right_cw,
            Properties.Resources.roof50_down_ccw,
            Properties.Resources.roof50_down_cw,
            Properties.Resources.roof50_left_ccw,
            Properties.Resources.roof50_left_cw
        };

        //текстуры камер
        private readonly Bitmap[] texturesCamera20 = new Bitmap[]
        {
            Properties.Resources.camera20_up,
            Properties.Resources.camera20_right,
            Properties.Resources.camera20_down,
            Properties.Resources.camera20_left
        };
        private readonly Bitmap[] texturesCamera50 = new Bitmap[]
        {
            Properties.Resources.camera50_up,
            Properties.Resources.camera50_right,
            Properties.Resources.camera50_down,
            Properties.Resources.camera50_left
        };
        private Bitmap[] texturesCamera;

        //текстуры окон
        private readonly Bitmap[] texturesWindow20 = new Bitmap[]
        {
            Properties.Resources.window20_h,
            Properties.Resources.window20_v,
            Properties.Resources.window20_h,
            Properties.Resources.window20_v
        };
        private readonly Bitmap[] texturesWindow50 = new Bitmap[]
        {
            Properties.Resources.window50_h,
            Properties.Resources.window50_v,
            Properties.Resources.window50_h,
            Properties.Resources.window50_v
        };
        private Bitmap[] texturesWindow;

        //текстуры 50 пожарных шестов
        private readonly Bitmap[] texturesFireTube50 = new Bitmap[]
        {
            null,
            null,
            null,
            Properties.Resources.fire_tube50_left
        };

        //текстуры ключевых объектов
        private readonly Bitmap[] texturesKeys20 = new Bitmap[]
        {
            null,
            Properties.Resources.key20_player,
            Properties.Resources.key20_enemy,
            Properties.Resources.key20_key
        };
        private readonly Bitmap[] texturesKeys50 = new Bitmap[]
        {
            null,
            Properties.Resources.key50_player,
            Properties.Resources.key50_enemy,
            Properties.Resources.key50_key
        };
        private Bitmap[] texturesKeys;

        //текстура энергетика
        private readonly Bitmap textureBottle20 = Properties.Resources.bottle20;
        private readonly Bitmap textureBottle50 = Properties.Resources.bottle50;
        private Bitmap textureBottle;

        //текстура дырки в полу
        private readonly Bitmap textureHole20 = Properties.Resources.hole20;
        private readonly Bitmap textureHole50 = Properties.Resources.hole50;
        private Bitmap textureHole;

        //текстуры 20 входных дверей
        private readonly Bitmap[] texturesDoorEntrance20 = new Bitmap[]
        {
            Properties.Resources.door_entrance20_up,
            Properties.Resources.door_entrance20_right,
            Properties.Resources.door_entrance20_down,
            Properties.Resources.door_entrance20_left
        };

        //текстуры цифр 20 украшений
        private readonly Bitmap[] digits20 = new Bitmap[]
        {
            Properties.Resources.digit0,
            Properties.Resources.digit1,
            Properties.Resources.digit2,
            Properties.Resources.digit3,
            Properties.Resources.digit4,
            Properties.Resources.digit5,
            Properties.Resources.digit6,
            Properties.Resources.digit7,
            Properties.Resources.digit8,
            Properties.Resources.digit9
        };

        private int CountX => (int)Math.Ceiling(Count / (float)CountY);  //количество столбцов этажей
        private int CountY => forGame ? 1 : (int)Math.Floor(Math.Sqrt(Count)); //количество строк этажей
        private int PeriodX => FloorImageWidth + Config.DistanceX; //период по X
        private int PeriodY => FloorImageHeight + Config.DistanceY + Config.UpperDistance; //период по Y
        private int ImageWidth => CountX * PeriodX - Config.DistanceX; //ширина картинки особняка
        private int ImageHeight => CountY * PeriodY - Config.DistanceY; //высота картинки особняка

        private bool forGame;
        private Bitmap image;
        private Graphics gImage;
        private Bitmap floorImage;
        private Graphics gFloorImage;

        public BitmapMazeDrawer(MultilevelMaze maze, MazeDrawerConfig config, bool forGame) : base(maze, config)
        {
            this.forGame = forGame;

            colors = forGame ? colors50 : colors20;
            colorOverlap = forGame ? colorOverlap50 : colorOverlap20;
            texturesCamera = forGame ? texturesCamera50 : texturesCamera20;
            texturesWindow = forGame ? texturesWindow50 : texturesWindow20;
            texturesKeys = forGame ? texturesKeys50 : texturesKeys20;
            textureBottle = forGame ? textureBottle50 : textureBottle20;
            textureHole = forGame ? textureHole50 : textureHole20;
        }

        public Bitmap Draw()
        {
            image = new Bitmap(ImageWidth, ImageHeight);
            gImage = Graphics.FromImage(image);
            gImage.Clear(colors[Utils.IndexAir]);
            SolidBrush brushStrings = new SolidBrush(colors[Utils.IndexWall]);
            Font font = new Font("Arial", Config.FontSize);
            for (int i = 0; i < Count; i++)
            {
                //картинка
                gImage.DrawImage(DrawFloorBitmap(i), i % CountX * PeriodX, i / CountX * PeriodY + Config.UpperDistance);

                //подпись
                Point textPoint = new Point(i % CountX * PeriodX, i / CountX * PeriodY);
                if (forGame)
                {
                    textPoint.X -= 7;
                    textPoint.Y += 2;
                }
                gImage.DrawString("Floor: " + (i + 1), font, brushStrings, textPoint);
            }
            return image;
        }

        public Bitmap DrawAtlas(int countX)
        {
            int countY = (int)Math.Ceiling(Count / (float)countX);
            image = new Bitmap(countX * FloorImageWidth, countY * FloorImageHeight);
            gImage = Graphics.FromImage(image);
            gImage.Clear(colors[Utils.IndexAir]);
            for (int i = 0; i < Count; i++)
                gImage.DrawImage(DrawFloorBitmap(i), i % countX * FloorImageWidth, i / countX * FloorImageHeight);
            return image;
        }

        public Bitmap DrawFloorBitmap(int index)
        {
            floorImage = new Bitmap(FloorImageWidth, FloorImageHeight);
            gFloorImage = Graphics.FromImage(floorImage);
            gFloorImage.Clear(colorOverlap);
            DrawFloor(index);
            return floorImage;
        }

        //перевести в прямоугольник System.Drawing.Rectangle
        private Rectangle ConvertToSystemDrawingRectangle(DrawingRectangle rect) =>
            new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);

        //отразить прямоугольник по-вертикали
        private DrawingRectangle MirrorRectangleVertical(DrawingRectangle rect)
        {
            rect.Y = FloorImageHeight - rect.Y - rect.Height;
            return rect;
        }

        //отразить точку по-вертикали
        private DrawingPoint MirrorPointVertical(DrawingPoint point)
        {
            point.Y = FloorImageHeight - point.Y;
            return point;
        }

        //нарисовать текстуру
        private void DrawImage(Bitmap texture, DrawingPoint point) =>
            gFloorImage.DrawImage(texture, ConvertToSystemDrawingRectangle(ConvertPointToRectangle(MirrorPointVertical(point), texture.Width, texture.Height)));

        protected override void DrawContour(int colorIndex) =>
            gFloorImage.DrawRectangle(new Pen(colors[colorIndex], 2), 0, 0, floorImage.Width, floorImage.Height);
        protected override void FillRectangle(int colorIndex, DrawingRectangle rect) =>
            gFloorImage.FillRectangle(new SolidBrush(colors[colorIndex]), ConvertToSystemDrawingRectangle(MirrorRectangleVertical(rect)));
        protected override void DrawArrow(int dirNumber, DrawingPoint point) => DrawImage(texturesArrow20[dirNumber], point);
        protected override void DrawArrowCorner(int dirNumber, DrawingPoint point) => DrawImage(texturesArrowCorner20[dirNumber], point);
        protected override void DrawStairs(int dirNumber, DrawingPoint point) => DrawImage(texturesStairs50[dirNumber], point);
        protected override void DrawSafetyRoom(int dirNumber, DrawingPoint point) => DrawImage(texturesSafetyRoom50[dirNumber], point);
        protected override void DrawSaveRoom(int dirNumber, DrawingPoint point) => DrawImage(texturesSaveRoom50[dirNumber], point);
        protected override void DrawRadarRoom(int dirNumber, DrawingPoint point) => DrawImage(texturesRadarRoom50[dirNumber], point);
        protected override void DrawLift(int dirNumber, DrawingPoint point) => DrawImage(texturesLift50[dirNumber], point);
        protected override void DrawRoof(int dirNumber, DrawingPoint point) => DrawImage(texturesRoof50[dirNumber], point);
        protected override void DrawCamera(int dirNumber, DrawingPoint point) => DrawImage(texturesCamera[dirNumber], point);
        protected override void DrawFireTube(int dirNumber, DrawingPoint point) => DrawImage(texturesFireTube50[dirNumber], point);
        protected override void DrawWindow(int dirNumber, DrawingPoint point) => DrawImage(texturesWindow[dirNumber], point);
        protected override void DrawBottleInRoom(DrawingPoint point) => DrawImage(textureBottle, point);
        protected override void DrawHole(DrawingPoint point) => DrawImage(textureHole, point);
        protected override void DrawKey(int itemKey, DrawingPoint point) => DrawImage(texturesKeys[itemKey], point);

        protected override void DrawDoorEntrance(int dirNumber, DrawingPoint point)
        {
            if (!forGame)
                DrawImage(texturesDoorEntrance20[dirNumber], point);
        }

        protected override void DrawDecoration(int decorIndex, DrawingRectangle rect)
        {
            rect = MirrorRectangleVertical(rect);
            gFloorImage.DrawImage(digits20[decorIndex % 10], rect.X + 13, rect.Y + 1);
            decorIndex /= 10;
            gFloorImage.DrawImage(digits20[decorIndex % 10], rect.X + 7, rect.Y + 1);
            gFloorImage.DrawImage(digits20[decorIndex / 10], rect.X + 1, rect.Y + 1);
        }
    }
}
