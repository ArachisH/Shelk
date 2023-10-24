using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Shelk.Binary;

public static class Primitives
{
    internal const int BYTE_SIZE_OF_GUID = 16;
    internal const int BYTE_SIZE_OF_UTCDATETIME = sizeof(long);

    public static T Peek<T>(ReadOnlySpan<byte> source) where T : struct
    {
        int size = Unsafe.SizeOf<T>();
        if (size > source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(source), "Not enough bytes available for reading the specified value type.");
        }

        // This method acts the same as a call to 'BinaryPrimitives.ReadXLittleEndian(source)' would, but we can advance the input/span when doing it through this method.
        return MemoryMarshal.Read<T>(source);
    }
    /// <summary>
    /// Performs a fast read from a <see cref="ReadOnlySpan{Byte}"/> into the desired struct type.
    /// </summary>
    /// <typeparam name="T">The value type to read.</typeparam>
    /// <param name="source">The source from where to read the value.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static T Read<T>(ref ReadOnlySpan<byte> source) where T : struct
    {
        T value = Peek<T>(source);

        int size = Unsafe.SizeOf<T>();
        source = source.Slice(size);

        return value;
    }
    /// <summary>
    /// Reads a <see cref="DateTime"/> value from a <see cref="ReadOnlySpan{T}"/> in UTC format.
    /// </summary>
    /// <param name="source">The source from where to read the value.</param>
    /// <returns></returns>
    public static DateTime ReadUniversalDateTime(ref ReadOnlySpan<byte> source)
    {
        long utcFileTime = Read<long>(ref source);
        return DateTime.FromFileTimeUtc(utcFileTime);
    }
    /// <summary>
    /// Slices a portion of memory of a specified <paramref name="length"/> out of <paramref name="source"/> while advancing the original <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The region of memory to slice, and advance.</param>
    /// <param name="length">The length in bytes to slice and advance the <paramref name="source"/></param>
    /// <returns></returns>
    public static ReadOnlySpan<byte> SliceThenAdvance(ref ReadOnlySpan<byte> source, int length)
    {
        ReadOnlySpan<byte> carved = source.Slice(0, length);
        source = source.Slice(length);
        return carved;
    }

    /// <summary>
    /// Reads a string of a certain length using either the default encoding or Unicode.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="charCount"></param>
    /// <param name="isUnicode"></param>
    /// <returns></returns>
    public static string ReadEncodedString(ref ReadOnlySpan<byte> source, int charCount, bool isUnicode = false)
    {
        /*
         * The below method allows us to decode a string without knowing the length in bytes beforehand.
         * This allows us to decode a Unicode string that could be 1-4 bytes per character.
         * 
         * The alternative is to iterate through a for loop and checking for a pair of zeroes when decoding a Unicode string,
         * but doing it this way means we may encounter a problem where more complex Unicode strings are present.
         */
        Span<char> valueBuffer = new char[charCount]; // DO NOT use stack allocation here, since we need a pointer on the heap for the return value to reference after we exit the scope of the method.
        Decoder decoder = (isUnicode ? Encoding.Unicode : Encoding.Default).GetDecoder();
        unsafe
        {
            fixed (byte* sourcePtr = source)
            fixed (char* valueBufferPtr = valueBuffer)
            {
                decoder.Convert(sourcePtr, source.Length, valueBufferPtr, charCount, true, out int bytesUsed, out _, out _);
                source = source.Slice(bytesUsed);
                return new string(valueBufferPtr);
            }
        }
    }
    /// <summary>
    /// Reads a null-terminated string of a perhaps non-specific size, OR one with padded zeroes using either the default encoding or Unicode.
    /// </summary>
    /// <param name="source">The source from where to read the value.</param>
    /// <param name="minBytesToRead">The minimum amount of bytes this read call should consume, disregarding the fact that the string may have already been fully read before this minimum limit.</param>
    /// <param name="isUnicode">Whether the string is Unicode-encoded.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public static string ReadNullTerminatedString(ref ReadOnlySpan<byte> source, int padding = 0, bool isUnicode = false)
    {
        if (source.Length < 1)
        {
            throw new ArgumentException("There is not enough data to search for a null-terminated string.", nameof(source));
        }

        int nullByteIndex = -1;
        if (isUnicode)
        {
            /*
             * Each character in a Unicode encoded string COULD be 1-4 bytes per character.
             * Although, it's pretty safe to assume a pair of zero bytes marks the end of it, but I could be wrong.. 0x0000 = '\0'
             */
            for (int i = 0; i < source.Length; i += 2)
            {
                if (source[i] != 0) continue;
                if (source[i + 1] != 0) continue;

                nullByteIndex = i;
                break;
            }
        }
        else nullByteIndex = source.IndexOf(byte.MinValue);

        // Unable to find the end of a null terminated string.
        if (nullByteIndex == -1)
        {
            throw new ArgumentException("Unable to locate a null byte/character.", nameof(source));
        }

        string? value = null;
        Encoding encoding = isUnicode ? Encoding.Unicode : Encoding.Default;
        unsafe
        {
            fixed (byte* sourcePtr = source)
            {
                value = encoding.GetString(sourcePtr, nullByteIndex);
            }
        }

        // Advance the source past the padding
        // OR
        // Advance the source past the encoded string, and null byte(s).
        source = source.Slice(padding > 0
            ? padding
            : nullByteIndex + (isUnicode ? 2 : 1));

        return value;
    }
}