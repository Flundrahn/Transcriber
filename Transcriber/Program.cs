using Transcriber;
using Transcriber.Animation;
using Transcriber.Clients;

await ExplicitMain();

static async Task ExplicitMain()
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

    Input.PressAnyKeyToExit();
}

static async Task<ClientResult<string>> Transcribe()
{
    string audioFilePath = Input.ChooseAudioFile();
    SupportedLanguage language = Input.ChooseLanguage();

    var client = new TranscriberClient(new LanguageMapper());

    Console.WriteLine();
    Console.WriteLine("Transcribing audio file...");
    Animation.ShowSpinner();
    ClientResult<string> response = await client.TranscribeAudio(audioFilePath, language);

    Animation.HideSpinner();
    if (!response.IsSuccessful)
    {
        Console.WriteLine("No results.");
        await ExplicitMain();
        return response;
    }

    Console.WriteLine($"Transcript: {response.GetResult()}");
    return response;
}

static async Task Translate(string text)
{
    Console.WriteLine("Would you like to translate the text? (y/n)");
    ConsoleKeyInfo input = Console.ReadKey();
    Console.WriteLine();

    switch (input.Key)
    {
        case ConsoleKey.Y:
            SupportedLanguage language = Input.ChooseLanguage();
            Console.WriteLine();
            Console.WriteLine("Translating text...");
            Animation.ShowSpinner();

            var client = new TranslatorClient(new LanguageMapper());
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
