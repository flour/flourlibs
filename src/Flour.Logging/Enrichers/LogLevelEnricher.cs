using Serilog.Core;
using Serilog.Events;

namespace Flour.Logging.Enrichers
{
    public class LogLevelEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var level = propertyFactory.CreateProperty("level", logEvent.Level);
            logEvent.AddPropertyIfAbsent(level);
        }
    }
}