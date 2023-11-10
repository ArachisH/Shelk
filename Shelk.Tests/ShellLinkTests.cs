using System.IO;
using System.Collections.Generic;

using Xunit;

namespace Shelk.Tests;

public class ShellLinkTests
{
    [Theory]
    public void Initialize_ShouldParseAll()
    {
        string testDataDirectory = Path.Combine("..", "..", "..", "..", "Test Data", "Passing");

        // Arrange
        IEnumerable<string> testDataPaths =
            Directory.EnumerateFiles(testDataDirectory, "*.lnk", SearchOption.AllDirectories);

        // Act
        void InitializeShellLink(string path)
        {
            // Doing it this way allows us to see which specific file threw an exception.
            var exception = Record.Exception(() => new ShellLink(path));
            Assert.Null(exception);
        }

        // Assert
        Assert.All(testDataPaths, InitializeShellLink);
    }

    [Theory]
    public void Initialize_ShouldNotParseAll()
    {
        string testDataDirectory = Path.Combine("..", "..", "..", "..", "Test Data", "Failing");

        // Arrange
        IEnumerable<string> testDataPaths =
            Directory.EnumerateFiles(testDataDirectory, "*.lnk", SearchOption.AllDirectories);

        // Act
        void InitializeShellLink(string path)
        {
            // Doing it this way allows us to see which specific file DID NOT throw an exception.
            var exception = Record.Exception(() => new ShellLink(path));
            Assert.NotNull(exception);
        }

        // Assert
        Assert.All(testDataPaths, InitializeShellLink);
    }
}