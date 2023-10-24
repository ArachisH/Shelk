using System;
using System.Collections.Generic;

using Shelk.Structures;
using Shelk.Tests.Helpers;

using Xunit;

namespace Shelk.Tests.Structures;

public class ShellLinkHeaderTests
{
    [Theory]
    [MemberData(nameof(GetPassingInitializeCases))]
    public void Initialize_ShouldEqual(string hex, ShellLinkHeader expected)
    {
        // Arrange
        ReadOnlySpan<byte> headerSpan = Convert.FromBase64String(hex);

        // Act
        var header = new ShellLinkHeader(ref headerSpan);

        // Assert
        Assert.Equal(expected, header);
    }

    [Theory]
    [InlineData("4C000000")]
    public void Initialize_ShouldThrowOnSmallData(string hex)
    {
        // Arange/Act
        void ShellLinkHeaderInitializer()
        {
            ReadOnlySpan<byte> headerSpan = BinaryConversion.GetBytes(hex);
            _ = new ShellLinkHeader(ref headerSpan);
        }

        // Assert
        Assert.Throws<ArgumentException>("source", ShellLinkHeaderInitializer);
    }

    [Theory]
    [InlineData("5C0000000114020000000000C0000000000000469F00000020000000587A54ABE74FD101B2DC56ABE74FD10100A0A02B3231C50100A800000000000001000000000000000000000000000000")]
    public void Initialize_ShouldThrowOnInvalidDecodedSize(string hex)
    {
        // Arange/Act
        void ShellLinkHeaderInitializer()
        {
            ReadOnlySpan<byte> headerSpan = BinaryConversion.GetBytes(hex);
            _ = new ShellLinkHeader(ref headerSpan);
        }

        // Assert
        Assert.Throws<ArgumentException>("source", ShellLinkHeaderInitializer);
    }

    [Theory]
    [InlineData("4C000000999999999999999999999999999999999F00000020000000587A54ABE74FD101B2DC56ABE74FD10100A0A02B3231C50100A800000000000001000000000000000000000000000000")]
    public void Initialize_ShoudThrowOnInvalidDecodedCLSID(string hex)
    {
        // Arange/Act
        void ShellLinkHeaderInitializer()
        {
            ReadOnlySpan<byte> headerSpan = BinaryConversion.GetBytes(hex);
            _ = new ShellLinkHeader(ref headerSpan);
        }

        // Assert
        Assert.Throws<ArgumentException>("source", ShellLinkHeaderInitializer);
    }

    [Theory]
    [InlineData("4C0000000114020000000000C0000000000000469B00080020000000D0E9EEF21515C901D0E9EEF21515C901D0E9EEF21515C901000000000000000001000000000099999999999999999999")]
    public void Initialize_ShouldThrowOnInvalidReservedFields(string hex)
    {
        // Arange/Act
        void ShellLinkHeaderInitializer()
        {
            ReadOnlySpan<byte> headerSpan = BinaryConversion.GetBytes(hex);
            _ = new ShellLinkHeader(ref headerSpan);
        }

        // Assert
        Assert.Throws<ArgumentException>("source", ShellLinkHeaderInitializer);
    }

    public static IEnumerable<object[]> GetPassingInitializeCases()
    {
        yield return new object[2]
        {
            "TAAAAAEUAgAAAAAAwAAAAAAAAEabAAgAIAAAANDp7vIVFckB0Onu8hUVyQHQ6e7yFRXJAQAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAA==",
            new ShellLinkHeader
            {
                Flags = (LinkFlags)524443,
                FileAttributes = (FileAttributesFlags)32,
                CreationTime = DateTime.FromFileTimeUtc(128657248371010000),
                AccessTime = DateTime.FromFileTimeUtc(128657248371010000),
                WriteTime = DateTime.FromFileTimeUtc(128657248371010000),
                FileSize = 0,
                IconIndex = 0,
                Command = (ShowCommand)1,
                HotKey = new HotKeyFlags(0, 0)
            }
        };

        yield return new object[2]
        {
            "TAAAAAEUAgAAAAAAwAAAAAAAAEafAAAAIAAAAFh6VKvnT9EBstxWq+dP0QEAoKArMjHFAQCoAAAAAAAAAQAAAAAAAAAAAAAAAAAAAA==",
            new ShellLinkHeader
            {
                Flags = (LinkFlags)159,
                FileAttributes = (FileAttributesFlags)32,
                CreationTime = DateTime.FromFileTimeUtc(130973720600935000),
                AccessTime = DateTime.FromFileTimeUtc(130973720601091250),
                WriteTime = DateTime.FromFileTimeUtc(127562256000000000),
                FileSize = 43008,
                IconIndex = 0,
                Command = (ShowCommand)1,
                HotKey = new HotKeyFlags(0, 0)
            }
        };

        yield return new object[2]
        {
            "TAAAAAEUAgAAAAAAwAAAAAAAAEaBAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAA==",
            new ShellLinkHeader
            {
                Flags = (LinkFlags)524417,
                FileAttributes = (FileAttributesFlags)0,
                CreationTime = DateTime.FromFileTimeUtc(0),
                AccessTime = DateTime.FromFileTimeUtc(0),
                WriteTime = DateTime.FromFileTimeUtc(0),
                FileSize = 0,
                IconIndex = 0,
                Command = (ShowCommand)1,
                HotKey = new HotKeyFlags(0, 0)
            }
        };
    }
}