using System.Collections.Generic;

namespace MultilevelLibrary.Generation.Model
{
    class GraphNodeLink
    {
        private GraphNode Node1 { get; }
        private GraphNode Node2 { get; }

        public GraphNodeLink(GraphNode node1, GraphNode node2)
        {
            Node1 = node1;
            Node2 = node2;
        }

        public void AddToHashSet(HashSet<GraphNode> nodes)
        {
            nodes.Add(Node1);
            nodes.Add(Node2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GraphNodeLink)) return false;

            GraphNodeLink other = (GraphNodeLink)obj;
            return (Node1 == other.Node1 && Node2 == other.Node2) ||
                 (Node1 == other.Node2 && Node2 == other.Node1);
        }

        public override int GetHashCode() =>
            Node1.GetHashCode() + Node2.GetHashCode();
    }
}
