using System;
using System.Collections.Generic;
using PrimitiveData3D;

namespace MultilevelLibrary
{
    public class GeneratorMain
    {
        private MultilevelMaze maze; //генерируемый лабиринт
        private List<LogicPos>[] stairsPositions; //список позиций для ступенек

        //сгенерировать помещения с лифтом и лестницами
        public void Generate(MultilevelMaze maze, int seed,
            int stairsCount, int deleteWalls,
            int liftPredel, int keyCount,
            bool layersShuffled, bool layers9, bool holesEnabled)
        {
            this.maze = maze;

            maze.SetRectangle(Vector3P.Zero, new Vector3P(maze.Width, maze.Height, maze.CountInside) * 2, Utils.IndexWall, RectangleType.FILL); //застроить стенами
            CliqueController.Init(maze); //проинициализировать контроллер клики

            //создать список позиций для ступенек
            stairsPositions = new List<LogicPos>[maze.CountInside];
            for (int i = 0; i < maze.CountInside; i++)
                stairsPositions[i] = new List<LogicPos>();
            for (int i = 0; i < 4; i++)
            {
                Vector3P direction = Vector3P.FromNumber(i);
                Vector3P pStart = Vector3P.Zero;
                Vector3P pEnd = new Vector3P(maze.Width, maze.Height, maze.CountInside);
                Vector3P offset = new Vector3P(Math.Abs(direction.X), Math.Abs(direction.Y), 0);
                pStart += offset;
                pEnd -= offset;
                if (direction.X == -1) pStart.X++;
                else if (direction.X == 1) pEnd.X--;
                else if (direction.Y == -1) pStart.Y++;
                else if (direction.Y == 1) pEnd.Y--;
                for (int z = 0; z < maze.CountInside; z++)
                    for (int y = pStart.Y; y < pEnd.Y; y++)
                        for (int x = pStart.X; x < pEnd.X; x++)
                        {
                            Vector3P position = new Vector3P(x, y, z) * 2 + 1;
                            stairsPositions[z].Add(new LogicPos(position, direction));
                        }
            }

            //поставить лифт
            if (liftPredel > 0)
            {
                //определить параметры лифта
                r.Init(seed);
                int liftDirNumber = r.Next(4);
                int minX = liftDirNumber == 3 ? maze.Width / 2 : 0;
                int maxX = liftDirNumber == 1 ? maze.Width - maze.Width / 2 : maze.Width;
                int minY = liftDirNumber == 2 ? maze.Height / 2 : 0;
                int maxY = liftDirNumber == 0 ? maze.Height - maze.Height / 2 : maze.Height;
                Vector3P liftPosition = new Vector3P(r.Next(minX, maxX), r.Next(minY, maxY), 0) * 2 + 1;
                Vector3P liftDirection = Vector3P.FromNumber(liftDirNumber);
                LogicPos liftLogicPos = new LogicPos(liftPosition, liftDirection);

                //пометить лифт на этажах
                SetLift(liftLogicPos, liftPredel * 2);
            }

            //поставить пожарный шест
            Vector3P fireTubePosition = new Vector3P(0, maze.Height - 1, maze.CountInside) * 2 + 1;
            maze.Map.Set(fireTubePosition, Vector3P.Left.Number + Utils.IndexFireTube);
            DeleteStairsPosition(fireTubePosition + Vector3P.Right * 4 + Vector3P.Down * 2, Vector3P.Left);
            DeleteStairsPosition(fireTubePosition + Vector3P.Back * 4 + Vector3P.Down * 2, Vector3P.Forward);

            //поставить лестницу на крышу
            r.Init(seed);
            LogicPos roofStairs = GenerateStairs(maze.CountInside - 1); //позиция лестницы на крышу
            Vector3P roofDirection = roofStairs.Direction; //направление лестницы на крышу

            //поставить выходную будку
            Vector3P roofPosition = roofStairs.Position + roofDirection * 4 + Vector3P.Up * 2; //позиция выходной будки
            bool ccworcw = maze.IsBorder(roofPosition, roofDirection.TurnCCW()); //вылет с крыши поворачивает угол CW
            maze.Map.Set(roofPosition, roofDirection.Number * 2 + (ccworcw ? 1 : 0) + Utils.IndexRoof);

            //огородить будку стенами и потолком
            Vector3P p1 = roofPosition + roofDirection + roofDirection.TurnCCW() + Vector3P.Down;
            Vector3P p2 = roofPosition + roofDirection.TurnOpposite() * 5 + roofDirection.TurnCW() + Vector3P.Up;
            maze.SetRectangle(p1, p2, Utils.IndexWall, RectangleType.BORDERS); //стены будки
            maze.SetRectangle(p1 + Vector3P.Up * 2, p2, Utils.IndexWall, RectangleType.FILL); //потолок будки
            maze.Map.Set(roofPosition + (ccworcw ? roofDirection.TurnCW() : roofDirection.TurnCCW()), Utils.IndexAir); //дырка на выход

            //расставить межэтажные лестницы
            r.Init(seed);
            for (int j = 0; j < stairsCount; j++) //поставить по заданному количеству лестниц
                for (int i = maze.CountInside - 2; i >= 0; i--) //на всех этажах, кроме последнего (там уже есть одна лестница на крышу)
                {
                    try
                    {
                        GenerateStairs(i);
                    }
                    catch (StairsException)
                    {
                        if (j == 0)
                            throw new GenerateException($"Ошибка. Этажи {i + 1} и {i + 2} не были связаны.");
                    }
                }

            //сгенерировать лабиринты на всех этажах
            r.Init(seed);
            for (int i = 0; i < maze.CountInside; i++)
                GenerateMaze(i * 2 + 1, deleteWalls);

            //найти доступные позиции для безопасных комнат
            r.Init(seed);
            List<LogicPos> roomPositions = maze.GetEmptyPositions(EmptyCondition.DEADEND, true);
            if (roomPositions.Count < keyCount)
                throw new GenerateException("Ошибка. Не хватает комнат для ключей.");
            maze.KeyCount = keyCount;

            //определить порядок цветов
            r.Init(seed);
            List<int> colorIndexesOrder = new List<int>();
            for (int i = 1; i <= maze.KeyCount; i++)
                colorIndexesOrder.Add(i);
            Shuffler<int>.ShuffleList(colorIndexesOrder);
            colorIndexesOrder.Insert(0, 0);

            //расставить безопасные комнаты с цветными замками и ключами
            for (int i = 0; i < maze.KeyCount; i++)
            {
                LogicPos room = roomPositions[i];
                maze.Map.Set(room.Position, room.Direction.Number + Utils.IndexSafetyRoom);
                maze.KeyMap.Set(room.Position / 2, colorIndexesOrder[i] * 100 + colorIndexesOrder[i + 1]);
            }

            //поставить цветной замок в выходную будку
            maze.KeyMap.Set(roofPosition / 2, colorIndexesOrder[maze.KeyCount] * 100);

            //задать позицию игрока
            r.Init(seed);
            List<LogicPos> playerPositions = maze.GetEmptyPositions(EmptyCondition.ABOVE_BORDER, Constants.PLAYER_FLOOR, false);
            if (playerPositions.Count == 0)
                throw new GenerateException("Ошибка. Не удалось задать позицию игрока.");
            maze.PlayerPosition = playerPositions[r.Next(playerPositions.Count)];
            maze.KeyMap.Set(maze.PlayerPosition.Position / 2, Utils.IndexPlayer);

            //получить стили слоёв
            int[] layerStyles = Utils.GetLayerStyles(maze, seed, layersShuffled, layers9);

            //расставить окна
            r.Init(seed);
            for (int i = 0; i < maze.CountInside; i++)
                if (layerStyles[i * 2 + 1] >= Constants.STYLE_WITH_WINDOWS_THRESHOLD)
                    GenerateWindows(i * 2 + 1);

            //продырявить полы
            if (holesEnabled)
            {
                r.Init(seed);
                for (int z = 0; z < maze.CountInside; z++)
                    for (int y = 0; y < maze.Height; y++)
                        for (int x = 0; x < maze.Width; x++)
                        {
                            Vector3P positionDown = new Vector3P(x, y, z) * 2 + 1;
                            Vector3P position = positionDown + Vector3P.Up;
                            Vector3P positionUp = position + Vector3P.Up;

                            int itemDown = maze.Map.Get(positionDown);
                            int item = maze.Map.Get(position);
                            int itemUp = maze.Map.Get(positionUp);
                            int itemUpKey = maze.KeyMap.Get(positionUp / 2);

                            if (item == Utils.IndexWall &&
                                (itemUp == Utils.IndexAir) &&
                                (itemDown == Utils.IndexAir || itemDown == Utils.IndexStairsP) &&
                                (itemUpKey == Utils.IndexAir) &&
                                r.Next(Constants.HOLE_CHANCE) == 0)
                                maze.Map.Set(position, Utils.IndexHole);
                        }
            }

            //расставить украшения
            r.Init(seed);
            for (int i = 0; i < maze.CountInside; i++)
                Decorator.DecorateFloor(maze, i, layerStyles[i * 2 + 1]);
        }

