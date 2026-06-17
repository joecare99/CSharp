using MathLibrary.RenderImage;
using System;

namespace RenderImage.Base.Model
{
    public sealed class BoundaryBox : RenderBoundary
    {
        private TFTriple _boxSize;

        private static bool IsOutsideHalfExtentAndMovingAway(double distance, double halfExtent, double direction)
            => (distance - halfExtent > 0 && direction <= 0)
                || (distance + halfExtent < 0 && direction >= 0);

        private static bool IsRayMissingBox(TFTriple localDistance, TFTriple boxSize, TFTriple direction)
        {
            double halfX = boxSize.X * 0.5;
            double halfY = boxSize.Y * 0.5;
            double halfZ = boxSize.Z * 0.5;

            return IsOutsideHalfExtentAndMovingAway(localDistance.X, halfX, direction.X)
                || IsOutsideHalfExtentAndMovingAway(localDistance.Y, halfY, direction.Y)
                || IsOutsideHalfExtentAndMovingAway(localDistance.Z, halfZ, direction.Z);
        }

        public BoundaryBox(RenderPoint position, RenderVector size) : base(position)
        {
            _boxSize = size.Value;
        }

        public override bool BoundaryTest(RenderRay ray, out double distance)
        {
            var lDist = Position.Value - ray.StartPoint.Value;
            if (Math.Abs(lDist.X) < _boxSize.X * 0.5 &&
            Math.Abs(lDist.Y) < _boxSize.Y * 0.5 &&
            Math.Abs(lDist.Z) < _boxSize.Z * 0.5)
            {
                distance = 0.0;
                return true; // Startpoint inside
            }
            distance = -1.0;
            var dir = ray.Direction.Value;
            if (IsRayMissingBox(lDist, _boxSize, dir))
                return false;
            // XY plane test (Z face)
            if (Math.Abs(dir.Z) > 1e-12)
            {
                distance = (lDist.Z - Math.Sign(lDist.Z) * 0.5 * _boxSize.Z) / dir.Z;
                var testPoint = ray.RayPoint(distance).Value - Position.Value;
                if (Math.Abs(testPoint.X) <= _boxSize.X * 0.5 && Math.Abs(testPoint.Y) <= _boxSize.Y * 0.5)
                    return true;
            }
            // XZ plane test (Y face)
            if (Math.Abs(dir.Y) > 1e-12)
            {
                distance = (lDist.Y - Math.Sign(lDist.Y) * 0.5 * _boxSize.Y) / dir.Y;
                var testPoint = ray.RayPoint(distance).Value - Position.Value;
                if (Math.Abs(testPoint.X) <= _boxSize.X * 0.5 && Math.Abs(testPoint.Z) <= _boxSize.Z * 0.5)
                    return true;
            }
            // YZ plane test (X face)
            if (Math.Abs(dir.X) > 1e-12)
            {
                distance = (lDist.X - Math.Sign(lDist.X) * 0.5 * _boxSize.X) / dir.X;
                var testPoint = ray.RayPoint(distance).Value - Position.Value;
                if (Math.Abs(testPoint.Y) <= _boxSize.Y * 0.5 && Math.Abs(testPoint.Z) <= _boxSize.Z * 0.5)
                    return true;
            }
            distance = -1.0;
            return false;
        }
    }
}
