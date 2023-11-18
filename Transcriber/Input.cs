using Transcriber.Clients;

namespace Transcriber;

internal static class Input
{
    internal static string ChooseAudioFile()
    {
        var audioFiles = Directory.GetFiles(AppSettings.AudioFilePath); // , "*.mp3"
        var audioFileNames = audioFiles.Select(f => Path.GetFileName(f)).ToArray();

        Console.WriteLine("Choose audio file to transcribe:");
        WriteOptions(audioFileNames);

        var input = Console.ReadLine();
        if (!TryParseSelection(input, audioFileNames.Length, out int index))
        {
            Console.WriteLine("Invalid input.");
            return ChooseAudioFile();
        }

        return audioFiles[index - 1];
    }

    private static void WriteOptions(IList<string> options)
    {
        for (int i = 0; i < options.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {options[i]}");
        }
    }

    internal static void PressAnyKeyToExit()
    {
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }

    private static bool TryParseSelection(string? input, int optionCount, out int selection)
    {
        return int.TryParse(input, out selection) && selection > 0 && selection <= optionCount;
    }

    internal static SupportedLanguage ChooseLanguage()
    {
        Console.WriteLine("Choose language:");
        var supportedLanguages = GetSupportedLanguages();

        WriteOptions(supportedLanguages);
        var input = Console.ReadLine();
        if (!TryParseSelection(input, supportedLanguages.Length, out int selection))
        {
            Console.WriteLine("Invalid input.");
            return ChooseLanguage();
        }

        return EnumUtils.Parse<SupportedLanguage>(selection - 1);
    }

    private static string[] GetSupportedLanguages()
    {
        var languages = EnumUtils.GetValues<SupportedLanguage>();
        return languages
            .Select(l => l.ToString())
            .ToArray();
    }
}
