using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using Transcriber.Utils;

namespace Transcriber.Infrastructure;

internal class GoogleCloudTranslatorClient
{
    private readonly TranslationServiceClient _translationServiceClient = TranslationServiceClient.Create();
    private readonly ILanguageMapper _languageMapper;
    private readonly AppSettings _appSettings;

    internal GoogleCloudTranslatorClient(ILanguageMapper languageMapper, AppSettings appSettings)
    {
        _languageMapper = languageMapper;
        _appSettings = appSettings;
    }

    internal async Task<Result<string>> TranslateText(string text, TranscriberSupportedLanguage language)
    {
        var request = new TranslateTextRequest
        {
            Contents = { text },
            TargetLanguageCode = _languageMapper.GetLanguageCode(language),
            Parent = new ProjectName(_appSettings.GoogleCloudProjectNumber).ToString(),
        };

        TranslateTextResponse response = await _translationServiceClient.TranslateTextAsync(request);
        if (response.Translations.Count == 0)
        {
            return Result<string>.Failure("Could not translate text.");
        }

        return Result<string>.Success(response.Translations[0].TranslatedText);
    }
}