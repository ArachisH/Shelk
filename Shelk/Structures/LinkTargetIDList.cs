using Shelk.Binary;

namespace Shelk.Structures;

/// <summary>
/// Represents the target of the link.
/// </summary>
public sealed class LinkTargetIDList : IShellLinkObject
{
    /// <summary>
    /// Specifies a read-only collection of type <see cref="ItemID"/> pertaining to the target link.
    /// </summary>
    public IDList IDs { get; }

    public LinkTargetIDList(ref ReadOnlySpan<byte> source)
    {
        int idListSize = Primitives.Read<ushort>(ref source);
        ReadOnlySpan<byte> idListSpan = Primitives.SliceThenAdvance(ref source, idListSize);

        // Just passing the relevant portion of memory makes it easier for sub-types to parse.
        IDs = new IDList(ref idListSpan);
    }

    public int GetSize()
    {
        return
            sizeof(ushort)
            + IDs.GetSize();
    }
    public string ToString(string? format, IFormatProvider? formatProvider) => throw new NotImplementedException();
}