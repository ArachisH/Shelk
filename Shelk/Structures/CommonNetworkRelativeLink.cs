namespace Shelk.Structures;

/// <summary>
/// Represents information about the network location where a link target is stored, including the mapped drive letter and the UNC path prefix.
/// </summary>
public sealed class CommonNetworkRelativeLink : IShellLinkObject
{
    public CommonNetworkRelativeLink(ref ReadOnlySpan<byte> source)
    {
        throw new NotImplementedException();
    }

    public int GetSize() => throw new NotImplementedException();
    public string ToString(string? format, IFormatProvider? formatProvider) => throw new NotImplementedException();
}