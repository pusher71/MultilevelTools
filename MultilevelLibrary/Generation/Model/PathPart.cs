using System.Collections.Generic;

namespace MultilevelLibrary.Generation.Model
{
    class PathPart
    {
        public List<PathPart> Neighbours { get; } //список соседних частей путей
        public HashSet<GraphNode> Nodes { get; } //набор позиций
        public bool IsCycledSafety { get; } //является безопасным от циклов

        public PathPart(bool isCycledSafety)
        {
            Neighbours = new List<PathPart>();
            Nodes = new HashSet<GraphNode>();
            IsCycledSafety = isCycledSafety;
        }
    }
}
