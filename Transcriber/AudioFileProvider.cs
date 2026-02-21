using System;
using System.Collections.Generic;
using System.Text;

namespace Transcriber;

internal record AudioFile(string FileName, string FilePath);

internal class AudioFileProvider
{
    private readonly AppSettings _appSettings;

    internal AudioFileProvider(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    internal IList<AudioFile> GetAudioFiles()
    {
        // This class can handle responsibility of filtering to only show audio files later
        var filePaths = Directory.GetFiles(_appSettings.AudioDirectoryPath);
        var audioFiles = new List<AudioFile>();
        foreach (string audioFile in filePaths)
        {
            audioFiles.Add(new AudioFile(Path.GetFileName(audioFile), audioFile));
        }

        return audioFiles;
    }
}
