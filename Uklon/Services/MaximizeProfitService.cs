namespace Uklon.Services
{
    public class MaximizeProfitService : IMaximizeProfitService
    {
        public double Calculate(double[] price)
        {
            var profit = 0.0;

            var j = 0;
            var priceLength = price.Length;
            for (var i = 1; i < priceLength; i++)
            {
                if (price[i - 1] > price[i])
                {
                    j = i;
                }

                if (price[i - 1] <= price[i] &&
                    (i + 1 == priceLength || price[i] > price[i + 1]))
                {
                    profit += (price[i] - price[j]);

                }
            }

            return profit;
        }
    }
}
