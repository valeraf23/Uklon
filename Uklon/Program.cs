using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Uklon.Helpers;
using Uklon.Services;

namespace Uklon
{
    internal class Program
    {
        private static IConfiguration _configuration;

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json");

            _configuration = builder.Build();
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
           await serviceProvider!.GetService<IProgramService>()!.Run();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(configure => configure.AddConsole());
            serviceCollection
                .AddPolicies(_configuration)
                .ConfigureServicesForFinanceYahooClient(_configuration);
            serviceCollection.AddSingleton<IMaximizeProfitService,MaximizeProfitService>();
            serviceCollection.AddScoped<IProgramService, ProgramService>();
        }
    }
}
