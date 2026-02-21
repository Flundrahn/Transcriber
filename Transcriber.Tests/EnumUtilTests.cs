using Transcriber.Utils;

namespace Transcriber.Tests;

public class UnitTest1
{
    [Fact]
    public void Parse_IfInputIsValidString_ShouldParse()
    {
        // Act
        var result = EnumUtils.Parse<Weekday>("Monday");

        // Assert
        Assert.Equal(Weekday.Monday, result);
    }

    [Fact]
    public void Parse_IfInputIsValidInt_ShouldParse()
    {
        // Act
        var result = EnumUtils.Parse<Weekday>(0);

        // Assert
        Assert.Equal(Weekday.Monday, result);
    }
}

public enum Weekday
{
    Monday,
    Tuesday,
    Wednesday,
}