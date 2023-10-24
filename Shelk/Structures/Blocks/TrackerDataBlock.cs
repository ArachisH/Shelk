using Shelk.Binary;

namespace Shelk.Structures.Blocks;

/// <summary>
/// Represents data that can be used to resolve a link target if it is not found in its original location when the link is resolved.
/// This data is passed to the Link Tracking service to find the link target.
/// </summary>
public sealed class TrackerDataBlock : DataBlock
{
    public override uint Signature => TRACKER_DATA_SIGNATURE;

    private const uint ABSOLUTE_LENGTH = 0x00000058;
    private const uint ABSOLUTE_VERSION = 0x00000000;

    /// <summary>
    /// Specifies the size of the rest of the <see cref="TrackerDataBlock"/> structure, including this <see cref="Length"/> field.
    /// This value MUST be 0x00000058.
    /// </summary>
    public uint Length { get; }

    /// <summary>
    /// The version of the data block.
    /// This value MUST be 0x00000000.
    /// </summary>
    public uint Version { get; }

    /// <summary>
    /// Specifies the NetBIOS name of the machine where the link target was last known to reside.
    /// </summary>
    public string MachineID { get; }

    /// <summary>
    /// Two values in <see cref="Guid"/> packet representation that are used to find the link target with the Link Tracking service.
    /// </summary>
    public ValueTuple<Guid, Guid> Droid { get; }
    /// <summary>
    /// Two values in <see cref="Guid"/> packet representation that are used to find the link target with the Link Tracking service.
    /// </summary>
    public ValueTuple<Guid, Guid> DroidBirth { get; }

    protected override int MustEqual => 0x00000060;

    public TrackerDataBlock(ref ReadOnlySpan<byte> source)
        : base(ref source)
    {
        Length = Primitives.Read<uint>(ref source);
        if (Length != ABSOLUTE_LENGTH)
        {
            throw new ArgumentException($"The decoded length field must equal '0x{ABSOLUTE_LENGTH:X8}'.", nameof(source));
        }

        Version = Primitives.Read<uint>(ref source);
        if (Version != ABSOLUTE_VERSION)
        {
            throw new ArgumentException($"The decoded version field must equal '0x{ABSOLUTE_VERSION:X8}'.", nameof(source));
        }

        MachineID = Primitives.ReadNullTerminatedString(ref source, 16);

        Droid = Primitives.Read<ValueTuple<Guid, Guid>>(ref source);
        DroidBirth = Primitives.Read<ValueTuple<Guid, Guid>>(ref source);
    }

    protected override int GetBodySize()
    {
        return sizeof(uint)
            + sizeof(uint)
            + 16 // Null terminated string with a constant amount padding.
            + Primitives.BYTE_SIZE_OF_GUID * 2
            + Primitives.BYTE_SIZE_OF_GUID * 2;
    }
}