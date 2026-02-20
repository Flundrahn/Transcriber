using Google.Cloud.Speech.V1;

namespace Transcriber.Clients;

public class LanguageMapper : ILanguageMapper
{
    public string GetLanguageCode(TranscriberSupportedLanguage language)
    {
        return language switch
        {
            TranscriberSupportedLanguage.Svenska => LanguageCodes.Swedish.Sweden,
            TranscriberSupportedLanguage.English => LanguageCodes.English.UnitedStates,
            TranscriberSupportedLanguage.Portugues => LanguageCodes.Portuguese.Brazil,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, "Language not supported")
        };
    }
}

public interface ILanguageMapper
{
    string GetLanguageCode(TranscriberSupportedLanguage language);
}

public enum TranscriberSupportedLanguage
{
    English,
    Svenska,
    Portugues,
}