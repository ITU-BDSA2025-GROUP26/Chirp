using System;
using System.Globalization;
using Xunit;
using Chirp.CLI;

namespace Chirp.CLI.Tests;

public class FormattingTests
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
    
    [Fact]
    public void PrettyMethod_AllowsSpecialCharacters()
    {
        var cheep = new Cheep("Alice", "Hello, world! 😊 #hashtag", 1757500427);

        var formatted = Formatting.Pretty(cheep);

        Assert.Contains("Alice", formatted);
        Assert.Contains("Hello, world! 😊 #hashtag", formatted);
    }
    
    [Fact]
    public void PrettyMethod_EmptyMessage()
    {
        var cheep = new Cheep("Bob", "", 1757500427);

        var formatted = Formatting.Pretty(cheep);

        Assert.Contains("Bob", formatted);
        Assert.Contains(": ", formatted); // message part still shows
    }

    [Fact]
    public void PrettyMethod_LongMessage()
    {
        var longMsg = new string('x', 500);
        var cheep = new Cheep("Charlie", longMsg, 1757500427);

        var formatted = Formatting.Pretty(cheep);

        Assert.Contains("Charlie", formatted);
        Assert.Contains(longMsg, formatted);
    }

    [Fact]
    public void PrettyMethod_HandlesUnixEpoch()
    {
        var cheep = new Cheep("System", "Epoch start", 0);

        var formatted = Formatting.Pretty(cheep);

        Assert.Contains("System", formatted);
        Assert.Contains("Epoch start", formatted);

        var local = DateTimeOffset.FromUnixTimeSeconds(0).ToLocalTime().DateTime;
        var expectedStamp = local.ToString("dd/MM/yy HH:mm:ss", CultureInfo.InvariantCulture);
        Assert.Contains(expectedStamp, formatted);
    }

    [Fact]
    public void PrettyMethod_FutureTimestamp()
    {
        var futureTs = DateTimeOffset.UtcNow.AddYears(5).ToUnixTimeSeconds();
        var cheep = new Cheep("FutureUser", "Hello from the future", futureTs);

        var formatted = Formatting.Pretty(cheep);

        Assert.Contains("FutureUser", formatted);
        Assert.Contains("Hello from the future", formatted);

        var local = DateTimeOffset.FromUnixTimeSeconds(futureTs).ToLocalTime().DateTime;
        var expectedStamp = local.ToString("dd/MM/yy HH:mm:ss", CultureInfo.InvariantCulture);
        Assert.Contains(expectedStamp, formatted);
    }
}
