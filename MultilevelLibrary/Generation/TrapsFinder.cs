using System.Collections.Generic;
using MultilevelLibrary.Generation.Model;
using PrimitiveData3D;

namespace MultilevelLibrary.Generation
{
    static class TrapsFinder
    {
        private static MultilevelMaze maze;
        private static Graph graph;
        private static List<LogicPos>[] trapsByFloor;

        //пометить ловушки и получить их список по этажам
        public static List<LogicPos>[] MarkTraps(MultilevelMaze maze, Graph graph, List<TrapsZone> trapsZones)
        {
            TrapsFinder.maze = maze;
            TrapsFinder.graph = graph;
            trapsByFloor = new List<LogicPos>[maze.CountInside];
            for (int i = 0; i < maze.CountInside; i++)
                trapsByFloor[i] = new List<LogicPos>();

            for (int i = 0; i < trapsZones.Count; i++)
            {
                TrapsZone trapsZone = trapsZones[i];

                //создать температурные слои
                List<IntensityMap> temperatureLayers = new List<IntensityMap>();
                for (int j = 0; j < trapsZone.SafetySources.Length; j++)
                    foreach (GraphNode node in trapsZone.SafetySources[j])
                    {
                        IntensityMap temperatureLayer = new IntensityMap(maze);
                        temperatureLayer.AddIntensity(node, trapsZone.Target.Nodes.Count + 1, true);
                        temperatureLayers.Add(temperatureLayer);
                    }

                //проверить все тупики на ловушки
                foreach (GraphNode node in trapsZone.Target.Nodes)
                    ProcessTrap(temperatureLayers, node, true);

                //проверить все угловые позиции на ловушки
                foreach (GraphNode node in trapsZone.Target.Nodes)
                    ProcessTrap(temperatureLayers, node, false);
            }

            return trapsByFloor;
        }

        //обработать возможную ловушку
        private static void ProcessTrap(List<IntensityMap> temperatureLayers, GraphNode node, bool checkOnlyDeadends)
        {
            if (IsTrap(temperatureLayers, node, checkOnlyDeadends))
            {
                node.IsTrap = true;
                Vector3P trapDir;
                List<Vector3P> dirs = node.GetDirections(); //направлений может быть 1 или 2
                if (dirs.Count == 2)
                {
                    //выбрать направление к устанавливаемой стене
                    int dirWallIndex = r.Next(dirs.Count);

                    //взять оставшееся направление как направление ловушки
                    trapDir = dirs[(dirWallIndex + 1) % 2];

                    //установить стену
                    Vector3P dirWall = dirs[dirWallIndex];
                    Vector3P wallPosition = node.Position * 2 + 1 + dirWall; //позиция стены
                    maze.Map.Set(wallPosition, Utils.IndexWall);
                    maze.Map.Set(wallPosition + dirWall.TurnCCW(), Utils.IndexWall);
                    maze.Map.Set(wallPosition + dirWall.TurnCW(), Utils.IndexWall);

                    //разорвать связь в графе
                    GraphNode nodeWall = node.GetNeighbour(dirWall); //вершина за стеной
                    node.BreakConnectionWithNeighbour(dirWall);
                    nodeWall.BreakConnectionWithNeighbour(dirWall.TurnOpposite());

                    //проверить позицию за установленной стеной на ловушку
                    ProcessTrap(temperatureLayers, nodeWall, checkOnlyDeadends);
                }
                else //взять единственное направление как направление ловушки
                    trapDir = dirs[0];

                trapsByFloor[node.Position.Z].Add(new LogicPos(node.Position * 2 + 1, trapDir));
            }
        }

        //позиция является ловушкой
        private static bool IsTrap(List<IntensityMap> temperatureLayers, GraphNode node, bool checkOnlyDeadends)
        {
            List<Vector3P> dirs = node.GetDirections();
            int item = maze.Map.Get(node.Position * 2 + 1);

            //ловушка должна быть окружена хотя бы 2 смежными стенами, не должна являться межэтажным транспортом и пока не должна быть отмечена
            if (dirs.Count < (checkOnlyDeadends ? 2 : 3) && (dirs.Count != 2 || dirs[0] + dirs[1] != Vector3P.Zero) &&
                !Utils.IsStairs(item) && item != Utils.IndexStairsP && !Utils.IsLift(item) && !node.IsTrap)
            {
                //проверить изменение температуры по всем слоям и по всем направлениям
                bool isTrap = false;
                for (int i = 0; i < temperatureLayers.Count && !isTrap; i++)
                {
                    if (!CanGoToSafety(temperatureLayers[i], node, temperatureLayers[i].Get(node.Position), false))
                        isTrap = true;
                    graph.ResetVisited();
                }
                return isTrap;
            }
            else
                return false;
        }

        //можно ли попасть в безопасное место
        private static bool CanGoToSafety(IntensityMap temperatureLayer, GraphNode node, int temperatureStart, bool wasTemperatureUppered)
        {
            node.WasVisited = true;
            bool isTemperatureUpper = temperatureLayer.Get(node.Position) > temperatureStart;

            if (isTemperatureUpper && wasTemperatureUppered)
                return false; //повторное повышение температуры не допускается
            else if (IsNodePositionSafety(temperatureLayer, node, temperatureStart))
                return true; //позиция является безопасной
            else //продолжить поиск по соседним позициям
            {
                List<Vector3P> dirs = node.GetDirections();
                bool safetyFound = false;
                for (int i = 0; i < dirs.Count && !safetyFound; i++)
                {
                    GraphNode neighbour = node.GetNeighbour(dirs[i]);
                    if (!neighbour.WasVisited && CanGoToSafety(temperatureLayer, neighbour, temperatureStart, isTemperatureUpper || wasTemperatureUppered))
                        safetyFound = true;
                }
                    
                return safetyFound;
            }
        }

        //позиция является безопасной
        private static bool IsNodePositionSafety(IntensityMap temperatureLayer, GraphNode node, int temperatureStart) =>
            temperatureLayer.Get(node.Position) < temperatureStart || node.IsTrap;
    }
}
