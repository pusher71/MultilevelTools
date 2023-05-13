using System.Collections.Generic;

namespace MultilevelLibrary.Generation.Model
{
    class SubPathGroup
    {
        private readonly List<SubPath> subPaths;
        private readonly HashSet<GraphNode> nodes; //все позиции подчастей путей

        public SubPathGroup(List<SubPath> subPaths)
        {
            this.subPaths = subPaths;

            //найти кратчайшую длину подчастей путей
            int minLength = Constants.MAX_MAZE_VOLUME;
            for (int i = 0; i < subPaths.Count; i++)
                if (minLength > subPaths[i].Nodes.Count)
                    minLength = subPaths[i].Nodes.Count;

            //пометить кратчайшие из подчастей путей
            for (int i = 0; i < subPaths.Count; i++)
                if (subPaths[i].Nodes.Count == minLength)
                    subPaths[i].IsShortest = true;

            //определить набор всех позиций подчастей путей
            nodes = new HashSet<GraphNode>();
            for (int i = 0; i < subPaths.Count; i++)
                for (int j = 0; j < subPaths[i].Nodes.Count; j++)
                    nodes.Add(subPaths[i].Nodes[j]);

            //добавить ссылки на себя во все позиции
            foreach (GraphNode node in nodes)
                node.SubPathGroups.Add(this);

            //обновить допустимость камер
            UpdateCameraPermission();
        }

        //получить набор позиций на кратчайших подчастях путей
        public HashSet<GraphNode> GetNodesFromShortestSubPaths()
        {
            HashSet<GraphNode> result = new HashSet<GraphNode>();
            for (int i = 0; i < subPaths.Count; i++)
                if (subPaths[i].IsShortest)
                    for (int j = 0; j < subPaths[i].Nodes.Count; j++)
                        result.Add(subPaths[i].Nodes[j]);

            return result;
        }

        //обработать поставленную камеру
        public void OnCameraPlaced(GraphNode cameraNode)
        {
            //лопнуть все подчасти путей, которые зацепила камера
            for (int i = subPaths.Count - 1; i >= 0; i--)
                if (IsSubPathTriggered(subPaths[i], cameraNode))
                    subPaths.RemoveAt(i);

            //переопределить допустимость камер
            UpdateCameraPermission();
        }

        //обновить допустимость камер на всех позициях подчастей путей
        private void UpdateCameraPermission()
        {
            foreach (GraphNode node in nodes)
                if (!node.IsCameraProhibited) //если текущая позиция пока не была запрещена для камер
                {
                    //запретить текущую позицию для камер, если она цепляет все подчасти путей
                    bool isNodeInAllSubPaths = true;
                    for (int i = 0; i < subPaths.Count; i++)
                        if (!IsSubPathTriggered(subPaths[i], node))
                            isNodeInAllSubPaths = false;
                    if (isNodeInAllSubPaths)
                        node.IsCameraProhibited = true;
                }
        }

        //позиция цепляет подчасть путей
        private bool IsSubPathTriggered(SubPath subPath, GraphNode node) =>
            subPath.Nodes.IndexOf(node) > Constants.BRIDGE_CAMERAS_THRESHOLD;
    }
}
