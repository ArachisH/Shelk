using System.Text;
using System.Reflection;
using System.Diagnostics;

using Shelk.Structures;
using Shelk.Structures.Blocks;

namespace Shelk.CLI;

public class Program
{
    private static readonly StringBuilder _dump;

    static Program()
    {
        _dump = new StringBuilder();
    }
    public static void Main(string[] args)
    {
        string? pathOrDirectory = null;
        if (args.Length > 0)
        {
            pathOrDirectory = args[0];
        }

        if (string.IsNullOrWhiteSpace(pathOrDirectory))
        {
            Console.WriteLine("No file path, or directory given...");
            return;
        }

        foreach (string shellLinkPath in GetShortcuts(pathOrDirectory))
        {
            _dump.AppendLine($" --------> {shellLinkPath} <--------");
            var shellLink = new ShellLink(shellLinkPath);

            WriteStructureRootName("SHELL_LINK_HEADER");
            WriteAllProperties(shellLink.Header);

            WriteStructureRootName("LINKTARGET_IDLIST");
            WriteAllProperties(shellLink.Targets);

            WriteStructureRootName("LINKINFO");
            WriteAllProperties(shellLink.Info);

            WriteStructureRootName("STRING_DATA");
            WriteAllProperties(shellLink.Strings);

            WriteStructureRootName("EXTRA_DATA");
            WriteAllProperties(shellLink.Extras);

            _dump.AppendLine();
        }

        string dump = _dump.ToString();

        var dumpDirectory = Directory.CreateDirectory("Shell Link Dumps");
        string dumpFilePath = Path.Combine(dumpDirectory.FullName, $"ShellLinkDump({Path.GetRandomFileName()}).txt");
        File.WriteAllText(dumpFilePath, dump);

        Console.Write(dump);
        Console.WriteLine($"Generated dump file at: \"{dumpFilePath}\"");

        if (Debugger.IsAttached)
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    private static void WriteStructureRootName(string name)
    {
        _dump.AppendLine();
        _dump.AppendLine($"[ {name} ]");
    }
    private static void WritePropertyName(string propertyName, int indentation = 1)
    {
        _dump.AppendLine($"{new string(' ', indentation * 2)}> {propertyName}");
    }
    private static void WriteProperty(string name, object value, int indentation = 1)
    {
        switch (value)
        {
            case string stringValue:
            value = $"\"{stringValue}\"";
            break;

            case IList<byte> byteList:
            value = VisualizeBytes(byteList);
            break;

            case IList<uint> uintList: // ColorTable most likely.
            value = $"{{ {string.Join(", ", uintList)} }}";
            break;

            case IReadOnlyList<DataBlock> dataBlockList:
            WritePropertyName(name, indentation);
            for (int i = 0; i < dataBlockList.Count; i++)
            {
                DataBlock block = dataBlockList[i];
                WritePropertyName($"{block.GetType().Name}[{i}]", indentation + 1);
                WriteProperty("BlockSize", block.GetSize(), indentation + 2);
                WriteProperty("BlockSignature", block.Signature, indentation + 2);
                WriteAllProperties(block, indentation + 2, (propertyName) => propertyName == nameof(DataBlock.Signature));
            }
            return;

            case IReadOnlyList<ItemID> idList:
            WritePropertyName(name, indentation);
            for (int i = 0; i < idList.Count; i++)
            {
                WritePropertyName($"ItemID[{i}]", indentation + 1);
                WriteAllProperties(idList[i], indentation + 2);
            }
            return;

            case IShellLinkObject childShellLinkObject:
            WritePropertyName(name, indentation);
            WriteAllProperties(childShellLinkObject, indentation + 1);
            return;
        }

        _dump.Append($"{new string(' ', indentation * 2)}- {name}");
        if (name.Contains("Offset") || name.Contains("Signature"))
        {
            value = $"0x{value:X8}";
        }
        _dump.AppendLine($": {value ?? "<NULL>"}");
    }

    private static string VisualizeBytes(IList<byte> data)
    {
        var characters = new char[data.Count];
        for (int i = 0; i < data.Count; i++)
        {
            var asciiChar = (char)data[i];
            if (char.IsWhiteSpace(asciiChar) || asciiChar == '\0')
            {
                asciiChar = '.';
            }
            characters[i] = asciiChar;
        }
        return new string(characters);
    }
    private static IEnumerable<string> GetShortcuts(string? pathOrDirectory = null)
    {
        while (string.IsNullOrWhiteSpace(pathOrDirectory))
        {
            Console.Write("Enter a file path, or specify a folder path to enumerate: ");
            pathOrDirectory = Console.ReadLine();
        }

        // If a path/file is dragged directly onto the console window, it'll automatically append quotes to the start and end.
        if (pathOrDirectory[0] == '"' && pathOrDirectory[pathOrDirectory.Length - 1] == '"')
        {
            pathOrDirectory = pathOrDirectory.Substring(1, pathOrDirectory.Length - 2);
        }

        if (Directory.Exists(pathOrDirectory))
        {
            foreach (var filePath in Directory.EnumerateFiles(pathOrDirectory, "*.lnk", SearchOption.AllDirectories))
            {
                yield return filePath;
            }
        }
        else if (File.Exists(pathOrDirectory))
        {
            yield return pathOrDirectory;
        }
    }
    private static void WriteAllProperties(IShellLinkObject? shellLinkObject, int indentation = 1, Predicate<string>? shouldSkipProperty = null)
    {
        if (shellLinkObject == null)
        {
            _dump.AppendLine("<NULL>");
            return;
        }

        Type shellLinkObjectType = shellLinkObject.GetType();
        foreach (PropertyInfo property in shellLinkObjectType.GetProperties())
        {
            if (shouldSkipProperty?.Invoke(property.Name) ?? false) continue;
            if (property.GetMethod?.GetParameters().Length > 0) continue;

            WriteProperty(property.Name, property.GetValue(shellLinkObject) ?? "<NULL>", indentation);
        }
    }
}