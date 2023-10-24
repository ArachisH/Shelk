namespace Shelk.Structures.Blocks;

/// <summary>
/// Represents an alternate IDList that can be used instead of the <see cref="LinkTargetIDList"/> structure(section 2.2) on platforms that support it.
/// </summary>
public sealed class VistaAndAboveIDListDataBlock : DataBlock
{
    public override uint Signature => VISTA_AND_ABOVE_IDLIST_DATA_SIGNATURE;

    public IDList IDs { get; }

    protected override int MustBeGreaterThanOrEqual => 0x0000000A;

    public VistaAndAboveIDListDataBlock(ref ReadOnlySpan<byte> source)
        : base(ref source)
    {
        IDs = new IDList(ref source);
    }

    protected override int GetBodySize() => IDs.GetSize();
}