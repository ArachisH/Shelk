# Shelk
Shelk is a .NET cross-platform library for parsing Microsoft's Shell Link(.LNK) binary file.

## Shell Link Format
|Structure|Description|
| ------- |---------- |
SHELL_LINK_HEADER|"A ShellLinkHeader structure (section 2.1), which contains identification information, timestamps, and flags that specify the presence of optional structures."
LINK_TARGET_IDLIST|"An optional LinkTargetIDList structure (section 2.2), which specifies the target of the link. The presence of this structure is specified by the HasLinkTargetIDList bit (LinkFlags section 2.1.1) in the ShellLinkHeader."
LINKINFO|"An optional LinkInfo structure (section 2.3), which specifies information necessary to resolve the link target. The presence of this structure is specified by the HasLinkInfo bit (LinkFlags section 2.1.1) in the ShellLinkHeader."
STRING_DATA|"Zero or more optional StringData structures (section 2.4), which are used to convey user interface and path identification information. The presence of these structures is specified by bits (LinkFlags section 2.1.1) in the ShellLinkHeader."
EXTRA_DATA|"Zero or more ExtraData structures (section 2.5)."

## Design
All non-primitive types should inherit from **IShellLinkObject**, so that we may retrieve the size in bytes of the object for buffering scenarios.
#### Example #1
```cs
// In this scenario we are serializing 'shellLinkObject' into a file named 'MyShortcut.lnk'.
int size = shellLinkObject.GetSize();
byte[] buffer = ArrayPool<byte>.Shared.Rent(size); // Rent a buffer from the shared buffer pool that can hold at least this many bytes.
if (shellLinkObject.TryFormat(buffer, out int bytesWritten))
{
    using (var fs = new FileStream("MyShortcut.lnk", FileMode.Create))
    {
        fs.Write(buffer, 0, size);
    }
}
// Return the shared buffer back to the pool.
ArrayPool<byte>.Shared.Return(buffer);
```
#### Example #2 (TODO: TryFormat not yet implemented)
```cs
Socket client = new();
int size = shellLinkObject.GetSize();
using (IMemoryOwner<byte> bufferOwner = MemoryPool<byte>.Shared.Rent(size))
if (shellLinkObject.TryFormat(bufferOwner.Memory.Span, out int bytesWritten))
{
    int bytesSent = await client.SendAsync(bufferOwner.Memory.Slice(0, bytesWritten));
}
```

### Parsing
All the parsing logic is located in the constructor of the type being parsed, but perhaps it may be preferable to move this logic into a static Parse interface method for each type that implements the **IShellLinkObject** interface. Aside from **ShellLink**, all other types that inherit from **IShellLinkObject** have a constructor with this signature:
```csharp
public _(ref ReadOnlySpan<byte> source)
```
Using this method we're able to *advance* the *stream* by doing something like:
```csharp
source = source.Slice(4);
```
It allows us to read primitive values from structure to structure without having to allocate a smaller buffer to pass to another structure for parsing. With the help of the internal static class **Primitives**, we're able to read a value from the buffer, and advance it at the same time, like so:
```csharp
  Size = Primitives.Read<uint>(ref source);
  HeaderSize = Primitives.Read<uint>(ref source);
  Flags = Primitives.Read<LinkInfoFlags>(ref source);
  VolumeIDOffset = Primitives.Read<uint>(ref source);
  LocalBasePathOffset = Primitives.Read<uint>(ref source);
  CommonNetworkRelativeLinkOffset = Primitives.Read<uint>(ref source);
  CommonPathSuffixOffset = Primitives.Read<uint>(ref source);
```
## Usage
To parse a Shell Link file we can either pass a byte array, or a path to the file:
```csharp
var shellLink = new ShellLink("MyShortcut.lnk");
```
```csharp
byte[] shellLinkBytes = File.ReadAllBytes("MyShortcut.lnk");
var shellLink = new ShellLink(shellLinkBytes);
```

Once initialized we can access the properties contained within the file like so:
```csharp
Console.WriteLine("Attributes: " + shellLink.Header.FileAttributes);
Console.WriteLine("Creation Time: " + shellLink.Header.CreationTime);
Console.WriteLine("Write Time: " + shellLink.Header.WriteTime);
Console.WriteLine("Access Time: " + shellLink.Header.AccessTime);

Console.WriteLine("Name: " + shellLink.Strings.Name);
Console.WriteLine("Relative Path: " + shellLink.Strings.RelativePath);
Console.WriteLine("WorkingDirectory: " + shellLink.Strings.WorkingDirectory);
Console.WriteLine("Command Line Arguments: " + shellLink.Strings.CommandLineArguments);
Console.WriteLine("IconLocation: " + shellLink.Strings.IconLocation);
```
Will produce something similar to:
```
Attributes: None
Creation Time: 1/1/1601 12:00:00 AM
Write Time: 1/1/1601 12:00:00 AM
Access Time: 1/1/1601 12:00:00 AM
Name: @%ProgramFiles%\Hyper-V\SnapInAbout.dll,-132
Relative Path:
WorkingDirectory: %ProgramFiles%\Hyper-V\
Command Line Arguments: "%windir%\System32\virtmgmt.msc"
IconLocation: %ProgramFiles%\Hyper-V\SnapInAbout.dll
```
### Demo
This project also contains a command line interface that allows you to pass a file path of a \*.lnk file, or a directory containing multiple of them.
```
Shelk.CLI.exe "C:\MyShortcut.lnk"
```

At the end of execution it will have generated a dump file in the same folder the application was ran from within a folder called **Shell Link Dumps**.

## Building & Running
To build the project open the command line in the location of the solution(Shelk.sln).
```csharp
dotnet build Shelk.sln
```
To run the demo application:
```csharp
dotnet run --project Shelk.CLI
```
To run all unit tests:
```csharp
dotnet test
```