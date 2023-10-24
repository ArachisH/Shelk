namespace Shelk.Structures;

/// <summary>
/// Represents a 32-bit unsigned integer that specifies the type of drive the link target is stored on.
/// </summary>
public enum DriveType : uint
{
    Unknown = 0,
    NoRootDir = 1,
    Removable = 2,
    Fixed = 3,
    Remote = 4,
    CDROM = 5,
    RamDisk = 6
}