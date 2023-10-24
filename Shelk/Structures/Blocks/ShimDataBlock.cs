using System.Text;

using Shelk.Binary;

namespace Shelk.Structures.Blocks;

/// <summary>
///  Represents the name of a shim that can be applied when activating a link target.
/// </summary>
public sealed class ShimDataBlock : DataBlock
{
    public override uint Signature => SHIM_DATA_SIGNATURE;

    public string LayerName { get; set; }

    protected override int MustBeGreaterThanOrEqual => 0x00000088;

    public ShimDataBlock(ref ReadOnlySpan<byte> source)
        : base(ref source, out int bodySize)
    {
        LayerName = Primitives.ReadEncodedString(ref source, bodySize / 2, true);
    }

    protected override int GetBodySize() => Encoding.Unicode.GetByteCountNullable(LayerName) + 2;
}