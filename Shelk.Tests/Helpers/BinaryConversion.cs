using System;

namespace Shelk.Tests.Helpers;

internal static class BinaryConversion
{
    public static ReadOnlySpan<byte> GetBytes(string hex)
    {
        Span<byte> data = new byte[hex.Length / 2];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return data;
    }
}