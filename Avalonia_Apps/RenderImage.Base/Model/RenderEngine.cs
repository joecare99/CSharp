using MathLibrary.RenderImage;
using System;
using System.Collections.Generic;

namespace RenderImage.Base.Model;

public class RenderEngine
{
    private readonly List<RenderBaseObject> _objects = new();
    private readonly List<RenderLightSource> _lightSources = new();
    private RenderCamera? _camera;
    private int _maxDepth = 100;
    private double _minShare = 1e-4;

    public int MaxDepth { get => _maxDepth; set => _maxDepth = value; }

    private static RenderColor Black => new() { Red = 0, Green = 0, Blue = 0 };

    private HitData TestRayIntersection(in RenderRay ray, out RenderBaseObject? hitObj)
    {
        hitObj = null;
        var best = new HitData { Distance = -1.0 };
        foreach (var obj in _objects)
        {
            if (!obj.BoundaryTest(ray, out var _)) continue;
            if (obj.HitTest(ray, out var hd) && (best.Distance < 0 || hd.Distance < best.Distance))
            {
                best = hd; hitObj = obj;
            }
        }
        return best;
    }

    public RenderColor Trace(RenderRay ray, double share, int depth)
    {
        if (depth > _maxDepth || share < _minShare) return Black;
        var hit = TestRayIntersection(ray, out var objHit);
        if (hit.Distance >= 0 && objHit != null)
        {
            // base color at hit point
            RenderColor colorAtHP = Black;
            if (objHit is SimpleObject so)
                colorAtHP = so.GetColorAt(hit.HitPoint);
            else if (objHit is IHasColor hc)
                colorAtHP = hc.GetColorAt(hit.HitPoint);
            var ambient = colorAtHP * (0.1 * hit.AmbientVal);

            // direct lighting
            var hpRay = RenderRay.Init(hit.HitPoint, hit.Normalvec);
            var direct = Black;
            double maxLight = 0.0;
            for (int i = 0; i < _lightSources.Count; i++)
            {
                var ls = _lightSources[i];
                var lightVec = ls.Position.Value - hit.HitPoint.Value;
                maxLight += ls.MaxIntensity(new RenderVector { Value = lightVec * (-1) });
                if ((lightVec * hit.Normalvec) > 0)
                {
                    hpRay.Direction = new RenderVector { Value = lightVec };
                    if (objHit.HitTest(hpRay, out var h2) && h2.Distance < 1e-6)
                        hpRay.StartPoint = new RenderPoint { Value = hit.HitPoint.Value + hit.Normalvec.Value * 1e-4 };
                    var block = TestRayIntersection(hpRay, out var blockObj);
                    var vecLen = lightVec.GLen();
                    if (block.Distance < 0 || ReferenceEquals(blockObj, ls) || block.Distance >= vecLen)
                    {
                        var intensity = ls.FalloffIntensity(new RenderVector { Value = lightVec * (-1) }) * (hpRay.Direction.Value * hit.Normalvec);
                        direct += intensity * colorAtHP.Filter(ls.ProjectedColor(new RenderVector { Value = lightVec * (-1) }));
                    }
                }
            }
            if (maxLight == 0.0)
                direct = colorAtHP * (0.9 * hit.AmbientVal);
            else
                direct = direct * (1.0 / maxLight) * (0.9 * hit.AmbientVal);

            // reflection
            RenderColor reflect;
            if (hit.ReflectionVal < 1e-4)
                reflect = Black;
            else
            {
                hpRay.Direction = ray.ReflectDir(hit.Normalvec);
                hpRay.StartPoint = new RenderPoint { Value = hit.HitPoint.Value + hpRay.Direction.Value * 1e-4 };
                reflect = Trace(hpRay, hit.ReflectionVal, depth + 1) * hit.ReflectionVal;
            }

            // refraction and phong not implemented
            return ambient + direct + reflect;
        }
        return Black; // sky
    }

    // System-agnostic render into a pixel buffer abstraction
    public void Render(IPixelBuffer buffer)
    {
        if (_camera == null) throw new InvalidOperationException("Camera not set");
        int width = buffer.Width;
        int height = buffer.Height;

        // Thread-sicher: erst in temporäres Array schreiben
        var tmp = new RenderColor[height, width];

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        Parallel.For(0, height, options, y =>
        {
            // pro Thread/Zeile ein eigener Ray (keine geteilten Mutationen)
            var ray = RenderRay.Init(_camera.Position, _camera.DefaultDirection);

            for (int x = 0; x < width; x++)
            {
                ray.Direction = _camera[new IntPoint(x, y)];
                tmp[y, x] = Trace(ray, 1.0, 1);
            }
        });

        // Serielles Kopieren ins Ziel (falls IPixelBuffer nicht thread-sicher ist)
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                buffer.SetPixel(x, y, tmp[y, x]);
            }
        }
    }

    // Convenience: render to a2D color array (no external dependencies)
    public RenderColor[,] RenderToArray()
    {
        if (_camera == null) throw new InvalidOperationException("Camera not set");
        int width = (int)Math.Round(_camera.Resolution.X);
        int height = (int)Math.Round(_camera.Resolution.Y);
        var buf = new ArrayPixelBuffer(Math.Max(1, width), Math.Max(1, height));
        Render(buf);
        return buf.Pixels;
    }

    public void Append(RenderBaseObject aObj)
    {
        _objects.Add(aObj);
        if (aObj is RenderCamera cam)
        {
            _camera = cam;
        }
        if (aObj is RenderLightSource ls)
        {
            _lightSources.Add(ls);
        }
    }
}
