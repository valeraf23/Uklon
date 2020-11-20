using System.Threading.Tasks;

namespace Uklon.Services
{
    public interface IMaxProfitFinanceYahooService
    {
        Task<double> GetMaxProfit(string companyKey, int from, long to, string interval);
    }
}