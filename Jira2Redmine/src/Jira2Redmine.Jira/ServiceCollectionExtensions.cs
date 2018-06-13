using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Jira2Redmine.Jira
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJiraWorkLogProvider(this IServiceCollection self)
        {
            return self
                .AddSingleton<IWorkLogProvider, CsvBasedWorkLogProvider>();
        }
    }
}
