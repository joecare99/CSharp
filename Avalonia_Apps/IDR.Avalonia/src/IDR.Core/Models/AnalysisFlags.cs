using System;

namespace IDR.Core.Models;

[Flags]
public enum AnalysisFlags : uint
{
    None = 0,
    Code = 1u << 0,
    Data = 1u << 1,
    Import = 1u << 2,
    FinallyExit = 1u << 9,
    Call = 1u << 3,
    ProcedureStart = 1u << 4,
    ProcedureEnd = 1u << 5,
    Rtti = 1u << 6,
    String = 1u << 7,
    Vmt = 1u << 8,
    ExceptionTable = 1u << 12,
    ExceptionHandler = 1u << 13,
    FinallyHandler = 1u << 14,
    ExceptionRegion = 1u << 15,
    FinallyRegion = 1u << 16,
    Int64Comparison = 1u << 17,
    Switch = 1u << 18,
    SwitchTable = 1u << 19,
    Loop = 1u << 20,
    Export = 1u << 21,
    FrameInstruction = 1u << 22,
    StackPush = 1u << 23,
    StackPop = 1u << 24,
    StackAdjustment = 1u << 25,
    ConstructorCandidate = 1u << 26,
    DestructorCandidate = 1u << 27,
    StackArgumentSizeMismatch = 1u << 28,
    FunctionCandidate = 1u << 29,
    DynamicArrayCandidate = 1u << 30,
    Instruction = 1u << 31
}

public enum DelphiTypeKind : byte
{
    Integer = 1,
    Char = 2,
    Enumeration = 3,
    Float = 4,
    String = 5,
    Set = 6,
    Class = 7,
    Method = 8,
    WideChar = 9,
    AnsiString = 10,
    WideString = 11,
    Variant = 12,
    Array = 13,
    Record = 14,
    Interface = 15,
    Int64 = 16,
    DynamicArray = 17,
    UnicodeString = 18,
    ClassReference = 19,
    Pointer = 20,
    Procedure = 21
}