        //поставить лифт
        private void SetLift(LogicPos lp, int high)
        {
            Vector3P position = lp.Position + Vector3P.Down;
            Vector3P positionAbove = position + lp.Direction;
            int item = lp.Direction.Number + Utils.IndexLift;
            for (int i = 0; i < high; i++)
            {
                if (position.Z % 2 != 0)
                {
                    maze.Map.Set(position, item);
                    maze.Map.Set(positionAbove, Utils.IndexAir);
                    DeleteStairsPositionsAbout(position, lp.Direction);
                }
                else
                    maze.Map.Set(position, Utils.IndexAir);
                position += Vector3P.Up;
                positionAbove += Vector3P.Up;
            }
        }

        //сгенерировать ступеньки на этаже (возвращает их позицию)
        private LogicPos GenerateStairs(int floor)
        {
            List<LogicPos> stairsPositionsOnFloor = stairsPositions[floor];

            //если доступные позиции имеются
            if (stairsPositionsOnFloor.Count > 0)
            {
                //поставить ступеньки
                LogicPos stairs;
                bool stairsInstalled = false;
                do
                {
                    int index = r.Next(stairsPositionsOnFloor.Count);
                    stairs = stairsPositionsOnFloor[index];
                    if (CheckStairsByClique(stairs))
                    {
                        SetStairs(stairs);
                        stairsInstalled = true;
                    }
                    else
                        stairsPositionsOnFloor.RemoveAt(index);
                }
                while (!stairsInstalled && stairsPositionsOnFloor.Count > 0);

                if (stairsInstalled)
                    return stairs;
                else throw new StairsException("Ступеньки не были поставлены.");
            }
            else
                throw new StairsException("Ступеньки не были поставлены.");
        }

