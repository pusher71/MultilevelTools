using System.Collections.Generic;
using MultilevelLibrary.Generation.Model;
using PrimitiveData3D;

namespace MultilevelLibrary.Generation
{
    static class TrapsZonesFinder
    {
        public static List<TrapsZone> GetTrapsZones(List<PathPart> pathParts)
        {
            List<TrapsZone> trapsZones = new List<TrapsZone>();
            for (int i = 0; i < pathParts.Count; i++)
            {
                PathPart current = pathParts[i];
                if (!current.IsCycledSafety)
                {
                    //создать зону ловушек
                    TrapsZone trapsZone = new TrapsZone(current);

                    //добавить источники безопасности в неё
                    foreach (GraphNode node in current.Nodes)
                    {
                        List<Vector3P> dirs = node.GetDirections();
                        for (int j = 0; j < dirs.Count; j++)
                        {
                            GraphNode neighbour = node.GetNeighbour(dirs[j]);
                            if (neighbour.IsCycledSafety)
                                trapsZone.AddNode(neighbour);
                        }
                    }

                    trapsZones.Add(trapsZone);
                }
            }

            return trapsZones;
        }
    }
}
