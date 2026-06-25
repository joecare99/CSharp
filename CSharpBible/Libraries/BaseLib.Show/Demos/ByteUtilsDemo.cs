using BaseLib.Helper;

namespace BaseLib.Show.Demos;

/// <summary>
/// Demonstrates bit helper extensions from BaseLib.
/// </summary>
internal sealed class ByteUtilsDemo : ShowcaseDemoBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ByteUtilsDemo"/> class.
    /// </summary>
    /// <param name="showcaseConsole">The showcase console renderer.</param>
    /// <param name="text">The localized text provider.</param>
    public ByteUtilsDemo(ShowcaseConsole showcaseConsole, ShowcaseText text)
        : base(showcaseConsole, text)
    {
    }

    /// <inheritdoc/>
    public override char SelectionKey => '3';

    /// <inheritdoc/>
    public override string TitleKey => "Demo_Byte_Title";

    /// <inheritdoc/>
    public override string MenuDescriptionKey => "Demo_Byte_Description";

    /// <inheritdoc/>
    protected override IEnumerable<DemoExample> CreateExamples()
    {
        yield return Example(
            "Example_Byte_BitMask",
            """
            for (int i = 0; i < 8; i++)
            {
                int mask = i.BitMask32();
            }
            """,
            () =>
            {
                for (int i = 0; i < 8; i++)
                {
                    int mask = i.BitMask32();
                    ShowcaseConsole.PrintResult(Text.Format("Byte_BitNumber", i), $"0x{mask:X8} (bin: {Convert.ToString(mask, 2).PadLeft(8, '0')})");
                }
            });

        yield return Example(
            "Example_Byte_SetClearGetSwitch",
            """
            int bitArray = 0;
            bitArray = bitArray.SetBit(0);
            bitArray = bitArray.SetBit(3);
            bitArray = bitArray.SetBit(7);
            bool thirdBit = bitArray.GetBit(3);
            bitArray = bitArray.ClearBit(3);
            bitArray = bitArray.SwitchBit(0);
            """,
            () =>
            {
                int bitArray = 0;
                ShowcaseConsole.PrintResult(Text.Get("Common_Start"), $"0x{bitArray:X8}");
                bitArray = bitArray.SetBit(0);
                ShowcaseConsole.PrintResult("SetBit(0)", $"0x{bitArray:X8}");
                bitArray = bitArray.SetBit(3);
                ShowcaseConsole.PrintResult("SetBit(3)", $"0x{bitArray:X8}");
                bitArray = bitArray.SetBit(7);
                ShowcaseConsole.PrintResult("SetBit(7)", $"0x{bitArray:X8}");
                ShowcaseConsole.PrintResult("GetBit(3)", bitArray.GetBit(3));
                ShowcaseConsole.PrintResult("GetBit(4)", bitArray.GetBit(4));
                bitArray = bitArray.ClearBit(3);
                ShowcaseConsole.PrintResult("ClearBit(3)", $"0x{bitArray:X8}");
                bitArray = bitArray.SwitchBit(0);
                ShowcaseConsole.PrintResult("SwitchBit(0)", $"0x{bitArray:X8}");
                bitArray = bitArray.SwitchBit(0);
                ShowcaseConsole.PrintResult("SwitchBit(0)", $"0x{bitArray:X8}");
            });

        yield return Example(
            "Example_Byte_BitCount",
            """
            long[] testValues = [0b1010101, 0xFF, 0xFFFF, 0x12345678];
            foreach (long value in testValues)
            {
                int count = value.BitCount();
            }
            """,
            () =>
            {
                long[] testValues = [0b1010101, 0xFF, 0xFFFF, 0x12345678];
                foreach (long value in testValues)
                {
                    ShowcaseConsole.PrintResult($"0x{value:X}", Text.Format("Byte_BitCount_Value", value.BitCount()));
                }
            });
    }
}
