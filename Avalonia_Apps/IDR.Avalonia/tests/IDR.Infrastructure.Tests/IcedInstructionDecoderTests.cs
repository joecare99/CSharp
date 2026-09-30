using IDR.Core.Services;
using IDR.Infrastructure.Disassembly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace IDR.Infrastructure.Tests;

[TestClass]
public sealed class IcedInstructionDecoderTests
{
    [TestMethod]
    public void DecodeReturnsInstructionLengthAndMnemonic()
    {
        IcedInstructionDecoder decoder = new();

        var instruction = decoder.Decode([0x90], 0x1000, 32);

        Assert.AreEqual(0x1000ul, instruction.Address);
        Assert.AreEqual(1, instruction.Length);
        Assert.AreEqual("Nop", instruction.Mnemonic);
        Assert.IsNull(instruction.NearBranchTarget);
    }

    [TestMethod]
    public void DecodeResolvesRelativeCallTarget()
    {
        IcedInstructionDecoder decoder = new();

        var instruction = decoder.Decode([0xE8, 0x05, 0, 0, 0], 0x1000, 32);

        Assert.AreEqual("Call", instruction.Mnemonic);
        Assert.AreEqual(0x100Aul, instruction.NearBranchTarget);
        Assert.AreEqual(InstructionFlowControl.Call, instruction.FlowControl);
        StringAssert.Contains(instruction.FormattedText, "call");
    }

    [TestMethod]
    public void DecodeClassifiesBranchesAndReturns()
    {
        IcedInstructionDecoder decoder = new();

        DecodedInstruction conditional = decoder.Decode([0x74, 0x02], 0x1000, 32);
        DecodedInstruction jump = decoder.Decode([0xEB, 0x02], 0x1000, 32);
        DecodedInstruction ret = decoder.Decode([0xC3], 0x1000, 32);

        Assert.AreEqual(InstructionFlowControl.ConditionalBranch, conditional.FlowControl);
        Assert.AreEqual(0x1004ul, conditional.NearBranchTarget);
        Assert.AreEqual(InstructionFlowControl.UnconditionalBranch, jump.FlowControl);
        Assert.AreEqual(InstructionFlowControl.Return, ret.FlowControl);
    }

    [TestMethod]
    public void DecodeReturnsTypedX86RegisterImmediateAndFsMemoryOperands()
    {
        IcedInstructionDecoder decoder = new();

        DecodedInstruction xor = decoder.Decode([0x33, 0xc0], 0x1000, 32);
        DecodedInstruction pushHandler = decoder.Decode(
            [0x68, 0x20, 0x10, 0x40, 0x00],
            0x1002,
            32);
        DecodedInstruction pushFs = decoder.Decode([0x64, 0xff, 0x30], 0x1007, 32);
        DecodedInstruction setFs = decoder.Decode([0x64, 0x89, 0x20], 0x100a, 32);

        Assert.AreEqual(DecodedOperandKind.Register, xor.Operands[0].Kind);
        Assert.AreEqual(xor.Operands[0].Register, xor.Operands[1].Register);
        Assert.AreEqual(DecodedOperandKind.Immediate, pushHandler.Operands[0].Kind);
        Assert.AreEqual(0x401020UL, pushHandler.Operands[0].Immediate);
        Assert.AreEqual(DecodedOperandKind.Memory, pushFs.Operands[0].Kind);
        Assert.AreEqual("FS", pushFs.Operands[0].SegmentRegister);
        Assert.AreEqual("EAX", pushFs.Operands[0].BaseRegister);
        Assert.AreEqual(4, pushFs.Operands[0].MemorySizeBytes);
        Assert.AreEqual("FS", setFs.Operands[0].SegmentRegister);
        Assert.AreEqual("ESP", setFs.Operands[1].Register);
    }

    [TestMethod]
    public void DecodeRecordsRepeatPrefixes()
    {
        IcedInstructionDecoder decoder = new();

        DecodedInstruction repeatedMove = decoder.Decode([0xf3, 0xa4], 0x1000, 32);
        DecodedInstruction repeatedScan = decoder.Decode([0xf2, 0xae], 0x1002, 32);
        DecodedInstruction plainMove = decoder.Decode([0xa4], 0x1004, 32);

        Assert.IsTrue(repeatedMove.HasRepeatPrefix);
        Assert.IsTrue(repeatedScan.HasRepeatPrefix);
        Assert.IsFalse(plainMove.HasRepeatPrefix);
    }

    [TestMethod]
    public void DecodeRejectsUnsupportedBitness()
    {
        IcedInstructionDecoder decoder = new();

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => decoder.Decode([0x90], 0x1000, 24));
    }

    [TestMethod]
    public void DecodeReportsInvalidInstructionAddress()
    {
        IcedInstructionDecoder decoder = new();

        InstructionDecodeException exception = Assert.ThrowsExactly<InstructionDecodeException>(
            () => decoder.Decode([0x0F], 0x1234, 32));

        Assert.AreEqual(0x1234ul, exception.Address);
    }
}
