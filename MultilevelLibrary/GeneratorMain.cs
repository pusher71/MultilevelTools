using System;
using System.Collections.Generic;
using System.Linq;
using MultilevelLibrary.Generation;
using MultilevelLibrary.Generation.Model;
using PrimitiveData3D;

namespace MultilevelLibrary
{
    public class GeneratorMain
    {
        private MultilevelMaze maze; //генерируемый лабиринт
        private List<LogicPos>[] stairsPositions; //список позиций для ступенек

        //сгенерировать помещения с лифтом и лестницами
        public void Generate(MultilevelMaze maze, int seed,
            int safetyCount, int savePeriod, int radarCount, int stairsCount, int deleteWalls,
            int liftPredel, int preferenceKeyCount, int preferenceBottlesCount, int preferenceCamerasCount, bool enableSafety,
            bool layersShuffled, bool holesEnabled, bool camerasEnabled)
        {
            this.maze = maze;

            maze.SetRectangle(Vector3P.Zero, new Vector3P(maze.Width, maze.Height, maze.CountInside) * 2, Utils.IndexWall, RectangleType.FILL); //застроить стенами
            CliqueController.Init(maze); //проинициализировать контроллер клики
            VisibilityProcessor.Init(maze); //проинициализировать вычислитель видимости

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
            LogicPos liftLogicPos = new LogicPos();
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
                liftLogicPos = new LogicPos(liftPosition, liftDirection);

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

            Graph graph = new Graph(maze);
            IntensityMap safetyIntensityMap = new IntensityMap(maze);

            //распространить интенсивность безопасности от лифта
            if (liftPredel > 0)
            {
                Vector3P liftPosition = liftLogicPos.Position;
                for (int i = 0; i < liftPredel; i++)
                {
                    GraphNode node = graph.GetGraphNode(liftPosition / 2);
                    safetyIntensityMap.AddIntensity(node, Constants.SAFETY_INTENSITY);
                    liftPosition += Vector3P.Up * 2;
                }
            }

            List<TrapsZone> trapsZones = null;
            List<LogicPos>[] trapsByFloor = null;
            if (enableSafety)
            {
                //пометить непросматриваемые циклы как безопасные позиции
                CyclesFinder.MarkSafetyCycles(graph);

                //получить части путей
                List<PathPart> pathParts = PathPartsFinder.GetPathParts(graph);

                //получить зоны ловушек
                trapsZones = TrapsZonesFinder.GetTrapsZones(pathParts);

                //получить позиции ловушек
                r.Init(seed);
                trapsByFloor = TrapsFinder.MarkTraps(maze, graph, trapsZones);
            }

            //найти доступные позиции для безопасных комнат
            r.Init(seed);
            List<LogicPos>[] deadendsByFloor = new List<LogicPos>[maze.CountInside]; //список тупиков по этажам
            List<LogicPos>[] roomPositionsByFloor = new List<LogicPos>[maze.CountInside]; //список позиций комнат по этажам
            List<LogicPos> roomPositionsRadar = new List<LogicPos>(); //список позиций радарных комнат
            List<LogicPos> roomPositionsUsed = new List<LogicPos>(); //список использованных позиций комнат
            for (int i = 0; i < maze.CountInside; i++)
            {
                roomPositionsByFloor[i] = new List<LogicPos>();

                //получить тупики с этажа
                deadendsByFloor[i] = maze.GetEmptyPositions(EmptyCondition.DEADEND, i, true);

                //добавить обязательные позиции комнат
                if (enableSafety)
                    for (int j = 0; j < trapsByFloor[i].Count; j++)
                    {
                        LogicPos room = trapsByFloor[i][j];
                        roomPositionsByFloor[i].Add(room);
                        deadendsByFloor[i].Remove(room);

                        //распространить интенсивность безопасности от комнаты
                        safetyIntensityMap.AddIntensity(graph.GetGraphNode(room.Position / 2), Constants.SAFETY_INTENSITY);
                    }
            }

            for (int i = 0; i < maze.CountInside; i++)
            {
                //добавить оставшиеся позиции комнат
                int requiredRemainder = Math.Min(deadendsByFloor[i].Count, safetyCount - roomPositionsByFloor[i].Count);
                for (int j = 0; j < requiredRemainder; j++)
                {
                    //отсортировать список по интенсивности безопасности
                    deadendsByFloor[i] = deadendsByFloor[i].OrderBy(lp => safetyIntensityMap.Get(lp.Position / 2)).ToList();

                    LogicPos room = deadendsByFloor[i][0];
                    roomPositionsByFloor[i].Add(room);
                    deadendsByFloor[i].Remove(room);

                    //распространить интенсивность безопасности от комнаты
                    safetyIntensityMap.AddIntensity(graph.GetGraphNode(room.Position / 2), Constants.SAFETY_INTENSITY);
                }

                //выбрать позиции для радарных комнат
                if (i <= Utils.GetRadarFloorMax(maze.CountInside, liftPredel > 0))
                    roomPositionsRadar.AddRange(roomPositionsByFloor[i]);
            }

            //расставить радарные комнаты
            Shuffler<LogicPos>.ShuffleList(roomPositionsRadar);
            for (int i = 0; i < Math.Min(roomPositionsRadar.Count, radarCount); i++)
            {
                LogicPos room = roomPositionsRadar[i];
                roomPositionsByFloor[room.Position.Z / 2].Remove(room);
                roomPositionsUsed.Add(room);
                maze.Map.Set(room.Position, room.Direction.Number + Utils.IndexRadarRoom);
            }

            //расставить сохраняющие комнаты
            if (savePeriod > 0)
            {
                int saveCount = maze.CountInside / savePeriod; //предпочитаемое количество сохраняющих комнат
                for (int i = 0; i < saveCount; i++)
                {
                    List<LogicPos> saves = new List<LogicPos>(); //список позиций в данном "трёхстишии"
                    for (int j = 0; j < savePeriod; j++)
                        if (maze.CountInside == 1 || i != 0 || j != 0)
                            saves.AddRange(roomPositionsByFloor[i * savePeriod + j]);

                    //нашлась хотя бы одна позиция в "трёхстишии"
                    if (saves.Count > 0)
                    {
                        LogicPos room = saves[r.Next(saves.Count)];
                        roomPositionsByFloor[room.Position.Z / 2].Remove(room);
                        roomPositionsUsed.Add(room);
                        maze.Map.Set(room.Position, room.Direction.Number + Utils.IndexSaveRoom);
                    }
                }
            }

            //расставить простые комнаты
            for (int i = 0; i < maze.CountInside; i++)
                for (int j = 0; j < roomPositionsByFloor[i].Count; j++)
                {
                    LogicPos room = roomPositionsByFloor[i][j];
                    roomPositionsUsed.Add(room);
                    maze.Map.Set(room.Position, room.Direction.Number + Utils.IndexSafetyRoom);
                }

            //расставить энергетики
            r.Init(seed);
            maze.BottlesCount = Math.Min(preferenceBottlesCount, roomPositionsUsed.Count);
            Shuffler<LogicPos>.ShuffleList(roomPositionsUsed);
            for (int i = 0; i < maze.BottlesCount; i++)
            {
                LogicPos bottlePos = roomPositionsUsed[i];
                maze.KeyMap.Set(bottlePos.Position / 2, Utils.IndexBottle);
            }

            //задать позицию игрока
            r.Init(seed);
            List<LogicPos> playerPositions = maze.GetEmptyPositions(EmptyCondition.ABOVE_BORDER, Constants.PLAYER_FLOOR, false);
            if (playerPositions.Count == 0)
                throw new GenerateException("Ошибка. Не удалось задать позицию игрока.");
            maze.PlayerPosition = playerPositions[r.Next(playerPositions.Count)];
            maze.KeyMap.Set(maze.PlayerPosition.Position / 2, Utils.IndexPlayer);

            //распространить интенсивность от игрока
            IntensityMap playerIntensityMap = new IntensityMap(maze);
            playerIntensityMap.AddIntensity(graph.GetGraphNode(maze.PlayerPosition.Position / 2), Constants.PLAYER_INTENSITY);

            //задать позицию врага
            r.Init(seed);
            int enemyFloor = r.Next(Math.Min(maze.CountInside - 1, Constants.ENEMY_FLOOR_NORMAL_THRESHOLD), maze.CountInside);
            IEnumerable<LogicPos> enemyPositions = maze.GetEmptyPositions(EmptyCondition.EMPTY, enemyFloor, true)
                        .OrderBy(lp => playerIntensityMap.Get(lp.Position / 2));
            if (enemyPositions.Count() == 0)
                throw new GenerateException("Ошибка. Не удалось задать позицию врага.");
            maze.EnemyPosition = enemyPositions.First();
            maze.KeyMap.Set(maze.EnemyPosition.Position / 2, Utils.IndexEnemy);

            if (camerasEnabled && preferenceCamerasCount > 0)
            {
                if (enableSafety)
                {
                    //пометить установленные безопасные комнаты
                    for (int i = 0; i < roomPositionsUsed.Count; i++)
                        graph.GetGraphNode(roomPositionsUsed[i].Position / 2).IsSafetyRoom = true;

                    //создать группы подчастей путей, записывающих себя в GraphNodes (для ограничения расставления камер)
                    SubPathGroupsFinder.CreateSubPathGroups(maze, trapsZones);
                }

                //расставить камеры
                IntensityMap camerasIntensityMap = new IntensityMap(maze);
                r.Init(seed);
                for (int i = 0; i < maze.CountInside; i++)
                    for (int j = 0; j < preferenceCamerasCount; j++)
                    {
                        //получить список позиций для камер
                        IEnumerable<LogicPos> cameraPositons = maze.GetEmptyPositions(EmptyCondition.CAMERA, i, true)
                            .Where(lp => !graph.GetGraphNode(lp.Position / 2).IsCameraProhibited)
                            .OrderBy(lp => camerasIntensityMap.Get(lp.Position / 2));

                        //завершить расставление камер на этаже, если позиций не осталось
                        if (cameraPositons.Count() == 0)
                            break;

                        //поставить камеру
                        LogicPos cameraPos = cameraPositons.First();
                        maze.Map.Set(cameraPos.Position, cameraPos.Direction.Number + Utils.IndexCamera);

                        //распространить интенсивность от камеры
                        GraphNode cameraNode = graph.GetGraphNode(cameraPos.Position / 2);
                        camerasIntensityMap.AddIntensity(cameraNode, Constants.CAMERA_INTENSITY);

                        //оповестить о поставленной камере
                        cameraNode.OnCameraPlaced();
                    }
            }

            //расставить ключи
            IntensityMap keysIntensityMap = new IntensityMap(maze);
            r.Init(seed);
            IEnumerable<LogicPos> keyPositions = maze.GetEmptyPositions(EmptyCondition.EMPTY, true); //список позиций для ключей
            if (keyPositions.Count() == 0)
                throw new GenerateException("Ошибка. Не удалось поставить ни один ключ.");
            maze.KeyCount = Math.Min(preferenceKeyCount, keyPositions.Count());
            maze.KeyPositions = new LogicPos[maze.KeyCount];
            maze.KeyOffsets = new Vector3P[maze.KeyCount];
            maze.KeyRotations = new int[maze.KeyCount];
            for (int i = 0; i < maze.KeyCount; i++)
            {
                //отсортировать список по интенсивности ключей
                keyPositions = keyPositions.OrderBy(lp => keysIntensityMap.Get(lp.Position / 2));

                //поставить ключ
                LogicPos keyPos = keyPositions.First();
                maze.KeyMap.Set(keyPos.Position / 2, Utils.IndexKey);
                maze.KeyPositions[i] = keyPos;
                maze.KeyOffsets[i] = new Vector3P(r.Next(400) - 200, r.Next(400) - 200, 0);
                maze.KeyRotations[i] = r.Next(360);

                //распространить интенсивность от ключа
                keysIntensityMap.AddIntensity(graph.GetGraphNode(keyPos.Position / 2), Constants.KEY_INTENSITY);
            }

            //получить стили слоёв
            int[] layerStyles = Utils.GetLayerStyles(maze, layersShuffled, seed);

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

                            if (item == Utils.IndexWall &&
                                (itemUp == Utils.IndexAir || Utils.IsCamera(itemUp)) &&
                                (itemDown == Utils.IndexAir || Utils.IsCamera(itemDown) || itemDown == Utils.IndexStairsP) &&
                                maze.KeyMap.Get(positionUp / 2) == Utils.IndexAir &&
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
                        if ((itemNeighbour == Utils.IndexAir || Utils.IsCamera(itemNeighbour)) &&
                            keyNeighbour != Utils.IndexPlayer && r.Next(Constants.WINDOW_CHANCE) == 0)
                            maze.Map.Set(position, direction.Number + Utils.IndexWindow);
                    }
                }
        }
    }
}
