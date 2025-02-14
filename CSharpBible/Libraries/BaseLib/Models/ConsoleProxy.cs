using BaseLib.Interfaces;
using System;
using System.Reflection;

namespace BaseLib.Models;

public class ConsoleProxy : IConsole
{
    MethodInfo _writeLineS = typeof(Console).GetMethod(nameof(Console.WriteLine), [typeof(string)])!;
    MethodInfo _beep = typeof(Console).GetMethod(nameof(Console.Beep), [typeof(int), typeof(int)])!;
#if NET5_0_OR_GREATER
    MethodInfo _GetCursorPosition = typeof(Console).GetMethod(nameof(Console.GetCursorPosition), [])!;
#else
    MethodInfo _CursorLeft = typeof(Console).GetMethod(nameof(Console.CursorLeft), [])!;
    MethodInfo _CursorTop = typeof(Console).GetMethod(nameof(Console.CursorTop), [])!;
#endif
    MethodInfo _ReadKey = typeof(Console).GetMethod(nameof(Console.ReadKey), [])!;
    MethodInfo _Readline = typeof(Console).GetMethod(nameof(Console.ReadLine), [])!;
    MethodInfo _setCursorPosition = typeof(Console).GetMethod(nameof(Console.SetCursorPosition), [typeof(int), typeof(int)])!;
    MethodInfo _writeCh = typeof(Console).GetMethod(nameof(Console.Write), [typeof(char)])!;
    MethodInfo _writeS = typeof(Console).GetMethod(nameof(Console.Write), [typeof(string)])!;
    MethodInfo _clear = typeof(Console).GetMethod(nameof(Console.Clear), [])!;

    PropertyInfo _ForegroundColor = typeof(Console).GetProperty(nameof(Console.ForegroundColor))!;
    PropertyInfo _BackgroundColor = typeof(Console).GetProperty(nameof(Console.BackgroundColor))!;
    PropertyInfo _IsOutputRedirected = typeof(Console).GetProperty(nameof(Console.IsOutputRedirected))!;
    PropertyInfo _KeyAvailable = typeof(Console).GetProperty(nameof(Console.KeyAvailable))!;
    PropertyInfo _LargestWindowHeight = typeof(Console).GetProperty(nameof(Console.LargestWindowHeight))!;
    PropertyInfo _Title = typeof(Console).GetProperty(nameof(Console.Title))!;
    PropertyInfo _WindowHeight = typeof(Console).GetProperty(nameof(Console.WindowHeight))!;
    PropertyInfo _WindowWidth = typeof(Console).GetProperty(nameof(Console.WindowWidth))!;

    public ConsoleColor ForegroundColor { get => _ForegroundColor?.GetValue(null) as ConsoleColor? ?? ConsoleColor.White; set => _ForegroundColor?.SetValue(null,value); }
    public ConsoleColor BackgroundColor { get => _BackgroundColor?.GetValue(null) as ConsoleColor? ?? ConsoleColor.Black; set => _BackgroundColor?.SetValue(null, value); }
    public bool IsOutputRedirected => _IsOutputRedirected?.GetValue(null) as bool? ?? false;
    public bool KeyAvailable => _KeyAvailable?.GetValue(null) as bool? ?? false;
    public int LargestWindowHeight => _LargestWindowHeight?.GetValue(null) as int? ?? 0;
    public string Title { get => _Title?.GetValue(null) as string ?? string.Empty; set => _Title?.SetValue(null, value); }
    public int WindowHeight { get => _WindowHeight?.GetValue(null) as int? ?? 0; set => _WindowHeight?.SetValue(null, value); }
    public int WindowWidth { get => _WindowWidth?.GetValue(null) as int? ?? 0; set => _WindowWidth?.SetValue(null, value); }


    public void Beep(int freq, int len) => _beep?.Invoke(null, [freq, len]);
    public void Clear() => _clear?.Invoke(null, []);
#if NET5_0_OR_GREATER
    public (int Left, int Top) GetCursorPosition() => _GetCursorPosition?.Invoke(null, []) as (int, int)? ?? (0, 0);
#else
    public (int Left, int Top) GetCursorPosition() => (_CursorLeft?.Invoke(null, []) as int? ?? 0, _CursorTop?.Invoke(null, []) as int? ?? 0);
#endif
    public ConsoleKeyInfo? ReadKey() => _ReadKey?.Invoke(null, []) as ConsoleKeyInfo?;
    public string ReadLine() => _Readline?.Invoke(null, [])?.ToString() ?? string.Empty;
    public void SetCursorPosition(int left, int top) => _setCursorPosition?.Invoke(null, [left, top]);
    public void Write(char ch) => _writeCh?.Invoke(null, [ch]);
    public void Write(string? st) => _writeS?.Invoke(null, [st]);
    public void WriteLine(string? v) => _writeLineS?.Invoke(null, [v]);
}
