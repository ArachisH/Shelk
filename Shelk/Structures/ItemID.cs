using Shelk.Binary;

namespace Shelk.Structures;

/// <summary>
/// The data stored in a given <see cref="ItemID"/> is defined by the source that corresponds to the location in the target namespace of the preceding ItemIDs.
/// This data uniquely identifies the items in that part of the namespace.
/// </summary>
public sealed class ItemID : IShellLinkObject
{
    /// <summary>
    /// The shell data source-defined data that specifies an item.
    /// </summary>
    public byte[]? Data { get; }

    public ItemID(ref ReadOnlySpan<byte> source)
    {
        int dataSize = Primitives.Read<ushort>(ref source);
        if (dataSize > 0)
        {
            dataSize -= sizeof(ushort); // The DataSize field is inclusive of its own size(2 Bytes), so do not include when slicing the data portion.

            ReadOnlySpan<byte> dataSpan = Primitives.SliceThenAdvance(ref source, dataSize);
            Data = dataSpan.ToArray();
        }
    }

    public int GetSize()
    {
        return
            sizeof(ushort)
            + Data?.Length ?? 0;
    }
    public string ToString(string? format, IFormatProvider? formatProvider) => throw new NotImplementedException();
}