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

        //текстура игрока
        private readonly Bitmap texturePlayer20 = Properties.Resources.player20;
        private readonly Bitmap texturePlayer50 = Properties.Resources.player50;
        private Bitmap texturePlayer;

        //цвета замков и ключей
        private readonly Color[] keyColors = new Color[]
        {
            Color.Blue,
            Color.Red,
            Color.Yellow,
            Color.Lime,
            Color.Magenta,
            Color.Orange,
            Color.White,
            Color.Aqua,
            Color.FromArgb(112, 48, 160),
            Color.FromArgb(170, 255, 0),
            Color.FromArgb(0, 255, 128),
            Color.DarkRed,
            Color.SaddleBrown,
            Color.Green,
            Color.Gray,
            Color.FromArgb(32, 32, 32)
        };

        //текстуры цветных замков
        private readonly Bitmap[] texturesLockColor20 = new Bitmap[]
        {
            Properties.Resources.lock_color20,
            Properties.Resources.lock_color20,
            Properties.Resources.lock_color20,
            Properties.Resources.lock_color20
        };
        private readonly Bitmap[] texturesLockColor50 = new Bitmap[]
        {
            Properties.Resources.lock_color50_h,
            Properties.Resources.lock_color50_v,
            Properties.Resources.lock_color50_h,
            Properties.Resources.lock_color50_v
        };
        private Bitmap[] texturesLockColor;

        //текстуры цветных ключей
        private readonly Bitmap[] texturesKeyColor20 = new Bitmap[]
        {
            Properties.Resources.key_color20,
            Properties.Resources.key_color20,
            Properties.Resources.key_color20,
            Properties.Resources.key_color20
        };
        private readonly Bitmap[] texturesKeyColor50 = new Bitmap[]
        {
            Properties.Resources.key_color50_v,
            Properties.Resources.key_color50_h,
            Properties.Resources.key_color50_v,
            Properties.Resources.key_color50_h
        };
        private Bitmap[] texturesKeyColor;

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

        //текстуры 20 иконок этажных стилей
        private readonly Bitmap[] layerStyles20 = new Bitmap[]
        {
            Properties.Resources.layer_style_1,
            Properties.Resources.layer_style_2,
            Properties.Resources.layer_style_3,
            Properties.Resources.layer_style_4,
            Properties.Resources.layer_style_5,
            Properties.Resources.layer_style_6,
            Properties.Resources.layer_style_7,
            Properties.Resources.layer_style_8,
            Properties.Resources.layer_style_9,
            Properties.Resources.layer_style_10,
            Properties.Resources.layer_style_11,
            Properties.Resources.layer_style_12,
            Properties.Resources.layer_style_13,
            Properties.Resources.layer_style_14
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
            texturesWindow = forGame ? texturesWindow50 : texturesWindow20;
            texturePlayer = forGame ? texturePlayer50 : texturePlayer20;
            texturesLockColor = forGame ? texturesLockColor50 : texturesLockColor20;
            texturesKeyColor = forGame ? texturesKeyColor50 : texturesKeyColor20;
            textureHole = forGame ? textureHole50 : textureHole20;
        }

        public Bitmap Draw(bool drawLayerStyles, int[] layerStyles)
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

                //иконка этажного стиля
                if (drawLayerStyles && !forGame)
                {
                    Point layerStylePoint = new Point(i % CountX * PeriodX + FloorImageWidth - 24, i / CountX * PeriodY + 4);
                    gImage.DrawImage(layerStyles20[layerStyles[i * 2 + 1]], layerStylePoint);
                }
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

        //окрасить текстуру
        private Bitmap PaintTexture(Bitmap original, Color color)
        {
            Bitmap texture = new Bitmap(original);
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    if (texture.GetPixel(x, y) == Color.FromArgb(255, 255, 255, 255))
                        texture.SetPixel(x, y, color);

            return texture;
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
        protected override void DrawStairs(int dirNumber, DrawingPoint point) => DrawImage(texturesStairs50[dirNumber], point);
        protected override void DrawSafetyRoom(int dirNumber, DrawingPoint point) => DrawImage(texturesSafetyRoom50[dirNumber], point);
        protected override void DrawLift(int dirNumber, DrawingPoint point) => DrawImage(texturesLift50[dirNumber], point);
        protected override void DrawRoof(int dirNumber, DrawingPoint point) => DrawImage(texturesRoof50[dirNumber], point);
        protected override void DrawFireTube(int dirNumber, DrawingPoint point) => DrawImage(texturesFireTube50[dirNumber], point);
        protected override void DrawWindow(int dirNumber, DrawingPoint point) => DrawImage(texturesWindow[dirNumber], point);
        protected override void DrawLockColorInRoom(int dirNumber, int colorIndex, DrawingPoint point) => DrawImage(PaintTexture(texturesLockColor[dirNumber], keyColors[colorIndex]), point);
        protected override void DrawKeyColorInRoom(int dirNumber, int colorIndex, DrawingPoint point) => DrawImage(PaintTexture(texturesKeyColor[dirNumber], keyColors[colorIndex]), point);
        protected override void DrawHole(DrawingPoint point) => DrawImage(textureHole, point);
        protected override void DrawPlayer(DrawingPoint point) => DrawImage(texturePlayer, point);

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
