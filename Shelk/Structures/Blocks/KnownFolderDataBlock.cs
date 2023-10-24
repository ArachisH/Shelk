using Shelk.Binary;

namespace Shelk.Structures.Blocks;

/// <summary>
/// Represents the location of a known folder.
/// This data can be used when a link target is a known folder to keep track of the folder so that the link target IDList can be translated when the link is loaded.
/// </summary>
public sealed class KnownFolderDataBlock : DataBlock
{
    public override uint Signature => KNOWN_FOLDER_DATA_SIGNATURE;

    /// <summary>
    /// The folder <see cref="Guid"/> ID.
    /// </summary>
    public Guid KnownFolderID { get; }
    /// <summary>
    /// Specifies the location of the <see cref="ItemID"/> of the first child segment of the IDList specified by <see cref="KnownFolderID"/>.
    /// This value is the offset, in bytes, into the link target IDList.
    /// </summary>
    public uint Offset { get; }

    protected override int MustBeGreaterThanOrEqual => 0x0000001C;

    public KnownFolderDataBlock(ref ReadOnlySpan<byte> source)
        : base(ref source)
    {
        KnownFolderID = Primitives.Read<Guid>(ref source);
        Offset = Primitives.Read<uint>(ref source);
    }

    protected override int GetBodySize() => Primitives.BYTE_SIZE_OF_GUID + sizeof(uint);
}