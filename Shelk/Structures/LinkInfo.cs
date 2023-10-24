using System.Text;

using Shelk.Binary;

namespace Shelk.Structures;

/// <summary>
/// Represents information necessary to resolve a link target if it is not found in its original location.
/// This includes information about the volume that the target was stored on, the mapped drive letter, and a Universal Naming Convention (UNC) form of the path if one existed when the link was created.
/// </summary>
public sealed class LinkInfo : IShellLinkObject
{
    private const uint MINIMUM_HEADER_SIZE_FOR_UNICODE = 0x00000024;

    /// <summary>
    /// Specifies the size, in bytes, of the <see cref="LinkInfo"/> structure.
    /// All offsets specified in this structure MUST be less than this value, and all strings contained in this structure MUST fit within the extent defined by this size.
    /// </summary>
    public uint Size { get; set; }

    /// <summary>
    /// Specifies the size, in bytes, of the <see cref="LinkInfo"/> header section, which is composed of the <see cref="Size"/>, <see cref="HeaderSize"/>, <see cref="Flags"/>, <see cref="VolumeIDOffset"/>,
    /// <see cref="LocalBasePathOffset"/>, <see cref="CommonNetworkRelativeLinkOffset"/>, <see cref="CommonPathSuffixOffset"/> fields, and, if included, the <see cref="LocalBasePathOffsetUnicode"/> and <see cref="CommonPathSuffixOffsetUnicode"/> fields.
    /// </summary>
    public uint HeaderSize { get; set; }

    /// <summary>
    /// Flags that specify whether the <see cref="VolumeID"/>, <see cref="LocalBasePath"/>, <see cref="LocalBasePathUnicode"/>, and <see cref="CommonNetworkRelativeLink"/> fields are present in this structure.
    /// </summary>
    public LinkInfoFlags Flags { get; set; }

    /// <summary>
    /// Specifies the location of the <see cref="VolumeID"/> field.
    /// If the <see cref="LinkInfoFlags.VolumeIDAndLocalBasePath"/> flag is set, this value is an offset, in bytes, from the start of the <see cref="LinkInfo"/> structure; otherwise, this value MUST be zero.
    /// </summary>
    public uint VolumeIDOffset { get; set; }

    /// <summary>
    /// Specifies the location of the <see cref="LocalBasePath"/> field.
    /// If the <see cref="LinkInfoFlags.VolumeIDAndLocalBasePath"/> flag is set, this value is an offset, in bytes, from the start of the <see cref="LinkInfo"/> structure; otherwise, this value MUST be zero.
    /// </summary>
    public uint LocalBasePathOffset { get; set; }

    /// <summary>
    /// Specifies the location of the <see cref="CommonNetworkRelativeLink"/> field.
    /// If the <see cref="LinkInfoFlags.CommonNetworkRelativeLinkAndPathSuffix"/> flag is set, this value is an offset, in bytes, from the start of the <see cref="LinkInfo"/> structure; otherwise, this value MUST be zero.
    /// </summary>
    public uint CommonNetworkRelativeLinkOffset { get; set; }

    /// <summary>
    /// Specifies the location of the <see cref="CommonPathSuffix"/> field.
    /// This value is an offset, in bytes, from the start of the <see cref="LinkInfo"/> structure.
    /// </summary>
    public uint CommonPathSuffixOffset { get; set; }

    /// <summary>
    /// Specifies the location of the <see cref="LocalBasePathUnicode"/> field.
    /// If the <see cref="LinkInfoFlags.VolumeIDAndLocalBasePath"/> flag is set, this value is an offset, in bytes, from the start of the <see cref="LinkInfo"/> structure; otherwise, this value MUST be zero.
    /// This field can be present only if the value of the <see cref="HeaderSize"/> field is greater than or equal to 0x00000024.
    /// </summary>
    public uint LocalBasePathOffsetUnicode { get; set; }

    /// <summary>
    /// Specifies the location of the <see cref="CommonPathSuffixUnicode"/> field.
    /// This value is an offset, in bytes, from the start of the <see cref="LinkInfo"/> structure.
    /// This field can be present only if the value of the <see cref="HeaderSize"/> field is greater than or equal to 0x00000024.
    /// </summary>
    public uint CommonPathSuffixOffsetUnicode { get; set; }

    /// <summary>
    /// Specifies information about the volume that the link target was on when the link was created.
    /// This field is present if the <see cref="LinkInfoFlags.VolumeIDAndLocalBasePath"/> flag is set.
    /// </summary>
    public VolumeID? VolumeID { get; set; }

    /// <summary>
    /// Used to construct the full path to the link item or link target by appending the string in the <see cref="CommonPathSuffix"/> field.
    /// This field is present if the <see cref="LinkInfoFlags.VolumeIDAndLocalBasePath"/> flag is set.
    /// </summary>
    public string? LocalBasePath { get; set; }

