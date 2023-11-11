using Google.Cloud.Speech.V1;
using static Google.Cloud.Speech.V1.RecognitionConfig.Types;

namespace Transcriber.Client;

internal class TranscriberClient
{
    private readonly SpeechClient _speechClient = SpeechClient.Create();
    private readonly ILanguageMapper _languageMapper;

    internal TranscriberClient(ILanguageMapper languageMapper)
    {
        _languageMapper = languageMapper;
    }

    internal Task<RecognizeResponse> TranscribeAudio(string audioFilePath, SupportedLanguage language)
    {
        var audio = RecognitionAudio.FromFile(audioFilePath);
        var config = new RecognitionConfig
        {
            Encoding = AudioEncoding.Mp3,
            SampleRateHertz = 16000,
            LanguageCode = _languageMapper.GetLanguageCode(language),
        };

        var request = new RecognizeRequest
        {
            Audio = audio,
            Config = config,
        };

        return _speechClient.RecognizeAsync(request);
    }
}

internal static class RecognizeResponseExtensions
{
    // Implementation is preliminary
    internal static bool IsSuccessful(this RecognizeResponse response) => response.Results.Count > 0;
}