using MathLibrary.RenderImage;
using System;

namespace RenderImage.Base.Model
{
 public sealed class BoundarySphere : RenderBoundary
 {
 private double _boundingRadius;

 public BoundarySphere(RenderPoint position, double radius) : base(position)
 {
 _boundingRadius = radius;
 }

 public override bool BoundaryTest(RenderRay ray, out double distance)
 {
 var vecToPos = (Position.Value - ray.StartPoint.Value);
 if (vecToPos.GLen() <= _boundingRadius)
 {
 distance =0.0;
 return true;
 }
 distance = -1.0;
 var llDist = vecToPos * ray.Direction.Value;
 if (llDist <0)
 return false;
 var lqDistq = vecToPos * vecToPos - llDist * llDist;
 var ok = lqDistq <= _boundingRadius * _boundingRadius;
 if (ok)
 distance = llDist - Math.Sqrt(_boundingRadius * _boundingRadius - lqDistq);
 return ok;
 }
 }
}
