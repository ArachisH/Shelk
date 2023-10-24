using Shelk.Binary;

namespace Shelk.Structures.Blocks;

/// <summary>
/// Represents an application identifier that can be used instead of a link target IDList to install an application when a shell link is activated.
/// </summary>
public sealed class DarwinDataBlock : DataBlock
{
    public override uint Signature => DARWIN_DATA_SIGNATURE;

    /// <summary>
    /// Specifies an application identifier.
    /// This field SHOULD be ignored.
    /// </summary>
    public string DarwinDataAnsi { get; }
    /// <summary>
    /// Specifies a Unicode application identifier.
    /// </summary>
    public string DarwinDataUnicode { get; }

    protected override int MustEqual => 0x00000314;

    public DarwinDataBlock(ref ReadOnlySpan<byte> source)
        : base(ref source)
    {
        DarwinDataAnsi = Primitives.ReadNullTerminatedString(ref source, 260);
        DarwinDataUnicode = Primitives.ReadNullTerminatedString(ref source, 520, true);
    }

    protected override int GetBodySize()
    {
        // Strings with a constant amount of padding mean they will always take up the same amount of space in bytes.
        return 260 + 520;
    }
}