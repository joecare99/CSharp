using System;

namespace IDR.Core.Services;

public sealed class InstructionDecodeException : Exception
{
    public InstructionDecodeException(ulong address)
        : base($"Instruction decoding failed at address 0x{address:X8}.")
    {
        Address = address;
    }

    public InstructionDecodeException(ulong address, Exception innerException)
        : base($"Instruction decoding failed at address 0x{address:X8}.", innerException)
    {
        Address = address;
    }

    public ulong Address { get; }
}
