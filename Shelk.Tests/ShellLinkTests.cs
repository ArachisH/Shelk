using System.IO;
using System.Collections.Generic;

using Xunit;

namespace Shelk.Tests;

public class ShellLinkTests
{
    [Theory]
    [InlineData(@"..\..\..\..\Test Data\Passing", "*.lnk")]
    public void Initialize_ShouldParseAll(string testDataDirectory, string searchPattern)
    {
        // Arrange
        IEnumerable<string> testDataPaths =
            Directory.EnumerateFiles(testDataDirectory, searchPattern, SearchOption.AllDirectories);

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
    [InlineData(@"..\..\..\..\Test Data\Failing", "*.lnk")]
    public void Initialize_ShouldNotParseAll(string testDataDirectory, string searchPattern)
    {
        // Arrange
        IEnumerable<string> testDataPaths =
            Directory.EnumerateFiles(testDataDirectory, searchPattern, SearchOption.AllDirectories);

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