using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Flour.Commons;

public static class StringExtensions
{
    public static string Sha256(this string input)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return GetStringFromHash(hash);
    }

    public static string Sha512(this string input)
    {
        var hash = SHA512.HashData(Encoding.UTF8.GetBytes(input));
        return GetStringFromHash(hash);
    }

    private static string GetStringFromHash(IEnumerable<byte> hash)
    {
        var result = new StringBuilder();
        foreach (var t in hash) result.Append(t.ToString("X2"));

        return result.ToString();
    }

    private const string EmptyObject = "{}";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    };

    public static readonly JsonSerializerOptions UnsafeJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private static readonly Dictionary<Type, Dictionary<int, string>> Cache = new();

    public static string ToJson(this object data, JsonSerializerOptions options = null)
    {
        return data is null ? EmptyObject : JsonSerializer.Serialize(data, options ?? JsonOptions);
    }

    public static T FromJson<T>(this string data, JsonSerializerOptions options = null)
    {
        return JsonSerializer.Deserialize<T>(
            string.IsNullOrWhiteSpace(data) ? EmptyObject : data, options ?? JsonOptions);
    }

    public static T FromJsonBase64<T>(this string data)
    {
        if (!string.IsNullOrWhiteSpace(data) && data.FromBase64(out var bytes))
            return JsonSerializer.Deserialize<T>(bytes, JsonOptions);

        return JsonSerializer.Deserialize<T>(EmptyObject, JsonOptions);
    }

    public static byte[] ToBytes(this string text)
        => Encoding.UTF8.GetBytes(text);

    public static string FromBytes(this byte[] buffer)
        => Encoding.UTF8.GetString(buffer);

    public static string ToBase64(this byte[] buffer)
        => Convert.ToBase64String(buffer);

    public static bool FromBase64(this string text, out byte[] data)
    {
        try
        {
            text = text.PadRight(text.Length + (4 - text.Length % 4) % 4, '=');
            data = Convert.FromBase64String(text);
            return true;
        }
        catch (Exception)
        {
            data = text.ToBytes();
            return false;
        }
    }

    public static string EnumToString(this Enum @enum)
    {
        var enumType = @enum.GetType();
        if (Cache.TryGetValue(enumType, out var dict))
        {
            var key = Convert.ToInt32(@enum);
            return dict.TryGetValue(key, out var value) ? value : @enum.ToString();
        }

        Cache.TryAdd(enumType, new Dictionary<int, string>());
        var values = Enum.GetValues(enumType);
        foreach (var value in values)
        {
            var memberInfo = enumType.GetMember(value.ToString() ?? string.Empty);
            var attributes = memberInfo.First().GetCustomAttribute<DescriptionAttribute>();

            Cache[enumType].Add(
                Convert.ToInt32(value),
                attributes is not null ? attributes.Description : value.ToString());
        }

        return @enum.EnumToString();
    }

    public static string Numbers(this string text)
        => string.IsNullOrWhiteSpace(text) ? text : new string(text.Where(char.IsDigit).ToArray());

    public static string MaskText(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        if (text.Length <= 10)
            return string.Join(string.Empty, Enumerable.Repeat("*", text.Length));

        var result = $"{text[..6]}{string.Join("", Enumerable.Repeat('*', text.Length - 10))}{text[^4..]}";
        return result;
    }

    public static string Sub(this string text, uint length)
        => string.IsNullOrWhiteSpace(text) || text.Length <= length ? text : text.Substring(0, (int) length);
}