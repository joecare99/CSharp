using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traingames.NetElements;
//using System.Windows.Forms;
using System.Drawing;

namespace ConsoleTools.NET
{
    class Program
    {
        static ConsoleFramework c = new ConsoleFramework();
        static public Point MousePos;
        static Button One = new Button();
        static Pixel Mouse = new Pixel();

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            // t.Draw(10, 40, ConsoleColor.Gray);
            One.Set(5, 10, "░░1░░", ConsoleColor.Gray);

            GUI.Add(One);
            GUI.CalculateOnStart();
            for (; ; )
            {
                MousePos = c.MousePos;
                if (One.Pressed(MousePos))
                {
                    Console.Write("1");
                }
                else
                {
                    
                }
                //   Console.Clear();
            }
        }
    }
}

