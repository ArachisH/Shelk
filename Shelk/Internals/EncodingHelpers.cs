using System.Text;

namespace Shelk;

internal static class EncodingHelpers
{
    public static int GetByteCountNullable(this Encoding encoding, ReadOnlySpan<char> chars) => encoding.GetByteCount(chars);
}