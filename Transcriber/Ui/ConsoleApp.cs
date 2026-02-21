using Spectre.Console;
using Transcriber.Infrastructure;
using Transcriber.Utils;

namespace Transcriber.Ui;

internal enum MainMenuOption
{
    TranscribeAudio,
    TranslateText,
    Exit
}

internal class ConsoleApp
{
    private readonly AudioFileProvider _audioFileProvider;
    private readonly AppSettings _appSettings;
    private readonly GoogleCloudTranscriberClient _transcriberClient;
    private readonly GoogleCloudTranslatorClient _translatorClient;

    internal ConsoleApp(AppSettings appSettings, AudioFileProvider audioFileProvider)
    {
        _appSettings = appSettings;
        _audioFileProvider = audioFileProvider;

        // TODO: inject later
        var languageMapper = new GoogleCloudLanguageMapper();
        _transcriberClient = new GoogleCloudTranscriberClient(languageMapper);
        _translatorClient = new GoogleCloudTranslatorClient(languageMapper, _appSettings);
    }

    internal async Task RunAsync()
    {
        ShowWelcome();

        while (true)
        {
            var choice = PromptMainMenu();

            try
            {
                switch (choice)
                {
                    case MainMenuOption.TranscribeAudio:
                        var transcript = await TranscribeWorkflow();
                        if (transcript is not null)
                            await PromptAndTranslate(transcript);
                        break;

                    case MainMenuOption.TranslateText:
                        await TranslateWorkflow();
                        break;

                    case MainMenuOption.Exit:
                        AnsiConsole.MarkupLine("\n[dim]Goodbye![/]");
                        return;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
            }

            AnsiConsole.WriteLine();
        }
    }

    private static void ShowWelcome()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("Transcriber")
                .Centered()
                .Color(Color.Blue));
        AnsiConsole.MarkupLine("[dim]Audio Transcription & Translation Tool[/]\n");
    }

    private static MainMenuOption PromptMainMenu()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<MainMenuOption>()
                .Title("[cyan]What would you like to do?[/]")
                .AddChoices(MainMenuOption.TranscribeAudio, MainMenuOption.TranslateText, MainMenuOption.Exit)
                .UseConverter(option => option switch
                {
                    MainMenuOption.TranscribeAudio => "Transcribe Audio File",
                    MainMenuOption.TranslateText   => "Translate Text",
                    MainMenuOption.Exit            => "Exit",
                    _ => option.ToString()
                }));
    }

    private async Task<string?> TranscribeWorkflow()
    {
        var audioFiles = _audioFileProvider.GetAudioFiles();
        if (audioFiles.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No audio files found in the configured directory.[/]");
            return null;
        }

        var audioFile = AnsiConsole.Prompt(
            new SelectionPrompt<AudioFile>()
                .Title("[green]Select an audio file:[/]")
                .PageSize(10)
                .AddChoices(audioFiles)
                .UseConverter(f => f.FileName));

        var language = PromptForLanguage("Select source language:");

        var result = await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync("Transcribing audio...", _ =>
                _transcriberClient.TranscribeAudio(audioFile.FilePath, language));

        if (!result.IsSuccessful || string.IsNullOrEmpty(result.GetResult()))
        {
            AnsiConsole.MarkupLine("[yellow]Transcription returned no results.[/]");
            return null;
        }

        var transcript = result.GetResult();
        AnsiConsole.Write(
            new Panel(transcript)
                .Header("[green]Transcript[/]")
                .BorderColor(Color.Green)
                .Padding(1, 0));

        return transcript;
    }

    private async Task TranslateWorkflow()
    {
        var text = AnsiConsole.Prompt(
            new TextPrompt<string>("[cyan]Enter text to translate:[/]")
                .Validate(t => !string.IsNullOrWhiteSpace(t), "[red]Text cannot be empty.[/]"));

        await TranslateText(text);
    }

    private async Task PromptAndTranslate(string transcript)
    {
        if (AnsiConsole.Confirm("\nWould you like to translate the transcript?"))
            await TranslateText(transcript);
    }

    private async Task TranslateText(string text)
    {
        var language = PromptForLanguage("Select target language:");

        var result = await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync("Translating text...", _ =>
                _translatorClient.TranslateText(text, language));

        if (!result.IsSuccessful || string.IsNullOrEmpty(result.GetResult()))
        {
            AnsiConsole.MarkupLine("[yellow]Translation returned no results.[/]");
            return;
        }

        AnsiConsole.Write(
            new Panel(result.GetResult())
                .Header("[blue]Translation[/]")
                .BorderColor(Color.Blue)
                .Padding(1, 0));
    }

    private static TranscriberSupportedLanguage PromptForLanguage(string title)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<TranscriberSupportedLanguage>()
                .Title($"[cyan]{title}[/]")
                .PageSize(10)
                .AddChoices(EnumUtils.GetValues<TranscriberSupportedLanguage>()));
    }
}
