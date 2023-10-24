using Shelk.Structures;
using Shelk.Tests.Helpers;

using System;
using System.Collections.Generic;

using Xunit;

namespace Shelk.Tests.Structures;

public class VolumeIDTests
{
    [Theory]
    [MemberData(nameof(GetPassingInitializeCases))]
    public void Initialize_ShouldEqual(string base64, VolumeID expected)
    {
        // Arrange
        ReadOnlySpan<byte> volumeIDSpan = Convert.FromBase64String(base64);

        // Act
        var actual = new VolumeID(ref volumeIDSpan);

        // Assert
        Assert.Equal(expected.Size, actual.Size);
        Assert.Equal(expected.Type, actual.Type);
        Assert.Equal(expected.DriveSerialNumber, actual.DriveSerialNumber);
        Assert.Equal(expected.VolumeLabelOffset, actual.VolumeLabelOffset);
        Assert.Equal(expected.VolumeLabelOffsetUnicode, actual.VolumeLabelOffsetUnicode);
    }

    [Theory]
    [InlineData("1100000018A7A301000000000")]
    [InlineData("FF00000003000000818A7A301000000000")]
    public void Initialize_ShouldThrowOnNotEnoughData(string hex)
    {
        // Arange/Act
        void VolumeIDInitializer()
        {
            ReadOnlySpan<byte> volumeIDSpan = BinaryConversion.GetBytes(hex);
            _ = new VolumeID(ref volumeIDSpan);
        }

        // Assert
        Assert.Throws<ArgumentException>("source", VolumeIDInitializer);
    }

    [Theory]
    [InlineData("1000000003000000818A7A301000000000")]
    public void Initialize_ShouldThrowOnInvalidDecodedSize(string hex)
    {
        // Arange/Act
        void VolumeIDInitializer()
        {
            ReadOnlySpan<byte> volumeIDSpan = BinaryConversion.GetBytes(hex);
            _ = new VolumeID(ref volumeIDSpan);
        }

        // Assert
        Assert.Throws<ArgumentException>("source", VolumeIDInitializer);
    }

    public static IEnumerable<object[]> GetPassingInitializeCases()
    {
        yield return new object[2]
        {
            "EQAAAAMAAAAxbxuAEAAAAAA=",
            new VolumeID
            {
                Size = 17,
                Type = (DriveType)3,
                DriveSerialNumber = 2149281585,
                VolumeLabelOffset = 16,
                VolumeLabelOffsetUnicode = 0,
                Data = Convert.FromBase64String("AA==")
            }
        };

        yield return new object[2]
        {
            "EQAAAAMAAAAvjACIEAAAAAA=",
            new VolumeID
            {
                Size = 17,
                Type = (DriveType)3,
                DriveSerialNumber = 2281737263,
                VolumeLabelOffset = 16,
                VolumeLabelOffsetUnicode = 0,
                Data = Convert.FromBase64String("AA==")
            }
        };

        yield return new object[2]
        {
            "FwAAAAMAAAAJx1zYEAAAAFRlc3RPUwA=",
            new VolumeID
            {
                Size = 23,
                Type = (DriveType)3,
                DriveSerialNumber = 3629958921,
                VolumeLabelOffset = 16,
                VolumeLabelOffsetUnicode = 0,
                Data = Convert.FromBase64String("VGVzdE9TAA==")
            }
        };
    }
}