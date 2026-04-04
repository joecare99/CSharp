using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace GenSecure.Core;

internal static class CryptoUtilities
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
    };

    public static JsonSerializerOptions JsonSerializerOptions => _jsonSerializerOptions;

    public static byte[] Encrypt(byte[] arrKey, byte[] arrPlaintext, out byte[] arrNonce, out byte[] arrTag)
    {
        ArgumentNullException.ThrowIfNull(arrKey);
        ArgumentNullException.ThrowIfNull(arrPlaintext);

        arrNonce = RandomNumberGenerator.GetBytes(NonceSize);
        arrTag = new byte[TagSize];
        byte[] arrCiphertext = new byte[arrPlaintext.Length];

        using var aesGcm = new AesGcm(arrKey, TagSize);
        aesGcm.Encrypt(arrNonce, arrPlaintext, arrCiphertext, arrTag);

        return arrCiphertext;
    }

    public static byte[] Decrypt(byte[] arrKey, byte[] arrNonce, byte[] arrCiphertext, byte[] arrTag)
    {
        ArgumentNullException.ThrowIfNull(arrKey);
        ArgumentNullException.ThrowIfNull(arrNonce);
        ArgumentNullException.ThrowIfNull(arrCiphertext);
        ArgumentNullException.ThrowIfNull(arrTag);

        byte[] arrPlaintext = new byte[arrCiphertext.Length];

        using var aesGcm = new AesGcm(arrKey, TagSize);
        aesGcm.Decrypt(arrNonce, arrCiphertext, arrTag, arrPlaintext);

        return arrPlaintext;
    }

    public static string ToBase64(byte[] arrValue)
    {
        ArgumentNullException.ThrowIfNull(arrValue);

        return Convert.ToBase64String(arrValue);
    }

    public static byte[] FromBase64(string sValue)
    {
        ArgumentNullException.ThrowIfNull(sValue);

        return Convert.FromBase64String(sValue);
    }

    public static string ToDeterministicFileId(string sPersonId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sPersonId);

        byte[] arrHash = SHA256.HashData(Encoding.UTF8.GetBytes(sPersonId));
        return Convert.ToHexString(arrHash).ToLowerInvariant();
    }

    public static T ReadJson<T>(string sFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sFilePath);

        string sJson = File.ReadAllText(sFilePath, Encoding.UTF8);
        T? value = JsonSerializer.Deserialize<T>(sJson, JsonSerializerOptions);
        if (value is null)
        {
            throw new InvalidDataException($"The JSON file '{sFilePath}' could not be deserialized.");
        }

        return value;
    }

    public static void WriteJson<T>(string sFilePath, T value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sFilePath);
        ArgumentNullException.ThrowIfNull(value);

        string? sDirectory = Path.GetDirectoryName(sFilePath);
        if (!string.IsNullOrWhiteSpace(sDirectory))
        {
            Directory.CreateDirectory(sDirectory);
        }

        string sJson = JsonSerializer.Serialize(value, JsonSerializerOptions);
        File.WriteAllText(sFilePath, sJson, Encoding.UTF8);
    }
}
