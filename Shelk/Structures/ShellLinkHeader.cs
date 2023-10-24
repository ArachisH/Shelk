using Shelk.Binary;

namespace Shelk.Structures;

/// <summary>
/// Represents identification information, timestamps, and flags that specify the presence of optional structures, including <see cref="LinkTargetIDList"/>(section 2.2), <see cref="LinkInfo"/>(section 2.3), and string data(section 2.4).
/// </summary>
public sealed class ShellLinkHeader : IShellLinkObject, IEquatable<ShellLinkHeader>
{
    /// <summary>
    /// Represents the minimum, and maximum size of this object. (76 Bytes)
    /// </summary>
    public const int ABSOLUTE_HEADER_SIZE = 0x0000004C;
    public static readonly Guid ABSOLUTE_CLSID = new("00021401-0000-0000-C000-000000000046");

    /// <summary>
    /// The size, in bytes, of this structure. This value MUST be 0x0000004C
    /// </summary>
    public int HeaderSize { get; }

    /// <summary>
    /// A class identifier (CLSID). This value MUST be 00021401-0000-0000-C000-000000000046
    /// </summary>
    public Guid CLSID { get; }

    /// <summary>
    /// Defines which shell link structures are present in the file format after the <see cref="ShellLinkHeader"/> structure.
    /// </summary>
    public LinkFlags Flags { get; set; }

    /// <summary>
    /// Defines the file attributes of the link target, if the target is a file system item.
    /// File attributes can be used if the link target is not available, or if accessing the target would be inefficient. It is possible for the target items attributes to be out of sync with this value.
    /// </summary>
    public FileAttributesFlags FileAttributes { get; set; }

    /// <summary>
    /// Specifies the creation time of the link target in UTC(Coordinated Universal Time).
    /// If the value is zero, there is no creation time set on the link target.
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// Specifies the access time of the link target in UTC(Coordinated Universal Time).
    /// If the value is zero, there is no access time set on the link target.
    /// </summary>
    public DateTime AccessTime { get; set; }

    /// <summary>
    /// Specifies the write time of the link target in UTC(Coordinated Universal Time).
    /// If the value is zero, there is no write time set on the link target.
    /// </summary>
    public DateTime WriteTime { get; set; }

    /// <summary>
    /// Specifies the size, in bytes, of the link target.
    /// If the link target file is larger than 0xFFFFFFFF, this value specifies the least significant 32 bits of the link target file size.
    /// </summary>
    public uint FileSize { get; set; }

    /// <summary>
    /// Specifies the index of an icon within a given icon location.
    /// </summary>
    public int IconIndex { get; set; }

    /// <summary>
    /// Specifies the expected window state of an application launched by the link.
    /// This value SHOULD be one of the following.
    /// </summary>
    public ShowCommand Command { get; set; }

    /// <summary>
    /// Specifies the keystrokes used to launch the application referenced by the shortcut key.
    /// This value is assigned to the application after it is launched, so that pressing the key activates that application.
    /// </summary>
    public HotKeyFlags HotKey { get; set; }

    public ShellLinkHeader()
    {
        CLSID = ABSOLUTE_CLSID;
        HeaderSize = ABSOLUTE_HEADER_SIZE;
    }
    public ShellLinkHeader(ref ReadOnlySpan<byte> source)
    {
        if (source.Length < ABSOLUTE_HEADER_SIZE)
        {
            throw new ArgumentException($"The minimum input size must be greater than or equal to '0x{ABSOLUTE_HEADER_SIZE:X8}'.", nameof(source));
        }

        HeaderSize = Primitives.Read<int>(ref source);
        if (HeaderSize != ABSOLUTE_HEADER_SIZE)
        {
            throw new ArgumentException($"The decoded field '{nameof(HeaderSize)}' must equal '0x{ABSOLUTE_HEADER_SIZE:X8}' ({ABSOLUTE_HEADER_SIZE}).", nameof(source));
        }

        CLSID = Primitives.Read<Guid>(ref source);
        if (CLSID != ABSOLUTE_CLSID)
        {
            throw new ArgumentException($"The decoded field '{nameof(CLSID)}' must equal '{ABSOLUTE_CLSID}'.", nameof(source));
        }

        Flags = Primitives.Read<LinkFlags>(ref source);
        FileAttributes = Primitives.Read<FileAttributesFlags>(ref source);
        CreationTime = Primitives.ReadUniversalDateTime(ref source);
        AccessTime = Primitives.ReadUniversalDateTime(ref source);
        WriteTime = Primitives.ReadUniversalDateTime(ref source);
        FileSize = Primitives.Read<uint>(ref source);
        IconIndex = Primitives.Read<int>(ref source);
        Command = Primitives.Read<ShowCommand>(ref source);
        HotKey = Primitives.Read<HotKeyFlags>(ref source);

        if (Primitives.Read<short>(ref source) != 0 ||
            Primitives.Read<int>(ref source) != 0 ||
            Primitives.Read<int>(ref source) != 0)
        {
            throw new ArgumentException("This is a reserved region in the file, and must all equal zero.", nameof(source));
        }
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(HeaderSize);
        hash.Add(CLSID);
        hash.Add(Flags);
        hash.Add(FileAttributes);
        hash.Add(CreationTime);
        hash.Add(AccessTime);
        hash.Add(WriteTime);
        hash.Add(FileSize);
        hash.Add(IconIndex);
        hash.Add(Command);
        hash.Add(HotKey);
        return hash.ToHashCode();
    }

    public bool Equals(ShellLinkHeader? other)
    {
        if (other == null) return false;
        if (other.Flags != Flags) return false;
        if (other.FileAttributes != FileAttributes) return false;
        if (other.CreationTime != CreationTime) return false;
        if (other.AccessTime != AccessTime) return false;
        if (other.WriteTime != WriteTime) return false;
        if (other.FileSize != FileSize) return false;
        if (other.IconIndex != IconIndex) return false;
        if (other.Command != Command) return false;
        return other.HotKey == HotKey;
    }
    public override bool Equals(object? obj) => Equals(obj as ShellLinkHeader);

    public int GetSize()
    {
        return
            sizeof(int)
            + Primitives.BYTE_SIZE_OF_GUID
            + sizeof(LinkFlags)
            + sizeof(FileAttributesFlags)
            + Primitives.BYTE_SIZE_OF_UTCDATETIME
            + Primitives.BYTE_SIZE_OF_UTCDATETIME
            + Primitives.BYTE_SIZE_OF_UTCDATETIME
            + sizeof(uint)
            + sizeof(int)
            + sizeof(ShowCommand)
            + HotKey.GetSize()
            + sizeof(short)
            + sizeof(int)
            + sizeof(int);
    }
    public string ToString(string? format, IFormatProvider? formatProvider) => throw new NotImplementedException();
}