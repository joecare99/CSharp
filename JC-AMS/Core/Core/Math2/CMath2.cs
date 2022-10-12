// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 09-22-2022
//
// Last Modified By : Mir
// Last Modified On : 09-24-2022
// ***********************************************************************
// <copyright file="CMath2.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;
using System.Numerics;
using JCAMS.Core.Logging;

namespace JCAMS.Core.Math2
{
    /// <summary>
    /// Class CMath2.
    /// </summary>
    public static class CMath2
    {
        #region Properties
        

        /// <summary>
        /// The is undefined point
        /// </summary>
        public static Func<Point, bool> IsUndefinedPoint = (Pt) => (Pt.X, Pt.Y) == (UndefinedPoint.X, UndefinedPoint.Y);

        /// <summary>
        /// The is undefined point f
        /// </summary>
        public static Func<PointF, bool> IsUndefinedPointF = (Pt) => float.IsNaN(Pt.X) || float.IsNaN(Pt.Y);

        /// <summary>
        /// The is undefined rectangle
        /// </summary>
        public static Func<Rectangle, bool> IsUndefinedRectangle = (Rt) => IsUndefinedPoint(Rt.Location) && Rt.Width == UndefinedRectangle.Width && Rt.Height == UndefinedRectangle.Height;

        /// <summary>
        /// The is undefined rectangle
        /// </summary>
        public static Func<RectangleF, bool> IsUndefinedRectangleF = (Rt) => IsUndefinedPointF(Rt.Location) && Rt.Width == UndefinedRectangleF.Width && Rt.Height == UndefinedRectangleF.Height;

        /// <summary>
        /// The deg to RAD
        /// </summary>
        public static Func<double, double> DegToRad = (angle) => angle * Math.PI / 180.0;

        /// <summary>
        /// The undefined point
        /// </summary>
        public static Point UndefinedPoint = new Point(int.MinValue, int.MinValue);

        /// <summary>
        /// The undefined point f
        /// </summary>
        public static PointF UndefinedPointF = new PointF(float.NaN, float.NaN);

        /// <summary>
        /// The undefined rectangle
        /// </summary>
        public static Rectangle UndefinedRectangle = new Rectangle(int.MinValue, int.MinValue, int.MinValue, int.MinValue);

        /// <summary>
        /// The undefined rectangle
        /// </summary>
        public static RectangleF UndefinedRectangleF = new RectangleF(float.NaN, float.NaN, float.NaN, float.NaN);
        #endregion

        #region Methods
        /// <summary>
        /// Initializes static members of the <see cref="CMath2"/> class.
        /// </summary>
        static CMath2()
        {
        }
        /// <summary>
        /// The is undefined point
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if the specified object is undefined; otherwise, <c>false</c>.</returns>
        public static bool IsUndefined<T>(T obj)
            => obj switch
            {
                Point p => IsUndefinedPoint(p),
                PointF p => IsUndefinedPointF(p),
                Rectangle r => IsUndefinedRectangle(r),
                RectangleF r => IsUndefinedRectangleF(r),
                _ => false
            };
        /// <summary>
        /// Abses the length of the arc.
        /// </summary>
        /// <param name="Deg">The angle in degr.</param>
        /// <param name="Radius">The radius.</param>
        /// <returns>System.Single.</returns>
        public static float AbsArcLength(double Deg, double Radius)
        {
            return (float)Math.Abs(CMath.Deg2Rad(Deg) * Radius);
        }

        /// <summary>
        /// Angles the specified a.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>System.Single.</returns>
        public static float Angle(PointF A, PointF B)
        {
            float Angle = 0f;
            if (IsUndefinedPointF(A) || IsUndefinedPointF(B))
            {
                return Angle;
            }
            if (Equals(A, B))
            {
                return 0f;
            }
            double OpppositeLeg = B.Y - A.Y;
            double AdjacentLeg = B.X - A.X;
            double Hypotenuse = DistanceAB(A, B);
            double Rad = OpppositeLeg / Hypotenuse;
            if (Rad > 1.0)
            {
                Rad = 1.0;
            }
            if (Rad < -1.0)
            {
                Rad = -1.0;
            }
            Angle = (float)CMath.Rad2Deg(Math.Asin(Rad));
            if (Math.Sign(AdjacentLeg) >= 0 && Math.Sign(OpppositeLeg) >= 0)
            {
                Angle = 0f + Angle;
            }
            else if (Math.Sign(AdjacentLeg) > 0 && Math.Sign(OpppositeLeg) < 0)
            {
                Angle = 360f + Angle;
            }
            else if (Math.Sign(AdjacentLeg) <= 0 && Math.Sign(OpppositeLeg) <= 0)
            {
                Angle = 180f - Angle;
            }
            else if (Math.Sign(AdjacentLeg) < 0 && Math.Sign(OpppositeLeg) > 0)
            {
                Angle = 180f - Angle;
            }
            return Angle;
        }

