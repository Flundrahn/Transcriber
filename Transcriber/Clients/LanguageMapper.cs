using Google.Cloud.Speech.V1;

namespace Transcriber.Clients;

public class LanguageMapper : ILanguageMapper
{
    public string GetLanguageCode(SupportedLanguage language)
    {
        return language switch
        {
            SupportedLanguage.Svenska => LanguageCodes.Swedish.Sweden,
            SupportedLanguage.English => LanguageCodes.English.UnitedStates,
            SupportedLanguage.Portugues => LanguageCodes.Portuguese.Brazil,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, "Language not supported")
        };
    }
}

public interface ILanguageMapper
{
    string GetLanguageCode(SupportedLanguage language);
}

public enum SupportedLanguage
{
    English,
    Svenska,
    Portugues,
}