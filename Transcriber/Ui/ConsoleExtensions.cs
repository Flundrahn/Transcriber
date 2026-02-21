namespace Transcriber.Ui;

public class ConsoleExtensions
{
    public static void WriteWarningLine(string? value)
    {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Warning]: {value}");
            Console.ResetColor();
    }

    public static void WriteErrorLine(string? value)
    {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error]: {value}");
            Console.ResetColor();
    }
}
