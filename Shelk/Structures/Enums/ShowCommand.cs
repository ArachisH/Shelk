namespace Shelk.Structures;

/// <summary>
/// Represents the expected window state of an application launched by the link.
/// </summary>
public enum ShowCommand : uint
{
    /// <summary>
    /// The application is open and its window is open in a normal fashion.
    /// </summary>
    Normal = 0x00000001,
    /// <summary>
    /// The application is open, and keyboard focus is given to the application, but its window is not shown.
    /// </summary>
    Maximized = 0x00000003,
    /// <summary>
    /// The application is open, but its window is not shown.
    /// It is not given the keyboard focus.
    /// </summary>
    MinNoActive = 0x00000007
}