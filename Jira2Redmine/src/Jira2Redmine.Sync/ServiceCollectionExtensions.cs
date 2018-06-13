using System;
using Jira2Redmine.Jira;
using Jira2Redmine.Redmine;
using Microsoft.Extensions.DependencyInjection;
using RedmineApi.Core;

namespace Jira2Redmine.Sync
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSyncService(this IServiceCollection self, string host, string apiKey)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (apiKey == null) throw new ArgumentNullException(nameof(apiKey));

            return self
                .AddRedmineClient(host, apiKey)
                .AddJiraWorkLogProvider()
                .AddSingleton<SyncService>();
        }
    }
}
