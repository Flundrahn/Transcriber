namespace Transcriber;

public static class EnumUtils
{
    public static TEnum[] GetValues<TEnum>() where TEnum : Enum
    {
        return (TEnum[])Enum.GetValues(typeof(TEnum));
    }

    public static TEnum Parse<TEnum>(string value) where TEnum : Enum
    {
        return (TEnum)Enum.Parse(typeof(TEnum), value);
    }

    public static TEnum Parse<TEnum>(int value) where TEnum : Enum
    {
        return (TEnum)Enum.Parse(typeof(TEnum), value.ToString());
    }
}
