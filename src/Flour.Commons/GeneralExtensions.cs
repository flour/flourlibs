namespace Flour.Commons;

public static class GeneralExtensions
{
    public static T ToEnum<T>(this string value) where T : struct
    {
        return Enum.TryParse<T>(value, true, out var result) ? result : default;
    }
}