using Shelk.Binary;

namespace Shelk.Structures.Blocks;

/// <summary>
/// Specifies a path to environment variable information when the link target refers to a location that has a corresponding environment variable.
/// </summary>
public sealed class EnvironmentVariableDataBlock : DataBlock
{
    public override uint Signature => ENVIRONMENT_VARIABLE_DATA_SIGNATURE;

    /// <summary>
    /// Specifies a path to environment variable information.
    /// </summary>
    public string TargetAnsi { get; }
    /// <summary>
    /// Specifies a Unicode path to environment variable information.
    /// </summary>
    public string TargetUnicode { get; }

    protected override int MustEqual => 0x00000314;

    public EnvironmentVariableDataBlock(ref ReadOnlySpan<byte> source)
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