using System.Collections.Generic;
using PrimitiveData3D;

namespace MultilevelLibrary
{
    public class Graph
    {
        private readonly int width;
        private readonly int height;
        private readonly int count;
        private readonly GraphNode[,,] nodes; //вершины графа

        public Vector3P PreRoofPosition { get; } //позиция тупика перед будкой
        public GraphNode PreRoof { get; } //тупик перед будкой

        public Graph(MultilevelMaze maze)
        {
            width = maze.Width;
            height = maze.Height;
            count = maze.Count;

            //создать вершины в каждой ячейке
            nodes = new GraphNode[width, height, count];
            for (int z = 0; z < count; z++)
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                        nodes[x, y, z] = new GraphNode(new Vector3P(x, y, z), maze.IsEnemyCanPlaced(new Vector3P(x, y, z) * 2 + 1));

            //связать вершины между собой и определить возможные направления
            for (int z = 0; z < count; z++)
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                    {
                        Vector3P position = new Vector3P(x, y, z);
                        GraphNode currentNode = GetGraphNode(position);
                        int item = maze.Map.Get(position * 2 + 1);

                        bool onStairsAngle = Utils.IsStairs(item); //ведущий угол ступенек
                        currentNode.EnemyHeightAdjustment = onStairsAngle ? 1.5f : 0; //определить прижатие к потолку над ступеньками
                        if (onStairsAngle)
                        {
                            Vector3P stairsDir = Vector3P.FromNumber(item - Utils.IndexStairs);
                            Vector3P stairsDirOpposite = stairsDir.TurnOpposite();

                            AddDirection(currentNode, stairsDirOpposite);
                            AddDirection(currentNode, Vector3P.Up);

                            //вершина сверху ступенек
                            GraphNode upperNode = currentNode.GetNeighbour(Vector3P.Up);
                            AddDirection(upperNode, Vector3P.Down);

                            //вершина внутри ступенек (пассивный блок)
                            GraphNode sideNode = GetGraphNode(position + stairsDir);
                            AddDirection(sideNode, Vector3P.Up);
                            sideNode.SetDirectionByType(GraphDirectionType.Bottom, stairsDirOpposite);
                            sideNode.SetDirectionByType(GraphDirectionType.Top, Vector3P.Up);
                            sideNode.SetDirectionByType(GraphDirectionType.Call, Vector3P.Up);
                        }
                        else
                        {
                            //добавить горизонтальные направления
                            for (int i = 0; i < 4; i++)
                            {
                                Vector3P direction = Vector3P.FromNumber(i);
                                if (maze.IsEverybodyCanGo(position * 2 + 1, direction))
                                    AddDirection(currentNode, direction);
                            }

                            if (Utils.IsLift(item)) //если лифт
                            {
                                if (z > 0)
                                    AddDirection(currentNode, Vector3P.Down);
                                if (z < maze.CountInside - 1 && Utils.IsLift(maze.Map.Get((position + Vector3P.Up) * 2 + 1)))
                                    AddDirection(currentNode, Vector3P.Up);
                            }
                            else if (Utils.IsRoof(item)) //если выходная будка
                            {
                                PreRoofPosition = position + Vector3P.FromNumber((item - Utils.IndexRoof) / 2).TurnOpposite();
                                PreRoof = GetGraphNode(PreRoofPosition);
                            }
                        }
                    }

            Polarize(Vector3P.Zero, GraphDirectionType.Bottom, false); //поляризовать вниз
            Polarize(PreRoofPosition, GraphDirectionType.Top, false); //поляризовать вверх
        }

        //получить вершину графа по Vector3P позиции
        public GraphNode GetGraphNode(Vector3P position) =>
            nodes[position.X, position.Y, position.Z];

        //добавить направление к вершине графа
        private void AddDirection(GraphNode node, Vector3P direction) =>
            node.AddDirectionAndNeighbour(direction, GetGraphNode(node.Position + direction));

        //поляризовать
        private static readonly Queue<GraphNode> queueNodes = new Queue<GraphNode>(); //очередь посещаемых вершин
        private static readonly Queue<Vector3P> queueBackDirs = new Queue<Vector3P>(); //очередь задних направлений
        public void Polarize(Vector3P finishPosition, GraphDirectionType dt, bool oneFloor)
        {
            GraphNode finishNode = GetGraphNode(finishPosition);

            queueNodes.Clear();
            queueBackDirs.Clear();
            queueNodes.Enqueue(finishNode);
            queueBackDirs.Enqueue(Vector3P.Zero);

            //пока очередь не закончилась
            while (queueNodes.Count > 0)
            {
                //вынуть из очереди вершину и её ROOT-направление
                GraphNode node = queueNodes.Dequeue();
                Vector3P backDir = queueBackDirs.Dequeue();

                //если требуемое направление не задано
                if (!node.IsSpecifiedByType(dt))
                {
                    //для каждого направления
                    foreach (Vector3P currentDir in node.GetDirectionsForEnemy())
                    {
                        if (currentDir == backDir) //если направление ведёт назад (к цели)
                            node.SetDirectionByType(dt, currentDir); //пометить его как направление следования

                        //добавить в очередь данные для обработки
                        if (!oneFloor || currentDir.Z == 0)
                        {
                            queueNodes.Enqueue(node.GetNeighbour(currentDir));
                            queueBackDirs.Enqueue(-currentDir);
                        }
                    }

                    if (!node.IsSpecifiedByType(dt) && dt == GraphDirectionType.Call) //если направление так и не удалось задать в режиме вызова
                        node.IsCallFinish = true; //пометить финишную точку
                }
            }
        }

        //нейтрализовать
        public void Unpolarize(GraphDirectionType dt)
        {
            for (int z = 0; z < count; z++)
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                    {
                        nodes[x, y, z].SetDirectionByType(dt, Vector3P.Zero);
                        nodes[x, y, z].IsCallFinish = false;
                    }
        }

        //сбросить посещаемость
        public void ResetVisited()
        {
            for (int z = 0; z < count; z++)
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                        nodes[x, y, z].WasVisited = false;
        }
    }
}
