using MathLibrary.RenderImage;
using System;

namespace RenderImage.Base.Model
{
 public sealed class Plane : SimpleObject
 {
 private readonly TFTriple _normal;

 public Plane(RenderPoint position, RenderVector normal, RenderColor baseColor)
 : this(position, normal, baseColor, new TFTriple { X =0.4, Y =0.6, Z =0.0 }) { }

 public Plane(RenderPoint position, RenderVector normal, RenderColor baseColor, TFTriple surface)
 : base(position, baseColor, surface)
 {
 var n = normal.Value;
 _normal = n / n.GLen();
 _boundary = null;
 }

 public override bool HitTest(RenderRay ray, out HitData hit)
 {
 hit = default;
 var footp = Position.Value - ray.StartPoint.Value;
 var sgn = Math.Sign(_normal * ray.Direction.Value);
 var ok = Math.Sign(footp * _normal) == sgn;
 if (ok)
 {
 hit.Distance = (footp * _normal) / (_normal * ray.Direction.Value);
 hit.HitPoint = ray.RayPoint(hit.Distance);
 hit.Normalvec = _normal * (-sgn);
 hit.AmbientVal = _surface.X;
 hit.ReflectionVal = _surface.Y;
 hit.Refraction = _surface.Z;
 }
 return ok;
 }

 public override bool BoundaryTest(RenderRay ray, out double distance)
 {
 var footp = Position.Value - ray.StartPoint.Value;
 distance = -1.0;
 var ok = Math.Sign(footp * _normal) == Math.Sign(_normal * ray.Direction.Value);
 if (ok)
 distance = (footp * _normal) / (_normal * ray.Direction.Value);
 return ok;
 }
 }
}
