using System.Collections;

using Shelk.Binary;

namespace Shelk.Structures;

public sealed class IDList : IShellLinkObject, IReadOnlyList<ItemID>
{
    private readonly ItemID[] _ids;

    public int Count => _ids.Length;
    public ItemID this[int index] => _ids[index];

    public IDList(ref ReadOnlySpan<byte> source)
    {
        if (source.Length < 2)
        {
            throw new ArgumentException("There must be a minimum of two bytes for the TerminalID field.", nameof(source));
        }

        ushort lastPeeked = 0;
        var ids = new List<ItemID>(8);
        while (source.Length >= 2)
        {
            lastPeeked = Primitives.Peek<ushort>(source);
            if (lastPeeked == 0 || lastPeeked > source.Length) break; // TerminalID

            ids.Add(new ItemID(ref source));
        }

        if (lastPeeked != 0) // The last field the while loop should have produced must have been 0(TerminalID), otherwise the data was corrupted.
        {
            throw new ArgumentException("The TerminalID property must be equal to zero.", nameof(source));
        }
        _ids = ids.ToArray();
    }

    IEnumerator IEnumerable.GetEnumerator() => _ids.GetEnumerator();
    public IEnumerator<ItemID> GetEnumerator() => ((IEnumerable<ItemID>)_ids).GetEnumerator();

    public int GetSize()
    {
        int size = 0;
        foreach (ItemID id in _ids)
        {
            size += id.GetSize();
        }
        size += sizeof(ushort);
        return size;
    }
    public string ToString(string? format, IFormatProvider? formatProvider) => throw new NotImplementedException();
}