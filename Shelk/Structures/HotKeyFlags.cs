namespace Shelk.Structures;

/// <summary>
/// Represents a combination of keyboard keys being pressed.
/// </summary>
public readonly struct HotKeyFlags : IShellLinkObject, IEquatable<HotKeyFlags>
{
    /// <summary>
    /// Represents a <see cref="HotKeyFlags"/> with no keyboard combinations.
    /// </summary>
    public static HotKeyFlags Empty = new(VirtualKeyCode.None);

    public VirtualKeyCode Key { get; }
    public KeyModifiers Modifiers { get; }

    public static bool operator !=(HotKeyFlags left, HotKeyFlags right) => !(left == right);
    public static bool operator ==(HotKeyFlags left, HotKeyFlags right) => left.Equals(right);

    public HotKeyFlags(VirtualKeyCode key)
        : this(key, KeyModifiers.None)
    { }
    public HotKeyFlags(VirtualKeyCode key, KeyModifiers modifiers)
    {
        Key = key;
        Modifiers = modifiers;
    }

    public bool Equals(HotKeyFlags other)
    {
        return other.Key == Key && other.Modifiers == Modifiers;
    }
    public override int GetHashCode() => (Key, Modifiers).GetHashCode();
    public override bool Equals(object? obj) => obj is HotKeyFlags flags && Equals(flags);

    public int GetSize() => sizeof(VirtualKeyCode) + sizeof(KeyModifiers);
    public string ToString(string? format, IFormatProvider? formatProvider) => $"{{Key: {Key}, Modifiers: {Modifiers}}}";
}