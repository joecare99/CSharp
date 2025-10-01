#if NET462 || NET472 || NET48 || NET481
// Compatibility definitions for C# 8 range/index on older .NET Framework targets
// Minimal implementation just to satisfy compiler when language version uses these types.
using System;
namespace System
{
    public readonly struct Index
    {
        private readonly int _value; // sign bit = from end
        public Index(int value, bool fromEnd = false)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
            _value = fromEnd ? value | int.MinValue : value;
        }
        private Index(int value) => _value = value;
        public static Index Start => new Index(0);
        public static Index End => new Index(int.MinValue);
        public static Index FromStart(int value) => new Index(value);
        public static Index FromEnd(int value) => new Index(value, true);
        public int GetOffset(int length)
        {
            int val = _value & 0x7FFFFFFF;
            return (_value & int.MinValue) != 0 ? length - val : val;
        }
        public bool IsFromEnd => (_value & int.MinValue) != 0;
        public int Value => _value & 0x7FFFFFFF;
        public override string ToString() => IsFromEnd ? ^Value + "" : Value.ToString();
    }

    public readonly struct Range
    {
        public Index Start { get; }
        public Index End { get; }
        public Range(Index start, Index end)
        { Start = start; End = end; }
        public static Range StartAt(Index start) => new Range(start, Index.End);
        public static Range EndAt(Index end) => new Range(Index.Start, end);
        public static Range All => new Range(Index.Start, Index.End);
        public (int Offset, int Length) GetOffsetAndLength(int length)
        {
            int start = Start.GetOffset(length);
            int end = End.GetOffset(length);
            return (start, end - start);
        }
        public override string ToString() => $"{Start}..{End}";
    }
}
#endif
