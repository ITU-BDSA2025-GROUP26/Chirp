using System;
using System.Globalization;
using Xunit;
using Chirp.CLI;

namespace Chirp.CLI.Tests;

public class UnitTest1
{
    [Fact]
    public void PrettyMethodTest()
    {
        var cheep = new Cheep("Karl", "Hej!", 1757500427);

        var formatted = Formatting.Pretty(cheep);

        Assert.Contains("Karl", formatted);
        Assert.Contains("Hej!", formatted);
                
        var local = DateTimeOffset.FromUnixTimeSeconds(1757500427).ToLocalTime().DateTime;
        var expectedStamp = local.ToString("dd/MM/yy HH:mm:ss", CultureInfo.InvariantCulture);
        Assert.Contains(expectedStamp, formatted);
    }
}
