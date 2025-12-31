using ColorVis.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

struct Vector3
{
    public float X, Y, Z;
    public Vector3(float x, float y, float z) { X = x; Y = y; Z = z; }
    public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vector3 operator *(Vector3 a, float s) => new Vector3(a.X * s, a.Y * s, a.Z * s);
}

class Program
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
        float distance = 2.0f; // Abstand Kamera -> Szene (in HSL-Einheiten)
        float baseline = 0.1f; // Augenabstand (klein, in HSL-Einheiten)
        float fovDeg = 45f;

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
                float pitch = DegToRad(pitchDeg);
                float yaw = DegToRad(yawDeg);
                var rot = MatrixRotateYawPitch(yaw, pitch);

                camForward = Transform(rot, camForward);
                camUp = Transform(rot, camUp);
                camRight = Transform(rot, camRight);

                // Camera position: place at sceneCenter + camForward * distance
                Vector3 camPos = sceneCenter + camForward * distance;

                // Stereo offset: shift camera along camRight by +/- baseline/2
                float eyeOffset = (view == 0) ? -baseline / 2f : baseline / 2f;
                camPos = camPos + camRight * eyeOffset;

                // Projection parameters
                float aspect = (float)viewWidth / viewHeight;
                float fov = DegToRad(fovDeg);
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
                    float cx = Dot(rel, camRight);
                    float cy = Dot(rel, camUp);
                    float cz = Dot(rel, camForward);

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
                    float radius = 2f + 64f / zproj;

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
                    RgbToHsl(node.ColorRgb, out node.H, out node.S, out node.L);
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

    static float Dot(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    static Matrix3x3 MatrixRotateYawPitch(float yaw, float pitch)
    {
        // yaw around Y, pitch around X: R = R_x(pitch) * R_y(yaw)
        float cy = (float)Math.Cos(yaw), sy = (float)Math.Sin(yaw);
        float cx = (float)Math.Cos(pitch), sx = (float)Math.Sin(pitch);

        // R_y(yaw)
        var Ry = new Matrix3x3(
            cy, 0, sy,
            0, 1, 0,
            -sy, 0, cy
        );
        // R_x(pitch)
        var Rx = new Matrix3x3(
            1, 0, 0,
            0, cx, -sx,
            0, sx, cx
        );
        return Multiply(Rx, Ry);
    }

    static Vector3 Transform(Matrix3x3 m, Vector3 v)
    {
        return new Vector3(
            m.M11 * v.X + m.M12 * v.Y + m.M13 * v.Z,
            m.M21 * v.X + m.M22 * v.Y + m.M23 * v.Z,
            m.M31 * v.X + m.M32 * v.Y + m.M33 * v.Z
        );
    }

    static Matrix3x3 Multiply(Matrix3x3 a, Matrix3x3 b)
    {
        return new Matrix3x3(
            a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31,
            a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32,
            a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33,

            a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31,
            a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32,
            a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33,

            a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31,
            a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32,
            a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33
        );
    }

    struct Matrix3x3
    {
        public float M11, M12, M13, M21, M22, M23, M31, M32, M33;
        public Matrix3x3(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
        { M11 = m11; M12 = m12; M13 = m13; M21 = m21; M22 = m22; M23 = m23; M31 = m31; M32 = m32; M33 = m33; }
    }

    static float DegToRad(float d) => (float)(d * Math.PI / 180.0);

    // Convert System.Drawing.Color to HSL (0..1)
    static void RgbToHsl(Color c, out float h, out float s, out float l)
    {
        float r = c.R / 255f;
        float g = c.G / 255f;
        float b = c.B / 255f;
        float max = Math.Max(r, Math.Max(g, b));
        float min = Math.Min(r, Math.Min(g, b));
        l = (max + min) / 2f;
        if (Math.Abs(max - min) < 1e-6f) { h = 0f; s = 0f; return; }
        float d = max - min;
        s = l > 0.5f ? d / (2f - max - min) : d / (max + min);
        if (max == r) h = (g - b) / d + (g < b ? 6f : 0f);
        else if (max == g) h = (b - r) / d + 2f;
        else h = (r - g) / d + 4f;
        h /= 6f;
        h = h - (float)Math.Floor(h);
    }
}
