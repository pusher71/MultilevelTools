using System;
using System.Collections.Generic;
using MultilevelLibrary.Generation;
using MultilevelLibrary.Generation.Model;
using PrimitiveData3D;

namespace MultilevelLibrary
{
    public class GraphNode
    {
        public Vector3P Position { get; } //позиция
        private readonly bool isEnemyCanPlaced; //враг может находиться
        private readonly Dictionary<Vector3P, GraphNode> neighbours; //соседние вершины
        private readonly List<Vector3P> directions; //направления
        private readonly List<Vector3P> directionsForEnemy; //направления для врага
        private Vector3P dirToBottom; //направление на 1 этаж
        private Vector3P dirToTop; //направление к выходной будке
        private Vector3P dirCall; //направление на вызов
        public float EnemyHeightAdjustment { get; set; } //регулировка высоты врага
        public bool IsCallFinish { get; set; } //вызов завершится на данной ячейке

        //для генерации
        internal MazeMap<bool> VisibilityMap { get; private set; } //карта видимости
        internal HashSet<Vector3P> DirectionsProhibited { get; } //запрещённые направления для обхода циклов
        internal bool IsCycledSafety { get; set; } //безопасная позиция цикла
        internal bool IsTrap { get; set; } //ловушка
        internal bool WasVisited { get; set; } //была посещена при обходе
        internal bool IsSafetyRoom { get; set; } //безопасная комната
        internal bool IsSubPathsFinish { get; set; } //целевая позиция подчастей путей
        internal HashSet<SubPathGroup> SubPathGroups { get; } //группы подчастей путей, которые проходят через данную позицию
        internal bool IsCameraProhibited { get; set; } //позиция запрещена для камер
        
        public GraphNode(Vector3P position, bool isEnemyCanPlaced)
        {
            Position = position;
            this.isEnemyCanPlaced = isEnemyCanPlaced;
            neighbours = new Dictionary<Vector3P, GraphNode>();
            directions = new List<Vector3P>();
            directionsForEnemy = new List<Vector3P>();
            dirToBottom = Vector3P.Zero;
            dirToTop = Vector3P.Zero;
            dirCall = Vector3P.Zero;
            IsCallFinish = false;

            DirectionsProhibited = new HashSet<Vector3P>();
            IsCycledSafety = false;
            IsTrap = false;
            WasVisited = false;
            IsSafetyRoom = false;
            IsSubPathsFinish = false;
            SubPathGroups = new HashSet<SubPathGroup>();
            IsCameraProhibited = false;
        }

        //получить соседнюю вершину по направлению
        internal GraphNode GetNeighbour(Vector3P direction) => neighbours[direction];

        //получить список направлений
        internal List<Vector3P> GetDirections() => directions;

        //получить список направлений для врага
        internal List<Vector3P> GetDirectionsForEnemy() => directionsForEnemy;

        //добавить направление и соседа по нему
        internal void AddDirectionAndNeighbour(Vector3P direction, GraphNode neighbour)
        {
            directions.Add(direction);
            neighbours.Add(direction, neighbour);
            if (neighbour.isEnemyCanPlaced)
                directionsForEnemy.Add(direction);
        }

        //разорвать связь с соседней вершиной по направлению
        internal void BreakConnectionWithNeighbour(Vector3P direction)
        {
            directions.Remove(direction);
            directionsForEnemy.Remove(direction);
            neighbours.Remove(direction);
        }

        //получить направление по типу
        public Vector3P GetDirectionByType(GraphDirectionType dt)
        {
            switch (dt)
            {
                case GraphDirectionType.Bottom:
                    return dirToBottom;
                case GraphDirectionType.Top:
                    return dirToTop;
                case GraphDirectionType.Call:
                    return dirCall;
                default:
                    throw new Exception("Unknown DirectionType.");
            }
        }

        //установить направление по типу
        internal void SetDirectionByType(GraphDirectionType dt, Vector3P direction)
        {
            switch (dt)
            {
                case GraphDirectionType.Bottom:
                    dirToBottom = direction;
                    break;
                case GraphDirectionType.Top:
                    dirToTop = direction;
                    break;
                case GraphDirectionType.Call:
                    dirCall = direction;
                    break;
                default:
                    throw new Exception("Unknown DirectionType.");
            }
        }

        //указано ли направление по типу
        public bool IsSpecifiedByType(GraphDirectionType dt)
        {
            switch (dt)
            {
                case GraphDirectionType.Bottom:
                    return dirToBottom != Vector3P.Zero;
                case GraphDirectionType.Top:
                    return dirToTop != Vector3P.Zero;
                case GraphDirectionType.Call:
                    return dirCall != Vector3P.Zero;
                default:
                    throw new Exception("Unknown DirectionType.");
            }
        }

        //получить случайное направление для врага, кроме заданного
        public Vector3P GetRandomDirectionForEnemy(Vector3P excluded)
        {
            Random r = new Random();
            Vector3P direction;
            if (directionsForEnemy.Count == 1)
                direction = directionsForEnemy[0];
            else
            {
                do direction = directionsForEnemy[r.Next(directionsForEnemy.Count)];
                while (direction == excluded);
            }
            return direction;
        }

        //получить карту видимости
        internal MazeMap<bool> GetVisibilityMap()
        {
            if (VisibilityMap == null)
                VisibilityMap = VisibilityProcessor.CalculateVisibilityMap(Position * 2 + 1);
            return VisibilityMap;
        }

        //камера поставлена
        internal void OnCameraPlaced()
        {
            //оповестить все группы подчастей путей
            foreach (SubPathGroup subPathGroup in SubPathGroups)
                subPathGroup.OnCameraPlaced(this);
        }

        public override int GetHashCode() => Position.GetHashCode();
    }
}
