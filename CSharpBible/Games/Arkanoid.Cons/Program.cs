using Arkanoid.Base;
using BaseLib.Interfaces;
using BaseLib.Models;
using ConsoleDisplay.View;
using ConsoleLib;
using ConsoleLib.Interfaces;
using System;
using System.Threading;

namespace Arkanoid.Cons
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IConsole console = new ConsoleProxy(); 
            console.CursorVisible = false;
            var engine = new GameEngine();

            var displayWidth = (int)engine.State.FieldWidth;
            var displayHeight = (int)engine.State.FieldHeight;
            var display = new Display(0, 0, displayWidth, displayHeight);

            // ExtendedConsole initialisieren, um echte Key-/Maus-Events zu bekommen
            var extConsole = new ExtendedConsole();

            bool leftDown = false;
            bool rightDown = false;
            bool useMouse = true;

            float paddleVelocity = 0f;
            const float paddleAcceleration = 60f;
            const float paddleFriction = 40f;

            // Key-Events (KeyDown/KeyUp)
            extConsole.KeyEvent += (s, e) =>
            {
                // Virtuelle Keycodes der Pfeiltasten
                const ushort VK_LEFT = 0x25;
                const ushort VK_RIGHT = 0x27;

                if (e.usKeyCode == VK_LEFT)
                {
                    leftDown = e.bKeyDown;
                    if (e.bKeyDown) rightDown = false;
                    e.Handled = true;
                }
                else if (e.usKeyCode == VK_RIGHT)
                {
                    rightDown = e.bKeyDown;
                    if (e.bKeyDown) leftDown = false;
                    e.Handled = true;
                }
                else if (e.usKeyCode == (ushort)ConsoleKey.Escape && e.bKeyDown)
                {
                    // ESC zum Beenden
                    extConsole.Stop();
                    engine.State.IsGameOver = true;
                    e.Handled = true;
                }
            };

            // Maussteuerung: Paddle per Maus-X-Position bewegen
            extConsole.MouseEvent += (s, me) =>
            {
                if (!useMouse) return;

                if (me.MouseMoved || me.ButtonEvent)
                {
                    var mouseX = me.MousePos.X;
                    // Paddle-Mittelpunkt an Maus-X ausrichten
                    var newX = mouseX - engine.State.Paddle.Width / 2f;
                    newX = Math.Clamp(newX, 0, engine.State.FieldWidth - engine.State.Paddle.Width);
                    engine.State.Paddle.Position = new Arkanoid.Base.Models.Vector2(newX, engine.State.Paddle.Position.Y);
                    me.Handled = true;
                }
            };

            var lastTime = DateTime.UtcNow;

            while (!engine.State.IsGameOver)
            {
                var now = DateTime.UtcNow;
                var delta = (float)(now - lastTime).TotalSeconds;
                if (delta <= 0f) delta = 0.001f;
                lastTime = now;

                // Tastatursteuerung (wenn Maus nicht benutzt wird)
                if (!useMouse)
                {
                    float moveDir = 0f;
                    if (leftDown && !rightDown) moveDir = -1f;
                    else if (rightDown && !leftDown) moveDir = 1f;
                    else
                    {
                        // Reibung, wenn keine Richtung gehalten wird
                        if (paddleVelocity > 0)
                        {
                            paddleVelocity -= paddleFriction * delta;
                            if (paddleVelocity < 0) paddleVelocity = 0;
                        }
                        else if (paddleVelocity < 0)
                        {
                            paddleVelocity += paddleFriction * delta;
                            if (paddleVelocity > 0) paddleVelocity = 0;
                        }
                    }

                    if (moveDir != 0f)
                    {
                        paddleVelocity += moveDir * paddleAcceleration * delta;
                    }

                    if (paddleVelocity != 0f)
                    {
                        var direction = Math.Sign(paddleVelocity);
                        engine.MovePaddle(direction, delta);
                    }
                }

                engine.Update(delta);

                // render
                display.Clear();

                // paddle
                for (int x = 0; x < (int)engine.State.Paddle.Width; x++)
                {
                    display.PutPixel((int)(engine.State.Paddle.Position.X + x), (int)engine.State.Paddle.Position.Y, ConsoleColor.Green);
                }

                // ball
                display.PutPixel((int)engine.State.Ball.Position.X, (int)engine.State.Ball.Position.Y, ConsoleColor.Yellow);

                // bricks
                foreach (var brick in engine.State.Bricks)
                {
                    if (brick.IsDestroyed) continue;
                    for (int bx = 0; bx < (int)brick.Width; bx++)
                    {
                        display.PutPixel((int)(brick.Position.X + bx), (int)brick.Position.Y, ConsoleColor.Red);
                    }
                }

                display.Update();

                Thread.Sleep(10);
            }

            console.ResetColor();
            console.CursorVisible = true;
        }
    }
}
