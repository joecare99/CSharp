using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestConsole;
using System.Windows.Forms;
using System.Threading;

namespace ConsoleDisplay.View.Tests
{
    /// <summary>
    /// Defines test class DisplayTests.
    /// </summary>
    [TestClass()]
    public class DisplayTests
    {
        MyConsoleBase? console = null;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize()]
        public void Init()
        {
            console = new TstConsole();
            console.Clear();
            Display.myConsole = console;
            Application.DoEvents();
            var rnd = new Random();
            var newPara = true;
            for (int i = 0; i < 300; i++)
            {
                var word = "";
                for (int j = 0; j < rnd.Next(3, 14); j++)
                {
                    word += (j == 0) && (rnd.Next(5) == 0 || newPara) ? (char)rnd.Next(65, 91) : (char)rnd.Next(97, 123);
                }
                newPara = false;
                if (console.GetCursorPosition().Left + word.Length + 2 > console.WindowWidth)
                    console.WriteLine();
                console.Write(word);
                switch (rnd.Next(8))
                {
                    case 0: console.Write(". "); newPara = true; break;
                    case 1: console.Write(", "); break;
                    default:
                        console.Write(" "); break;
                }
                Thread.Sleep(0);
            }
        }

        /// <summary>
        /// Defines the test method DisplayTest.
        /// </summary>
        [TestMethod()]
        public void DisplayTest()
        {
            var display = new Display(2, 2, 20, 20);
            for (int i = 0; i < 600; i++) // 600 Frames;
            {
                for (int y = 0; y < 20; y++)
                    for (int x = 0; x < 20; x++)
                        display.PutPixel(x, y, (ConsoleColor)(i % 2==0?((x/2+i/2)%16):(y/2+i/2)%16));
                display.Update();
                Application.DoEvents();
                Thread.Sleep(0);
            }
                
        }


        /// <summary>
        /// Defines the test method DisplayPixelTest2.
        /// </summary>
        [TestMethod()]
        public void DisplayPixelTest2()
        {
            var display = new Display(5, 5, 20, 20);
            for (int i = 0; i < 600; i++) // 600 Frames;
            {
                for (int y = 0; y < 20; y++)
                    for (int x = 0; x < 20; x++)
                        display.PutPixel(x, y, (byte)(((i+x) % 8)*32), (byte)(((i+y) % 10)*25), (byte)(((x+y+i)%12)*20) );
                display.Update();
                Application.DoEvents();
                Thread.Sleep(0);
            }

        }

        /// <summary>
        /// Defines the test method DisplayLineTest.
        /// </summary>
        [TestMethod()]
        public void DisplayLineTest()
        {
            var display = new Display(55, 1, 20, 20);
            for (int i = 600; i > 400; i--) // 600 Frames;
            {
//                display.Clear();
                int x;int y;
                switch (i / 20 % 2)
                {
                    case 0: x = 0; y = i % 20; break;
                    case 1: y = 0; x = 19-i % 20; break;
                    default: x = 0;y = 0;
                        break;
                }
                //for (int x = 0; x < 20; x++)
                display.PutLine(x, y, 19 - x, 19 - y, (ConsoleColor)(i % 2 == 0 ? ((i / 2) % 16) : (8 + i / 2) % 16));
                display.Update();
                Application.DoEvents();
                Thread.Sleep(0);
            }
            for (int j = 400; j >0; j--) // 600 Frames;
            {
                display.Clear();
                int x; int y;
                for (int i = j*11/10 ; i >= j; i--)
                {
                    switch (i / 20 % 2)
                    {
                        case 0: x = 0; y = i % 20; break;
                        case 1: y = 0; x = 19 - i % 20; break;
                        default:
                            x = 0; y = 0;
                            break;
                    }
                    display.PutLine(x, y, 19 - x, 19 - y, (ConsoleColor)(i % 2 == 0 ? ((i / 2) % 16) : (8 + i / 2) % 16));
                }
                display.Update();
                Application.DoEvents();
                Thread.Sleep(0);
            }

        }
    }
}