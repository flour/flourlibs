using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Flour.Vault.Internals;

internal class JsonConfigurationParser
{
    private readonly IDictionary<string, string> _mappings =
        new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    private readonly Stack<string> _stack = new();

    private string _currentPath;

    public IDictionary<string, string> Parse(JsonDocument jObject)
    {
        VisitJObject(jObject.RootElement);

        return _mappings;
    }

    private void VisitJObject(JsonElement jObject)
    {
        foreach (var property in jObject.EnumerateObject())
        {
            EnterContext(property.Name);
            VisitProperty(property);
            ExitContext();
        }
    }

    private void VisitProperty(JsonProperty property)
    {
        VisitToken(property.Value);
    }

    private void VisitToken(JsonElement token)
    {
        switch (token.ValueKind)
        {
            case JsonValueKind.Object:
                VisitJObject(token);
                break;
            case JsonValueKind.Array:
                VisitArray(token);
                break;
            case JsonValueKind.Number:
            case JsonValueKind.String:
            case JsonValueKind.False:
            case JsonValueKind.True:
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                VisitPrimitive(token);
                break;
            default:
                throw new FormatException($"Invalid JSON token: {token}");
        }
    }

    private void VisitArray(JsonElement array)
    {
        for (var i = 0; i < array.GetArrayLength(); i++)
        {
            EnterContext(i.ToString());
            VisitToken(array[i]);
            ExitContext();
        }
    }

    private void VisitPrimitive(JsonElement data)
    {
        var key = _currentPath;

        if (_mappings.ContainsKey(key)) throw new FormatException($"Duplicated key: '{key}'");

        _mappings[key] = data.ToString();
    }

    private void EnterContext(string context)
    {
        _stack.Push(context);
        _currentPath = ConfigurationPath.Combine(_stack.Reverse());
    }

    private void ExitContext()
    {
        _stack.Pop();
        _currentPath = ConfigurationPath.Combine(_stack.Reverse());
    }
}