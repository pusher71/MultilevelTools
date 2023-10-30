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
                        nodes[x, y, z] = new GraphNode(new Vector3P(x, y, z));

            //связать вершины между собой и определить возможные направления
            for (int z = 0; z < count; z++)
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                    {
                        Vector3P position = new Vector3P(x, y, z);
                        GraphNode currentNode = GetGraphNode(position);
                        int item = maze.Map.Get(position * 2 + 1);

                        bool onStairsAngle = Utils.IsStairs(item); //ведущий угол ступенек
                        if (onStairsAngle)
                        {
                            Vector3P stairsDir = Vector3P.FromNumber(item - Utils.IndexStairs);
                            Vector3P stairsDirOpposite = stairsDir.TurnOpposite();

                            AddDirection(currentNode, stairsDirOpposite);
                            AddDirection(currentNode, Vector3P.Up);

                            //вершина сверху ступенек
                            GraphNode upperNode = currentNode.GetNeighbour(Vector3P.Up);
                            AddDirection(upperNode, Vector3P.Down);
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

                        //добавить направления на дырки в полу
                        if (maze.Map.Get(position * 2 + 1 + Vector3P.Up) == Utils.IndexHole)
                            AddDirection(currentNode, Vector3P.Up);
                        if (maze.Map.Get(position * 2 + 1 + Vector3P.Down) == Utils.IndexHole)
                            AddDirection(currentNode, Vector3P.Down);
                    }
        }

        //получить вершину графа по Vector3P позиции
        public GraphNode GetGraphNode(Vector3P position) =>
            nodes[position.X, position.Y, position.Z];

        //добавить направление к вершине графа
        private void AddDirection(GraphNode node, Vector3P direction) =>
            node.AddDirectionAndNeighbour(direction, GetGraphNode(node.Position + direction));
    }
}
