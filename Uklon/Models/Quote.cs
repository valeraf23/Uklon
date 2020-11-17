using System.Collections.Generic;

namespace Uklon.Models
{
    public class Quote
    {
        public List<double?> open { get; set; }
        public List<double?> volume { get; set; }
        public List<double?> close { get; set; }
        public List<double?> high { get; set; }
        public List<double?> low { get; set; }
    }
}