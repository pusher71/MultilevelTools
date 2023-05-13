using System.Collections.Generic;
using MultilevelLibrary.Generation.Model;
using PrimitiveData3D;

namespace MultilevelLibrary.Generation
{
    static class CyclesFinder
    {
        private static List<Cycle> cycles;

        //пометить непросматриваемые циклы как безопасные позиции
        public static void MarkSafetyCycles(Graph graph)
        {
            cycles = new List<Cycle>();

            //получить все циклы
            GraphNode nodeStart = graph.PreRoof;
            nodeStart.IsCycledSafety = true;
            Go(new List<GraphNode>(), nodeStart);

            //сбросить за собой посещаемость
            graph.ResetVisited();

            //применить Union нормализацию для циклов
            for (int i = cycles.Count - 1; i > 0; i--)
                for (int j = i - 1; j >= 0; j--)
                    Cycle.NormalizeUnion(cycles[i], cycles[j]);

            //проверить циклы на просматриваемость
            for (int i = 0; i < cycles.Count; i++)
            {
                HashSet<GraphNode> cycleNodes = cycles[i].GetNodes();

                //проверить цикл на просматриваемость хотя бы с одной позиции
                bool isCycleVisible = false;
                foreach (GraphNode node in cycleNodes)
                    if (AreAllNodesVisible(node.GetVisibilityMap(), cycleNodes))
                        isCycleVisible = true;

                //отметить цикл как безопасный, если он не просматривается ни с одной позиции
                if (!isCycleVisible)
                    foreach (GraphNode node in cycleNodes)
                        node.IsCycledSafety = true;
            }
        }

        private static void Go(List<GraphNode> nodeStack, GraphNode node)
        {
            node.WasVisited = true; //отметить посещение
            nodeStack.Add(node); //добавить в стек

            List<Vector3P> dirs = node.GetDirections();
            for (int i = 0; i < dirs.Count; i++)
                if (!node.DirectionsProhibited.Contains(dirs[i]))
                {
                    GraphNode neighbour = node.GetNeighbour(dirs[i]);
                    if (!neighbour.WasVisited) //если соседняя позиция не была посещена
                        Go(nodeStack, neighbour); //продолжить обход позиций
                    else if (neighbour != nodeStack[nodeStack.Count - 2]) //иначе если соседняя позиция не является предыдущей
                    {
                        //взять часть последних позиций как новый цикл
                        int indexStart = nodeStack.IndexOf(neighbour);
                        Cycle cycle = new Cycle();
                        for (int j = indexStart; j < nodeStack.Count - 1; j++)
                            cycle.Links.Add(new GraphNodeLink(nodeStack[j], nodeStack[j + 1]));
                        cycle.Links.Add(new GraphNodeLink(nodeStack[nodeStack.Count - 1], nodeStack[indexStart]));

                        //нормализовать со всеми предыдущими циклами
                        for (int j = 0; j < cycles.Count; j++)
                            Cycle.Normalize(cycle, cycles[j]);

                        //добавить в список
                        cycles.Add(cycle);

                        //запретить соседней позиции направление на текущую
                        neighbour.DirectionsProhibited.Add(-dirs[i]);
                    }
                }

            nodeStack.RemoveAt(nodeStack.Count - 1); //удалить из стека
        }

        //проверить видимость всего набора позиций
        private static bool AreAllNodesVisible(MazeMap<bool> visibilityMap, HashSet<GraphNode> nodes)
        {
            bool result = true;
            foreach (GraphNode node in nodes)
                if (!visibilityMap.Get(node.Position))
                    result = false;
            return result;
        }
    }
}