        /// <summary>
        /// Arcs the length.
        /// </summary>
        /// <param name="Deg">The deg.</param>
        /// <param name="Radius">The radius.</param>
        /// <returns>System.Single.</returns>
        public static float ArcLength(double Deg, double Radius)
        {
            return (float)(CMath.Deg2Rad(Deg) * Radius);
        }

        /// <summary>
        /// Bases the of point orthographic to straight.
        /// </summary>
        /// <param name="Point">The point.</param>
        /// <param name="StraightPoint1">The straight point1.</param>
        /// <param name="StraightPoint2">The straight point2.</param>
        /// <param name="Base">The base.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool BaseOfPointOrthographicToStraight(PointF Point, PointF StraightPoint1, PointF StraightPoint2, out PointF Base)
        {
            double A = Angle(StraightPoint1, StraightPoint2);
            PointF U = default;
            Base = new PointF(0f, 0f);
            U.X = StraightPoint2.X - StraightPoint1.X;
            U.Y = StraightPoint2.Y - StraightPoint1.Y;
            float Len = (float)Math.Sqrt(U.X * U.X + U.Y * U.Y);
            if (Len <= 0f)
            {
                return false;
            }
            U.X /= Len;
            U.Y /= Len;
            float Lambda = (Point.X - StraightPoint1.X) * U.X + (Point.Y - StraightPoint1.Y) * U.Y;
            Base.X = StraightPoint1.X + Lambda * U.X;
            Base.Y = StraightPoint1.Y + Lambda * U.Y;
            return true;
        }

        /// <summary>
        /// Bisectors the specified a.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>Point.</returns>
        public static Point Bisector(Point A, Point B)
        {
            return ToPoint(MiddlePoint(ToPointF(A), ToPointF(B)));
        }

        /// <summary>
        /// Bisectors the specified a.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>PointF.</returns>
        public static PointF MiddlePoint(PointF A, PointF B)
        {
            return new PointF( (B.X + A.X) / 2f,  (B.Y + A.Y) / 2f);
        }

        /// <summary>
        /// Circles the center.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <param name="C">The c.</param>
        /// <param name="Center">The center.</param>
        /// <param name="Radius">The radius.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CircleCenter(PointF A, PointF B, PointF C, out PointF Center, out double Radius)
        {
            Center = new PointF(float.NaN, float.NaN);
            Radius = double.NaN;
            double offset = Math.Pow(B.X, 2.0) + Math.Pow(B.Y, 2.0);
            double bc = (Math.Pow(A.X, 2.0) + Math.Pow(A.Y, 2.0) - offset) / 2.0;
            double cd = (offset - Math.Pow(C.X, 2.0) - Math.Pow(C.Y, 2.0)) / 2.0;
            double det = (A.X - B.X) * (B.Y - C.Y) - (B.X - C.X) * (A.Y - B.Y);
            if (Math.Abs(det) < 1E-05)
            {
                return false;
            }
            double idet = 1.0 / det;
            double centerx = (bc * (double)(B.Y - C.Y) - cd * (double)(A.Y - B.Y)) * idet;
            double centery = (cd * (double)(A.X - B.X) - bc * (double)(B.X - C.X)) * idet;
            Radius = Math.Sqrt(Math.Pow((double)B.X - centerx, 2.0) + Math.Pow((double)B.Y - centery, 2.0));
            Center = new PointF((float)centerx, (float)centery);
            return true;
        }

