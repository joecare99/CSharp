using IDR.Core.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace IDR.Infrastructure.Pe;

internal sealed class DelphiBinaryFormReader
{
    private const int MaximumDepth = 128;
    private const int MaximumComponents = 10000;
    private const int MaximumPropertiesPerComponent = 4096;
    private const int MaximumValueLength = 16 * 1024 * 1024;
    private const int MaximumValueDepth = 32;
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    private readonly ReadOnlyMemory<byte> _data;
    private int _position;
    private int _componentCount;

    public DelphiBinaryFormReader(ReadOnlyMemory<byte> data)
    {
        _data = data;
    }

    public DelphiFormComponent Read()
    {
        if (_data.Length < 4 || !_data.Span[..4].SequenceEqual("TPF0"u8))
        {
            throw new InvalidDataException("The Delphi form resource has no TPF0 signature.");
        }

        _position = 4;
        DelphiFormComponent root = ReadComponent(0);
        while (_position < _data.Length && PeekByte() == 0)
        {
            _position++;
        }

        if (_position != _data.Length)
        {
            throw new InvalidDataException("The Delphi form resource has trailing data.");
        }

        return root;
    }

    private DelphiFormComponent ReadComponent(int depth)
    {
        if (depth > MaximumDepth || ++_componentCount > MaximumComponents)
        {
            throw new InvalidDataException("The Delphi form resource exceeds the component limits.");
        }

        string className = ReadShortString();
        string name = ReadShortString();
        if (string.IsNullOrWhiteSpace(className))
        {
            throw new InvalidDataException("A Delphi form component has no class name.");
        }

        List<DelphiFormProperty> properties = ReadProperties();
        List<DelphiFormComponent> children = [];
        while (_position < _data.Length && PeekByte() != 0)
        {
            children.Add(ReadComponent(depth + 1));
        }

        if (_position >= _data.Length)
        {
            throw new InvalidDataException("The Delphi form component tree is unterminated.");
        }

        _position++;
        return new DelphiFormComponent(className, name, properties, children);
    }

    private List<DelphiFormProperty> ReadProperties()
    {
        List<DelphiFormProperty> properties = [];
        while (true)
        {
            string propertyName = ReadShortString();
            if (propertyName.Length == 0)
            {
                return properties;
            }

            if (properties.Count >= MaximumPropertiesPerComponent)
            {
                throw new InvalidDataException("A Delphi form component contains too many properties.");
            }

            byte valueType = ReadByte();
            if (valueType == 0)
            {
                return properties;
            }

            properties.Add(new DelphiFormProperty(propertyName, ReadValue(valueType, 0)));
        }
    }

    private string ReadValue(byte valueType, int depth)
    {
        if (depth > MaximumValueDepth)
        {
            throw new InvalidDataException("The Delphi form property value is nested too deeply.");
        }

        switch (valueType)
        {
            case 1:
            {
                List<string> values = [];
                while (true)
                {
                    byte itemType = ReadByte();
                    if (itemType == 0)
                    {
                        return "[" + string.Join(", ", values) + "]";
                    }

                    values.Add(ReadValue(itemType, depth + 1));
                }
            }
            case 2:
                return unchecked((sbyte)ReadByte()).ToString(CultureInfo.InvariantCulture);
            case 3:
                return ReadInt16().ToString(CultureInfo.InvariantCulture);
            case 4:
                return ReadInt32().ToString(CultureInfo.InvariantCulture);
            case 5:
                return "Extended(0x" + Convert.ToHexString(ReadBytes(10)) + ")";
            case 6:
            case 7:
                return ReadShortString();
            case 8:
                return bool.FalseString;
            case 9:
                return bool.TrueString;
            case 10:
            {
                int length = ReadBoundedLength();
                byte[] prefix = ReadBytes(Math.Min(length, 32));
                SkipBytes(length - prefix.Length);
                string preview = prefix.Length == 0 ? string.Empty : ", 0x" + Convert.ToHexString(prefix);
                return "Binary(" + length.ToString(CultureInfo.InvariantCulture)
                    + " bytes" + preview + ")";
            }
            case 11:
            {
                List<string> values = [];
                while (true)
                {
                    string value = ReadShortString();
                    if (value.Length == 0)
                    {
                        return "[" + string.Join(", ", values) + "]";
                    }

                    values.Add(value);
                }
            }
            case 12:
                return ReadLengthPrefixedString(Encoding.Latin1);
            case 13:
                return "nil";
            case 14:
                return ReadCollection(depth + 1);
            case 15:
                return ReadSingle().ToString("R", CultureInfo.InvariantCulture);
            case 16:
                return (ReadInt64() / 10000.0).ToString(CultureInfo.InvariantCulture);
            case 17:
            {
                double date = ReadDouble();
                try
                {
                    return DateTime.FromOADate(date).ToString("O", CultureInfo.InvariantCulture);
                }
                catch (ArgumentException exception)
                {
                    throw new InvalidDataException("The Delphi form contains an invalid date value.", exception);
                }
            }
            case 18:
                return ReadLengthPrefixedString(Encoding.Unicode, sizeof(char));
            case 19:
                return ReadInt64().ToString(CultureInfo.InvariantCulture);
            case 20:
                return ReadLengthPrefixedString(StrictUtf8);
            case 21:
                return ReadLengthPrefixedString(Encoding.Unicode, sizeof(char));
            default:
                throw new InvalidDataException($"The Delphi form uses unsupported value type {valueType}.");
        }
    }

