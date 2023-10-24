using System.Text;

using Shelk.Binary;

namespace Shelk.Structures;

/// <summary>
/// Represents a string value that can convey user interface and path identification information.
/// </summary>
public sealed class StringData : IShellLinkObject
{
    /// <summary>
    /// Determines whether the strings are to be read/written using the Unicode encoding.
    /// </summary>
    public bool IsUnicode { get; set; }

    /// <summary>
    /// The name of the shell link file.
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// The relative path of the shell link file.
    /// </summary>
    public string? RelativePath { get; set; }
    /// <summary>
    /// The working directory of the shell link file.
    /// </summary>
    public string? WorkingDirectory { get; set; }
    /// <summary>
    /// The command line arguments tied to the shell link file.
    /// </summary>
    public string? CommandLineArguments { get; set; }
    /// <summary>
    /// The icon location of the shell link file.
    /// </summary>
    public string? IconLocation { get; set; }

    public StringData()
    { }
    public StringData(ref ReadOnlySpan<byte> source, LinkFlags flags)
    {
        IsUnicode = flags.HasFlag(LinkFlags.IsUnicode);
        if (flags.HasFlag(LinkFlags.HasName))
        {
            Name = ReadString(ref source, IsUnicode);
        }
        if (flags.HasFlag(LinkFlags.HasRelativePath))
        {
            RelativePath = ReadString(ref source, IsUnicode);
        }
        if (flags.HasFlag(LinkFlags.HasWorkingDir))
        {
            WorkingDirectory = ReadString(ref source, IsUnicode);
        }
        if (flags.HasFlag(LinkFlags.HasArguments))
        {
            CommandLineArguments = ReadString(ref source, IsUnicode);
        }
        if (flags.HasFlag(LinkFlags.HasIconLocation))
        {
            IconLocation = ReadString(ref source, IsUnicode);
        }
    }

    private static string ReadString(ref ReadOnlySpan<byte> source, bool isUnicode)
    {
        ushort countCharacters = Primitives.Read<ushort>(ref source);
        return Primitives.ReadEncodedString(ref source, countCharacters, isUnicode);
    }

    public int GetSize()
    {
        Encoding encoding = IsUnicode ? Encoding.Unicode : Encoding.Default;
        return
            (sizeof(ushort) * 5) // The length field indicators for the encoded strings.
            + encoding.GetByteCountNullable(Name)
            + encoding.GetByteCountNullable(RelativePath)
            + encoding.GetByteCountNullable(WorkingDirectory)
            + encoding.GetByteCountNullable(CommandLineArguments)
            + encoding.GetByteCountNullable(IconLocation);
    }
    public string ToString(string? format, IFormatProvider? formatProvider) => throw new NotImplementedException();
}