        /// <summary>
        /// Circles the center.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <param name="Radius">The radius.</param>
        /// <param name="CenterA">The center a.</param>
        /// <param name="CenterB">The center b.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CircleCenter(PointF A, PointF B, double Radius, out PointF CenterA, out PointF CenterB)
        {

            double twoabAX = -2f * A.X;
            double twoabAY = -2f * A.Y;
            double twoabBX = -2f * B.X;
            double twoabBY = -2f * B.Y;
            double AXsquare = A.X * A.X;
            double AYsquare = A.Y * A.Y;
            double BXsquare = B.X * B.X;
            double BYsquare = B.Y * B.Y;
            double radiussquare = Radius * Radius;
            double AX_twoab_BX = twoabAX + -1.0 * twoabBX;
            double AY_twoab_BY = twoabAY + -1.0 * twoabBY;
            double AX_square_BX = AXsquare + -1.0 * BXsquare;
            double AY_square_BY = AYsquare + -1.0 * BYsquare;
            double b_square_coefficient = AY_twoab_BY * AY_twoab_BY;
            CenterA = PointF.Empty;
            CenterB = PointF.Empty;
            try
            {
                if (AY_twoab_BY < 0.0)
                {
                    double a_square = b_square_coefficient + AX_twoab_BX * AX_twoab_BX;
                    double temp = AX_square_BX + AY_square_BY - (double)(A.Y * -1f) * AY_twoab_BY;
                    double a = b_square_coefficient * twoabAX + 2.0 * AX_twoab_BX * temp;
                    double constant = b_square_coefficient * AXsquare - b_square_coefficient * radiussquare + temp * temp;
                    double center_a = (-1.0 * a + Math.Sqrt(a * a - 4.0 * a_square * constant)) / (2.0 * a_square);
                    double center_b = (AX_twoab_BX * center_a + (AX_square_BX + AY_square_BY)) / (-1.0 * AY_twoab_BY);
                    if (!double.IsNaN(center_a) && !double.IsNaN(center_b))
                    {
                        CenterA = new PointF((float)center_a, (float)center_b);
                    }
                    center_a = (-1.0 * a - Math.Sqrt(a * a - 4.0 * a_square * constant)) / (2.0 * a_square);
                    center_b = (AX_twoab_BX * center_a + (AX_square_BX + AY_square_BY)) / (-1.0 * AY_twoab_BY);
                    if (!double.IsNaN(center_a) && !double.IsNaN(center_b))
                    {
                        CenterB = new PointF((float)center_a, (float)center_b);
                    }
                }
                else
                {
                    double a_square = b_square_coefficient + AX_twoab_BX * AX_twoab_BX;
                    double temp = -1.0 * (AX_square_BX + AY_square_BY) - (double)A.Y * AY_twoab_BY;
                    double a = b_square_coefficient * twoabAX + -2.0 * AX_twoab_BX * temp;
                    double constant = b_square_coefficient * AXsquare - b_square_coefficient * radiussquare + temp * temp;
                    double center_a = (-1.0 * a + Math.Sqrt(a * a - 4.0 * a_square * constant)) / (2.0 * a_square);
                    double center_b = (-1.0 * (AX_square_BX + AY_square_BY) + -1.0 * AX_twoab_BX * center_a) / AY_twoab_BY;
                    if (!double.IsNaN(center_a) && !double.IsNaN(center_b))
                    {
                        CenterA = new PointF((float)center_a, (float)center_b);
                    }
                    center_a = (-1.0 * a - Math.Sqrt(a * a - 4.0 * a_square * constant)) / (2.0 * a_square);
                    center_b = (-1.0 * (AX_square_BX + AY_square_BY) + -1.0 * AX_twoab_BX * center_a) / AY_twoab_BY;
                    if (!double.IsNaN(center_a) && !double.IsNaN(center_b))
                    {
                        CenterB = new PointF((float)center_a, (float)center_b);
                    }
                }
                return true;
            }
            catch (Exception Ex)
            {
                TLogging.Log(Ex);
                return false;
            }
        }

