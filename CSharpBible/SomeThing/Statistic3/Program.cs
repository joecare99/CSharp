using System;
using System.Collections.Generic;
using System.Threading;
class P
{
    class V { public int a, b, c; public bool d; public Random r = new Random(); }
    static void Main()
    {
        var s = new List<int[]> { new[] { 300, 365, 410, 397, 408, 415, 401 }, new[] { 250, 336, 347, 364, 355, 347, 360, 372 }, new[] { 240, 310, 351, 357, 354, 345, 341, 354 }, new[] { 200, 277, 311, 310, 316, 301 }, new[] { 260, 335, 371, 378, 357, 374, 365, 357, 370, 382 }, new[] { 150, 196, 196, 196 }, new[] { 100, 132 }, new[] { 180, 238, 212 }, new[] { 200, 297, 298, 303, 301, 315, 299, 304, 308, 311, 315, 315, 301, 310 }, new[] { 220, 307, 321, 334, 336 }, new[] { 210, 318, 315, 311, 313, 326 }, new[] { 190, 275, 268, 274, 259, 272 }, new[] { 300, 520, 366, 369, 382 }, new[] { 160, 260, 261, 269 }, new[] { 230, 299, 344, 349, 327, 344, 346, 347, 340, 333, 345, 328, 331, 344, 331, 335, 329, 334 }, new[] { 210, 289, 322, 326, 315, 319, 307, 318, 311 }, new[] { 250, 326, 496, 365, 367, 360, 353 }, new[] { 200, 303, 301, 302, 317, 310, 300, 301, 310 }, new[] { 120, 153 }, new[] { 240, 308, 357, 354, 339, 344, 355, 339, 344, 350, 345, 356, 356, 355, 359, 341, 354, 356 }, new[] { 190, 265, 301, 304, 304, 291, 298, 287, 306, 295, 301, 300 }, new[] { 50, 60 }, new[] { 200, 266, 305, 316, 316, 301 }, new[] { 150, 240, 247, 254, 258 }, new[] { 170, 271, 275, 280, 273, 271, 268, 271, 280 }, new[] { 140, 186 } };
        var w = new List<int[]> { new[] { 0, 5, 21 }, new[] { 1, 5, 21 }, new[] { 2, 5, 21 }, new[] { 3, 5, 21 }, new[] { 4, 5, 21 }, new[] { 22, 6, 23, 6, 24, 7, 21 }, new[] { 0, 6, 8, 7, 9, 6, 10, 6, 11, 6, 13, 6, 14, 25, 21 }, new[] { 0, 6, 8, 7, 9, 6, 10, 6, 12, 6, 13, 6, 14, 25, 21 }, new[] { 0, 6, 8, 7, 15, 6, 16, 6, 17, 18, 21 } };
        var l = new List<int[]> { new[] { 19, 7 }, new[] { 1, 7 }, new[] { 20, 7 } };
        var x = new[] { 0, 1, 2, 3, 4 };
        int[] p = { 1, 20, 6, 5, 2, 0, 8, 0, 9, 20, 7, 0, 7, 1, 7, 2, 10, (6 << 8) | 10, 3, 0, 5, 36, 4, 0, 5, 28, 11, 32, 6, 6, 11, 2, 6, 7, 11, 2, 6, 8, 99, 0 };
        q(new V(), p, s, w, l, x);
    }
    static void y(int[] a, List<int[]> b) { for (int i = 0; i < a.Length; i++) { var c = b[a[i]]; int d = c[0]; for (int j = 1; j < c.Length; j++) Console.Write((char)(c[j] - d)); } }
    static void q(V v, int[] p, List<int[]> s, List<int[]> w, List<int[]> l, int[] x)
    {
        while (v.c < p.Length)
        {
            int o = p[v.c++]; int a = p[v.c++]; switch (o)
            {
                case 1: v.a = v.r.Next(1, a + 1); break;
                case 2: string t = Console.ReadLine(); int u; v.b = int.TryParse(t, out u) ? u : int.MinValue; break;
                case 3: v.d = (v.b == v.a); break;
                case 4: v.d = (v.b < v.a); break;
                case 5: if (v.d) v.c = a; break;
                case 11: v.c = a; break;
                case 6: y(w[a], s); break;
                case 7: y(l[a], s); if (a == 0) Console.WriteLine(v.r.Next(10, 100)); else if (a == 1) Console.WriteLine(Math.Round(v.r.NextDouble(), 3)); else Console.WriteLine(Math.Round(v.r.NextDouble(), 2)); break;
                case 8: int m = x[v.r.Next(x.Length)]; y(w[m], s); break;
                case 9: for (int i = 0; i <= a; i++) { Console.Write("[" + new string('#', i) + new string(' ', a - i) + $"] {(i * 100 / a),3}%\r"); Thread.Sleep(24); } Console.WriteLine(); break;
                case 10: int n = (a >> 8) & 255; int o2 = a & 255; int b = v.r.Next(n, o2 + 1); for (int i = 0; i < b; i++) { int h = v.r.Next(5, 20); for (int j = 0; j < h; j++) Console.Write('|'); Console.Write('\n'); } break;
                case 99: return;
            }
        }
    }
}
