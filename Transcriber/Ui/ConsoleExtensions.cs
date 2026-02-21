using Spectre.Console;

namespace Transcriber.Ui;

public class ConsoleExtensions
{
    public static void WriteWarningLine(string? value) =>
        AnsiConsole.MarkupLine($"[yellow][[Warning]]: {value?.EscapeMarkup()}[/]");

    public static void WriteErrorLine(string? value) =>
        AnsiConsole.MarkupLine($"[red][[Error]]: {value?.EscapeMarkup()}[/]");
}
