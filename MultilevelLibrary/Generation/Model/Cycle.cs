using System.Collections.Generic;

namespace MultilevelLibrary.Generation.Model
{
    class Cycle
    {
        public HashSet<GraphNodeLink> Links { get; }

        public Cycle() => Links = new HashSet<GraphNodeLink>();

        public HashSet<GraphNode> GetNodes()
        {
            HashSet<GraphNode> nodes = new HashSet<GraphNode>();
            foreach (GraphNodeLink link in Links)
                link.AddToHashSet(nodes);
            return nodes;
        }

        public static void Normalize(Cycle cycle1, Cycle cycle2)
        {
            HashSet<GraphNodeLink> intersect = Intersect(cycle1, cycle2);
            if (intersect.Count > 0)
            {
                HashSet<GraphNodeLink> path1 = Except(cycle1, cycle2);
                HashSet<GraphNodeLink> path2 = Except(cycle2, cycle1);
                if (path1.Count < path2.Count)
                {
                    if (path1.Count < intersect.Count) //если путь 1 минимальный
                    {
                        //сократить цикл 2
                        cycle2.Links.ExceptWith(intersect);
                        cycle2.Links.UnionWith(path1);
                    }
                }
                else if (path2.Count < path1.Count)
                {
                    if (path2.Count < intersect.Count) //если путь 2 минимальный
                    {
                        //сократить цикл 1
                        cycle1.Links.ExceptWith(intersect);
                        cycle1.Links.UnionWith(path2);
                    }
                }
            }
        }

        public static void NormalizeUnion(Cycle cycle1, Cycle cycle2)
        {
            HashSet<GraphNodeLink> intersect = Intersect(cycle1, cycle2);
            if (intersect.Count > 0)
            {
                HashSet<GraphNodeLink> path1 = Except(cycle1, cycle2);
                HashSet<GraphNodeLink> path2 = Except(cycle2, cycle1);
                if (path1.Count < path2.Count)
                {
                    if (path1.Count == intersect.Count) //если путь 1 минимальный и равен общему
                        cycle2.Links.UnionWith(path1); //дополнить цикл 2
                }
                else if (path2.Count < path1.Count)
                {
                    if (path2.Count == intersect.Count) //если путь 2 минимальный и равен общему
                        cycle1.Links.UnionWith(path2); //дополнить цикл 1
                }
            }
        }

        private static HashSet<GraphNodeLink> Intersect(Cycle cycle1, Cycle cycle2)
        {
            HashSet<GraphNodeLink> result = new HashSet<GraphNodeLink>();
            foreach (GraphNodeLink link1 in cycle1.Links)
                if (cycle2.Links.Contains(link1))
                    result.Add(link1);
            return result;
        }

        private static HashSet<GraphNodeLink> Except(Cycle cycle1, Cycle cycle2)
        {
            HashSet<GraphNodeLink> result = new HashSet<GraphNodeLink>();
            foreach (GraphNodeLink link1 in cycle1.Links)
                if (!cycle2.Links.Contains(link1))
                    result.Add(link1);
            return result;
        }
    }
}
