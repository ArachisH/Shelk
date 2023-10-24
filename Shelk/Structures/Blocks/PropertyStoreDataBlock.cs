using Shelk.Binary;

namespace Shelk.Structures.Blocks;

/// <summary>
/// Represents a set of properties that can be used by applications to store extra data in the shell link.
/// </summary>
public sealed class PropertyStoreDataBlock : DataBlock
{
    public override uint Signature => PROPERTY_STORE_DATA_SIGNATURE;

    public byte[] Data { get; set; }

    protected override int MustBeGreaterThanOrEqual => 0x0000000C;

    public PropertyStoreDataBlock(ref ReadOnlySpan<byte> source)
        : base(ref source, out int bodySize)
    {
        // TODO: There is another spec sheet available to parse this data even further.
        Data = Primitives.SliceThenAdvance(ref source, bodySize).ToArray();
    }

    protected override int GetBodySize() => Data?.Length ?? 0;
}