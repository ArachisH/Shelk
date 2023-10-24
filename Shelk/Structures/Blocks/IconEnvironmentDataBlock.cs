using Shelk.Binary;

namespace Shelk.Structures.Blocks;

/// <summary>
/// Represents the paths to an icon.
/// The path is encoded using environment variables, which makes it possible to find the icon across machines where the locations vary but are expressed using environment variables.
/// </summary>
public sealed class IconEnvironmentDataBlock : DataBlock
{
    public override uint Signature => ICON_ENVIRONMENT_DATA_SIGNATURE;

    /// <summary>
    /// Specifies a path that is constructed with environment variables
    /// </summary>
    public string TargetAnsi { get; }
    /// <summary>
    /// Specifies a Unicode path that is constructed with environment variables
    /// </summary>
    public string TargetUnicode { get; }

    protected override int MustEqual => 0x00000314;

    public IconEnvironmentDataBlock(ref ReadOnlySpan<byte> source)
        : base(ref source)
    {
        TargetAnsi = Primitives.ReadNullTerminatedString(ref source, 260);
        TargetUnicode = Primitives.ReadNullTerminatedString(ref source, 520, true);
    }

    protected override int GetBodySize()
    {
        // Strings with a constant amount of padding mean they will always take up the same amount of space in bytes.
        return 260 + 520;
    }
}