        /// <summary>
        /// Circles the intersection point.
        /// </summary>
        /// <param name="Pt1">The PT1.</param>
        /// <param name="R1">The r1.</param>
        /// <param name="Pt2">The PT2.</param>
        /// <param name="R2">The r2.</param>
        /// <param name="S1">The s1.</param>
        /// <param name="S2">The s2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CircleIntersectionPoint(PointF Pt1, double R1, PointF Pt2, double R2, out PointF S1, out PointF S2)
        {
            S1 = PointF.Empty;
            S2 = PointF.Empty;
            double dX = Pt2.X - Pt1.X;
            double dY = Pt2.Y - Pt1.Y;
            if (dX == 0.0 && dY == 0.0)
            {
                return false;
            }
            double d = Math.Sqrt(dX * dX + dY * dY);
            double a = (R1 * R1 - R2 * R2 + d * d) / (2.0 * d);
            double h2 = R1 * R1 - a * a;
            if (h2 < 0.0)
            {
                return false;
            }
            double h = Math.Sqrt(h2);
            S1.X = (float)((double)Pt1.X + a / d * dX - h / d * dY);
            S1.Y = (float)((double)Pt1.Y + a / d * dY + h / d * dX);
            S2.X = (float)((double)Pt1.X + a / d * dX + h / d * dY);
            S2.Y = (float)((double)Pt1.Y + a / d * dY - h / d * dX);
            return true;
        }

