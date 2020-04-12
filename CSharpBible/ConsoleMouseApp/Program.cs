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

            App.visible = false;
            Console.ForegroundColor = ConsoleColor.White;
            App.Boarder = ConsoleFramework.singleBoarder;
            var cl = ConsoleFramework.Canvas.ClipRect;
            cl.Inflate(-3, -3); 
            App.ForeColor = ConsoleColor.Gray;
            App.BackColor = ConsoleColor.DarkGray;
            App.BoarderColor = ConsoleColor.Green;
            App.dimension = cl;

            // t.Draw(10, 40, ConsoleColor.Gray);
            App.Add(One);
            App.Add(Mouse);
            One.ForeColor = ConsoleColor.White;
            One.shaddow = true;
            One.Set(5, 10, "░░1░░", ConsoleColor.Gray);
            Mouse.Set(0,0," ");
            Mouse.BackColor = ConsoleColor.Red;

            var Panel2 = new Panel();
            App.Add(Panel2);
            Panel2.Boarder = ConsoleFramework.doubleBoarder;
            cl = new Rectangle(3,15,30,10);
            Panel2.ForeColor = ConsoleColor.Blue;
            Panel2.BackColor = ConsoleColor.DarkBlue;
            Panel2.BoarderColor = ConsoleColor.Green;
            Panel2.dimension = cl;
            Panel2.shaddow = true;

            var btnOK = new Button();
            Panel2.Add(btnOK);
            btnOK.ForeColor = ConsoleColor.White;
            btnOK.shaddow = true;
            btnOK.Set(2, 2, "░░░OK░░░", ConsoleColor.Gray);

            var btnCancel = new Button();
            Panel2.Add(btnCancel);
            btnCancel.ForeColor = ConsoleColor.White;
            btnCancel.shaddow = true;
            btnCancel.Set(14, 2, "░Cancel░", ConsoleColor.Gray);

            App.visible = true;
            App.Draw();
            Point _MousePos = ConsoleFramework.MousePos;

            for (; ; )
            {
                MousePos = ConsoleFramework.MousePos;
                if (One.Pressed(MousePos))
                {
                    Console.Write("1");
                }
                else if (btnCancel.Pressed(MousePos))
                {
                    break;
                }
                else if(btnOK.Pressed(MousePos))
                {
                    Console.Write("OK"); 
                }
                else if (_MousePos != MousePos)
                {
                    _MousePos = MousePos;
                    if (ConsoleFramework.Canvas.ClipRect.Contains(MousePos))
                    {
                        Mouse.Set(Point.Subtract(MousePos, (Size)Mouse.parent.position));
                    }

                }
                else
                    Thread.Sleep(1);
                //   Console.Clear();
            }

            Console.Write("Programm end ...");
            ExtendedConsole.Stop();
        }
    }
}

