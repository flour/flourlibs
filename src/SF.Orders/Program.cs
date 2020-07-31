using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Flour.Logging;
using Flour.Tracing.Jaeger;

namespace SF.API
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            var host = CreateHostBuilder(args).Build();

            try
            {
                host.Run();
                return 1;
            }
            catch (System.Exception ex)
            {
                Log.Fatal(ex, "API host terminated dramatically");
                return -1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseUrls("http://localhost:6101")
                        .UseLogging()
                        .ConfigureServices(services =>
                        {
                            services
                                .AddJaeger()
                                .AddControllers();
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();
                            app.UseEndpoints(endpoints => endpoints.MapControllers());
                        });
                });
    }
}
