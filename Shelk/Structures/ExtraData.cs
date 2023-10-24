using System.Collections.ObjectModel;

using Shelk.Binary;
using Shelk.Structures.Blocks;

namespace Shelk.Structures;

/// <summary>
/// Represents a container for a set of structures that convey additional information about a link target.
/// </summary>
public sealed class ExtraData : IShellLinkObject
{
    private const uint TERMINAL_BLOCK = 0x00000004;

    /// <summary>
    /// The read-only collection of data blocks belonging to the target link.
    /// </summary>
    public IReadOnlyList<DataBlock> Blocks { get; }

    public ExtraData(ref ReadOnlySpan<byte> source)
    {
        var blocks = new List<DataBlock>(2);
        while (source.Length > 4) // The last remaining field SHOULD be the TerminalID.
        {
            // This will allow us to have the data block types verify the decoded size field themselves.
            ReadOnlySpan<byte> tempBlockHeaderSpan = source;
            uint blockSize = Primitives.Read<uint>(ref tempBlockHeaderSpan);
            uint blockSignature = Primitives.Read<uint>(ref tempBlockHeaderSpan);

            ReadOnlySpan<byte> blockSpan = Primitives.SliceThenAdvance(ref source, (int)blockSize);
            blocks.Add(DataBlock.Parse(ref blockSpan, blockSignature));
        }

        // Calling ToArray() here trims the list, and returns a non-growing array for the read-only collection to use.
        Blocks = new ReadOnlyCollection<DataBlock>(blocks.ToArray());

        if (Primitives.Read<uint>(ref source) >= TERMINAL_BLOCK)
        {
            throw new ArgumentException($"The decoded TerminalBlock field must be less than {TERMINAL_BLOCK}.", nameof(source));
        }
    }

    public int GetSize()
    {
        int size = 0;
        foreach (DataBlock block in Blocks)
        {
            size += block.GetSize();
        }
        return size;
    }
    public string ToString(string? format, IFormatProvider? formatProvider) => throw new NotImplementedException();
}