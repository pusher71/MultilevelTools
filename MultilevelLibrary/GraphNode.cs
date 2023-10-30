using System.Collections.Generic;
using PrimitiveData3D;

namespace MultilevelLibrary
{
    public class GraphNode
    {
        public Vector3P Position { get; } //позиция
        private readonly Dictionary<Vector3P, GraphNode> neighbours; //соседние вершины
        private readonly List<Vector3P> directions; //направления
        
        public GraphNode(Vector3P position)
        {
            Position = position;
            neighbours = new Dictionary<Vector3P, GraphNode>();
            directions = new List<Vector3P>();
        }

        //получить соседнюю вершину по направлению
        internal GraphNode GetNeighbour(Vector3P direction) => neighbours[direction];

        //получить список направлений
        internal List<Vector3P> GetDirections() => directions;

        //добавить направление и соседа по нему
        internal void AddDirectionAndNeighbour(Vector3P direction, GraphNode neighbour)
        {
            directions.Add(direction);
            neighbours.Add(direction, neighbour);
        }

        //разорвать связь с соседней вершиной по направлению
        internal void BreakConnectionWithNeighbour(Vector3P direction)
        {
            directions.Remove(direction);
            neighbours.Remove(direction);
        }

        public override int GetHashCode() => Position.GetHashCode();
    }
}
