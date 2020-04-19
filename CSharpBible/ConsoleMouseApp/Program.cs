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
        private static ConsoleLib.CommonControls.Button One;
        private static Pixel Mouse = new Pixel();
        private static ConsoleLib.CommonControls.Application App;
        private static ConsoleLib.CommonControls.Label lblMousePos;

        static void Main(string[] args)
        {

            var cl = ConsoleFramework.Canvas.ClipRect;
            cl.Inflate(-3, -3);
            Console.ForegroundColor = ConsoleColor.White;

            App = new ConsoleLib.CommonControls.Application
            {
                visible = false,
                Boarder = ConsoleFramework.singleBoarder,
                ForeColor = ConsoleColor.Gray,
                BackColor = ConsoleColor.DarkGray,
                BoarderColor = ConsoleColor.Green,
                dimension = cl
            };

            // t.Draw(10, 40, ConsoleColor.Gray);
            Mouse.parent = App;
            Mouse.Set(0, 0, " ");
            Mouse.BackColor = ConsoleColor.Red;

            One = new ConsoleLib.CommonControls.Button
            {
                parent = App,
                ForeColor = ConsoleColor.White,
                BackColor = ConsoleColor.Gray,
                shaddow = true,
                position = new Point(5, 10),
                Text = "░░1░░"
            };
            One.OnClick += One_Click;

            cl = new Rectangle(3, 15, 30, 10);
            var Panel2 = new ConsoleLib.CommonControls.Panel
            {
                parent = App,
                Boarder = ConsoleFramework.doubleBoarder,
                ForeColor = ConsoleColor.Blue,
                BackColor = ConsoleColor.DarkBlue,
                BoarderColor = ConsoleColor.Green,
                dimension = cl,
                shaddow = true
            };

            var btnOK = new ConsoleLib.CommonControls.Button
            {
                parent = Panel2,
                ForeColor = ConsoleColor.White,
                BackColor = ConsoleColor.Gray,
                shaddow = true,
                position = new Point(2,2),
                Text = "░░░OK░░░",
            };
            btnOK.OnClick += btnOK_Click;

            var btnCancel = new ConsoleLib.CommonControls.Button
            {
                parent = Panel2,
                ForeColor = ConsoleColor.White,
                BackColor = ConsoleColor.Gray,
                shaddow = true,
                position = new Point(14, 2),
                Text = "░Cancel░",
            };
            btnCancel.OnClick += btnCancel_Click;

            lblMousePos = new ConsoleLib.CommonControls.Label
            {
                parent = App,
                ForeColor = ConsoleColor.Gray,
                ParentBackground = true,
                position = new Point(40, 2),
                Text = "lblMousePos",
                size = new Size(15, 1)
            };

            App.visible = true;
            App.Draw();
            App.OnMouseMove += App_MouseMove;
            App.OnCanvasResize += App_CanvasResize;

            App.Run();          

            Console.Write("Programm end ...");
            ExtendedConsole.Stop();
        }

        private static void App_CanvasResize(object sender, Point e)
        {
            var cl = ConsoleFramework.Canvas.ClipRect;
            cl.Inflate(-3, -3);
            App.dimension = cl;
        }

        private static void btnCancel_Click(object sender, EventArgs e)
        {
            App.Stop();
        }

        private static void App_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Set(Point.Subtract(e.Location, (Size)Mouse.parent.position));
            lblMousePos.Text = e.Location.ToString();
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

