using Shelk.Structures;

using System;
using System.Collections.Generic;

using Xunit;

namespace Shelk.Tests.Structures;

public class StringDataTests
{
    [Theory]
    [MemberData(nameof(GetPassingInitializeCases))]
    public void Initialize_ShouldEqual(string base64, StringData expected, LinkFlags expectedFlags)
    {
        // Arrange
        ReadOnlySpan<byte> stringDataSpan = Convert.FromBase64String(base64);

        // Act
        var actual = new StringData(ref stringDataSpan, expectedFlags);

        // Assert
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.RelativePath, actual.RelativePath);
        Assert.Equal(expected.WorkingDirectory, actual.WorkingDirectory);
        Assert.Equal(expected.CommandLineArguments, actual.CommandLineArguments);
        Assert.Equal(expected.IconLocation, actual.IconLocation);
    }

    public static IEnumerable<object[]> GetPassingInitializeCases()
    {
        yield return new object[3]
        {
            "KgBAACUAUwB5AHMAdABlAG0AUgBvAG8AdAAlAFwAcwB5AHMAdABlAG0AMwAyAFwAeABwAHMAcAAyAHIAZQBzAC4AZABsAGwALAAtADEANgAyADAAMgAvAC4ALgBcAC4ALgBcAC4ALgBcAC4ALgBcAC4ALgBcAC4ALgBcAFcASQBOAEQATwBXAFMAXABzAHkAcwB0AGUAbQAzADIAXAByAHUAbgBkAGwAbAAzADIALgBlAHgAZQAVACUASABPAE0ARQBEAFIASQBWAEUAJQAlAEgATwBNAEUAUABBAFQASAAlAC8AcwBoAGUAbABsADMAMgAuAGQAbABsACwAQwBvAG4AdAByAG8AbABfAFIAdQBuAEQATABMACAATgBlAHQAUwBlAHQAdQBwAC4AYwBwAGwALABAADAALABXAE4AUwBXACIAJQBTAHkAcwB0AGUAbQBSAG8AbwB0ACUAXABzAHkAcwB0AGUAbQAzADIAXAB4AHAAcwBwADIAcgBlAHMALgBkAGwAbAA=",
            new StringData
            {
                Name = @"@%SystemRoot%\system32\xpsp2res.dll,-16202",
                RelativePath = @"..\..\..\..\..\..\WINDOWS\system32\rundll32.exe",
                WorkingDirectory = @"%HOMEDRIVE%%HOMEPATH%",
                CommandLineArguments = "shell32.dll,Control_RunDLL NetSetup.cpl,@0,WNSW",
                IconLocation = @"%SystemRoot%\system32\xpsp2res.dll"
            },
            (LinkFlags)767
        };

        yield return new object[3]
        {
            "LQBAACUAUwB5AHMAdABlAG0AUgBvAG8AdAAlAFwAcwB5AHMAdABlAG0AMwAyAFwAbQBpAGcAdQBpAHIAZQBzAG8AdQByAGMAZQAuAGQAbABsACwALQAyADAAMgAyAC4ALgBcAC4ALgBcAC4ALgBcAC4ALgBcAC4ALgBcAC4ALgBcAC4ALgBcAFcAaQBuAGQAbwB3AHMAXABTAHkAcwB0AGUAbQAzADIAXAB0AGEAcwBrAHMAYwBoAGQALgBtAHMAYwACAC8AcwAnACUAUwB5AHMAdABlAG0AUgBvAG8AdAAlAFwAcwB5AHMAdABlAG0AMwAyAFwAbQBpAGcAdQBpAHIAZQBzAG8AdQByAGMAZQAuAGQAbABsAA==",
            new StringData
            {
                Name = @"@%SystemRoot%\system32\miguiresource.dll,-202",
                RelativePath = @"..\..\..\..\..\..\..\Windows\System32\taskschd.msc",
                WorkingDirectory = null,
                CommandLineArguments = "/s",
                IconLocation = @"%SystemRoot%\system32\miguiresource.dll"
            },
            (LinkFlags)751
        };

        yield return new object[3]
        {
            "GwBAACUAdwBpAG4AZABpAHIAJQBcAGUAeABwAGwAbwByAGUAcgAuAGUAeABlACwALQAzADAANAAiACUAUwB5AHMAdABlAG0AUgBvAG8AdAAlAFwAcwB5AHMAdABlAG0AMwAyAFwAaQBtAGEAZwBlAHIAZQBzAC4AZABsAGwA",
            new StringData
            {
                Name = @"@%windir%\explorer.exe,-304",
                RelativePath = null,
                WorkingDirectory = null,
                CommandLineArguments = null,
                IconLocation = @"%SystemRoot%\system32\imageres.dll"
            },
            (LinkFlags)197
        };

        yield return new object[3]
        {
            "LABAACUAUwB5AHMAdABlAG0AUgBvAG8AdAAlAFwAcwB5AHMAdABlAG0AMwAyAFwAbwBvAGIAZQBcAG0AcwBvAG8AYgBlAC4AZQB4AGUALAAtADIAMAAwADEAKQAuAC4AXAAuAC4AXAAuAC4AXABXAEkATgBEAE8AVwBTAFwAcwB5AHMAdABlAG0AMwAyAFwAbwBvAGIAZQBcAG0AcwBvAG8AYgBlAC4AZQB4AGUAFQAlAEgATwBNAEUARABSAEkAVgBFACUAJQBIAE8ATQBFAFAAQQBUAEgAJQACAC8AQQA=",
            new StringData
            {
                Name = @"@%SystemRoot%\system32\oobe\msoobe.exe,-2001",
                RelativePath = @"..\..\..\WINDOWS\system32\oobe\msoobe.exe",
                WorkingDirectory = "%HOMEDRIVE%%HOMEPATH%",
                CommandLineArguments = "/A",
                IconLocation = null
            },
            (LinkFlags)703
        };
    }
}