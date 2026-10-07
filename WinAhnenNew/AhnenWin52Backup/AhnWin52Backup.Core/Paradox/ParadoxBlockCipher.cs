using System;

namespace AhnWin52Backup.Core.Paradox;

// Ported from pxlib 0.6.8 (src/px_crypt.c); see the pxlib GPL-2.0 license.
internal static class ParadoxBlockCipher
{
    private static readonly byte[] EncryptionTableA = Convert.FromHexString(
        "B2A50CDD38FECB5B0C23EC6A953ED52D2CF72D30EA1598B45F82D3AFF44C841" +
        "674170511ACDB43919C77A038BED68F25B1EE6D803714A97A57386C2FA64F7C38" +
        "71D40B51F6B945211D6C4D876EA7E7210D85F4CE3A816A3ED732A423ACE90191" +
        "B0EDC74612AC153DFF1B7B3BBAEE2C2C68A66260ED5B843361623979D4D2EB60" +
        "B8251DDBFF3A219AB0C3F76352F52294F9B4B7BE9F543BCEE13C73CBEA2D45009" +
        "DC278E7960F143AA4DC01648DD6594E390F157DF08C8AC578034AE8FBEF18B452" +
        "3179D08EAAC3C6CEEDE9425AD62D06C79AB13862D97E6174D246DECB2B0C590B" +
        "649D1C4FB206919B63B5B2A9FDAD5A38F9136E9D2D4B02F9136F50CB2109FA");

    private static readonly byte[] EncryptionTableB = Convert.FromHexString(
        "61A7790237346F8101C2B2B3D64D3E030660984446681CEB104A5BAE22482442" +
        "9F5590C17D2F6C414E8256D81E3228C6EDBC3C3AE9873B8F8608A3FBA46299FF" +
        "59B9DE2D58931BB1762BAAD92AACCAF9E0B7051177A98EEFB5BB26EA8D189BFEC" +
        "7F85A83FD2E6B8433FA69D474BFCBC419963FE525F2A5D595F3A039DCE4A17F7" +
        "B497AF1EEF7750991679738D38947311F2C0ECE20CC9C2317920FF4136471C8CD" +
        "3DB4CFE15052AF6D27B69430048C534BD24F21296A1AEC5C7E519A0D85E61DC5" +
        "88A8DD9EF65FDABD6E9D54DB5EC0805DB840631512A20C07AD70147216D0A665" +
        "35BAE7ABFCC3C9BE0BB0F545E24C730A3678438B8AF066D100DF7CE3E857D7");

    private static readonly byte[] EncryptionTableC = Convert.FromHexString(
        "F908030FAD5210D83987F0E9D7BC929A1853D59CDBD4DD985D70B64616BF2C909" +
        "4B31C971E745AA92EB44C4991436525AC8F2D6805E1F1048B7B333632A10E72D" +
        "2271FF313EC148E1D8119B0EE0D28B2A5BAA6AFCB212AFE4440621AB8D0CDC611" +
        "003DD39DE0F789156AB56626A8C906DA9EDCEF6C864854F2028280FB24B97FCF0" +
        "7296367BD3835C18DF5F4AE5C563722889945AB78718A123EBB5E96CAC4207A7E" +
        "D63A3C76DF01C2554AE44DD91BA7EA470A0BA258AA51D141E25931C8E86DCE230" +
        "9E66FC02F609FE74E6E95A093A3DEF8B18C6B770CFF2B4BC7CC7CC342FA50A4ED" +
        "FC7D73BEE3FD345B17B7308457F68375799BEBC585614F693BE5645F3F");

    public static void DecryptDatabaseBlock(Span<byte> block, uint encryption, int blockNumber) =>
        DecryptBlock(block, encryption, 0, (byte)blockNumber, true);

    public static void EncryptDatabaseBlock(Span<byte> block, uint encryption, int blockNumber)
    {
        ValidateBlock(block);

        byte a = (byte)encryption;
        byte b = (byte)(encryption >> 8);
        Span<byte> encrypted = stackalloc byte[256];
        for (int chunkIndex = 0; chunkIndex < block.Length / 256; chunkIndex++)
        {
            int chunkOffset = chunkIndex * 256;
            Span<byte> source = block.Slice(chunkOffset, 256);
            for (int index = 0; index < 256; index++)
            {
                int tableIndex = (EncryptionTableC[index] - (byte)blockNumber) & 0xFF;
                encrypted[tableIndex] = (byte)(
                    source[index] ^
                    EncryptionTableA[(index + a) & 0xFF] ^
                    EncryptionTableB[(tableIndex + b) & 0xFF] ^
                    EncryptionTableC[(tableIndex + chunkIndex) & 0xFF]);
            }

            encrypted.CopyTo(source);
        }
    }

    public static void DecryptMemoBlock(Span<byte> block, uint encryption) =>
        DecryptBlock(
            block,
            encryption,
            (byte)((encryption & 0xFF) + 1),
            (byte)(((encryption >> 8) & 0xFF) + 1),
            false);

    public static void EncryptMemoBlock(Span<byte> block, uint encryption)
    {
        ValidateBlock(block);

        byte a = (byte)encryption;
        byte b = (byte)(encryption >> 8);
        byte c = unchecked((byte)(a + 1));
        byte d = unchecked((byte)(b + 1));
        Span<byte> encrypted = stackalloc byte[256];
        for (int chunkIndex = 0; chunkIndex < block.Length / 256; chunkIndex++)
        {
            Span<byte> source = block.Slice(chunkIndex * 256, 256);
            for (int index = 0; index < 256; index++)
            {
                int tableIndex = (EncryptionTableC[index] - d) & 0xFF;
                encrypted[tableIndex] = (byte)(
                    source[index] ^
                    EncryptionTableA[(index + a) & 0xFF] ^
                    EncryptionTableB[(tableIndex + b) & 0xFF] ^
                    EncryptionTableC[(tableIndex + c) & 0xFF]);
            }

            encrypted.CopyTo(source);
        }
    }

    private static void DecryptBlock(Span<byte> block, uint encryption, byte c, byte d, bool incrementChunk)
    {
        ValidateBlock(block);

        byte a = (byte)encryption;
        byte b = (byte)(encryption >> 8);
        Span<byte> temporary = stackalloc byte[256];

        for (int chunkIndex = 0; chunkIndex < block.Length / 256; chunkIndex++)
        {
            int chunkOffset = chunkIndex * 256;
            Span<byte> source = block.Slice(chunkOffset, 256);
            byte chunkKey = (byte)(c + (incrementChunk ? chunkIndex : 0));
            for (int index = 0; index < 256; index++)
            {
                int tableIndex = (EncryptionTableC[index] - d) & 0xFF;
                temporary[index] = (byte)(
                    source[tableIndex] ^
                    EncryptionTableA[(index + a) & 0xFF] ^
                    EncryptionTableB[(tableIndex + b) & 0xFF] ^
                    EncryptionTableC[(tableIndex + chunkKey) & 0xFF]);
            }

            temporary.CopyTo(source);
        }
    }

    private static void ValidateBlock(ReadOnlySpan<byte> block)
    {
        if (block.Length == 0 || block.Length % 256 != 0)
        {
            throw new ArgumentException("Paradox encrypted blocks must contain a non-empty multiple of 256 bytes.", nameof(block));
        }
    }
}