        //проверить ступеньки на клику
        private bool CheckStairsByClique(LogicPos lp)
        {
            Vector3P[] wastePositions = new Vector3P[4] //позиции мусора
            {
                lp.Position,
                lp.Position + lp.Direction * 2,
                lp.Position + Vector3P.Up * 2,
                lp.Position + lp.Direction * 2 + Vector3P.Up * 2
            };

            //установить мусор
            for (int i = 0; i < wastePositions.Length; i++)
                maze.Map.Set(wastePositions[i], Utils.IndexStairsP);

            //проверить клику
            bool isCliqueOne = CliqueController.IsCliqueOne(wastePositions[0] + lp.Direction.TurnOpposite() * 2);
            bool isCliqueOneUp = CliqueController.IsCliqueOne(wastePositions[3] + lp.Direction * 2);

            //удалить мусор
            for (int i = 0; i < wastePositions.Length; i++)
                maze.Map.Set(wastePositions[i], wastePositions[i].Z < maze.CountInside * 2 ? Utils.IndexWall : Utils.IndexAir);

            return isCliqueOne && isCliqueOneUp;
        }

        //поставить ступеньки
        private void SetStairs(LogicPos lp)
        {
            Vector3P position = lp.Position;
            Vector3P direction = lp.Direction;

            Vector3P above_1 = position + direction.TurnOpposite();
            Vector3P above1 = position + direction;
            Vector3P above2 = position + direction * 2;
            Vector3P above3 = position + direction * 3;

            //корневая часть
            maze.Map.Set(above_1, Utils.IndexAir);
            maze.Map.Set(position, direction.Number + Utils.IndexStairs);
            maze.Map.Set(above1, Utils.IndexStairsP);
            maze.Map.Set(above2, Utils.IndexStairsP);
            DeleteStairsPositionsAbout(position, direction.TurnOpposite());
            DeleteStairsPositionsAbout(above2, Vector3P.Zero);

            //верхняя часть
            maze.Map.Set(position + Vector3P.Up * 2, Utils.IndexStairsP);
            maze.Map.Set(above1 + Vector3P.Up * 2, Utils.IndexStairsP);
            maze.Map.Set(above2 + Vector3P.Up * 2, Utils.IndexStairsP);
            maze.Map.Set(above3 + Vector3P.Up * 2, Utils.IndexAir);
            DeleteStairsPositionsAbout(position + Vector3P.Up * 2, Vector3P.Zero);
            DeleteStairsPositionsAbout(above2 + Vector3P.Up * 2, direction);

            //дыра в перекрытии
            maze.Map.Set(position + Vector3P.Up, Utils.IndexStairsP);
            maze.Map.Set(above1 + Vector3P.Up, Utils.IndexStairsP);
            maze.Map.Set(above2 + Vector3P.Up, Utils.IndexStairsP);

            //запретить смежные сонаправленные ступеньки
            Vector3P aboveCCW = position + direction.TurnCCW() * 2;
            Vector3P aboveCW = position + direction.TurnCW() * 2;
            DeleteStairsPosition(aboveCCW, direction);
            DeleteStairsPosition(aboveCW, direction);
            DeleteStairsPosition(aboveCCW + direction * 2, direction);
            DeleteStairsPosition(aboveCW + direction * 2, direction);
            DeleteStairsPosition(aboveCCW - direction * 2, direction);
            DeleteStairsPosition(aboveCW - direction * 2, direction);
        }

