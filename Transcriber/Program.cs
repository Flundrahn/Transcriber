using Google.Cloud.Speech.V1;
using Transcriber;
using Transcriber.Animation;
using Transcriber.Client;

await ExplicitMain();

static async Task ExplicitMain()
{
    try
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", AppSettings.GoogleCredentialsFilePath);

        string audioFilePath = Input.ChooseAudioFile();
        SupportedLanguage language = Input.ChooseLanguage();
        var client = new TranscriberClient(new LanguageMapper());

        Console.WriteLine("Transcribing audio file...");
        Animation.ShowSpinner();
        RecognizeResponse response = await client.TranscribeAudio(audioFilePath, language);

        if (!response.IsSuccessful())
        {
            Animation.HideSpinner();
            Console.WriteLine("No results.");

            await ExplicitMain();
            return;
        }

        Animation.HideSpinner();
        Console.WriteLine("Success!");
        foreach (var result in response.Results)
        {
            foreach (var alternative in result.Alternatives)
            {
                Console.WriteLine($"Transcript: {alternative.Transcript}");
            }
        }
    }
    catch (Exception ex)
    {
        Animation.HideSpinner();
        Console.WriteLine($"Error: {ex.Message}");
    }

    Animation.HideSpinner();
    Input.PressAnyKeyToExit();
}
