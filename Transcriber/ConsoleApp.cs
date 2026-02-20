using Microsoft.Extensions.Configuration;
using Transcriber.Clients;
using Transcriber.Animations;

namespace Transcriber;

internal class ConsoleApp
{
    private readonly InputHelper _inputHelper;
    internal AppSettings AppSettings { get; private init; }

    internal ConsoleApp()
    {
        AppSettings appSettings = ConfigureApp();
        ValidateAppSettings(appSettings);
        AppSettings = appSettings;
        _inputHelper = new InputHelper(AppSettings);
    }

    private AppSettings ConfigureApp()
    {
        string? environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
            .Build();

        return config.Get<AppSettings>()
            ?? throw new InvalidOperationException("Failed to bind AppSettings, please check the settings json and try again.");
    }

    private void ValidateAppSettings(AppSettings appSettings)
    {
        var settingsValidator = new AppSettingsValidator();
        settingsValidator.Validate(appSettings);

        foreach(string warning in settingsValidator.Warnings)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Warning]: {warning}");
            Console.ResetColor();
        }

        foreach(string error in settingsValidator.Errors)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error]: {error}");
            Console.ResetColor();
        }

        if (settingsValidator.Errors.Count > 0)
        {
            throw new InvalidOperationException("App settings are invalid, please fix the errors and try again.");
        }
    }

    internal async Task Main()
    {
        try
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", AppSettings.GoogleCredentialsFilePath);
            var response = await Transcribe();
            await Translate(response.GetResult());
        }
        catch (Exception ex)
        {
            Animation.HideSpinner();
            Console.WriteLine($"Error: {ex.Message}");
        }

        _inputHelper.PressAnyKeyToExit();
    }

    internal async Task<ClientResult<string>> Transcribe()
    {
        string audioFilePath = _inputHelper.ChooseAudioFile();
        TranscriberSupportedLanguage language = _inputHelper.ChooseLanguage();

        var client = new TranscriberClient(new LanguageMapper());

        Console.WriteLine();
        Console.WriteLine("Transcribing audio file...");
        Animation.ShowSpinner();
        ClientResult<string> response = await client.TranscribeAudio(audioFilePath, language);

        Animation.HideSpinner();
        if (!response.IsSuccessful)
        {
            Console.WriteLine("No results.");
            await Main();
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

                var client = new TranslatorClient(new LanguageMapper(), AppSettings);
                ClientResult<string> response = await client.TranslateText(text, language);

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
