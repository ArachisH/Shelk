namespace Shelk.Structures;

/// <summary>
/// Represents 8-bit flags that correspond to modifier keys on the keyboard.
/// </summary>
[Flags]
public enum KeyModifiers : byte
{
    None = 0,
    Shift = 1,
    Control = 2,
    Alt = 4
}