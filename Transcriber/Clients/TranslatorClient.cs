using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;

namespace Transcriber.Clients;

internal class TranslatorClient
{
    private readonly TranslationServiceClient _translationServiceClient = TranslationServiceClient.Create();
    private readonly ILanguageMapper _languageMapper;

    internal TranslatorClient(ILanguageMapper languageMapper)
    {
        _languageMapper = languageMapper;
    }

    internal async Task<ClientResult<string>> TranslateText(string text, TranscriberSupportedLanguage language)
    {
        var request = new TranslateTextRequest
        {
            Contents = { text },
            TargetLanguageCode = _languageMapper.GetLanguageCode(language),
            Parent = new ProjectName(AppSettings.GoogleCloudProjectNumber).ToString(),
        };

        TranslateTextResponse response = await _translationServiceClient.TranslateTextAsync(request);
        if (response.Translations.Count == 0)
        {
            return ClientResult<string>.Failure("Could not translate text.");
        }

        return ClientResult<string>.Success(response.Translations[0].TranslatedText);
    }
}