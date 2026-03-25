using ColorVis.Data;
using System.Drawing;

public static class MathHelpers
{

    public static float DegToRad(float d) => (float)(d * Math.PI / 180.0);

    public static float Dot(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    public static Matrix3x3 MatrixRotateYawPitch(float yaw, float pitch)
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

    public static Matrix3x3 Multiply(Matrix3x3 a, Matrix3x3 b)
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

    // Convert System.Drawing.Color to HSL (0..1)
    public static void RgbToHsl(Color c, out float h, out float s, out float l)
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

    public static Vector3 Transform(Matrix3x3 m, Vector3 v)
    {
        return new Vector3(
            m.M11 * v.X + m.M12 * v.Y + m.M13 * v.Z,
            m.M21 * v.X + m.M22 * v.Y + m.M23 * v.Z,
            m.M31 * v.X + m.M32 * v.Y + m.M33 * v.Z
        );
    }
}