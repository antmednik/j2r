using System;
using Microsoft.Extensions.DependencyInjection;
using RedmineApi.Core;

namespace Jira2Redmine.Redmine
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedmineClient(this IServiceCollection self, string host, string apiKey)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (apiKey == null) throw new ArgumentNullException(nameof(apiKey));

            return self
                .AddSingleton<IRedmineClient, RestRedmineClient>()
                .AddSingleton(serviceProvider => new RedmineManager(host, apiKey));
        }
    }
}
