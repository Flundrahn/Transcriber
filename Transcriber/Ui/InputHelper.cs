using Transcriber.Infrastructure;
using Transcriber.Utils;

namespace Transcriber.Ui;

internal class InputHelper
{
    private readonly AppSettings _appSettings;

    internal InputHelper(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    internal string ChooseAudioFile()
    {
        var audioFiles = Directory.GetFiles(_appSettings.AudioDirectoryPath);
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

    private void WriteOptions(IList<string> options)
    {
        for (int i = 0; i < options.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {options[i]}");
        }
    }

    internal void PressAnyKeyToExit()
    {
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }

    private bool TryParseSelection(string? input, int optionCount, out int selection)
    {
        return int.TryParse(input, out selection) && selection > 0 && selection <= optionCount;
    }

    internal TranscriberSupportedLanguage ChooseLanguage()
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

        return EnumUtils.Parse<TranscriberSupportedLanguage>(selection - 1);
    }

    private string[] GetSupportedLanguages()
    {
        var languages = EnumUtils.GetValues<TranscriberSupportedLanguage>();
        return languages
            .Select(l => l.ToString())
            .ToArray();
    }
}
