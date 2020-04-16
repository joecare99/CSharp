using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
//using System.Windows.Forms;
using System.Drawing;
using ConsoleLib.CommonControls;
using ConsoleLib;
using System.Windows.Forms;

namespace ConsoleTools.NET
{
    class Program
    {        
        static public Point MousePos;
        static ConsoleLib.CommonControls.Button One = new ConsoleLib.CommonControls.Button();
        static Pixel Mouse = new Pixel();
        static ConsoleLib.CommonControls.Application App = new ConsoleLib.CommonControls.Application();

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
            App.Add(Mouse);

            One.parent = App;
            One.ForeColor = ConsoleColor.White;
            One.shaddow = true;
            One.Set(5, 10, "░░1░░", ConsoleColor.Gray);
            One.OnClick += One_Click;

            Mouse.Set(0,0," ");
            Mouse.BackColor = ConsoleColor.Red;

            var Panel2 = new ConsoleLib.CommonControls.Panel();
            Panel2.parent = App;
            Panel2.Boarder = ConsoleFramework.doubleBoarder;
            cl = new Rectangle(3,15,30,10);
             
            Panel2.ForeColor = ConsoleColor.Blue;
            Panel2.BackColor = ConsoleColor.DarkBlue;
            Panel2.BoarderColor = ConsoleColor.Green;
            Panel2.dimension = cl;
            Panel2.shaddow = true;

            var btnOK = new ConsoleLib.CommonControls.Button();
            btnOK.parent = Panel2;
            btnOK.ForeColor = ConsoleColor.White;
            btnOK.shaddow = true;
            btnOK.OnClick += btnOK_Click;
            btnOK.Set(2, 2, "░░░OK░░░", ConsoleColor.Gray);

            var btnCancel = new ConsoleLib.CommonControls.Button();
            btnCancel.parent = Panel2;
            btnCancel.ForeColor = ConsoleColor.White;
            btnCancel.shaddow = true;
            btnCancel.OnClick += btnCancel_Click;
            btnCancel.Set(14, 2, "░Cancel░", ConsoleColor.Gray);

            App.visible = true;
            App.Draw();
            Point _MousePos = ConsoleFramework.MousePos;
            App.OnMouseMove += App_MouseMove;
            App.Run();          

            Console.Write("Programm end ...");
            ExtendedConsole.Stop();
        }

        private static void btnCancel_Click(object sender, EventArgs e)
        {
            App.Stop();
        }

        private static void App_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Set(Point.Subtract(e.Location, (Size)Mouse.parent.position));
        }

        private static void btnOK_Click(object sender, EventArgs e)
        {
            Console.Write("OK");
        }

        private static void One_Click(object sender, EventArgs e)
        {
            Console.Write("1");
        }
    }
}

