using MultilevelLibrary.Generation.Model;
using PrimitiveData3D;

namespace MultilevelLibrary.Generation
{
    static class VisibilityProcessor
    {
        private static MultilevelMaze maze;
        private static MazeMap<bool> visibilityMap;

        //проинициализировать
        public static void Init(MultilevelMaze maze) =>
            VisibilityProcessor.maze = maze;

        //рассчитать карту видимости
        public static MazeMap<bool> CalculateVisibilityMap(Vector3P position)
        {
            visibilityMap = new MazeMap<bool>(maze.Width, maze.Height, maze.Count);

            for (int i = 0; i < 4; i++)
            {
                Vector3P directionL = Vector3P.FromNumber(i);
                Vector3P directionR = Vector3P.FromNumber((i + 1) % 4);
                Vector3P cornerL = position + directionL - directionR;
                Vector3P cornerR = position - directionL + directionR;
                Vector3P cornerBack = position - directionL - directionR;
                Ray rayL = new Ray(cornerR, directionL.ToAngle(), false);
                Ray rayR = new Ray(cornerL, directionR.ToAngle(), true);
                StartVisibilityArea(cornerL, cornerR, cornerBack, cornerBack, directionL, directionR, rayL, rayR, true, false);
            }

            return visibilityMap;
        }

        //начать новую область
        private static void StartVisibilityArea(Vector3P cornerL, Vector3P cornerR, Vector3P previousCornerL, Vector3P previousCornerR,
            Vector3P directionL, Vector3P directionR, Ray rayL, Ray rayR, bool isVisibleEverywhere, bool stopIfInvisible)
        {
            //проверить попадание света
            if (stopIfInvisible && !IsPositionVisible(cornerL, cornerR, rayL, rayR))
                return;

            //провернуть лазеры
            if (!isVisibleEverywhere)
            {
                //R
                if (((cornerR - previousCornerL) * directionL).IsPositive())
                {
                    Ray rayConstraintR = new Ray(rayR.Position, cornerR, true);
                    Ray rayToGoalR = new Ray(previousCornerL, cornerR, true);
                    if (rayToGoalR >= rayL)
                        return;
                    else if (rayToGoalR > rayConstraintR)
                        rayR = rayToGoalR;
                    else
                        rayR.RotateTo(cornerR);
                }
                else
                    rayR.RotateTo(cornerR);

                //L
                if (((cornerL - previousCornerR) * directionR).IsPositive())
                {
                    Ray rayConstraintL = new Ray(rayL.Position, cornerL, false);
                    Ray rayToGoalL = new Ray(previousCornerR, cornerL, false);
                    if (rayToGoalL <= rayR)
                        return;
                    else if (rayToGoalL < rayConstraintL)
                        rayL = rayToGoalL;
                    else
                        rayL.RotateTo(cornerL);
                }
                else
                    rayL.RotateTo(cornerL);
            }

            //пойти по области
            Vector3P position = cornerL * directionR.Abs() + cornerR * directionL.Abs() + directionL + directionR + cornerL * Vector3P.Up;
            int maxLength = Constants.MAX_FLOOR_EDGE;
            AreaInfo areaInfo = new AreaInfo(cornerL, cornerR);
            while (true)
            {
                //начать новую линию
                maxLength = StartVisibilityLine(position, directionL, directionR, rayL, rayR,
                    isVisibleEverywhere, areaInfo, maxLength);

                //выйти из цикла
                if (!maze.IsEverybodyCanGo(position, directionL))
                    break;

                //продвинуться
                position += directionL * 2;
            }
        }

        //начать новую линию
        private static int StartVisibilityLine(Vector3P position, Vector3P directionL, Vector3P directionR, Ray rayL, Ray rayR,
            bool isVisibleEverywhere, AreaInfo areaInfo, int maxLength)
        {
            int length = 0; //длина линии
            bool isStrangerL = false; //на L чужие области в данный момент
            int passageWidthOurL = 0; //ширина похода на свою область на L
            while (true)
            {
                //проверить верхний правый угол
                Vector3P cornerR = position + directionL + directionR;
                bool cornerRWall = maze.Map.Get(cornerR) == Utils.IndexWall;

                //если сверху уже чужие области
                if (isStrangerL)
                {
                    //если сверху дырка
                    if (maze.IsEverybodyCanGo(position, directionL))
                    {
                        //проверить верхний левый угол
                        Vector3P cornerL = position + directionL - directionR;
                        if (maze.Map.Get(cornerL) == Utils.IndexWall)
                            areaInfo.FutureCornerL = cornerL;

                        //начать новую область наверх
                        if (cornerRWall && areaInfo.IsExistFutureCornerL)
                            StartVisibilityArea(areaInfo.FutureCornerL, cornerR, areaInfo.StartCornerL, areaInfo.StartCornerR,
                                directionL, directionR, rayL, rayR, false, !isVisibleEverywhere);
                    }
                }
                else if (cornerRWall) //если упёрлись в тормозной угол, пока сверху ещё своя область
                {
                    //притормозить на этом углу
                    isStrangerL = true;
                    passageWidthOurL = length;
                }

                //отметить видимость позиции
                if (isVisibleEverywhere ||
                    (!visibilityMap.Get(position / 2) && IsPositionVisible(position + directionL - directionR, position + directionR - directionL, rayL, rayR)))
                    visibilityMap.Set(position / 2, true);

                //выйти из цикла по длине линии
                if (!maze.IsEverybodyCanGo(position, directionR) || length == maxLength)
                    break;

                //продвинуться по линии
                position += directionR * 2;
                length++;
            }

            //проверить дырку справа и нижний правый угол
            if (maze.IsEverybodyCanGo(position, directionR))
            {
                Vector3P cornerR = position + directionR - directionL;
                if (maze.Map.Get(cornerR) == Utils.IndexWall)
                    areaInfo.FutureCornerR = cornerR;
            }

            //начать новую область направо
            if (areaInfo.IsExistFutureCornerR)
            {
                if (areaInfo.IsExistFutureCornerL)
                    StartVisibilityArea(areaInfo.FutureCornerL, areaInfo.FutureCornerR, areaInfo.StartCornerL, areaInfo.StartCornerR,
                        directionL, directionR, rayL, rayR, false, !isVisibleEverywhere);
                else
                {
                    Vector3P cornerFront = position + directionL + directionR;
                    if (maze.Map.Get(cornerFront) == Utils.IndexWall)
                        StartVisibilityArea(cornerFront, areaInfo.FutureCornerR, areaInfo.StartCornerL, areaInfo.StartCornerR,
                            directionL, directionR, rayL, rayR, false, !isVisibleEverywhere);
                }
            }

            return isStrangerL ? passageWidthOurL : length;
        }

        //проверить, попадает ли позиция в область видимости
        private static bool IsPositionVisible(Vector3P cornerL, Vector3P cornerR, Ray rayL, Ray rayR)
        {
            Ray angleL = new Ray(rayR.Position, cornerL, false);
            Ray angleR = new Ray(rayL.Position, cornerR, true);
            return angleL > rayR && angleR < rayL;
        }
    }
}
