using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Uklon.Client;

namespace Uklon.Services
{
    public class ProgramService : IProgramService
    {
        private readonly FinanceYahooClient _client;
        private readonly ILogger _logger;

        public ProgramService(FinanceYahooClient client, ILoggerFactory logger)
        {
            _client = client;
            _logger = logger.CreateLogger<FinanceYahooClient>();
        }

        public async Task Run()
        {
            var googleTask = _client.GetGoogle().ContinueWith(r => _logger.LogInformation($"Google : {r.Result}"));
            var aaplTask = _client.GetAapl().ContinueWith(r => _logger.LogInformation($"AAPL : {r.Result}"));
            var msftTask = _client.GetMsft().ContinueWith(r => _logger.LogInformation($"MSFT : {r.Result}"));
            Task.WaitAll(googleTask, aaplTask, msftTask);
            aaplTask.GetAwaiter().GetResult();
            await Task.CompletedTask;
            _logger.LogInformation("Finish");
        }
    }
}