        //удалить позиции ступенек возле нового объекта
        private void DeleteStairsPositionsAbout(Vector3P position, Vector3P direction)
        {
            //предотвратить залезание ступенек на объект
            DeleteStairsPositionsOn(position);

            //предотвратить загораживание входа объектом
            for (int i = 0; i < 4; i++)
            {
                Vector3P dir = Vector3P.FromNumber(i);
                DeleteStairsPosition(position + dir * 2, dir);
                DeleteStairsPosition(position + dir * 4 + Vector3P.Down * 2, dir.TurnOpposite());
            }

            //предотвратить загораживание входа на объект
            if (direction != Vector3P.Zero)
                DeleteStairsPositionsOn(position + direction * 2);
        }

        //удалить позиции ступенек на объекте
        private void DeleteStairsPositionsOn(Vector3P position)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector3P direction = Vector3P.FromNumber(i);
                DeleteStairsPosition(position, direction);
                DeleteStairsPosition(position + Vector3P.Down * 2, direction);
                DeleteStairsPosition(position + direction * 2, direction.TurnOpposite());
                DeleteStairsPosition(position + direction * 2 + Vector3P.Down * 2, direction.TurnOpposite());
            }
        }

        //удалить позицию ступенек
        private void DeleteStairsPosition(Vector3P position, Vector3P direction)
        {
            int floor = position.Z / 2;
            if (floor >= 0 && floor < maze.CountInside)
                stairsPositions[floor].Remove(new LogicPos(position, direction));
        }

        //сгенерировать лабиринт на этаже
        private void GenerateMaze(int z, int deleteWalls)
        {
            Digger.Dig(maze, z); //прокопать

            //выбрать позиции удаляемых стен
            List<Vector3P> positions = new List<Vector3P>();
            for (int y = 1; y < maze.Height * 2; y++)
                for (int x = y % 2 + 1; x < maze.Width * 2; x += 2)
                {
                    Vector3P position = new Vector3P(x, y, z);
                    Vector3P direction = Vector3P.FromNumber(x % 2 + 1);
                    if (maze.Map.Get(position) == Utils.IndexWall && //только пустые позиции от удаляемой стены
                        maze.Map.Get(position + direction) == Utils.IndexAir &&
                        maze.Map.Get(position + direction.TurnOpposite()) == Utils.IndexAir)
                        positions.Add(position);
                }
            Shuffler<Vector3P>.ShuffleList(positions);

            //удалить некоторые стены для циклов
            for (int i = 0; i < Math.Min(positions.Count, deleteWalls); i++)
            {
                Vector3P position = positions[i];
                maze.Map.Set(position, Utils.IndexAir);

                //избавиться от одиночных колонн
                Vector3P direction = Vector3P.FromNumber(position.X % 2);
                position += direction;
                if (maze.IsSinglePillar(position)) maze.Map.Set(position, Utils.IndexAir);
                position += direction.TurnOpposite() * 2;
                if (maze.IsSinglePillar(position)) maze.Map.Set(position, Utils.IndexAir);
            }
        }

        //сгенерировать окна на этаже
        private void GenerateWindows(int z)
        {
            for (int x = 0; x <= maze.Width * 2; x++)
                for (int y = (x + 1) % 2; y <= maze.Height * 2; y += 2)
                {
                    Vector3P position = new Vector3P(x, y, z);
                    if (maze.IsAboveBorder(position, out List<Vector3P> dirs, 1))
                    {
                        Vector3P direction = dirs[0];
                        Vector3P positionNeighbour = position + direction.TurnOpposite();
                        int itemNeighbour = maze.Map.Get(positionNeighbour);
                        int keyNeighbour = maze.KeyMap.Get(positionNeighbour / 2);
                        if (itemNeighbour == Utils.IndexAir &&
                            keyNeighbour != Utils.IndexPlayer && r.Next(Constants.WINDOW_CHANCE) == 0)
                            maze.Map.Set(position, direction.Number + Utils.IndexWindow);
                    }
                }
        }
    }
}