        /// <summary>
        /// Circles the intersection straight.
        /// </summary>
        /// <param name="M">The m.</param>
        /// <param name="R">The r.</param>
        /// <param name="Pt1">The PT1.</param>
        /// <param name="Pt2">The PT2.</param>
        /// <param name="S1">The s1.</param>
        /// <param name="S2">The s2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CircleIntersectionStraight(PointF M, double R, PointF Pt1, PointF Pt2, ref PointF S1, ref PointF S2)
        {
            S1 = PointF.Empty;
            S2 = PointF.Empty;
            if (!BaseOfPointOrthographicToStraight(M, Pt1, Pt2, out PointF Base))
            {
                return false;
            }
            float D = DistanceAB(M, Base);
            if (R - (double)D < 0.0)
            {
                return false;
            }
            float R2 = (float)Math.Sqrt(R * R - (double)(D * D));
            if ((double)R2 == R)
            {
                S1 = PointVectorToPoint(M, Angle(Pt1, Pt2), R);
                S2 = PointVectorToPoint(M, Angle(Pt1, Pt2) + 180f, R);
                return true;
            }
            if (!CircleIntersectionPoint(M, R, Base, R2, out S1, out S2))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Distances the ab.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>System.Single.</returns>
        public static float DistanceAB(Point A, Point B)
        {
            try
            {
                double d2 = (A.X - B.X) * (double)(A.X - B.X) + (A.Y - B.Y) * (double)(A.Y - B.Y);
                return (float)Math.Sqrt(d2);
            }
            catch (Exception Ex)
            {
                TLogging.Log(Ex);
                return float.NaN;
            }
        }

        /// <summary>
        /// Distances the ab.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>System.Single.</returns>
        public static float DistanceAB(PointF A, PointF B)
        {
            try
            {
                return (float)Math.Sqrt((A.X - B.X) * (A.X - B.X) + (A.Y - B.Y) * (A.Y - B.Y));
            }
            catch (Exception Ex)
            {
                TLogging.Log(Ex);
                return float.NaN;
            }
        }

        /// <summary>
        /// Distances the point segment.
        /// </summary>
        /// <param name="Point">The point.</param>
        /// <param name="SegStartPoint">The seg start point.</param>
        /// <param name="SegStopPoint">The seg stop point.</param>
        /// <param name="D">The d.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DistancePointSegment(PointF Point, PointF SegStartPoint, PointF SegStopPoint, out double D)
        {
            D = 9999.0;
            if (!BaseOfPointOrthographicToStraight(Point, SegStartPoint, SegStopPoint, out PointF Base))
            {
                return false;
            }
            PointF M = MiddlePoint(SegStartPoint, SegStopPoint);
            if (DistanceAB(M, Base) > DistanceAB(M, SegStartPoint))
            {
                D = Math.Min(DistanceAB(SegStartPoint, Point), DistanceAB(SegStopPoint, Point));
            }
            else
            {
                D = DistanceAB(Point, Base);
            }
            return true;
        }

        /// <summary>
        /// Distances the point straight.
        /// </summary>
        /// <param name="Point">The point.</param>
        /// <param name="StraightPoint1">The straight point1.</param>
        /// <param name="StraightPoint2">The straight point2.</param>
        /// <param name="D">The d.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DistancePointStraight(PointF Point, PointF StraightPoint1, PointF StraightPoint2, out double D)
        {
            D = 0.0;
            if (!BaseOfPointOrthographicToStraight(Point, StraightPoint1, StraightPoint2, out PointF Base))
            {
                return false;
            }
            D = DistanceAB(Point, Base);
            return true;
        }

        /// <summary>
        /// Intersections the line segment line segment.
        /// </summary>
        /// <param name="IntersectionPoint">The intersection point.</param>
        /// <param name="PtA1">The pt a1.</param>
        /// <param name="PtA2">The pt a2.</param>
        /// <param name="PtB1">The pt b1.</param>
        /// <param name="PtB2">The pt b2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool IntersectionLineSegmentLineSegment(out PointF IntersectionPoint, PointF PtA1, PointF PtA2, PointF PtB1, PointF PtB2)
        {
            IntersectionPoint = PointF.Empty;
            double a1 = PtA2.Y - PtA1.Y;
            double b1 = PtA1.X - PtA2.X;
            double c1 = PtA2.X * PtA1.Y - PtA1.X * PtA2.Y;
            double r3 = a1 * (double)PtB1.X + b1 * (double)PtB1.Y + c1;
            double r4 = a1 * (double)PtB2.X + b1 * (double)PtB2.Y + c1;
            if (r3 != 0.0 && r4 != 0.0 && Math.Sign(r3) == Math.Sign(r4))
            {
                return false;
            }
            double a2 = PtB2.Y - PtB1.Y;
            double b2 = PtB1.X - PtB2.X;
            double c2 = PtB2.X * PtB1.Y - PtB1.X * PtB2.Y;
            double r1 = a2 * (double)PtA1.X + b2 * (double)PtA1.Y + c2;
            double r2 = a2 * (double)PtA2.X + b2 * (double)PtA2.Y + c2;
            if (r1 != 0.0 && r2 != 0.0 && Math.Sign(r1) == Math.Sign(r2))
            {
                return false;
            }
            double denom = a1 * b2 - a2 * b1;
            if (denom == 0.0)
            {
                return false;
            }
            double x = (b1 * c2 - b2 * c1) / denom;
            double y = (a2 * c1 - a1 * c2) / denom;
            IntersectionPoint = new PointF((float)x, (float)y);
            return true;
        }

        /// <summary>
        /// Intersections the line segment line segment.
        /// </summary>
        /// <param name="IntersectionPointX">The intersection point x.</param>
        /// <param name="IntersectionPointY">The intersection point y.</param>
        /// <param name="PtA1X">The pt a1 x.</param>
        /// <param name="PtA1Y">The pt a1 y.</param>
        /// <param name="PtA2X">The pt a2 x.</param>
        /// <param name="PtA2Y">The pt a2 y.</param>
        /// <param name="PtB1X">The pt b1 x.</param>
        /// <param name="PtB1Y">The pt b1 y.</param>
        /// <param name="PtB2X">The pt b2 x.</param>
        /// <param name="PtB2Y">The pt b2 y.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool IntersectionLineSegmentLineSegment(out double IntersectionPointX, out double IntersectionPointY, double PtA1X, double PtA1Y, double PtA2X, double PtA2Y, double PtB1X, double PtB1Y, double PtB2X, double PtB2Y)
        {
            IntersectionPointX = 0.0;
            IntersectionPointY = 0.0;
            double a1 = PtA2Y - PtA1Y;
            double b1 = PtA1X - PtA2X;
            double c1 = PtA2X * PtA1Y - PtA1X * PtA2Y;
            double r3 = a1 * PtB1X + b1 * PtB1Y + c1;
            double r4 = a1 * PtB2X + b1 * PtB2Y + c1;
            if (r3 != 0.0 && r4 != 0.0 && Math.Sign(r3) == Math.Sign(r4))
            {
                return false;
            }
            double a2 = PtB2Y - PtB1Y;
            double b2 = PtB1X - PtB2X;
            double c2 = PtB2X * PtB1Y - PtB1X * PtB2Y;
            double r1 = a2 * PtA1X + b2 * PtA1Y + c2;
            double r2 = a2 * PtA2X + b2 * PtA2Y + c2;
            if (r1 != 0.0 && r2 != 0.0 && Math.Sign(r1) == Math.Sign(r2))
            {
                return false;
            }
            double denom = a1 * b2 - a2 * b1;
            if (denom == 0.0)
            {
                return false;
            }
            double x = (b1 * c2 - b2 * c1) / denom;
            double y = (a2 * c1 - a1 * c2) / denom;
            IntersectionPointX = x;
            IntersectionPointY = y;
            return true;
        }

        /// <summary>
        /// Offsets the specified points.
        /// </summary>
        /// <param name="Points">The points.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Offset(ref Point[] Points, int x, int y)
        {
            for (int I = 0; I < Points.Length; I++)
            {
                Points[I].Offset(x, y);
            }
            return true;
        }

        /// <summary>
        /// ps the specified pt.
        /// </summary>
        /// <param name="Pt">The pt.</param>
        /// <returns>PointF.</returns>
        public static PointF ToPointF(this Point Pt)
        {
            //            return new PointF(Pt.X, Pt.Y);
            return Pt;
        }

        /// <summary>
        /// ps the specified pt.
        /// </summary>
        /// <param name="Pt">The pt.</param>
        /// <returns>PointF.</returns>
        public static PointF ToPointF(this Vector2 v)
        {
            return new PointF(v.X, v.Y);
        }

        /// <summary>
        /// ps the specified pt.
        /// </summary>
        /// <param name="Pt">The pt.</param>
        /// <returns>Point.</returns>
        public static Point ToPoint(this PointF Pt)
        {
            return Point.Round(Pt);
        }

        /// <summary>
        /// ps the specified pt.
        /// </summary>
        /// <param name="v">The pt.</param>
        /// <returns>Point.</returns>
        public static Point ToPoint(this Vector2 v)
        {
            return v.ToPointF().ToPoint();
        }

        /// <summary>
        /// Computes a Vector[2] by an angle an a length
        /// </summary>
        /// <param name="dAngle">The vector angle. [rad]</param>
        /// <param name="dLength">Length of the vector.</param>
        /// <returns>PointF.</returns>
        public static Vector2 Vector2al(double dAngle, double dLength)
        {
            return new Vector2((float)(Math.Cos(dAngle) * dLength),(float)(Math.Sin(dAngle) * dLength));
        }

        /// <summary>
        /// Points the vector to point.
        /// </summary>
        /// <param name="Pt">The pt.</param>
        /// <param name="VectorAngle">The vector angle.</param>
        /// <param name="VectorLength">Length of the vector.</param>
        /// <returns>PointF.</returns>
        public static PointF PointVectorToPoint(PointF Pt, double VectorAngle, double VectorLength)
        {
            PointF result = Pt;
            result.X += (float)(Math.Cos(CMath.Deg2Rad(VectorAngle)) * VectorLength);
            result.Y += (float)(Math.Sin(CMath.Deg2Rad(VectorAngle)) * VectorLength);
            return result;
        }

        /// <summary>
        /// Rotates the specified p.
        /// </summary>
        /// <param name="P">The p.</param>
        /// <param name="Angle">The angle.</param>
        /// <param name="Center">The center.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Rotate(ref PointF P, double Angle, PointF Center)
        {
            PointF H = default;

            double Arcus = CMath.Deg2Rad(Angle);
            double Cosinus = Math.Cos(Arcus);
            double Sinus = Math.Sin(Arcus);
            H.X = P.X - Center.X;
            H.Y = P.Y - Center.Y;
            P.X = (float)(Cosinus * (double)H.X - Sinus * (double)H.Y + (double)Center.X);
            P.Y = (float)(Sinus * (double)H.X + Cosinus * (double)H.Y + (double)Center.Y);
            return true;
        }

        /// <summary>
        /// Rotates the specified p.
        /// </summary>
        /// <param name="P">The value.</param>
        /// <param name="Angle">The angle. [Rad]</param>
        /// <returns>the rotated point</returns>
        public static PointF Rotate(this PointF pValue, double dAngle)
        {
#if NET6_0_OR_GREATER
            (double s,double c) sc=Math.SinCos(dAngle);
#else
            (double s, double c) sc = (Math.Sin(dAngle), Math.Cos(dAngle));
#endif
            return new PointF((float)(pValue.X*sc.c+pValue.Y*sc.s),(float)(pValue.Y*sc.c-pValue.X*sc.s));
        }

        /// <summary>
        /// Rotates the specified p.
        /// </summary>
        /// <param name="P">The p.</param>
        /// <param name="Angle">The angle.</param>
        /// <param name="Center">The center.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Rotate(ref Point P, double Angle, Point Center)
        {
            Point H = default;
            double Arcus = CMath.Deg2Rad(Angle);
            double Cosinus = Math.Cos(Arcus);
            double Sinus = Math.Sin(Arcus);
            H.X = P.X - Center.X;
            H.Y = P.Y - Center.Y;
            P.X = (int)(Cosinus * H.X - Sinus * H.Y + Center.X);
            P.Y = (int)(Sinus * H.X + Cosinus * H.Y + Center.Y);
            return true;
        }

        /// <summary>
        /// Rotates the specified points.
        /// </summary>
        /// <param name="Points">The points.</param>
        /// <param name="RotationAngleDeg">The rotation angle deg.</param>
        /// <param name="Turnpoint">The turnpoint.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Rotate(ref Point[] Points, double RotationAngleDeg, Point Turnpoint)
        {
            for (int I = 0; I < Points.Length; I++)
            {
                Rotate(ref Points[I], RotationAngleDeg, Turnpoint);
            }
            return true;
        }

        /// <summary>
        /// Rotates the point.
        /// </summary>
        /// <param name="thePoint">The point.</param>
        /// <param name="theOrigin">The origin.</param>
        /// <param name="theRotationDeg">The rotation deg.</param>
        /// <returns>PointF.</returns>
        public static PointF RotatePoint(PointF thePoint, PointF theOrigin, float theRotationDeg)
        {
            PointF aTranslatedPoint = default;
            double RotationRad = CMath.Deg2Rad(theRotationDeg);
            double Sin = Math.Sin(RotationRad);
            double Cos = Math.Cos(RotationRad);
            aTranslatedPoint.X = (float)((double)theOrigin.X + (double)(thePoint.X - theOrigin.X) * Cos - (double)(thePoint.Y - theOrigin.Y) * Sin);
            aTranslatedPoint.Y = (float)((double)theOrigin.Y + (double)(thePoint.Y - theOrigin.Y) * Cos + (double)(thePoint.X - theOrigin.X) * Sin);
            return aTranslatedPoint;
        }

        /// <summary>
        /// Stretches the specified points.
        /// </summary>
        /// <param name="Points">The points.</param>
        /// <param name="Ref">The reference.</param>
        /// <param name="dX">The d x.</param>
        /// <param name="dY">The d y.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Stretch(ref Point[] Points, Point Ref, double dX, double dY)
        {
            for (int I = 0; I < Points.Length; I++)
            {
                Points[I].X = (int)Math.Floor((Points[I].X - Ref.X) * dX);
                Points[I].Y = (int)Math.Floor((Points[I].Y - Ref.Y) * dY);
            }
            return true;
        }

        /// <summary>
        /// Stretches the specified rect.
        /// </summary>
        /// <param name="Rect">The rect.</param>
        /// <param name="Ref">The reference.</param>
        /// <param name="dX">The d x.</param>
        /// <param name="dY">The d y.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Stretch(ref Rectangle Rect, Point Ref, double dX, double dY)
        {
            int X = (int)Math.Floor((Rect.X - Ref.X) * dX);
            int Y = (int)Math.Floor((Rect.Y - Ref.Y) * dY);
            int W = (int)Math.Floor(Rect.Width * dX);
            int H = (int)Math.Floor(Rect.Height * dY);
            Rect = new Rectangle(X, Y, W, H);
            return true;
        }

        /// <summary>
        /// Converts to array.
        /// </summary>
        /// <param name="Rect">The rect.</param>
        /// <returns>Point[].</returns>
        public static Point[] ToArray(Rectangle Rect)
        {
            return new Point[5]
            {
                new Point(Rect.Left, Rect.Top),
                new Point(Rect.Right, Rect.Top),
                new Point(Rect.Right, Rect.Bottom),
                new Point(Rect.Left, Rect.Bottom),
                new Point(Rect.Left, Rect.Top)
            };
        }
#endregion
    }
}