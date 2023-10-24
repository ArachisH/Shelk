using Shelk.Binary;

namespace Shelk.Structures;

/// <summary>
/// Represents the volume information of a link target.
/// </summary>
public sealed class VolumeID : IShellLinkObject
{
    /// <summary>
    /// Defines the non-inclusive minimum size for this type, and acts as the offset limit to its' properties.
    /// </summary>
    private const uint MUST_BE_GREATER_THAN = 0x00000010;

    /// <summary>
    /// If the value of <see cref="VolumeLabelOffset"/> is equal to this value, then use <see cref="VolumeLabelOffsetUnicode"/> to read a Unicode string from <see cref="Data"/> instead.
    /// </summary>
    private const uint IS_LABEL_UNICODE_EQUALS = 0x00000014;

    /// <summary>
    /// Specifies the size, in bytes, of this structure.
    /// This value MUST be greater than 0x00000010.
    /// All offsets specified in this structure MUST be less than this value, and all strings contained in this structure MUST fit within the extent defined by this size.
    /// </summary>
    public uint Size { get; set; }

    /// <summary>
    /// The type of drive link the target is stored on.
    /// </summary>
    public DriveType Type { get; set; }

    /// <summary>
    /// Specifies the drive serial number of the volume the link target is stored on.
    /// </summary>
    public uint DriveSerialNumber { get; set; }

    /// <summary>
    /// Specifies the location of a string that contains the volume label of the drive that the link target is stored on.
    /// This value is an offset, in bytes, from the start of the <see cref="VolumeID"/> structure to a null-terminated string of characters, defined by the system default code page.
    /// The volume label string is located in the <see cref="Data"/> field of this structure.
    /// If the value of this field is 0x00000014, it MUST be ignored, and the value of the <see cref="VolumeLabelOffsetUnicode"/> field MUST be used to locate the volume label string.
    /// </summary>
    public uint VolumeLabelOffset { get; set; }

    /// <summary>
    /// Specifies the location of a string that contains the volume label of the drive that the link target is stored on.
    /// This value is an offset, in bytes, from the start of the <see cref="VolumeID"/> structure to a null-terminated string of Unicode characters.
    /// The volume label string is located in the <see cref="Data"/> field of this structure.
    /// </summary>
    public uint VolumeLabelOffsetUnicode { get; set; }

    /// <summary>
    /// Specifies a buffer that contains the volume label of the drive as a string defined by the system default code page or Unicode characters, as specified by preceding fields.
    /// </summary>
    public byte[]? Data { get; set; }

    public VolumeID()
    { }
    public VolumeID(ref ReadOnlySpan<byte> source)
    {
        Size = Primitives.Read<uint>(ref source);
        if (Size - 4 > source.Length)
        {
            throw new ArgumentException("The decoded size is greater than the provided input.", nameof(source));
        }

        if (Size <= MUST_BE_GREATER_THAN)
        {
            throw new ArgumentException($"The size of this structure must be greater than '0x{MUST_BE_GREATER_THAN:X8}'.", nameof(source));
        }

        // We only want the portion of data related to this structure, so that we can identify everything that is left over as the Data field.
        ReadOnlySpan<byte> volumeIDSpan = Primitives.SliceThenAdvance(ref source, (int)Size - sizeof(uint));

        Type = Primitives.Read<DriveType>(ref volumeIDSpan);
        DriveSerialNumber = Primitives.Read<uint>(ref volumeIDSpan);
        VolumeLabelOffset = Primitives.Read<uint>(ref volumeIDSpan);
        if (VolumeLabelOffset == IS_LABEL_UNICODE_EQUALS)
        {
            VolumeLabelOffsetUnicode = Primitives.Read<uint>(ref volumeIDSpan);
        }

        // TODO: Avoid allocation here?
        // The remaining bytes are considered the Data field.
        Data = volumeIDSpan.ToArray();
    }

    public int GetSize()
    {
        return
            sizeof(uint)
            + sizeof(DriveType)
            + sizeof(uint)
            + sizeof(uint)
            + sizeof(uint)
            + Data?.Length ?? 0;
    }
    public string ToString(string? format, IFormatProvider? formatProvider) => throw new NotImplementedException();
}