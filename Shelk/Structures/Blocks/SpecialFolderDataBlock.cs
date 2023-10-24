using Shelk.Binary;

namespace Shelk.Structures.Blocks;

/// <summary>
/// Represents the location of a special folder.
/// This data can be used when a link target is a special folder to keep track of the folder, so that the link target IDList can be translated when the link is loaded.
/// </summary>
public sealed class SpecialFolderDataBlock : DataBlock
{
    public override uint Signature => SPECIAL_FOLDER_DATA_SIGNATURE;

    /// <summary>
    /// The folder integer ID.
    /// </summary>
    public uint SpecialFolderID { get; }
    /// <summary>
    /// Specifies the location of the <see cref="ItemID"/> of the first child segment of the IDList specified by <see cref="SpecialFolderID"/>.
    /// This value is the offset, in bytes, into the link target IDList.
    /// </summary>
    public uint Offset { get; }

    protected override int MustEqual => 0x00000010;

    public SpecialFolderDataBlock(ref ReadOnlySpan<byte> source)
        : base(ref source)
    {
        SpecialFolderID = Primitives.Read<uint>(ref source);
        Offset = Primitives.Read<uint>(ref source);
    }

    protected override int GetBodySize() => sizeof(uint) + sizeof(uint);
}