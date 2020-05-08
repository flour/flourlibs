using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Flour.Logging;
using Flour.RabbitMQ;

namespace SF.API
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            try
            {
                host.Run();
                return 1;
            }
            catch
            {
                return -1;
            }
            finally
            {
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseLogger()
                        .ConfigureServices(services =>
                        {
                            services.AddControllers();
                            // services.AddRabbitMQ();
                        })
                        .Configure(app =>
                        {
                            // app.UseRabbitMQ();
                            app.UseRouting();
                            app.UseEndpoints(endpoints => endpoints.MapControllers());
                        });
                });
    }
}
