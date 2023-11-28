using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace CreateCards.Utilities
{
    public static class GraphicUtils
    {
        [DllImport("User32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        public static Graphics GraphicsWriter(this Stream metaStream)
        {
            var bm = new Bitmap(50, 50);
            bm.SetResolution(96, 96);
            Graphics grfx = Graphics.FromImage(bm);
            IntPtr ipHdc = grfx.GetHdc();
            var mf = new Metafile(metaStream, ipHdc);
            grfx.ReleaseHdc(ipHdc);
            grfx.Dispose();

            var result = Graphics.FromImage(mf);
            result.Flush();
            return result;
        }

        public static void DrawToScreen(this Image ig, Rectangle? r = null)
        {
            using (Graphics g = Graphics.FromHdc(GetDC(IntPtr.Zero)))
            {
                if (r == null)
                    r = new Rectangle(0, 0, ig.Width, ig.Height);
                g.DrawImage(ig, (Rectangle)r);
            }
        }
    }
}
