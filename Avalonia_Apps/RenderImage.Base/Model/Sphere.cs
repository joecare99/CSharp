using MathLibrary.RenderImage;
using System;

namespace RenderImage.Base.Model
{
 public sealed class Sphere : SimpleObject
 {
 private readonly double _radius;

 public Sphere(RenderPoint position, double radius, RenderColor baseColor)
 : this(position, radius, baseColor, new TFTriple { X =0.4, Y =0.6, Z =0.0 }) { }

 public Sphere(RenderPoint position, double radius, RenderColor baseColor, TFTriple surface)
 : base(position, baseColor, surface)
 {
 _radius = radius;
 _boundary = new BoundarySphere(position, radius);
 }

 public override bool HitTest(RenderRay ray, out HitData hit)
 {
 hit = default;
 var footVec = Position.Value - ray.StartPoint.Value;
 var footLen = footVec.GLen();
 var inside = footLen <= _radius;
 var footp = footVec * ray.Direction.Value;
 if (footp >0 || inside)
 {
 var offset = Math.Sqrt(Math.Abs(Math.Pow((Position.Value - ray.StartPoint.Value).GLen(),2) - footp * footp));
 var ok = offset <= _radius;
 if (ok)
 {
 if (!inside)
 {
 hit.Distance = footp - Math.Sqrt(_radius * _radius - offset * offset);
 hit.HitPoint = ray.RayPoint(hit.Distance);
 hit.Normalvec = (hit.HitPoint.Value - Position.Value) / _radius;
 }
 else
 {
 hit.Distance = footp + Math.Sqrt(_radius * _radius - offset * offset);
 hit.HitPoint = ray.RayPoint(hit.Distance);
 hit.Normalvec = (Position.Value - hit.HitPoint.Value) / _radius;
 }
 hit.AmbientVal = _surface.X;
 hit.ReflectionVal = _surface.Y;
 hit.Refraction = _surface.Z;
 return true;
 }
 }
 return false;
 }
 }
}
