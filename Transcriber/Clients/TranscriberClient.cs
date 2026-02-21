using System.Text;
using Google.Cloud.Speech.V1;
using Transcriber.Utils;
using static Google.Cloud.Speech.V1.RecognitionConfig.Types;

namespace Transcriber.Clients;

internal class TranscriberClient
{
    private readonly SpeechClient _speechClient = SpeechClient.Create();
    private readonly ILanguageMapper _languageMapper;

    internal TranscriberClient(ILanguageMapper languageMapper)
    {
        _languageMapper = languageMapper;
    }

    internal async Task<Result<string>> TranscribeAudio(string audioFilePath, TranscriberSupportedLanguage language)
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

        RecognizeResponse response = await _speechClient.RecognizeAsync(request);

        if (!response.IsSuccessful())
        {
            return Result<string>.Failure("Could not transcribe audio.");
        }

        string transcriptionResult = ConcatenateAlternativeLines(response.Results);

        return Result<string>.Success(transcriptionResult);
    }

    private static string ConcatenateAlternativeLines(IEnumerable<SpeechRecognitionResult> responseResults)
    {
        var transcriptionResultBuilder = new StringBuilder();
        foreach (var result in responseResults)
        {
            foreach (var alternative in result.Alternatives)
            {
                transcriptionResultBuilder.AppendLine(alternative.Transcript);
            }
        }

        return transcriptionResultBuilder.ToString();
    }
}

internal static class RecognizeResponseExtensions
{
    internal static bool IsSuccessful(this RecognizeResponse response) => response.Results.Count > 0;
}