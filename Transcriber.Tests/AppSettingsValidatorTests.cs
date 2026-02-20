using Transcriber;
using System.IO.Abstractions.TestingHelpers;

namespace Transcriber.Tests;

public class AppSettingsValidatorTests
{
    private MockFileSystem _mockFileSystem;

    public AppSettingsValidatorTests()
    {
        _mockFileSystem = new MockFileSystem();
    }

    [Fact]
    public void Validate_ShouldAddError_WhenAudioDirectoryPathIsNullOrEmpty()
    {
        var settings = new AppSettings { AudioDirectoryPath = "" };
        var validator = new AppSettingsValidator(_mockFileSystem);
        validator.Validate(settings);
        Assert.Contains(validator.Errors, e => e.Contains("AudioDirectoryPath is required"));
    }

    [Fact]
    public void Validate_ShouldAddError_WhenAudioDirectoryPathIsNotRooted()
    {
        var settings = new AppSettings { AudioDirectoryPath = "relative/path" };
        var validator = new AppSettingsValidator(_mockFileSystem);
        validator.Validate(settings);
        Assert.Contains(validator.Errors, e => e.Contains("AudioDirectoryPath must be an absolute path"));
    }

    [Fact]
    public void Validate_ShouldAddError_WhenAudioDirectoryPathDoesNotExist()
    {
        var path = _mockFileSystem.Path.Combine(_mockFileSystem.Path.GetTempPath(), "notfound", "audio");
        var settings = new AppSettings { AudioDirectoryPath = path };
        var validator = new AppSettingsValidator(_mockFileSystem);
        validator.Validate(settings);
        Assert.Contains(validator.Errors, e => e.Contains("AudioDirectoryPath directory does not exist"));
    }

    [Fact]
    public void Validate_ShouldAddError_WhenGoogleCredentialsFilePathIsNullOrEmpty()
    {
        var validator = new AppSettingsValidator(_mockFileSystem);
        var settings = new AppSettings { GoogleCredentialsFilePath = "" };
        validator.Validate(settings);
        Assert.Contains(validator.Errors, e => e.Contains("GoogleCredentialsFilePath is required"));
    }

    [Fact]
    public void Validate_ShouldAddError_WhenGoogleCredentialsFilePathIsNotRooted()
    {
        var validator = new AppSettingsValidator(_mockFileSystem);
        var settings = new AppSettings { GoogleCredentialsFilePath = "relative/file.json" };
        validator.Validate(settings);
        Assert.Contains(validator.Errors, e => e.Contains("GoogleCredentialsFilePath must be an absolute path"));
    }

    [Fact]
    public void Validate_ShouldAddError_WhenGoogleCredentialsFilePathDirectoryDoesNotExist()
    {
        var path = _mockFileSystem.Path.Combine(_mockFileSystem.Path.GetTempPath(), "notfound", "file.json");
        var settings = new AppSettings { GoogleCredentialsFilePath = path };
        var validator = new AppSettingsValidator(_mockFileSystem);
        validator.Validate(settings);
        Assert.Contains(validator.Errors, e => e.Contains("GoogleCredentialsFilePath directory does not exist"));
    }

    [Fact]
    public void Validate_ShouldAddError_WhenGoogleCredentialsFileDoesNotExist()
    {
        var dir = _mockFileSystem.Path.Combine(_mockFileSystem.Path.GetTempPath(), "creds");
        _mockFileSystem.AddDirectory(dir);
        var file = _mockFileSystem.Path.Combine(dir, "file.json");
        var settings = new AppSettings { GoogleCredentialsFilePath = file };
        var validator = new AppSettingsValidator(_mockFileSystem);
        validator.Validate(settings);
        Assert.Contains(validator.Errors, e => e.Contains("GoogleCredentialsFilePath file does not exist"));
    }

    [Fact]
    public void Validate_ShouldAddError_WhenGoogleCredentialsFilePathIsNotJson()
    {
        var dir = _mockFileSystem.Path.Combine(_mockFileSystem.Path.GetTempPath(), "creds");
        _mockFileSystem.AddDirectory(dir);
        var file = _mockFileSystem.Path.Combine(dir, "file.txt");
        _mockFileSystem.AddFile(file, new MockFileData("{}"));
        var settings = new AppSettings { GoogleCredentialsFilePath = file };
        var validator = new AppSettingsValidator(_mockFileSystem);
        validator.Validate(settings);
        Assert.Contains(validator.Errors, e => e.Contains("GoogleCredentialsFilePath must point to a .json file"));
    }

    [Fact]
    public void Validate_ShouldAddWarning_WhenGoogleCloudProjectNumberIsNotNumeric()
    {
        var validator = new AppSettingsValidator(_mockFileSystem);
        var settings = new AppSettings { GoogleCloudProjectNumber = "notanumber" };
        validator.Validate(settings);
        Assert.Contains(validator.Warnings, w => w.Contains("GoogleCloudProjectNumber is not a number"));
    }

    [Fact]
    public void Validate_ShouldAddWarning_WhenGoogleCloudProjectNumberIsTooShort()
    {
        var validator = new AppSettingsValidator(_mockFileSystem);
        var settings = new AppSettings { GoogleCloudProjectNumber = "123" };
        validator.Validate(settings);
        Assert.Contains(validator.Warnings, w => w.Contains("fewer than 12 digits"));
    }

    [Fact]
    public void Validate_ShouldAddWarning_WhenGoogleCloudProjectNumberIsTooLong()
    {
        var validator = new AppSettingsValidator(_mockFileSystem);
        var settings = new AppSettings { GoogleCloudProjectNumber = "1234567890123" };
        validator.Validate(settings);
        Assert.Contains(validator.Warnings, w => w.Contains("more than 12 digits"));
    }

    [Fact]
    public void Validate_ShouldNotAddErrorsOrWarnings_WhenAllSettingsAreValid()
    {
        var dir = _mockFileSystem.Path.Combine(_mockFileSystem.Path.GetTempPath(), "validdir");
        _mockFileSystem.AddDirectory(dir);
        var file = _mockFileSystem.Path.Combine(dir, "creds.json");
        _mockFileSystem.AddFile(file, new MockFileData("{}"));
        var settings = new AppSettings
        {
            AudioDirectoryPath = dir,
            GoogleCredentialsFilePath = file,
            GoogleCloudProjectNumber = "123456789012"
        };
        var validator = new AppSettingsValidator(_mockFileSystem);
        validator.Validate(settings);
        Assert.Empty(validator.Errors);
        Assert.Empty(validator.Warnings);
    }
}
