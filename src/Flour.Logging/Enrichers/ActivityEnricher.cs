using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace Flour.Logging;

public class ActivityEnricher : ILogEventEnricher
{
    private readonly string[] _keys;

    public ActivityEnricher(params string[] keys)
    {
        _keys = keys;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        void SetKeyValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            var value = Activity.Current?.GetBaggageItem(key);
            if (string.IsNullOrWhiteSpace(value))
                return;

            Patch(key, value);
        }

        void Patch(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key)) return;

            var property = propertyFactory.CreateProperty(key, value, true);
            logEvent.AddPropertyIfAbsent(property);
        }

        if (_keys == null)
            return;

        foreach (var key in _keys) SetKeyValue(key);
    }
}