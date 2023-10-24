using System.Runtime.InteropServices;

using Shelk.Binary;

namespace Shelk.Structures.Blocks;

/// <summary>
/// Represents the display settings to use when a link target specifies an application that is run in a console window.
/// </summary>
public sealed class ConsoleDataBlock : DataBlock
{
    public override uint Signature => CONSOLE_DATA_SIGNATURE;

    /// <summary>
    /// Specifies the fill attributes that control the foreground and background text colors in the console window.
    /// </summary>
    public ushort FillAttributes { get; }
    /// <summary>
    /// Specifies the fill attributes that control the foreground and background text color in the console window popup.
    /// The values are the same as for the <see cref="FillAttributes"/> field.
    /// </summary>
    public ushort PopupFillAttributes { get; }
    /// <summary>
    /// Specifies the horizontal size (X axis), in characters, of the console window buffer.
    /// </summary>
    public ushort ScreenBufferSizeX { get; }
    /// <summary>
    /// Specifies the vertical size (Y axis), in characters, of the console window buffer
    /// </summary>
    public ushort ScreenBufferSizeY { get; }
    /// <summary>
    /// Specifies the horizontal size (X axis), in characters, of the console window
    /// </summary>
    public ushort WindowSizeX { get; }
    /// <summary>
    /// Specifies the vertical size (Y axis), in characters, of the console window.
    /// </summary>
    public ushort WindowSizeY { get; }
    /// <summary>
    /// Specifies the horizontal coordinate (X axis), in pixels, of the console window origin.
    /// </summary>
    public ushort WindowOriginX { get; }
    /// <summary>
    /// Specifies the vertical coordinate (Y axis), in pixels, of the console window origin
    /// </summary>
    public ushort WindowOriginY { get; }

    /// <summary>
    /// Specifies the size, in pixels, of the font used in the console window.The two most significant bytes contain the font height and the two least significant bytes contain the font width.
    /// For vector fonts, the width is set to zero.
    /// </summary>
    public uint FontSize { get; }
    /// <summary>
    /// Specifies the family of the font used in the console window.This value MUST be comprised of a font family and a font pitch.
    /// </summary>
    public uint FontFamily { get; }
    /// <summary>
    /// Specifies the stroke weight of the font used in the console window.
    /// </summary>
    public uint FontWeight { get; }

    /// <summary>
    /// Specifies the face name in Unicode of the font used in the console window
    /// </summary>
    public string FaceName { get; }

    /// <summary>
    /// Specifies the size of the cursor, in pixels, used in the console window.
    /// </summary>
    public uint CursorSize { get; }
    /// <summary>
    /// Specifies whether to open the console window in full-screen mode.
    /// </summary>
    public uint FullScreen { get; }
    /// <summary>
    /// Specifies whether to open the console window in QuikEdit mode.
    /// In QuickEdit mode, the mouse can be used to cut, copy, and paste text in the console window.
    /// </summary>
    public uint QuickEdit { get; }
    /// <summary>
    /// Specifies insert mode in the console window.
    /// </summary>
    public uint InsertMode { get; }
    /// <summary>
    /// Specifies auto-position mode of the console window.
    /// </summary>
    public uint AutoPosition { get; }
    /// <summary>
    /// Specifies the size, in characters, of the buffer that is used to store a history of user input into the console window.
    /// </summary>
    public uint HistoryBufferSize { get; }
    /// <summary>
    /// Specifies the number of history buffers to use.
    /// </summary>
    public uint NumberOfHistoryBuffers { get; }
    /// <summary>
    /// Specifies whether to remove duplicates in the history buffer
    /// </summary>
    public uint HistoryNoDup { get; }
    /// <summary>
    /// Specifying the RGB colors that are used for text in the console window.
    /// The values of the fill attribute fields <see cref="FillAttributes"/> and <see cref="PopupFillAttributes"/> are used as indexes into this table to specify the final foreground and background color for a character.
    /// </summary>
    public uint[] ColorTable { get; }

    protected override int MustEqual => 0x000000CC;

    public ConsoleDataBlock(ref ReadOnlySpan<byte> source)
        : base(ref source)
    {
        FillAttributes = Primitives.Read<ushort>(ref source);
        PopupFillAttributes = Primitives.Read<ushort>(ref source);
        ScreenBufferSizeX = Primitives.Read<ushort>(ref source);
        ScreenBufferSizeY = Primitives.Read<ushort>(ref source);
        WindowSizeX = Primitives.Read<ushort>(ref source);
        WindowSizeY = Primitives.Read<ushort>(ref source);
        WindowOriginX = Primitives.Read<ushort>(ref source);
        WindowOriginY = Primitives.Read<ushort>(ref source);

        // Skip unused fields.
        Primitives.Read<uint>(ref source);
        Primitives.Read<uint>(ref source);

        FontSize = Primitives.Read<uint>(ref source);
        FontFamily = Primitives.Read<uint>(ref source);
        FontWeight = Primitives.Read<uint>(ref source);

        FaceName = Primitives.ReadNullTerminatedString(ref source, 64, true);

        CursorSize = Primitives.Read<uint>(ref source);
        FullScreen = Primitives.Read<uint>(ref source);
        QuickEdit = Primitives.Read<uint>(ref source);
        InsertMode = Primitives.Read<uint>(ref source);
        AutoPosition = Primitives.Read<uint>(ref source);
        HistoryBufferSize = Primitives.Read<uint>(ref source);
        NumberOfHistoryBuffers = Primitives.Read<uint>(ref source);
        HistoryNoDup = Primitives.Read<uint>(ref source);

        // Convert the last portion of bytes into an array of uint value to represent the color table field.
        ReadOnlySpan<byte> colorTableSpan = Primitives.SliceThenAdvance(ref source, sizeof(uint) * 16);
        ColorTable = MemoryMarshal.Cast<byte, uint>(colorTableSpan).ToArray();
    }

    protected override int GetBodySize()
    {
        // This may seem wild, but the idea is to show others the primitive structure of this type.
        // The compiler will also shorten this into about 1-3 IL instructions.
        return
            sizeof(ushort)
            + sizeof(ushort)
            + sizeof(ushort)
            + sizeof(ushort)
            + sizeof(ushort)
            + sizeof(ushort)
            + sizeof(ushort)
            + sizeof(ushort)
            + sizeof(uint) // Unused field.
            + sizeof(uint) // Unused field.
            + sizeof(uint)
            + sizeof(uint)
            + sizeof(uint)
            + sizeof(byte) * 64 // This null terminated Unicode string will ALWAYS use up this amount of space in bytes due to padding.
            + sizeof(uint)
            + sizeof(uint)
            + sizeof(uint)
            + sizeof(uint)
            + sizeof(uint)
            + sizeof(uint)
            + sizeof(uint)
            + sizeof(uint)
            + sizeof(uint) * ColorTable.Length; // An array of 16 uint values.
    }
}