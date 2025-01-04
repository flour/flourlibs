using Serilog;

namespace Flour.Logging.Options;

public enum FilterType
{
    Exclude,
    IncludeOnly
}

public class Filter
{
    public FilterType Type { get; set; }
    public string Expression { get; set; }
}

public class FilteringOptions : ILoggerOptions
{
    public List<Filter> Filters { get; set; } = [];
    public bool Enabled { get; set; }

    public void Configure(LoggerConfiguration configuration)
    {
        if (!Enabled)
            return;

        Filters.ForEach(f => ConfigureFilter(configuration, f));
    }

    private void ConfigureFilter(LoggerConfiguration configuration, Filter filter)
    {
        if (configuration is null || filter is null || string.IsNullOrWhiteSpace(filter.Expression))
            return;

        switch (filter.Type)
        {
            case FilterType.Exclude:
                configuration.Filter.ByExcluding(filter.Expression);
                break;
            case FilterType.IncludeOnly:
                configuration.Filter.ByIncludingOnly(filter.Expression);
                break;
            default:
                return;
        }
    }
}