    private string ReadCollection(int depth)
    {
        List<string> items = [];
        while (true)
        {
            byte marker = ReadByte();
            if (marker == 0)
            {
                return "[" + string.Join("; ", items) + "]";
            }

            if (marker != 1)
            {
                throw new InvalidDataException("A Delphi form collection has an invalid item marker.");
            }

            List<string> properties = [];
            while (true)
            {
                string name = ReadShortString();
                if (name.Length == 0)
                {
                    break;
                }

                byte valueType = ReadByte();
                properties.Add(name + "=" + ReadValue(valueType, depth + 1));
            }

            items.Add("{" + string.Join(", ", properties) + "}");
        }
    }

    private string ReadLengthPrefixedString(Encoding encoding, int unitsPerCharacter = 1)
    {
        int length = ReadBoundedLength();
        int byteCount = checked(length * unitsPerCharacter);
        byte[] bytes = ReadBytes(byteCount);
        try
        {
            return encoding.GetString(bytes);
        }
        catch (DecoderFallbackException exception)
        {
            throw new InvalidDataException("The Delphi form contains an invalid encoded string.", exception);
        }
    }

    private int ReadBoundedLength()
    {
        int length = ReadInt32();
        if (length < 0 || length > MaximumValueLength)
        {
            throw new InvalidDataException("A Delphi form value has an invalid length.");
        }

        return length;
    }

    private string ReadShortString()
    {
        int length = ReadByte();
        if (length == byte.MaxValue)
        {
            length = ReadUInt16();
        }

        return Encoding.Latin1.GetString(ReadBytes(length));
    }

    private byte ReadByte()
    {
        EnsureAvailable(sizeof(byte));
        return _data.Span[_position++];
    }

    private byte PeekByte()
    {
        EnsureAvailable(sizeof(byte));
        return _data.Span[_position];
    }

    private ushort ReadUInt16()
    {
        EnsureAvailable(sizeof(ushort));
        ushort value = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(
            _data.Span.Slice(_position, sizeof(ushort)));
        _position += sizeof(ushort);
        return value;
    }

    private short ReadInt16() => unchecked((short)ReadUInt16());

    private int ReadInt32()
    {
        EnsureAvailable(sizeof(int));
        int value = System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(
            _data.Span.Slice(_position, sizeof(int)));
        _position += sizeof(int);
        return value;
    }

    private long ReadInt64()
    {
        EnsureAvailable(sizeof(long));
        long value = System.Buffers.Binary.BinaryPrimitives.ReadInt64LittleEndian(
            _data.Span.Slice(_position, sizeof(long)));
        _position += sizeof(long);
        return value;
    }

    private float ReadSingle()
    {
        EnsureAvailable(sizeof(float));
        float value = BitConverter.ToSingle(_data.Span.Slice(_position, sizeof(float)));
        _position += sizeof(float);
        return value;
    }

    private double ReadDouble()
    {
        EnsureAvailable(sizeof(double));
        double value = BitConverter.ToDouble(_data.Span.Slice(_position, sizeof(double)));
        _position += sizeof(double);
        return value;
    }

    private byte[] ReadBytes(int count)
    {
        EnsureAvailable(count);
        byte[] value = _data.Span.Slice(_position, count).ToArray();
        _position += count;
        return value;
    }

    private void SkipBytes(int count)
    {
        EnsureAvailable(count);
        _position += count;
    }

    private void EnsureAvailable(int count)
    {
        if (count < 0 || count > _data.Length - _position)
        {
            throw new InvalidDataException("The Delphi form resource is truncated.");
        }
    }
}
