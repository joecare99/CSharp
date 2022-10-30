using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestConsole;

namespace ConsoleDisplay.View.Tests
{
    /// <summary>
    /// Defines test class DisplayTests.
    /// </summary>
    [TestClass()]
    public class DisplayTests
    {
        const string cExpTsTest= "ConsoleDisplay.View.Display.(7;9),0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDE";
        const string cExpLineTest1= "ConsoleDisplay.View.Display.(20;20),7F6E5D4C3B2A1908F7ED446E5D4C3B2A1908F7D5DD46ED4C3B2A198F7D5C55D465DC3B2A90F7D5C4EE5D4ED43B21987D5C4B66E5D4543B210FD5C4B3FFF6E54D3B29FDC4B33A777FF654CBA8DCB33AA2000077FE4B1D43AA222988888880744708888881111111111114EF77000899999992AE17456FF7702222AA34E8AE745E6FF7AAA33BCEF92D674D5E6F333B4CEF01245674D5E6BB4C5E789124DEF74D5E44C5E7F09A2CD56F74D5CC5E7F891A2C4DE6F74D55E7F8091A2C4D5E6F74DE7F8091A2B3C4D5E6F7";
        const string cExpLineTest2= "ConsoleDisplay.View.Display.(20;20),0000000000000000000088000000000000000000008000000000000000000008000000000000000000008000000000000000000008000000000000000000008000000000000000000008000000000000000000008000000000000000000008800000000000000000000800000000000000000000800000000000000000000800000000000000000000800000000000000000000800000000000000000000800000000000000000000800000000000000000000800000000000000000000800000000000000000000";
        const string cExpDispTest598= "ConsoleDisplay.View.Display.(20;20),BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344BBCCDDEEFF0011223344";
        const string cExpDispTest599= "ConsoleDisplay.View.Display.(20;20),BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000000000000000000000000000000000001111111111111111111111111111111111111111222222222222222222222222222222222222222233333333333333333333333333333333333333334444444444444444444444444444444444444444";
        const string cExpPixTest598= "ConsoleDisplay.View.Display.(20;20),FFAAAAEEEFBBBBEEEEAAFEAAAAEEFFBBBAEEEEAACC00555DDD9944CCCC19CC0155DDDD9044C5CC99CC1155DDDD004455CD99CC33777DDC226777DD39CC3377DDCC227777DD99CC3379D6CC237777DD92EFBBBBEEEE3377FFFFAAFFBBB6EEEE3377FFFEAAFFBBAAEEEEABBBFFEEAAFFBAAAEEEEBBBBFEEEAADD004455CD9999CCCC11DC004555DD9994CCCC11CC00555DDD9944CCCC19CC237777DD926667CC33CC337777DD226677CD33CC33777DDC226777DD39EE3B77FFEEAA777FFFBBEEBB7BFEEEA377FFFFBA";
        const string cExpPixTest599= "ConsoleDisplay.View.Display.(20;20),FAAAAEEEFBBBBEEEEAAAC004555DD9994CCCC115C00555DDD9944CCCC195C0155DDDD9044C5CC995C337777DD226677CD339C33777DDC226777DD399C3377DDCC227777DD996EBB7BFEEEA377FFFFBA6FBBBBEEEE3377FFFFAA6FBBBAEEEEAABBFFFEAAAFBBAAEEEEABBBFFEEAAAD9044C5CC9959DCCC015D004455CD9999CCCC115C004555DD9994CCCC115C227777DD996666CC337C237777DD926667CC337C337777DD226677CD339E3377FFFEAA6777FFBBBE3B77FFEEAA777FFFBB6EBBBBFEEEAAAAFFFFBAA";
        static MyConsoleBase? console = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayTests"/> class.
        /// </summary>
        public DisplayTests()
        {
            if (console == null)
            {
                console = new TstConsole();
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
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize()]
        public void Init()
        {
            Display.myConsole = console ?? Display.myConsole;
            Application.DoEvents();
        }

        /// <summary>
        /// Dones this instance.
        /// </summary>
        [TestCleanup()]
        public void Done()
        {
            Application.DoEvents();
            Thread.Sleep(500);
        }

        /// <summary>
        /// Defines the test method DisplayTest.
        /// </summary>
        [TestMethod()]
        public void DisplayTest()
        {
            var display = new Display(2, 2, 20, 20);
            for (int i = 0; i < 120; i++) // 600 Frames;
            {
                for (int y = 0; y < 20; y++)
                    for (int x = 0; x < 20; x++)
                        display.PutPixel(x, y, (ConsoleColor)(i % 2 == 0 ? ((x / 2 + i / 2) % 16) : (y / 2 + i / 2) % 16));
                display.Update();
                Application.DoEvents();
                Thread.Sleep(0);
                if (i == 118)
                    Assert.AreEqual(cExpDispTest598, display.ToString());
            }
            Assert.AreEqual(cExpDispTest599, display.ToString());


        }


        /// <summary>
        /// Defines the test method DisplayPixelTest2.
        /// </summary>
        [TestMethod()]
        public void DisplayPixelTest2()
        {
            var display = new Display(5, 12, 20, 20);
            for (int i = 0; i < 120; i++) // 600 Frames;
            {
                for (int y = 0; y < 20; y++)
                    for (int x = 0; x < 20; x++)
                        display.PutPixel(x, y, (byte)(((i + x) % 8) * 32), (byte)(((i + y) % 10) * 25), (byte)(((x + y + i) % 12) * 20));
                display.Update();
                Application.DoEvents();
                Thread.Sleep(0);
                if (i == 598)
                    Assert.AreEqual(cExpPixTest598, display.ToString());
            }
            Assert.AreEqual(cExpPixTest599, display.ToString());


        }

        /// <summary>
        /// Defines the test method DisplayLineTest.
        /// </summary>
        [TestMethod()]
        public void DisplayLineTest()
        {
            var display = new Display(55, 1, 20, 20);
            for (int i = 160; i > 120; i--) // 30 Frames;
            {
                //                display.Clear();
                DisplayLine(display, i);
                display.Update();
                Application.DoEvents();
                Thread.Sleep(0);
            }
            Assert.AreEqual(cExpLineTest1, display.ToString());
            for (int j = 120; j > 0; j--) // 600 Frames;
            {
                display.Clear();
                for (int i = j * 4 / 3; i >= j; i--)
                {
                    DisplayLine(display, i);
                }
                display.Update();
                Application.DoEvents();
                Thread.Sleep(0);
            }
            Assert.AreEqual(cExpLineTest2, display.ToString());

            static void DisplayLine(Display display, int i)
            {
                int x; int y;
                switch (i / 20 % 2)
                {
                    case 0: x = 0; y = i % 20; break;
                    case 1: y = 0; x = 19 - i % 20; break;
                    default:
                        x = 0; y = 0;
                        break;
                }
                //for (int x = 0; x < 20; x++)
                display.PutLine(x, y, 19 - x, 19 - y, (ConsoleColor)(i % 2 == 0 ? ((i / 2) % 16) : (8 + i / 2) % 16));
            }
        }
        /// <summary>
        /// Defines the test method ToStringTest.
        /// </summary>
        [TestMethod()]
        public void ToStringTest()
        {
            var display = new Display(42, 1, 7, 9);
            for (var i = 0; i < 63; i++)
                display.PutPixel(i % 7, i / 7, (ConsoleColor)(i % 16));
            display.Update();
            Assert.AreEqual(cExpTsTest,display.ToString());
        }
    }
}