// ***********************************************************************
// Assembly         : Basic_Del04_TestImposibleStuff_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="MainWindowTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using ConsoleDisplay.View;
using System;

namespace Basic_Del04_TestImposibleStuff.Views.Tests
{
    public class TestConsole : IConsole
    {
        private Action<string> _dolog;

        public ConsoleColor ForegroundColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ConsoleColor BackgroundColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsOutputRedirected => throw new NotImplementedException();

        public bool KeyAvailable => throw new NotImplementedException();

        public int LargestWindowHeight => throw new NotImplementedException();

        public string Title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int WindowHeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int WindowWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TestConsole(Action<String> doLog) {
            _dolog = doLog;
        }
        public void Beep(int freq, int len)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public (int Left, int Top) GetCursorPosition()
        {
            throw new NotImplementedException();
        }

        public ConsoleKeyInfo? ReadKey()
        {
            throw new NotImplementedException();
        }

        public string ReadLine()
        {
            throw new NotImplementedException();
        }

        public void SetCursorPosition(int left, int top)
        {
            throw new NotImplementedException();
        }

        public void Write(char ch)
        {
            throw new NotImplementedException();
        }

        public void Write(string? st)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string? st = "")
        {
            _dolog($"WriteLine({st})");
        }
    }
}
