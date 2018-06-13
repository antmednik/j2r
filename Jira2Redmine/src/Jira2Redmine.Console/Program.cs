using Jira2Redmine.Jira;
using System;
using CommandLine;
using Jira2Redmine.Redmine;
using Jira2Redmine.Sync;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jira2Redmine.Console
{
    internal class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<ConsoleArgsParsingOptions>(args)
                .MapResult(options =>
                    {
                        if (options == null)
                        {
                            System.Console.WriteLine("parsing error");
                            return 1;
                        }

                        return Sync(options);
                    },
                    _ => 1);
        }

        private static int Sync(ConsoleArgsParsingOptions options)
        {
            try
            {
                var serviceProvider = CreateServiceProvider(options.RedmineUri, options.RedmineApiKey);

                var syncService = serviceProvider.GetRequiredService<SyncService>();

                syncService.SyncAsync(options.JiraWorkLogFilePath, options.RedmineProjectId, options.RedmineIssueId).GetAwaiter().GetResult();

                return 0;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                throw;
            }
        }

        private static IServiceProvider CreateServiceProvider(string redmineUri, string redmineApiKey)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddLogging()
                .AddSyncService(redmineUri, redmineApiKey);//"https://mc.usetech.ru/redmine", "5e446a7fbdf9064af025cbfe994bba616e2016be");

            var serviceProvider = serviceCollection.BuildServiceProvider();

            serviceProvider
                .GetRequiredService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);

            return serviceProvider;
        }
    }
}
