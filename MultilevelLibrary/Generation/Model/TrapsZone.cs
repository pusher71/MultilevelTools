using System.Collections.Generic;

namespace MultilevelLibrary.Generation.Model
{
    class TrapsZone
    {
        public PathPart Target { get; }
        public HashSet<GraphNode>[] SafetySources { get; }

        public TrapsZone(PathPart target)
        {
            Target = target;
            SafetySources = new HashSet<GraphNode>[Target.Neighbours.Count];
        }

        public void AddNode(GraphNode node)
        {
            int index = Target.Neighbours.FindIndex((n) => n.Nodes.Contains(node));
            if (SafetySources[index] == null)
                SafetySources[index] = new HashSet<GraphNode>();
            SafetySources[index].Add(node);
        }
    }
}