    /// <summary>
    /// Specifies information about the network location where the link target is stored.
    /// </summary>
    public CommonNetworkRelativeLink? CommonNetworkRelativeLink { get; set; }

    /// <summary>
    /// Used to construct the full path to the link item or link target by being appended to the string in the <see cref="LocalBasePath"/> field.
    /// </summary>
    public string? CommonPathSuffix { get; set; }

    /// <summary>
    /// Used to construct the full path to the link item or link target by appending the string in the <see cref="CommonPathSuffixUnicode"/> field.
    /// This field can be present only if the <see cref="LinkInfoFlags.VolumeIDAndLocalBasePath"/> flag is set and the value of the <see cref="HeaderSize"/> field is greater than or equal to 0x00000024.
    /// </summary>
    public string? LocalBasePathUnicode { get; set; }

    /// <summary>
    /// Used to construct the full path to the link item or link target by being appended to the string in the <see cref="LocalBasePathUnicode"/> field.
    /// This field can be present only if the value of the <see cref="HeaderSize"/> field is greater than or equal to 0x00000024.
    /// </summary>
    public string? CommonPathSuffixUnicode { get; set; }

    public LinkInfo()
    { }
    public LinkInfo(ref ReadOnlySpan<byte> source)
    {
        Size = Primitives.Read<uint>(ref source);
        HeaderSize = Primitives.Read<uint>(ref source);
        Flags = Primitives.Read<LinkInfoFlags>(ref source);
        VolumeIDOffset = Primitives.Read<uint>(ref source);
        LocalBasePathOffset = Primitives.Read<uint>(ref source);
        CommonNetworkRelativeLinkOffset = Primitives.Read<uint>(ref source);
        CommonPathSuffixOffset = Primitives.Read<uint>(ref source);

        // These field can be present only if the value of HeaderSize is greater than or equal to 0x00000024.
        if (HeaderSize >= MINIMUM_HEADER_SIZE_FOR_UNICODE)
        {
            LocalBasePathOffsetUnicode = Primitives.Read<uint>(ref source);
            CommonPathSuffixOffsetUnicode = Primitives.Read<uint>(ref source);
        }

        if (Flags.HasFlag(LinkInfoFlags.VolumeIDAndLocalBasePath))
        {
            VolumeID = new VolumeID(ref source);
            LocalBasePath = Primitives.ReadNullTerminatedString(ref source);
        }

        if (Flags.HasFlag(LinkInfoFlags.CommonNetworkRelativeLinkAndPathSuffix))
        {
            CommonNetworkRelativeLink = new CommonNetworkRelativeLink(ref source);
        }

        CommonPathSuffix = Primitives.ReadNullTerminatedString(ref source);

        // These field can be present only if the value of HeaderSize is greater than or equal to 0x00000024.
        if (HeaderSize >= MINIMUM_HEADER_SIZE_FOR_UNICODE)
        {
            if (Flags.HasFlag(LinkInfoFlags.VolumeIDAndLocalBasePath))
            {
                LocalBasePathUnicode = Primitives.ReadNullTerminatedString(ref source, isUnicode: true);
            }
            CommonPathSuffixUnicode = Primitives.ReadNullTerminatedString(ref source, isUnicode: true);
        }
    }

    public int GetSize()
    {
        int size = 0;
        size += sizeof(uint);
        size += sizeof(uint);
        size += sizeof(LinkInfoFlags);
        size += sizeof(uint);
        size += sizeof(uint);
        size += sizeof(uint);
        size += sizeof(uint);

        if (HeaderSize >= MINIMUM_HEADER_SIZE_FOR_UNICODE)
        {
            size += sizeof(uint);
            size += sizeof(uint);
        }

        if (Flags.HasFlag(LinkInfoFlags.VolumeIDAndLocalBasePath))
        {
            size += VolumeID.GetSize();
            size += Encoding.Default.GetByteCountNullable(LocalBasePath) + sizeof(byte); // Null terminated string.
        }

        if (Flags.HasFlag(LinkInfoFlags.CommonNetworkRelativeLinkAndPathSuffix))
        {
            size += CommonNetworkRelativeLink.GetSize();
        }

        if (HeaderSize >= MINIMUM_HEADER_SIZE_FOR_UNICODE)
        {
            if (Flags.HasFlag(LinkInfoFlags.VolumeIDAndLocalBasePath))
            {
                size += Encoding.Unicode.GetByteCountNullable(LocalBasePathUnicode) + sizeof(byte) + sizeof(byte); // + '\0'
            }
            size += Encoding.Unicode.GetByteCountNullable(LocalBasePathUnicode) + sizeof(byte) + sizeof(byte); // + '\0'
        }

        return size;
    }
    public string ToString(string? format, IFormatProvider? formatProvider) => throw new NotImplementedException();
}