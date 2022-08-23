using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace Flour.Vault.Internals;

internal class JsonConfigurationParser
{
    private readonly IDictionary<string, string> _mappings =
        new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    private readonly Stack<string> _stack = new();

    private string _currentPath;

    public IDictionary<string, string> Parse(JObject jObject)
    {
        VisitJObject(jObject);

        return _mappings;
    }

    private void VisitJObject(JObject jObject)
    {
        foreach (var property in jObject.Properties())
        {
            EnterContext(property.Name);
            VisitProperty(property);
            ExitContext();
        }
    }

    private void VisitProperty(JProperty property)
    {
        VisitToken(property.Value);
    }

    private void VisitToken(JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                VisitJObject(token.Value<JObject>());
                break;
            case JTokenType.Array:
                VisitArray(token.Value<JArray>());
                break;
            case JTokenType.Integer:
            case JTokenType.Float:
            case JTokenType.String:
            case JTokenType.Boolean:
            case JTokenType.Bytes:
            case JTokenType.Raw:
            case JTokenType.Null:
            case JTokenType.Date:
            case JTokenType.Guid:
            case JTokenType.Uri:
            case JTokenType.TimeSpan:
                VisitPrimitive(token);
                break;
            default:
                throw new FormatException($"Invalid JSON token: {token}");
        }
    }

    private void VisitArray(JArray array)
    {
        for (var i = 0; i < array.Count; i++)
        {
            EnterContext(i.ToString());
            VisitToken(array[i]);
            ExitContext();
        }
    }

    private void VisitPrimitive(JToken data)
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