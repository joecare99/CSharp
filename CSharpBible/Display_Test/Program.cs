using ConsoleDisplay.View;

namespace Display_Test
{
    /// <summary>
    /// Class Program.
    /// </summary>
    public class Program
    {
        static Display display1 = new Display(1, 1, 20, 20);
        static Display display2 = new Display(40, 5, 10, 10);
        static Display display3 = new Display(10, 23, 50, 2);

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static public void Main(string[] args)
        {
            var rnd = new Random();
            Test1(rnd);
            Test2();
            Test3();
        }

        private static void Test3()
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < display1.ScreenBuffer.Length; j++)
                    display1.ScreenBuffer[j] = (ConsoleColor)(Math.Round(Math.Sin((j % 20 - 10) * (0.05 + Math.Sin(i * 0.6) * 0.04) + i * 0.05) * 16 + Math.Cos((j / 20 - 10) * (0.05 + Math.Sin(i * 0.55) * 0.04) + i * 0.05) * 16 + 32) % 16);

                for (int j = 0; j < display2.ScreenBuffer.Length; j++)
                {
                    int r = (int)Math.Round(1.5 + Math.Sin((j % 10 - i * 0.4) * -0.6) * 1.5);
                    int g = (int)Math.Round(1.5 + Math.Sin((j / 10 + (j % 10) / 2 + i * 0.4) * 0.6) * 1.5);
                    int b = (int)Math.Round(1.5 + Math.Sin((j / 10 - (j % 10) / 2 - i * 0.4) * 0.6) * 1.5);
                    display2.PutPixel((j / 10), (j % 10), (byte)(r * 64), (byte)(g * 64), (byte)(b * 64));
                }

                display3.ScreenBuffer[i / 2 + (((i + 1) / 2) % 2) * 50] = ConsoleColor.Green;

                display1.Update();
                display2.Update();
                display3.Update();
                Thread.Sleep(40);
            }
        }

        private static void Test2()
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < display1.ScreenBuffer.Length; j++)
                    display1.PutPixel((j / 20), (j % 20), (ConsoleColor)((i + j) % 16));

                for (int j = 0; j < display2.ScreenBuffer.Length; j++)
                    display2.ScreenBuffer[j] = (ConsoleColor)(Math.Round(Math.Sin((j % 10 - 5) * (0.07 + Math.Sin(i * 0.5) * 0.05) + i * 0.05) * 16 + Math.Cos((j / 10 - 5) * (0.07 + Math.Sin(i * 0.5) * 0.05) + i * 0.05) * 16 + 32) % 16);

                display3.ScreenBuffer[i / 2 + ((i / 2) % 2) * 50] = ConsoleColor.Red;

                display1.Update();
                display2.Update();
                display3.Update();
                Thread.Sleep(40);
            }
        }

        private static void Test1(Random rnd)
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < display1.ScreenBuffer.Length; j++)
                    display1.ScreenBuffer[j] = (ConsoleColor)((i + j) % 16);

                for (int j = 0; j < 5; j++)
                    display2.ScreenBuffer[rnd.Next(display2.ScreenBuffer.Length)] = (ConsoleColor)((i / 10) % 16);

                display3.ScreenBuffer[i / 2 + (i % 2) * 50] = ConsoleColor.Yellow;

                display1.Update();
                display2.Update();
                display3.Update();
                Thread.Sleep(40);
            }
        }
    }
}