using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Flour.IOBox
{
    public interface IOutboxConfig
    {
        IServiceCollection Services { get; }
        IOptions<InOutSettings> OutboxSettings { get; }
    }

    internal class OutboxConfig : IOutboxConfig
    {
        public IServiceCollection Services { get; }
        public IOptions<InOutSettings> OutboxSettings { get; }

        public OutboxConfig(IServiceCollection services, IOptions<InOutSettings> settings)
        {
            Services = services;
            OutboxSettings = settings;
        }
    }
}