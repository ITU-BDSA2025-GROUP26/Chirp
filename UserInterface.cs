namespace Chirp.CLI;

public class UserInterface
{
    public static void ShowUsage()
    {
        Console.Error.WriteLine("Usage: read | cheep \"message\"");
    }

    public static void ShowCheep(Cheep c)
    {
        Console.WriteLine(Formatting.Pretty(c));
    }

    public static void ShowCheepError()
    {
        Console.Error.WriteLine("cheep requires a message, e.g. cheep \"Hello, world!\"");
    }

    public static void ShowUnknownCommand(string command)
    { Console.Error.WriteLine($"Unknown command: {command}");
    }
}

