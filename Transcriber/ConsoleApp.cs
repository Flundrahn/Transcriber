using Transcriber.Infrastructure;
using Transcriber.Animations;
using Transcriber.Utils;

namespace Transcriber;

internal class ConsoleApp
{
    private readonly InputHelper _inputHelper;
    private readonly AppSettings _appSettings;

    internal ConsoleApp(AppSettings appSettings, InputHelper inputHelper)
    {
        _appSettings = appSettings;
        _inputHelper = inputHelper;
    }

    internal async Task RunAsync()
    {
        try
        {

            var response = await Transcribe();
            await Translate(response.GetResult());
        }
        catch (Exception ex)
        {
            Animation.HideSpinner();
            ConsoleExtensions.WriteErrorLine(ex.Message);
        }

        _inputHelper.PressAnyKeyToExit();
    }

    internal async Task<Result<string>> Transcribe()
    {
        string audioFilePath = _inputHelper.ChooseAudioFile();
        TranscriberSupportedLanguage language = _inputHelper.ChooseLanguage();

        var client = new TranscriberClient(new LanguageMapper());

        Console.WriteLine();
        Console.WriteLine("Transcribing audio file...");
        Animation.ShowSpinner();
        Result<string> response = await client.TranscribeAudio(audioFilePath, language);

        Animation.HideSpinner();
        if (!response.IsSuccessful)
        {
            Console.WriteLine("No successful results.");
            await RunAsync();
            return response;
        }

        Console.WriteLine($"Transcript: {response.GetResult()}");
        return response;
    }
    internal async Task Translate(string text)
    {
        Console.WriteLine("Would you like to translate the text? (y/n)");
        ConsoleKeyInfo input = Console.ReadKey();
        Console.WriteLine();

        switch (input.Key)
        {
            case ConsoleKey.Y:
                TranscriberSupportedLanguage language = _inputHelper.ChooseLanguage();
                Console.WriteLine();
                Console.WriteLine("Translating text...");
                Animation.ShowSpinner();

                var client = new TranslatorClient(new LanguageMapper(), _appSettings);
                Result<string> response = await client.TranslateText(text, language);

                Animation.HideSpinner();
                Console.WriteLine($"Translation: {response.GetResult()}");
                break;
            case ConsoleKey.N:
                Console.WriteLine();
                Console.WriteLine("Exiting...");
                break;
            default:
                Console.WriteLine();
                Console.WriteLine("Invalid input.");
                await Translate(text);
                break;
        }
    }
}
