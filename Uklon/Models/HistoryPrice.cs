using System;
using Newtonsoft.Json;

namespace Uklon.Models
{
    public class HistoryPrice
    {
        public DateTime Date { get; set; }

        [JsonProperty("open")]
        public double? Open { get; set; }
        [JsonProperty("high")]
        public double? High { get; set; }
        [JsonProperty("low")]
        public double? Low { get; set; }
        [JsonProperty("close")]
        public double? Close { get; set; }
        [JsonProperty("volume")]
        public double? Volume { get; set; }
    }
}