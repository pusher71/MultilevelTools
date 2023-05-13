using System.Collections.Generic;
using MultilevelLibrary.Generation.Model;
using PrimitiveData3D;

namespace MultilevelLibrary.Generation
{
    static class PathPartsFinder
    {
        private static List<PathPart> pathParts;

        //определить части путей
        public static List<PathPart> GetPathParts(Graph graph)
        {
            pathParts = new List<PathPart>();
            StartPathPart(new List<PathPart>(), graph.PreRoof);
            graph.ResetVisited();
            return pathParts;
        }

        //начать новую часть путей
        private static void StartPathPart(List<PathPart> pathPartHistory, GraphNode node)
        {
            PathPart pathPart = new PathPart(node.IsCycledSafety);

            //связать с корневой частью путей, если она есть
            PathPart root = pathPartHistory.Count != 0 ? pathPartHistory[pathPartHistory.Count - 1] : null;
            if (root != null)
            {
                pathPart.Neighbours.Add(root);
                root.Neighbours.Add(pathPart);
            }

            pathParts.Add(pathPart); //добавить часть путей в общий список
            pathPartHistory.Add(pathPart); //добавить часть путей в историю

            //обработать её и получить список для новых частей путей
            HashSet<GraphNode> nodesNew = new HashSet<GraphNode>();
            ProcessPathPart(pathPartHistory, new List<GraphNode>(), pathPart, node, nodesNew);

            //отметить посещение части путей
            foreach (GraphNode nodeVisited in pathPart.Nodes)
                nodeVisited.WasVisited = true;

            //создать новые части путей из списка
            foreach (GraphNode nodeNew in nodesNew)
                if (!nodeNew.WasVisited)
                    StartPathPart(pathPartHistory, nodeNew);

            pathPartHistory.RemoveAt(pathPartHistory.Count - 1); //удалить часть путей из истории
        }

        //обработать часть путей, получив список позиций для новых частей путей
        private static void ProcessPathPart(List<PathPart> pathPartHistory, List<GraphNode> graphNodeHistory, PathPart pathPart, GraphNode node, HashSet<GraphNode> nodesNew)
        {
            //узнать, совпадают ли статусы у текущей позиции и у текущей части путей
            bool isStatusSame = node.IsCycledSafety == pathPart.IsCycledSafety;

            //если текущая позиция не была посещена
            if (!node.WasVisited)
            {
                //если статус тот же самый
                if (isStatusSame)
                {
                    pathPart.Nodes.Add(node); //добавить позицию в текущую часть путей

                    graphNodeHistory.Add(node); //добавить позицию в историю
                    node.WasVisited = true; //отметить посещение позиции

                    //продолжить распространение части путей на соседние позиции
                    List<Vector3P> dirs = node.GetDirections();
                    for (int i = 0; i < dirs.Count; i++)
                    {
                        Vector3P dir = dirs[i];
                        GraphNode neighbour = node.GetNeighbour(dir);
                        ProcessPathPart(pathPartHistory, graphNodeHistory, pathPart, neighbour, nodesNew);
                    }

                    graphNodeHistory.RemoveAt(graphNodeHistory.Count - 1); //удалить позицию из истории
                }
                else //иначе добавить позицию в список для новых части путей
                    nodesNew.Add(node);
            }
            else if (!isStatusSame) //иначе если статус не тот же самый
            {
                //определить индекс начала возможного цикла
                int index = pathPartHistory.Count;
                while (!pathPartHistory[--index].Nodes.Contains(node));

                //связать части путей, если они не были связаны
                PathPart neighbour = pathPartHistory[index];
                if (!pathPart.Neighbours.Contains(neighbour))
                {
                    pathPart.Neighbours.Add(neighbour);
                    neighbour.Neighbours.Add(pathPart);
                }
            }
        }
    }
}
