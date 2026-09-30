using Iced.Intel;
using IDR.Core.Services;
using System;
using System.Linq;

namespace IDR.Infrastructure.Disassembly;

public sealed class IcedInstructionDecoder : IInstructionDecoder
{
    public DecodedInstruction Decode(ReadOnlySpan<byte> bytes, ulong address, int bitness)
    {
        if (bytes.IsEmpty)
        {
            throw new ArgumentException("Instruction bytes are required.", nameof(bytes));
        }

        if (bitness is not (16 or 32 or 64))
        {
            throw new ArgumentOutOfRangeException(nameof(bitness), bitness, "Bitness must be 16, 32, or 64.");
        }

        ByteArrayCodeReader reader = new(bytes.ToArray());
        Decoder decoder = Decoder.Create(bitness, reader, DecoderOptions.None);
        decoder.IP = address;
        decoder.Decode(out Instruction instruction);
        if (instruction.IsInvalid)
        {
            throw new InstructionDecodeException(address);
        }

        ulong? nearBranchTarget = HasNearBranchOperand(instruction)
            ? instruction.NearBranchTarget
            : null;
        InstructionFlowControl flowControl = instruction.FlowControl switch
        {
            FlowControl.Call or FlowControl.IndirectCall => InstructionFlowControl.Call,
            FlowControl.ConditionalBranch => InstructionFlowControl.ConditionalBranch,
            FlowControl.UnconditionalBranch => nearBranchTarget.HasValue
                ? InstructionFlowControl.UnconditionalBranch
                : InstructionFlowControl.IndirectBranch,
            FlowControl.IndirectBranch => InstructionFlowControl.IndirectBranch,
            FlowControl.Return => InstructionFlowControl.Return,
            FlowControl.Interrupt or FlowControl.Exception or FlowControl.XbeginXabortXend =>
                InstructionFlowControl.Interrupt,
            _ => InstructionFlowControl.Next
        };
        IntelFormatter formatter = new();
        StringOutput formattedOutput = new();
        formatter.Format(instruction, formattedOutput);

        return new DecodedInstruction(
            instruction.IP,
            instruction.Length,
            instruction.Mnemonic.ToString(),
            nearBranchTarget,
            flowControl,
            formattedOutput.ToString())
        {
            Operands = DecodeOperands(instruction),
            HasRepeatPrefix = instruction.HasRepPrefix || instruction.HasRepnePrefix
        };
    }

    private static DecodedOperand[] DecodeOperands(Instruction instruction)
    {
        DecodedOperand[] operands = new DecodedOperand[instruction.OpCount];
        for (int index = 0; index < operands.Length; index++)
        {
            OpKind kind = instruction.GetOpKind(index);
            operands[index] = kind switch
            {
                OpKind.Register => new DecodedOperand(
                    DecodedOperandKind.Register,
                    Register: instruction.GetOpRegister(index).ToString()),
                OpKind.Memory => new DecodedOperand(
                    DecodedOperandKind.Memory,
                    SegmentRegister: instruction.MemorySegment.ToString(),
                    BaseRegister: instruction.MemoryBase == Register.None
                        ? null
                        : instruction.MemoryBase.ToString(),
                    IndexRegister: instruction.MemoryIndex == Register.None
                        ? null
                        : instruction.MemoryIndex.ToString(),
                    Displacement: instruction.MemoryDisplacement64,
                    MemorySizeBytes: instruction.MemorySize.GetSize()),
                OpKind.NearBranch16 or OpKind.NearBranch32 or OpKind.NearBranch64 =>
                    new DecodedOperand(
                        DecodedOperandKind.NearBranch,
                        Immediate: instruction.NearBranchTarget),
                OpKind.Immediate8
                    or OpKind.Immediate8_2nd
                    or OpKind.Immediate16
                    or OpKind.Immediate32
                    or OpKind.Immediate64
                    or OpKind.Immediate8to16
                    or OpKind.Immediate8to32
                    or OpKind.Immediate8to64
                    or OpKind.Immediate32to64 => new DecodedOperand(
                        DecodedOperandKind.Immediate,
                        Immediate: instruction.GetImmediate(index)),
                _ => new DecodedOperand(DecodedOperandKind.Other)
            };
        }

        return operands;
    }

    private static bool HasNearBranchOperand(Instruction instruction)
    {
        return instruction.Op0Kind is OpKind.NearBranch16
            or OpKind.NearBranch32
            or OpKind.NearBranch64
            || instruction.Op1Kind is OpKind.NearBranch16
            or OpKind.NearBranch32
            or OpKind.NearBranch64;
    }
}
