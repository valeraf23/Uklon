using System.Threading.Tasks;
using Uklon.Helpers;
using Uklon.Services;

namespace Uklon.Client
{
    public class FinanceYahooClient
    {
        private readonly IMaxProfitFinanceYahooService _maxProfitFinanceYahooService;

        public FinanceYahooClient(IMaxProfitFinanceYahooService maximizeProfit)
        {
            _maxProfitFinanceYahooService = maximizeProfit;
        }

        public async Task<double> GetGoogle(int from = DefaultQueryParameters.From,
            long to = DefaultQueryParameters.To,
            string interval = DefaultQueryParameters.Interval)
        {
            const string companyKey = "GOOGL";
            return await _maxProfitFinanceYahooService.GetMaxProfit(companyKey, from, to, interval);
        }

        public async Task<double> GetAapl(int from = DefaultQueryParameters.From,
            long to = DefaultQueryParameters.To,
            string interval = DefaultQueryParameters.Interval)
        {
            const string companyKey = "AAPL";
            return await _maxProfitFinanceYahooService.GetMaxProfit(companyKey, from, to, interval);
        }

        public async Task<double> GetMsft(int from = DefaultQueryParameters.From,
            long to = DefaultQueryParameters.To,
            string interval = DefaultQueryParameters.Interval)
        {
            const string companyKey = "MSFT";
            return await _maxProfitFinanceYahooService.GetMaxProfit(companyKey, from, to, interval);
        }
    }
}