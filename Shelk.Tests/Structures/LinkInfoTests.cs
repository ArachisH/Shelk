using System;
using System.Collections.Generic;

using Shelk.Structures;

using Xunit;

namespace Shelk.Tests.Structures;

public class LinkInfoTests
{
    [Theory]
    [MemberData(nameof(GetPassingInitializeCases))]
    public void Initialize_ShouldEqual(string hex, LinkInfo expected)
    {
        // Arrange
        ReadOnlySpan<byte> headerSpan = Convert.FromBase64String(hex);

        // Act
        var actual = new LinkInfo(ref headerSpan);

        // Assert
        Assert.Equal(expected.Size, actual.Size);
        Assert.Equal(expected.HeaderSize, actual.HeaderSize);
        Assert.Equal(expected.Flags, actual.Flags);
        Assert.Equal(expected.VolumeIDOffset, actual.VolumeIDOffset);
        Assert.Equal(expected.CommonNetworkRelativeLinkOffset, actual.CommonNetworkRelativeLinkOffset);
        Assert.Equal(expected.CommonPathSuffixOffset, actual.CommonPathSuffixOffset);
        Assert.Equal(expected.LocalBasePathOffsetUnicode, actual.LocalBasePathOffsetUnicode);
        Assert.Equal(expected.CommonPathSuffixOffsetUnicode, actual.CommonPathSuffixOffsetUnicode);
        //Assert.Equal(expected.VolumeID, actual.VolumeID);
        Assert.Equal(expected.LocalBasePath, actual.LocalBasePath);
        //Assert.Equal(expected.CommonNetworkRelativeLink, actual.CommonNetworkRelativeLink);
        Assert.Equal(expected.CommonPathSuffix, actual.CommonPathSuffix);
        Assert.Equal(expected.LocalBasePathUnicode, actual.LocalBasePathUnicode);
        Assert.Equal(expected.CommonPathSuffixUnicode, actual.CommonPathSuffixUnicode);
    }

    public static IEnumerable<object[]> GetPassingInitializeCases()
    {
        yield return new object[2]
        {
            "QAAAABwAAAABAAAAHAAAAC0AAAAAAAAAPwAAABEAAAADAAAAMW8bgBAAAAAAQzpceHdmXG1hbnVhbC5wZGYAAA==",
            new LinkInfo
            {
                Size = 64,
                HeaderSize = 28,
                Flags = (LinkInfoFlags)1,
                VolumeIDOffset = 28,
                LocalBasePathOffset = 45,
                CommonNetworkRelativeLinkOffset = 0,
                CommonPathSuffixOffset = 63,
                LocalBasePathOffsetUnicode = 0,
                CommonPathSuffixOffsetUnicode = 0,
                LocalBasePath = @"C:\xwf\manual.pdf",
                CommonPathSuffix = "",
                LocalBasePathUnicode = null
            }
        };

        yield return new object[2]
        {
            "ZAAAABwAAAABAAAAHAAAAC0AAAAAAAAAYwAAABEAAAADAAAAtbjq6BAAAAAAQzpcUHJvZ3JhbSBGaWxlcyAoeDg2KVxJbnRlcm5ldCBFeHBsb3JlclxpZXhwbG9yZS5leGUAAA==",
            new LinkInfo
            {
                Size = 100,
                HeaderSize = 28,
                Flags = (LinkInfoFlags)1,
                VolumeIDOffset = 28,
                LocalBasePathOffset = 45,
                CommonNetworkRelativeLinkOffset = 0,
                CommonPathSuffixOffset = 99,
                LocalBasePathOffsetUnicode = 0,
                CommonPathSuffixOffsetUnicode = 0,
                LocalBasePath = @"C:\Program Files (x86)\Internet Explorer\iexplore.exe",
                CommonPathSuffix = "",
                LocalBasePathUnicode = null
            }
        };

        yield return new object[2]
        {
            "UgAAABwAAAABAAAAHAAAAC0AAAAAAAAAUQAAABEAAAADAAAAR+j34BAAAAAAQzpcV0lORE9XU1xzeXN0ZW0zMlx1c210XG1pZ3dpei5leGUAAA==",
            new LinkInfo
            {
                Size = 82,
                HeaderSize = 28,
                Flags = (LinkInfoFlags)1,
                VolumeIDOffset = 28,
                LocalBasePathOffset = 45,
                CommonNetworkRelativeLinkOffset = 0,
                CommonPathSuffixOffset = 81,
                LocalBasePathOffsetUnicode = 0,
                CommonPathSuffixOffsetUnicode = 0,
                LocalBasePath = @"C:\WINDOWS\system32\usmt\migwiz.exe",
                CommonPathSuffix = "",
                LocalBasePathUnicode = null
            }
        };

        yield return new object[2]
        {
            "eAAAABwAAAABAAAAHAAAAC0AAAAAAAAAdwAAABEAAAADAAAAR+j34BAAAAAAQzpcRG9jdW1lbnRzIGFuZCBTZXR0aW5nc1xBbGwgVXNlcnNcRG9jdW1lbnRzXE15IFBpY3R1cmVzXFNhbXBsZSBQaWN0dXJlcwAA",
            new LinkInfo
            {
                Size = 120,
                HeaderSize = 28,
                Flags = (LinkInfoFlags)1,
                VolumeIDOffset = 28,
                LocalBasePathOffset = 45,
                CommonNetworkRelativeLinkOffset = 0,
                CommonPathSuffixOffset = 119,
                LocalBasePathOffsetUnicode = 0,
                CommonPathSuffixOffsetUnicode = 0,
                LocalBasePath = @"C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures",
                CommonPathSuffix = "",
                LocalBasePathUnicode = null
            }
        };
    }
}