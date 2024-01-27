using System;

namespace TitleGen
{
    public static class Program
    {
        const string cc = " ▀▄█";
        static void Main(string[] args)
        {
            var bm = Properties.Resource.SEW_Eurodrive;
            for (var y= 0;y<bm.Height;y+=2 )
            {
                var sLine = "";
                for (var x = 0; x < bm.Width; x++)
                   sLine+=cc[TBool2Int(bm.GetPixel(x, y).GetBrightness() < 0.9, 
                       bm.GetPixel(x, y+1).GetBrightness() < 0.9)];
                Console.Write(sLine.TrimEnd());
                if (sLine.TrimEnd().Length <= 79) Console.WriteLine();
            }

        }

        public static int TBool2Int(params bool[] b)
        {
            int result = 0;
            for (int i=0;i<b.Length;i++)
                result += b[i] ? 1<<i : 0;
            return result;
        }
    }
}