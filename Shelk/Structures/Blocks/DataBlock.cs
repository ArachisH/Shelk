using Shelk.Binary;

namespace Shelk.Structures.Blocks;

public abstract class DataBlock : IShellLinkObject
{
    #region Data Block Signatures
    // We could perhaps expose these constants with the public modifier when needed.
    protected const uint ENVIRONMENT_VARIABLE_DATA_SIGNATURE = 0xA0000001;
    protected const uint CONSOLE_DATA_SIGNATURE = 0xA0000002;
    protected const uint TRACKER_DATA_SIGNATURE = 0xA0000003;
    protected const uint CONSOLE_FE_DATA_SIGNATURE = 0xA0000004;
    protected const uint SPECIAL_FOLDER_DATA_SIGNATURE = 0xA0000005;

    protected const uint DARWIN_DATA_SIGNATURE = 0xA0000006;
    protected const uint ICON_ENVIRONMENT_DATA_SIGNATURE = 0xA0000007;
    protected const uint SHIM_DATA_SIGNATURE = 0xA0000008;
    protected const uint PROPERTY_STORE_DATA_SIGNATURE = 0xA0000009;
    protected const uint KNOWN_FOLDER_DATA_SIGNATURE = 0xA000000B;
    protected const uint VISTA_AND_ABOVE_IDLIST_DATA_SIGNATURE = 0xA000000C;
    #endregion

    public abstract uint Signature { get; }

    protected virtual int MustEqual { get; }
    protected virtual int MustBeGreaterThanOrEqual { get; }

    protected DataBlock(ref ReadOnlySpan<byte> source)
        : this(ref source, out _)
    { }
    protected DataBlock(ref ReadOnlySpan<byte> source, out int bodySize)
    {
        uint blockSize = Primitives.Read<uint>(ref source);
        _ = Primitives.Read<uint>(ref source); // Discard the signature field, since we currently don't need it right now.

        // Did neither of these properties get overridden, or were they both overridden? Either way, not good.
        if ((MustEqual == 0 && MustBeGreaterThanOrEqual == 0) || (MustEqual != 0 && MustBeGreaterThanOrEqual != 0))
        {
            throw new NotImplementedException($"Derived types of {nameof(DataBlock)} must override {nameof(MustEqual)} or {nameof(MustBeGreaterThanOrEqual)} but not both.");
        }

        if (MustEqual != 0 && blockSize != MustEqual)
        {
            throw new ArgumentException($"The data block size must equal: 0x{MustEqual:X8};\r\nInstead: 0x{blockSize:X8}");
        }

        if (MustBeGreaterThanOrEqual != 0 && blockSize < MustBeGreaterThanOrEqual)
        {
            throw new ArgumentException($"The data block size must equal or be greater than: 0x{MustBeGreaterThanOrEqual:X8};\r\nInstead: 0x{blockSize:X8}");
        }

        // This out parameter allows present, or future derived types not yet implemented to 'skip' over the source.
        bodySize = (int)blockSize - sizeof(uint) - sizeof(uint);
    }

    protected abstract int GetBodySize();

    public int GetSize()
    {
        return
            sizeof(uint)
            + sizeof(uint)
            + GetBodySize();
    }
    public virtual string ToString(string? format, IFormatProvider? formatProvider) => throw new NotImplementedException();

    public static DataBlock Parse(ref ReadOnlySpan<byte> source, uint blockSignature) => blockSignature switch
    {
        ENVIRONMENT_VARIABLE_DATA_SIGNATURE => new EnvironmentVariableDataBlock(ref source),
        CONSOLE_DATA_SIGNATURE => new ConsoleDataBlock(ref source),
        TRACKER_DATA_SIGNATURE => new TrackerDataBlock(ref source),
        CONSOLE_FE_DATA_SIGNATURE => new ConsoleFEDataBlock(ref source),
        SPECIAL_FOLDER_DATA_SIGNATURE => new SpecialFolderDataBlock(ref source),
        DARWIN_DATA_SIGNATURE => new DarwinDataBlock(ref source),
        ICON_ENVIRONMENT_DATA_SIGNATURE => new IconEnvironmentDataBlock(ref source),
        SHIM_DATA_SIGNATURE => new ShimDataBlock(ref source),
        PROPERTY_STORE_DATA_SIGNATURE => new PropertyStoreDataBlock(ref source),
        KNOWN_FOLDER_DATA_SIGNATURE => new KnownFolderDataBlock(ref source),
        VISTA_AND_ABOVE_IDLIST_DATA_SIGNATURE => new VistaAndAboveIDListDataBlock(ref source),
        _ => throw new NotSupportedException($"The decoded block signature is currently not supported. '0x{blockSignature:X8}'"),
    };
}