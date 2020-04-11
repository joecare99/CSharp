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
        static public Point MousePos;
        static Button One = new Button();
        static Pixel Mouse = new Pixel();
//        static Application App = new Application();

        static void Main(string[] args)
        {

            Console.ForegroundColor = ConsoleColor.White;
            // t.Draw(10, 40, ConsoleColor.Gray);
            One.Set(5, 10, "░░1░░", ConsoleColor.Gray);
            Mouse.Set(0,0," ");
            Mouse.BackColor = ConsoleColor.Red;

            //  App.Add(One);
            Point _MousePos = ConsoleFramework.MousePos;

            for (; ; )
            {
                MousePos = ConsoleFramework.MousePos;
                if (One.Pressed(MousePos))
                {
                    Console.Write("1");
                }
                else if (_MousePos != MousePos)
                {
                    _MousePos = MousePos;
                    if (ConsoleFramework.Canvas.ClipRect.Contains(MousePos))
                    {
                        Mouse.Set(MousePos);
                    }

                }
                //   Console.Clear();
            }
        }
    }
}

