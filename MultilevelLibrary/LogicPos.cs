using PrimitiveData3D;

namespace MultilevelLibrary
{
    [System.Serializable]
    public struct LogicPos
    {
        public Vector3P Position { get; set; }
        public Vector3P Direction { get; set; }

        public LogicPos(Vector3P position, Vector3P direction)
        {
            Position = position;
            Direction = direction;
        }

        public static bool operator ==(LogicPos lp1, LogicPos lp2) => lp1.Equals(lp2);
        public static bool operator !=(LogicPos lp1, LogicPos lp2) => !lp1.Equals(lp2);

        public override bool Equals(object obj)
        {
            if (!(obj is LogicPos)) return false;

            LogicPos other = (LogicPos)obj;
            return Position == other.Position &&
                Direction == other.Direction;
        }

        public override int GetHashCode() => Position.Z * 100 + Position.X * 60 + Position.Y;
    }
}
