using MathLibrary.RenderImage;
using System;

namespace RenderImage.Base.Model
{
 public sealed class Disc : SimpleObject
 {
 private readonly TFTriple _normal;
 private readonly double _radius;

 public Disc(RenderPoint position, RenderVector normal, double radius, RenderColor baseColor)
 : this(position, normal, radius, baseColor, new TFTriple { X =0.6, Y =0.4, Z =0.0 }) { }

 public Disc(RenderPoint position, RenderVector normal, double radius, RenderColor baseColor, TFTriple surface)
 : base(position, baseColor, surface)
 {
 var n = normal.Value;
 _normal = n / n.GLen();
 _radius = radius;
 _boundary = null;
 }

 public override bool HitTest(RenderRay ray, out HitData hit)
 {
 hit = default;
 var place = Position.Value - ray.StartPoint.Value;
 var sgn = Math.Sign(_normal * ray.Direction.Value);
 var ok = Math.Sign(place * _normal) == sgn;
 if (ok)
 {
 hit.Distance = (place * _normal) / (_normal * ray.Direction.Value);
 hit.HitPoint = ray.RayPoint(hit.Distance);
 ok = (hit.HitPoint.Value - Position.Value).GLen() <= _radius;
 if (ok)
 {
 hit.Normalvec = _normal * (-sgn);
 hit.AmbientVal = _surface.X;
 hit.ReflectionVal = _surface.Y;
 hit.Refraction = _surface.Z;
 }
 else
 hit.Distance = -1.0;
 }
 return ok;
 }

 public override bool BoundaryTest(RenderRay ray, out double distance)
 {
 var place = Position.Value - ray.StartPoint.Value;
 distance = -1.0;
 var ok = Math.Sign(place * _normal) == Math.Sign(_normal * ray.Direction.Value);
 if (ok)
 {
 distance = (place * _normal) / (_normal * ray.Direction.Value);
 var hitPoint = ray.RayPoint(distance).Value;
 ok = (hitPoint - Position.Value).GLen() <= _radius;
 if (!ok) distance = -1.0;
 }
 return ok;
 }
 }
}
