using System;
using PrimitiveData3D;

namespace MultilevelLibrary.Generation.Model
{
    struct Ray
    {
        public Vector3P Position { get; }
        private double angle;
        private readonly bool canRiseOnly;

        public Ray(Vector3P position, double angle, bool canRiseOnly)
        {
            Position = position;
            this.angle = angle;
            this.canRiseOnly = canRiseOnly;
            Normalize();
        }

        public Ray(Vector3P position, Vector3P target, bool canRiseOnly)
        {
            Position = position;
            angle = GetAngle(Position, target);
            this.canRiseOnly = canRiseOnly;
            Normalize();
        }

        public void RotateTo(Vector3P position)
        {
            double angleNew = GetAngle(Position, position);
            if ((angle > angleNew && !canRiseOnly) ||
                (angle < angleNew && canRiseOnly))
            {
                angle = angleNew;
                Normalize();
            }
        }

        private void Normalize()
        {
            if (canRiseOnly &&
                angle == Math.PI)
                angle = -Math.PI;
        }

        public static bool operator ==(Ray r1, Ray r2) => r1.angle == r2.angle;
        public static bool operator !=(Ray r1, Ray r2) => r1.angle != r2.angle;
        public static bool operator <(Ray r1, Ray r2) => r1.angle < r2.angle;
        public static bool operator >(Ray r1, Ray r2) => r1.angle > r2.angle;
        public static bool operator <=(Ray r1, Ray r2) => r1.angle <= r2.angle;
        public static bool operator >=(Ray r1, Ray r2) => r1.angle >= r2.angle;

        public override bool Equals(object obj)
        {
            if (!(obj is Ray)) return false;

            Ray other = (Ray)obj;
            return Position == other.Position &&
                angle == other.angle;
        }

        public override int GetHashCode() => Position.Z * 100 + Position.X * 60 + Position.Y;
        public static double GetAngle(Vector3P position1, Vector3P position2) => Math.Atan2(position2.Y - position1.Y, position2.X - position1.X);
    }
}
