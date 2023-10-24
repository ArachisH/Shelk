using System;

using Shelk.Structures;
using Shelk.Tests.Helpers;

using Xunit;

namespace Shelk.Tests.Structures;

public class IDListTests
{
    [Theory]
    [InlineData("0000", 0)]
    [InlineData("14001F8094E69953E56C6C4D8FCE1D8870FDCBA00000", 1)]
    [InlineData("14001F80DF8F22EDA89E704883B196B02CFE0D522000000047465349E0F7A7D1E9D4E849BF2CCEAA01D2E67000000000000000000000", 2)]
    public void Initialize_ShouldParseElements(string hex, int expected)
    {
        // Arrange
        ReadOnlySpan<byte> idListSpan = BinaryConversion.GetBytes(hex);

        // Act
        var iDList = new IDList(ref idListSpan);

        // Assert
        Assert.Equal(expected, iDList.Count);
    }

    [Theory]
    [InlineData("00")]
    public void Initialize_ShouldThrowOnNotEnoughData(string hex)
    {
        // Arange/Act
        void IDListInitializer()
        {
            ReadOnlySpan<byte> idListSpan = BinaryConversion.GetBytes(hex);
            _ = new IDList(ref idListSpan);
        }

        // Assert
        Assert.Throws<ArgumentException>("source", IDListInitializer);
    }

    [Theory]
    [InlineData("0001")]
    [InlineData("14001F8094E69953E56C6C4D8FCE1D8870FDCBA00001")]
    public void Initialize_ShouldThrowOnInvalidTerminalID(string hex)
    {
        // Arange/Act
        void IDListInitializer()
        {
            ReadOnlySpan<byte> idListSpan = BinaryConversion.GetBytes(hex);
            _ = new IDList(ref idListSpan);
        }

        // Assert
        Assert.Throws<ArgumentException>("source", IDListInitializer);
    }
}