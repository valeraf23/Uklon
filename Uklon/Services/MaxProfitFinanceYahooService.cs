using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Uklon.Helpers;
using Uklon.Models;

namespace Uklon.Services
{
    public class MaxProfitFinanceYahooService : IMaxProfitFinanceYahooService
    {
        private readonly HttpClient _client;
        private readonly IMaximizeProfitService _maximizeProfit;

        public MaxProfitFinanceYahooService(IMaximizeProfitService maximizeProfit, IHttpClientFactory client)
        {
            _maximizeProfit = maximizeProfit;
            _client = client.CreateClient(HttpClientName.FinanceYahooClient);
        }

        public async Task<double> GetMaxProfit(string companyKey, int from, long to, string interval)
        {
            var historyPrices = await GetHistoryPricesFor(companyKey, from, to, interval);
            var prices = historyPrices.Where(h => h.Open.HasValue && h.Close.HasValue)
                .SelectMany(x => new[] {x.Open!.Value, x.Close!.Value}).ToArray();
            var res = _maximizeProfit.Calculate(prices);
            return res;
        }

        private async Task<List<HistoryPrice>> GetHistoryPricesFor(string companyKey,
            int from,
            long to,
            string interval)
        {
            var urlPart = GetRequestString(companyKey, from, to, interval);
            using var response = await _client.GetAsync(urlPart);
            ThrowIfHttpError(response, urlPart);
            var str = await response.Content.ReadAsStreamAsync();
            var jo = str.ReadJObject();
            return ConvertToHistoryPrices(jo);
        }

        private static List<HistoryPrice> ConvertToHistoryPrices(JObject jo)
        {
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
            return jo.SelectToken(quotePath)?.ToObject<Quote>();
        }

        private static IEnumerable<DateTime> GetDates(JObject jo)
        {
            const string timestampPath = "chart.result[0].timestamp";
            var timestamps = jo.SelectToken(timestampPath)?
                .Select(t => UnixTimeToDateTime(t.Value<long>()))
                .ToList();
            return timestamps;
        }

        private static DateTime UnixTimeToDateTime(long unixTime)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTime).ToLocalTime();
            return dtDateTime;
        }

        private static bool IsHttpError(HttpResponseMessage response)
        {
            var num = (int) response.StatusCode / 100;
            return num == 4 || num == 5;
        }

        private static void ThrowIfHttpError(HttpResponseMessage response, string url)
        {
            if (!IsHttpError(response))
                return;
            throw new HttpException(response.StatusCode,
                $"Error: {response.ReasonPhrase}\nHTTP: {response.StatusCode}\nURL: {url}");
        }

        private static string GetRequestString(string companyKey, int from, long to, string interval)
        {
            var parameters = new Dictionary<string, string>
            {
                {"symbol", companyKey},
                {"period1", from.ToString()},
                {"period2", to.ToString()},
                {"interval", interval}
            };
            return QueryString.Create(parameters).Value;
        }
    }
}