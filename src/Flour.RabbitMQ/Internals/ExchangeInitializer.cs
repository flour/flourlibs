using System.Reflection;
using Flour.BrokersContracts;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Flour.RabbitMQ.Internals;

internal class ExchangeInitializer
{
    private readonly IConnection _connection;
    private readonly ILogger<ExchangeInitializer> _logger;
    private readonly RabbitMqOptions _options;

    public ExchangeInitializer(
        IConnection connection,
        RabbitMqOptions options,
        ILogger<ExchangeInitializer> logger)
    {
        _connection = connection;
        _options = options;
        _logger = logger;
    }

    public Task Initialize()
    {
        var exchanges = AppDomain.CurrentDomain.GetAssemblies().SelectMany(e => e.GetTypes())
            .Where(e => e.IsDefined(typeof(MessagingAttribute), false))
            .Select(e => e.GetCustomAttribute<MessagingAttribute>().Exchange ?? e.Name)
            .Where(e => !e.Equals(_options.Exchange?.Name ?? string.Empty))
            .Distinct()
            .ToList();

        using var channel = _connection.CreateModel();
        if (_options.Exchange.Declare)
        {
            _logger.LogInformation("Declaring new exchange {Exchange} with {Type}",
                _options.Exchange.Name, _options.Exchange.Type);
            channel.ExchangeDeclare(_options.Exchange.Name, _options.Exchange.Type, _options.Exchange.Durable,
                _options.Exchange.AutoDelete);
        }

        exchanges.ForEach(e =>
        {
            _logger.LogInformation("Declaring new exchange {Exchange} with {Type}", e, "topic");
            channel.ExchangeDeclare(e, "topic", true);
        });

        channel.Close();
        return Task.CompletedTask;
    }
}