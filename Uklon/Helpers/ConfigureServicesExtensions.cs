using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Uklon.Client;
using Uklon.Policy;

namespace Uklon.Helpers
{
    public static class ConfigureServicesExtensions
    {
        private const string PoliciesConfigurationSectionName = "Policies";
        private const string BaseUrl = "BaseUrl";
      

        public static IServiceCollection ConfigureServicesForFinanceYahooClient(this IServiceCollection services, IConfiguration configuration)
        {
            var baseUrl = configuration.GetSection(BaseUrl);

            services.AddHttpClient(HttpClientName.FinanceYahooClient, client =>
                {
                    client.Timeout = TimeSpan.FromMinutes(5);
                    client.BaseAddress = new Uri(baseUrl.Value);
                    client.DefaultRequestHeaders.Clear();
                }).AddPolicyHandlerFromRegistry(PolicyName.HttpRetry)
                .AddPolicyHandlerFromRegistry(PolicyName.HttpCircuitBreaker);
            services.AddScoped<FinanceYahooClient>();
            return services;
        }

        public static IServiceCollection AddPolicies(
            this IServiceCollection services,
            IConfiguration configuration,
            string configurationSectionName = PoliciesConfigurationSectionName)
        {
            var section = configuration.GetSection(configurationSectionName);
            services.Configure<PolicyOptions>(configuration);
            var policyOptions = section.Get<PolicyOptions>();

            var policyRegistry = services.AddPolicyRegistry();
            policyRegistry.Add(
                PolicyName.HttpRetry,
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        policyOptions.HttpRetry.Count,
                        retryAttempt =>
                            TimeSpan.FromSeconds(Math.Pow(policyOptions.HttpRetry.Power, retryAttempt))));
            policyRegistry.Add(
                PolicyName.HttpCircuitBreaker,
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: policyOptions.HttpCircuitBreaker
                            .ExceptionsAllowedBeforeBreaking,
                        durationOfBreak: policyOptions.HttpCircuitBreaker.DurationOfBreak));

            return services;
        }
    }
}