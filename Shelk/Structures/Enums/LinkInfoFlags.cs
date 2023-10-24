namespace Shelk.Structures;

/// <summary>
/// Represents 32-bit unsigned integer flags for parsing a <see cref="LinkInfo"/> structure.
/// </summary>
[Flags]
public enum LinkInfoFlags : uint
{
    None = 0,
    /// <summary>
    /// If set, the <see cref="LinkInfo.VolumeID"/> and <see cref="LinkInfo.LocalBasePath"/> fields are present, and their locations are specified by the values of the <see cref="LinkInfo.VolumeIDOffset"/> and <see cref="LinkInfo.LocalBasePathOffset"/> fields, respectively.
    /// If the value of the <see cref="LinkInfo.HeaderSize"/> field is greater than or equal to 0x00000024, the <see cref="LinkInfo.LocalBasePathUnicode"/> field is present, and its location is specified by the value of the <see cref="LinkInfo.LocalBasePathOffsetUnicode"/> field.
    /// If not set, the <see cref="LinkInfo.VolumeID"/>, <see cref="LinkInfo.LocalBasePath"/>, and <see cref="LinkInfo.LocalBasePathUnicode"/> fields are not present, and the values of the <see cref="LinkInfo.VolumeIDOffset"/> and <see cref="LinkInfo.LocalBasePathOffset"/> fields are zero.
    /// If the value of the <see cref="LinkInfo.HeaderSize"/> field is greater than or equal to 0x00000024, the value of the <see cref="LinkInfo.LocalBasePathOffsetUnicode"/> field is zero.
    /// </summary>
    VolumeIDAndLocalBasePath = 1,
    /// <summary>
    /// If set, the <see cref="LinkInfo.CommonNetworkRelativeLink"/> field is present, and its location is specified by the value of the <see cref="LinkInfo.CommonNetworkRelativeLinkOffset"/> field.
    /// If not set, the <see cref="LinkInfo.CommonNetworkRelativeLink"/> field is not present, and the value of the <see cref="LinkInfo.CommonNetworkRelativeLinkOffset"/> field is zero.
    /// </summary>
    CommonNetworkRelativeLinkAndPathSuffix = 2,
}