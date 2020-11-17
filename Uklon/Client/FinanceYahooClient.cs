using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Uklon.Helpers;
using Uklon.Models;
using Uklon.Services;

namespace Uklon.Client
{
    public class FinanceYahooClient
    {
        private readonly IMaximizeProfitService _maximizeProfit;

        private readonly HttpClient _client;

        public FinanceYahooClient(IHttpClientFactory httpClient, IMaximizeProfitService maximizeProfit)
        {
            _maximizeProfit = maximizeProfit;
            _client = httpClient.CreateClient(HttpClientName.FinanceYahooClient);
        }

        private async Task<List<HistoryPrice>> GetHistoryPricesFor(string companyKey)
        {
            var urlPart = GetRequestString(companyKey);
            using var response = await _client.GetAsync(urlPart);
            ThrowIfHttpError(response, urlPart);
            var str = await response.Content.ReadAsStreamAsync();
            var jo = str.ReadJObject();
            var dates = GetDates(jo);
            var quote = GetQuote(jo);

            var historyPrices = dates.Select((t, i) => new HistoryPrice
                {
                    Date = t,
                    Close = quote.close[i],
                    High = quote.high[i],
                    Low = quote.low[i],
                    Open = quote.open[i],
                    Volume = quote.volume[i]
                })
                .ToList();
            return historyPrices;
        }

        private static Quote GetQuote(JObject jo)
        {
            const string quotePath = "chart.result[0].indicators.quote[0]";
            return jo.SelectToken(quotePath)
                .ToObject<Quote>();
        }

        private IEnumerable<DateTime> GetDates(JObject jo)
        {
            const string timestampPath = "chart.result[0].timestamp";
            var timestamps = jo.SelectToken(timestampPath)
                .Select(t => UnixTimeToDateTime(t.Value<long>()))
                .ToList();
            return timestamps;
        }

        public async Task<double> GetGoogle()
        {
            const string companyKey = "GOOGL";
            return await GetMaxProfit(companyKey);
        }

        private async Task<double> GetMaxProfit(string companyKey)
        {
            var historyPrices = await GetHistoryPricesFor(companyKey);
            var prices = historyPrices.Where(h => h.Open != null && h.Close != null)
                .Select(x => new[] {x.Open.Value, x.Close.Value}).SelectMany(p => p).ToArray();
            var res = _maximizeProfit.Calculate(prices);
            return res;
        }

        public async Task<double> GetAAPL()
        {
            const string companyKey = "AAPL";
            return await GetMaxProfit(companyKey);
        }
        
        public async Task<double> GetMSFT()
        {
            const string companyKey = "MSFT";
            return await GetMaxProfit(companyKey);
        }

        public DateTime UnixTimeToDateTime(long unixTime)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTime).ToLocalTime();
            return dtDateTime;
        }

        private static bool IsHttpError(HttpResponseMessage response)
        {
            var num = (int) response.StatusCode / 100;

            return (num == 4 || num == 5);
        }

        private static void ThrowIfHttpError(HttpResponseMessage response, string url)
        {
            if (!IsHttpError(response))
                return;
            throw new HttpException(response.StatusCode,
                $"Error: {response.ReasonPhrase}\nHTTP: {response.StatusCode}\nURL: {url}");
        }

        private static string GetRequestString(string companyKey) =>
            $"?symbol={companyKey}&period1=0&period2=9999999999&interval=1d";
    }
}