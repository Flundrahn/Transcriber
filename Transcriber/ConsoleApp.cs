using Microsoft.Extensions.Configuration;
using Transcriber.Clients;
using Transcriber.Animations;
using Transcriber.Utils;

namespace Transcriber;

internal class ConsoleApp
{
    private readonly InputHelper _inputHelper;
    private readonly AppSettings _appSettings;

    internal ConsoleApp()
    {
        AppSettings appSettings = ConfigureApp();
        ValidateAppSettings(appSettings);
        _appSettings = appSettings;
        _inputHelper = new InputHelper(_appSettings);
    }

    private AppSettings ConfigureApp()
    {
        string? environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
            .Build();

        var appSettings = config.Get<AppSettings>()
            ?? throw new InvalidOperationException("Failed to bind AppSettings, please check the settings json and try again.");

        // So Google Cloud libraries can find the credentials file path 
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", appSettings.GoogleCredentialsFilePath);

        return appSettings;
    }

    private void ValidateAppSettings(AppSettings appSettings)
    {
        var settingsValidator = new AppSettingsValidator();
        settingsValidator.Validate(appSettings);

        foreach(string warning in settingsValidator.Warnings)
        {
            ConsoleExtensions.WriteWarningLine(warning);
        }

        foreach(string error in settingsValidator.Errors)
        {
            ConsoleExtensions.WriteErrorLine(error);
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
            Console.WriteLine("No successful results.");
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

                var client = new TranslatorClient(new LanguageMapper(), _appSettings);
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
