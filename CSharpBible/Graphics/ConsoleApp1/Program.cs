using ColorVis.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

partial class Program
{
    static void Main()
    {
        int levels = 8;
        var nodes = CreateNodes(levels);
        BuildNeighbors(nodes);

        // Bildparameter
        int viewWidth = 4000;
        int viewHeight = 4000;
        int spacing = -1000;
        int totalWidth = viewWidth * 2 + spacing;
        int totalHeight = viewHeight;

        // Kamera / Stereo-Parameter
        float yawDeg = -5f;   // Drehung um Y (horizontal)
        float pitchDeg = -18f;  // Drehung um X (vertical)
        float distance = 3.0f; // Abstand Kamera -> Szene (in HSL-Einheiten)
        float baseline = 0.1f; // Augenabstand (klein, in HSL-Einheiten)
        float fovDeg = 20f;

        // Szene: H,S,L liegen in [0,1]. Wir zentrieren die Szene um (0.5,0.5,0.5)
        Vector3 sceneCenter = new Vector3(0.5f, 0.5f, 0.5f);

        using (var bmp = new Bitmap(totalWidth, totalHeight))
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            // Erzeuge beide Ansichten (left, right)
            for (int view = 0; view < 2; view++)
            {
                // Kamera-Basis (vor Rotation): Kamera schaut auf -Z
                Vector3 camForward = new Vector3(0, 0, -1);
                Vector3 camUp = new Vector3(0, 1, 0);
                Vector3 camRight = new Vector3(1, 0, 0);

                // Rotation matrices (yaw then pitch)
                float pitch = MathHelpers.DegToRad(pitchDeg);
                float yaw = MathHelpers.DegToRad(yawDeg);
                var rot = MathHelpers.MatrixRotateYawPitch(yaw, pitch);

                camForward = MathHelpers.Transform(rot, camForward);
                camUp = MathHelpers.Transform(rot, camUp);
                camRight = MathHelpers.Transform(rot, camRight);

                // Camera position: place at sceneCenter + camForward * distance
                Vector3 camPos = sceneCenter + camForward * distance;

                // Stereo offset: shift camera along camRight by +/- baseline/2
                float eyeOffset = (view == 0) ? -baseline / 2f : baseline / 2f;
                camPos = camPos + camRight * eyeOffset;

                // Projection parameters
                float aspect = (float)viewWidth / viewHeight;
                float fov = MathHelpers.DegToRad(fovDeg);
                float focal = 1f / (float)Math.Tan(fov / 2f); // simple focal

                // Project nodes to 2D for this view
                var projected = new List<(int idx, float depth, PointF p, Color color, float radius)>();
                for (int i = 0; i < nodes.Count; i++)
                {
                    var n = nodes[i];
                    // world position relative to camera
                    Vector3 world = n.Pos3;
                    Vector3 rel = world - camPos;

                    // camera coordinate system: x = right, y = up, z = forward
                    float cx = MathHelpers.Dot(rel, camRight);
                    float cy = MathHelpers.Dot(rel, camUp);
                    float cz = MathHelpers.Dot(rel, camForward);

                    // if cz <= 0, point is behind camera; still project but mark depth
                    float eps = 1e-6f;
                    float zproj = cz;
                    if (zproj > -eps)
                        zproj = eps; // avoid division by zero / flip

                    // perspective projection
                    float px = (cx * focal) / zproj;
                    float py = (cy * focal) / zproj;

                    // map projected normalized coords to pixel coords in view rectangle
                    // choose a scale so that H,S in [0,1] nicely fill the view
                    float scale = 0.9f * Math.Min(viewWidth, viewHeight) / 2f;
                    float cxPixel = view * (viewWidth + spacing) + viewWidth / 2f + px * scale;
                    float cyPixel = viewHeight / 2f - py * scale;

                    // radius from L (use L to scale)
                    float radius = 2f + 100f / zproj;

                    projected.Add((i, zproj, new PointF(cxPixel, cyPixel), n.ColorRgb, radius));
                }

                // Depth sort: draw far to near (larger zproj = farther if camForward points outward)
                // Here cz is positive when point is in front (since camForward points outward), but zproj used above may be positive small; sort by depth descending (far first)
                projected.Sort((a, b) => a.depth.CompareTo(b.depth)); // smaller depth (near) last

                // Draw edges first (semi-transparent), sorted by average depth of endpoints
                using (var penEdge = new Pen(Color.FromArgb(60, 0, 0, 0), 1))
                {
                    // compute edge list with average depth
                    var edges = new List<(float depth, PointF a, PointF b)>();
                    foreach (var p in projected)
                    {
                        var node = nodes[p.idx];
                        foreach (var nb in node.Neighbors)
                        {
                            // draw each edge once: only if neighbor index > idx
                            if (nb <= p.idx) continue;
                            var q = projected.Find(x => x.idx == nb);
                            if (q.idx == 0 && q.idx != nb && !projected.Any(x => x.idx == nb)) continue;
                            float avgDepth = (p.depth + q.depth) * 0.5f;
                            edges.Add((avgDepth, p.p, q.p));
                        }
                    }
                    edges.Sort((a, b) => a.depth.CompareTo(b.depth));
                    foreach (var e in edges)
                    {
                        g.DrawLine(penEdge, e.a, e.b);
                    }
                }

                // Draw nodes (far -> near)
                foreach (var p in projected)
                {
                    float r = p.radius;
                    var rect = new RectangleF(p.p.X - r, p.p.Y - r, 2 * r, 2 * r);
                    using (var brush = new SolidBrush(p.color))
                    {
                        g.FillEllipse(brush, rect);
                    }
                    using (var pen = new Pen(Color.FromArgb(140, Color.Black), 0.5f))
                    {
                        g.DrawEllipse(pen, rect);
                    }
                }

                // Draw view separator and small label
                using (var font = new Font("Arial", 10))
                using (var brush = new SolidBrush(Color.Black))
                {
                    string label = (view != 0) ? "Left eye" : "Right eye";
                    float lx = view * (viewWidth + spacing) + 8;
                    float ly = 8;
                    g.DrawString(label, font, brush, lx, ly);
                }
            }

            // Save result (PNG)
            bmp.Save("stereo_layout.png", ImageFormat.Png);
        }

        Console.WriteLine("Stereo-Ansicht erzeugt.");
    }

    // Helpers

    static List<RgbNode> CreateNodes(int levels)
    {
        var list = new List<RgbNode>();
        for (int r = 0; r < levels; r++)
            for (int g = 0; g < levels; g++)
                for (int b = 0; b < levels; b++)
                {
                    var node = new RgbNode { R = r, G = g, B = b };
                    node.ColorRgb = Color.FromArgb(
                        (int)Math.Round((r + 0.5) * 255.0 / levels),
                        (int)Math.Round((g + 0.5) * 255.0 / levels),
                        (int)Math.Round((b + 0.5) * 255.0 / levels)
                    );
                    MathHelpers.RgbToHsl(node.ColorRgb, out node.H, out node.S, out node.L);
                    list.Add(node);
                }
        return list;
    }

    static void BuildNeighbors(List<RgbNode> nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            var a = nodes[i];
            for (int j = 0; j < nodes.Count; j++)
            {
                if (i == j) continue;
                var b = nodes[j];
                int manhattan = Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B);
                if (manhattan == 1) a.Neighbors.Add(j);
            }
        }
    }
}
