using Shelk.Structures;

namespace Shelk;

/// <summary>
/// Represents a set of objects that make up a Windows Shell Link file, otherwise known as a Windows Shortcut.
/// </summary>
public sealed class ShellLink : IShellLinkObject
{
    /// <summary>
    /// Specifies link information to resolve a target, and other volume specific information.
    /// </summary>
    public LinkInfo? Info { get; }
    /// <summary>
    /// The header of the file, which contains basic information regarding the shell link file itself.
    /// </summary>
    public ShellLinkHeader Header { get; }
    /// <summary>
    /// The target(s) related to the shell link file.
    /// </summary>
    public LinkTargetIDList? Targets { get; }

    /// <summary>
    /// Contains string information like the name, relative path, working directory, command line arguments, and the icon location of the shell link file.
    /// </summary>
    public StringData Strings { get; }

    /// <summary>
    /// The extra data appended to the shell link file.
    /// </summary>
    public ExtraData Extras { get; }

    /// <summary>
    /// Initializes a parsed shell link object from a file path.
    /// </summary>
    /// <param name="path">The file path of the shortcut.</param>
    public ShellLink(string path)
        : this(ReadOrThrow(path))
    { }
    /// <summary>
    /// Initializes a parsed shell link object from a contiguous region of arbitrary memory
    /// </summary>
    /// <param name="source">The region of memory containing the shell link object in byte form.</param>
    public ShellLink(ReadOnlySpan<byte> source)
    {
        Header = new ShellLinkHeader(ref source);
        if (Header.Flags.HasFlag(LinkFlags.HasLinkTargetIDList))
        {
            Targets = new LinkTargetIDList(ref source);
        }
        if (Header.Flags.HasFlag(LinkFlags.HasLinkInfo))
        {
            Info = new LinkInfo(ref source);
        }
        Strings = new StringData(ref source, Header.Flags);
        Extras = new ExtraData(ref source);
    }

    /// <summary>
    /// This method allows us to throw an exception when calling a different constructor, from the initially invoked constructor.
    /// </summary>
    /// <param name="path">The file path of the shortcut.</param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    private static ReadOnlySpan<byte> ReadOrThrow(string path) =>
        File.Exists(path)
        ? File.ReadAllBytes(path)
        : throw new FileNotFoundException("Failed to located the specified shell link file.", path);

    public int GetSize()
    {
        int size = 0;
        size += Header.GetSize();
        if (Header.Flags.HasFlag(LinkFlags.HasLinkTargetIDList))
        {
            if (Targets == null)
            {
                throw new Exception($"Header contains '{nameof(LinkFlags.HasLinkTargetIDList)}', but '{nameof(Targets)}' = null");
            }
            size += Targets.GetSize();
        }
        if (Header.Flags.HasFlag(LinkFlags.HasLinkInfo))
        {
            if (Info == null)
            {
                throw new Exception($"Header contains '{nameof(LinkFlags.HasLinkInfo)}', but '{nameof(Info)}' = null");
            }
            size += Info.GetSize();
        }
        size += Strings.GetSize();
        size += Extras.GetSize();
        return size;
    }
    public string ToString(string? format, IFormatProvider? formatProvider) => throw new NotImplementedException();
}