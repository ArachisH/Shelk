using System;
using System.Text;

using Shelk.Binary;

using Xunit;

namespace Shelk.Tests.Binary;

public class PrimitivesTests
{
    [Theory]
    [InlineData(0, TypeCode.Byte)]
    [InlineData(0, TypeCode.Boolean)]
    [InlineData(1, TypeCode.UInt16)]
    [InlineData(1, TypeCode.Int16)]
    [InlineData(3, TypeCode.UInt32)]
    [InlineData(3, TypeCode.Int32)]
    [InlineData(7, TypeCode.UInt64)]
    [InlineData(7, TypeCode.Int64)]
    public void Read_ThrowOnNotEnoughData(int dataSize, TypeCode typeCode)
    {
        // Arrange/Act
        void Read()
        {
            ReadOnlySpan<byte> dataSpan = new byte[dataSize];
            switch (typeCode)
            {
                case TypeCode.Byte: Primitives.Read<byte>(ref dataSpan); break;
                case TypeCode.Boolean: Primitives.Read<bool>(ref dataSpan); break;
                case TypeCode.UInt16: Primitives.Read<ushort>(ref dataSpan); break;
                case TypeCode.Int16: Primitives.Read<short>(ref dataSpan); break;
                case TypeCode.UInt32: Primitives.Read<uint>(ref dataSpan); break;
                case TypeCode.Int32: Primitives.Read<int>(ref dataSpan); break;
                case TypeCode.UInt64: Primitives.Read<ulong>(ref dataSpan); break;
                case TypeCode.Int64: Primitives.Read<long>(ref dataSpan); break;
            }
        }

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>("source", Read);
    }

    [Theory]
    [InlineData(new byte[1] { 1 }, TypeCode.Byte, (byte)1)]
    [InlineData(new byte[1] { 1 }, TypeCode.Boolean, true)]
    [InlineData(new byte[2] { 0xFF, 0xFF }, TypeCode.UInt16, ushort.MaxValue)]
    [InlineData(new byte[2] { 0xFF, 0x7F }, TypeCode.Int16, short.MaxValue)]
    [InlineData(new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF }, TypeCode.UInt32, uint.MaxValue)]
    [InlineData(new byte[4] { 0xFF, 0xFF, 0xFF, 0x7F }, TypeCode.Int32, int.MaxValue)]
    [InlineData(new byte[8] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, TypeCode.UInt64, ulong.MaxValue)]
    [InlineData(new byte[8] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F }, TypeCode.Int64, long.MaxValue)]
    public void Read_ReturnValue(byte[] data, TypeCode typeCode, IComparable expected)
    {
        // Arrange
        ReadOnlySpan<byte> dataSpan = data;

        // Act
        IComparable value = null;
        switch (typeCode)
        {
            case TypeCode.Byte: value = Primitives.Read<byte>(ref dataSpan); break;
            case TypeCode.Boolean: value = Primitives.Read<bool>(ref dataSpan); break;
            case TypeCode.UInt16: value = Primitives.Read<ushort>(ref dataSpan); break;
            case TypeCode.Int16: value = Primitives.Read<short>(ref dataSpan); break;
            case TypeCode.UInt32: value = Primitives.Read<uint>(ref dataSpan); break;
            case TypeCode.Int32: value = Primitives.Read<int>(ref dataSpan); break;
            case TypeCode.UInt64: value = Primitives.Read<ulong>(ref dataSpan); break;
            case TypeCode.Int64: value = Primitives.Read<long>(ref dataSpan); break;
        }

        // Assert
        Assert.Equal(expected, value);
    }

    [Theory]
    [InlineData(true, 0, "Testing123")]
    [InlineData(false, 0, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    [InlineData(true, 260, "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    [InlineData(false, 520, "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    public void ReadNullTerminatedString_ReturnValue(bool isUnicode, int padding, string expected)
    {
        // Arrange
        int nullBytes = isUnicode ? 2 : 1;
        Encoding encoding = isUnicode ? Encoding.Unicode : Encoding.Default;

        byte[] encoded = new byte[encoding.GetByteCount(expected) + nullBytes + padding];
        encoding.GetBytes(expected, 0, expected.Length, encoded, 0);

        ReadOnlySpan<byte> source = encoded;

        // Act
        string actual = Primitives.ReadNullTerminatedString(ref source, isUnicode: isUnicode);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(true, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    [InlineData(false, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    public void ReadEncodedString_ReturnValue(bool isUnicode, string expected)
    {
        // Arrange
        Encoding encoding = isUnicode ? Encoding.Unicode : Encoding.Default;
        ReadOnlySpan<byte> source = encoding.GetBytes(expected);

        // Act
        string actual = Primitives.ReadEncodedString(ref source, expected.Length, isUnicode);

        // Assert
        Assert.Equal(expected, actual);
    }
}