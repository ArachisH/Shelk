using Shelk.Binary;

namespace Shelk.Structures.Blocks;

/// <summary>
/// Represents the code page to use for displaying text when a link target specifies an application that is run in a console window.
/// </summary>
public sealed class ConsoleFEDataBlock : DataBlock
{
    public override uint Signature => CONSOLE_FE_DATA_SIGNATURE;

    /// <summary>
    /// Specifies a code page language code identifier.
    /// </summary>
    public uint CodePage { get; }

    protected override int MustEqual => 0x0000000C;

    public ConsoleFEDataBlock(ref ReadOnlySpan<byte> source)
        : base(ref source)
    {
        CodePage = Primitives.Read<uint>(ref source);
    }

    protected override int GetBodySize() => sizeof(uint);
}