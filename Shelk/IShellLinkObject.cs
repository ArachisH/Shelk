namespace Shelk;

/*
 * This interface is meant to act as a roadmap for each shell link object that implements it.
 * 
 * The 'int GetSize' function should return the size in bytes that a structure will require for buffering.
 * The 'bool TryFormat' methods are yet to be implemented, but they will allow us to write a structure directly to a char buffer, or a byte buffer.
 * Both of these methods are useful when used in combination, as GetSize() will provide a size to pre-allocate a buffer with a specific size.
 * Alternatively, we may use the size, and pass it to the 'ArrayPool<byte>.Share.Rent(size)' or 'MemoryPool<byte>.Share.Rent(size)' methods to borrow/rent a buffer without causing any new heap allocations during runtime.
 * 
 * The method not seen here is 'IFormattable.ToString(string, IFormatProvider)', this method is meant to return a readable formatted string back to the caller.
 */
public interface IShellLinkObject: IFormattable
{
    int GetSize();
    //bool TryFormat(Span<byte> destination, out int bytesWritten);
    //bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider);
}