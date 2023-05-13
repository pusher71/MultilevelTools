using System.Collections.Generic;

namespace MultilevelLibrary.Generation.Model
{
    class SubPath
    {
        public List<GraphNode> Nodes { get; } //список позиций
        public bool IsShortest { get; set; } //является кратчайшим в данном цвете

        public SubPath()
        {
            Nodes = new List<GraphNode>();
            IsShortest = false;
        }

        public SubPath Clone()
        {
            SubPath copy = new SubPath();
            for (int i = 0; i < Nodes.Count; i++)
                copy.Nodes.Add(Nodes[i]);
            copy.IsShortest = IsShortest;
            return copy;
        }
    }
}
