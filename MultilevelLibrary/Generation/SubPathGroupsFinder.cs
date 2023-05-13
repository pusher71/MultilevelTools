using System.Collections.Generic;
using System.Linq;
using MultilevelLibrary.Generation.Model;
using PrimitiveData3D;

namespace MultilevelLibrary.Generation
{
    static class SubPathGroupsFinder
    {
        //создать группы подчастей путей, записывающих себя в GraphNodes
        public static void CreateSubPathGroups(MultilevelMaze maze, List<TrapsZone> trapsZones)
        {
            //для каждой зоны ловушек
            for (int i = 0; i < trapsZones.Count; i++)
            {
                TrapsZone trapsZone = trapsZones[i];
                IntensityMap temperatureLayerRoot = new IntensityMap(maze);
                HashSet<GraphNode> safetySourcesRoot = trapsZone.SafetySources[0];

                //получить список нижних безопасных мест
                List<GraphNode> safetyDowns = new List<GraphNode>();
                foreach (GraphNode node in trapsZone.Target.Nodes)
                    if (node.IsSafetyRoom)
                        safetyDowns.Add(node);
                for (int j = 1; j < trapsZone.SafetySources.Length; j++)
                    foreach (GraphNode node in trapsZone.SafetySources[j])
                        safetyDowns.Add(node);

                //обезопасить пути до нижних безопасных мест
                int safetyDownsCount = safetyDowns.Count;
                for (int j = 0; j < safetyDownsCount; j++)
                {
                    //обновить корневой температурный слой
                    foreach (GraphNode node in safetySourcesRoot)
                        temperatureLayerRoot.AddIntensity(node, trapsZone.Target.Nodes.Count, true);

                    //получить ближайшее нижнее безопасное место
                    safetyDowns = safetyDowns.OrderByDescending(n => temperatureLayerRoot.Get(n.Position)).ToList();
                    GraphNode nearestSafetyRoom = safetyDowns[0];
                    safetyDowns.RemoveAt(0);

                    //создать группу подчастей путей до него
                    List<SubPath> subPaths = GetSubPaths(nearestSafetyRoom, safetySourcesRoot);
                    SubPathGroup subPathGroup = new SubPathGroup(subPaths);

                    //добавить позиции группы в корневые источники
                    safetySourcesRoot.UnionWith(subPathGroup.GetNodesFromShortestSubPaths());
                }
            }
        }

        //получить все подчасти путей между источником и финишными позициями
        private static List<SubPath> GetSubPaths(GraphNode source, HashSet<GraphNode> finishes)
        {
            //отметить финишные позиции подчастей путей
            foreach (GraphNode node in finishes)
                node.IsSubPathsFinish = true;

            //получить подчасти путей между двумя частями путей
            List<SubPath> subPathsFinished = new List<SubPath>();
            ProcessSubPath(new SubPath(), source, subPathsFinished);

            return subPathsFinished;
        }

        //обработать подчасть путей
        private static void ProcessSubPath(SubPath subPath, GraphNode node, List<SubPath> subPathsFinished)
        {
            subPath.Nodes.Add(node); //добавить позицию в текущую подчасть путей
            node.WasVisited = true; //отметить посещение позиции

            List<Vector3P> dirs = node.GetDirections();
            for (int i = 0; i < dirs.Count; i++)
            {
                Vector3P dir = dirs[i];
                GraphNode neighbour = node.GetNeighbour(dir);
                if (neighbour.IsSubPathsFinish) //если попали на финиш
                {
                    //записать подчасть путей, как финишную
                    SubPath subPathFinished = subPath.Clone();
                    subPathFinished.Nodes.Add(neighbour);
                    subPathFinished.Nodes.Reverse();
                    subPathsFinished.Add(subPathFinished);
                }
                else if (!neighbour.WasVisited && //иначе если соседняя позиция не была посещена
                    !neighbour.IsCycledSafety) //и находится не на безопасной позиции цикла
                    ProcessSubPath(subPath, neighbour, subPathsFinished); //продолжить обрабатывать подчасть путей
            }

            subPath.Nodes.RemoveAt(subPath.Nodes.Count - 1); //удалить позицию из текущей подчасти путей
            node.WasVisited = false; //удалить отметку посещения
        }
    }
}
