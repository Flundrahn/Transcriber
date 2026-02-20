using System.Collections.ObjectModel;
using System.IO.Abstractions;

namespace Transcriber;

public class AppSettings
{
    public string? Environment { get; set; }
    public string GoogleCredentialsFilePath { get; set; } = string.Empty;
    public string AudioDirectoryPath { get; set; } = string.Empty;
    public string GoogleCloudProjectNumber { get; set; } = string.Empty;
}

internal class AppSettingsValidator
{
    private readonly IFileSystem _fileSystem;
    private List<string> _warnings = [];
    internal ReadOnlyCollection<string> Warnings => _warnings.AsReadOnly();

    private List<string> _errors = [];
    internal ReadOnlyCollection<string> Errors => _errors.AsReadOnly();

    internal AppSettingsValidator() : this(new FileSystem())
    {
    }

    // Ctor for unit tests
    internal AppSettingsValidator(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    internal void Validate(AppSettings appSettings)
    {
        _warnings.Clear();
        _errors.Clear();
        ValidateAudioDirectoryPath(appSettings.AudioDirectoryPath);
        ValidateApiCredentialsPath(appSettings.GoogleCredentialsFilePath);
        ValidateGoogleCloudProjectNumber(appSettings.GoogleCloudProjectNumber);
    }

    private void ValidateAudioDirectoryPath(string? audioDirectoryPath)
    {
        if (string.IsNullOrWhiteSpace(audioDirectoryPath))
        {
            _errors.Add($"{nameof(AppSettings.AudioDirectoryPath)} is required.");
            return;
        }

        if (!_fileSystem.Path.IsPathRooted(audioDirectoryPath))
        {
            _errors.Add($"{nameof(AppSettings.AudioDirectoryPath)} must be an absolute path.");
            return;
        }

        var directory = _fileSystem.Path.GetDirectoryName(audioDirectoryPath);
        if (string.IsNullOrWhiteSpace(directory) || !_fileSystem.Directory.Exists(directory))
        {
            _errors.Add($"{nameof(AppSettings.AudioDirectoryPath)} directory does not exist: {directory}");
            return;
        }
    }

    private void ValidateApiCredentialsPath(string? apiCredentialsPath)
    {
        if (string.IsNullOrWhiteSpace(apiCredentialsPath))
        {
            _errors.Add($"{nameof(AppSettings.GoogleCredentialsFilePath)} is required.");
            return;
        }

        if (!_fileSystem.Path.IsPathRooted(apiCredentialsPath))
        {
            _errors.Add($"{nameof(AppSettings.GoogleCredentialsFilePath)} must be an absolute path.");
            return;
        }

        string? directory = _fileSystem.Path.GetDirectoryName(apiCredentialsPath);
        if (string.IsNullOrWhiteSpace(directory) || !_fileSystem.Directory.Exists(directory))
        {
            _errors.Add($"{nameof(AppSettings.GoogleCredentialsFilePath)} directory does not exist: {directory}");
            return;
        }

        if (!_fileSystem.File.Exists(apiCredentialsPath))
        {
            _errors.Add($"{nameof(AppSettings.GoogleCredentialsFilePath)} file does not exist: {apiCredentialsPath}");
            return;
        }

        if (!apiCredentialsPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            _errors.Add($"{nameof(AppSettings.GoogleCredentialsFilePath)} must point to a .json file.");
            return;
        }
    }

    private void ValidateGoogleCloudProjectNumber(string? projectNumber)
    {
        if (string.IsNullOrWhiteSpace(projectNumber))
        {
            _errors.Add($"{nameof(AppSettings.GoogleCloudProjectNumber)} is required.");
            return;
        }

        if (!ulong.TryParse(projectNumber, out ulong projectNumberAsULong))
        {
            _warnings.Add($"{nameof(AppSettings.GoogleCloudProjectNumber)} is not a number, this is unusual and may indicate a misconfiguration.");
            return;
        }

        if (projectNumberAsULong < 10000000000)
        {
            _warnings.Add($"{nameof(AppSettings.GoogleCloudProjectNumber)} has fewer than 12 digits, this is unusual and may indicate a misconfiguration. If Google Cloud connection works as expected this warning can be safely ignored.");
            return;
        }
        
        if (projectNumberAsULong > 999999999999)
        {
            _warnings.Add($"{nameof(AppSettings.GoogleCloudProjectNumber)} has more than 12 digits, this is unusual and may indicate a misconfiguration. If Google Cloud connection works as expected this warning can be safely ignored.");
        }
    }
}
