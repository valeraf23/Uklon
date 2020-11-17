namespace Uklon.Policy
{
    public class RetryPolicyOptions
    {
        public int Count { get; set; } = 3;
        public int Power { get; set; } = 2;
    }
}