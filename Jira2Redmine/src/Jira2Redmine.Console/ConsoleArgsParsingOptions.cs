using CommandLine;

namespace Jira2Redmine.Console
{
    internal class ConsoleArgsParsingOptions
    {
        [Option('f', Required = true, HelpText = "path to file with JIRA work log", Hidden = false)]
        public string JiraWorkLogFilePath { get; set; }

        [Option('r', Required = true, HelpText = "URI of Redmine", Hidden = false)]
        public string RedmineUri { get; set; }

        [Option('k', Required = true, HelpText = "ApiKey for Redmine", Hidden = false)]
        public string RedmineApiKey { get; set; }

        [Option('p', Required = true, HelpText = "project id in Redmine for logging time", Hidden = false)]
        public string RedmineProjectId { get; set; }

        [Option('i', Required = true, HelpText = "issue id in Redmine for logging time", Hidden = false)]
        public string RedmineIssueId { get; set; }
    }
}
