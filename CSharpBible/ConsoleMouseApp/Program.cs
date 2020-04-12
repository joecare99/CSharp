using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
//using System.Windows.Forms;
using System.Drawing;
using ConsoleLib.CommonControls;
using ConsoleLib;

namespace ConsoleTools.NET
{
    class Program
    {        
        static public Point MousePos;
        static Button One = new Button();
        static Pixel Mouse = new Pixel();
        static Panel App = new Panel();

        static void Main(string[] args)
        {

            Console.ForegroundColor = ConsoleColor.White;
            App.Boarder = ConsoleFramework.singleBoarder;
            var cl = ConsoleFramework.Canvas.ClipRect;
            cl.Inflate(-3, -3); 
            App.dimension = cl ;

            // t.Draw(10, 40, ConsoleColor.Gray);
            App.Add(One);
            App.Add(Mouse);
            One.Set(5, 10, "░░1░░", ConsoleColor.Gray);
            One.shaddow = true;
            Mouse.Set(0,0," ");
            Mouse.BackColor = ConsoleColor.Red;

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
                else
                    Thread.Sleep(1);
                //   Console.Clear();
            }
        }
    }
}

