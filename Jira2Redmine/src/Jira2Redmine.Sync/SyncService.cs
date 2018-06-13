using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Jira2Redmine.Jira;
using Jira2Redmine.Jira.Domain;
using Jira2Redmine.Redmine;
using Jira2Redmine.Redmine.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Jira2Redmine.Sync
{
    public class SyncService
    {
        private readonly IWorkLogProvider _workLogProvider;
        private readonly IRedmineClient _redmineClient;
        private readonly ILogger _logger;

        public SyncService(
            IWorkLogProvider workLogProvider, 
            IRedmineClient redmineClient,
            ILoggerFactory loggerFactory = null)
        {
            _workLogProvider = workLogProvider ?? throw new ArgumentNullException(nameof(workLogProvider));
            _redmineClient = redmineClient ?? throw new ArgumentNullException(nameof(redmineClient));
            _logger = loggerFactory?.CreateLogger<SyncService>() ?? NullLogger<SyncService>.Instance;
        }

        public async Task SyncAsync(string workLogFilePath, string redmineProjectId, string redmineIssueId)
        {
            if (string.IsNullOrEmpty(workLogFilePath)) throw new ArgumentNullException(nameof(workLogFilePath));
            if (string.IsNullOrEmpty(redmineProjectId)) throw new ArgumentNullException(nameof(redmineProjectId));
            if (string.IsNullOrEmpty(redmineIssueId)) throw new ArgumentNullException(nameof(redmineIssueId));

            try
            {
                var workLog = ReadWorkLog(workLogFilePath);

                await LogWorkInRedmineAsync(redmineProjectId, redmineIssueId, workLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "unable to sync");
            }
        }

        private WorkLog ReadWorkLog(string workLogFilePath)
        {
            try
            {
                using (var stream = new FileStream(workLogFilePath, FileMode.Open, FileAccess.Read))
                {
                    var workLog = _workLogProvider.Get(stream);

                    _logger.LogInformation("work log read from '{0}'", workLogFilePath);

                    return workLog;
                }
            }
            catch (Exception ex)
            {
                throw new SyncException("unable to read work log", ex);
            }
        }

        private async Task LogWorkInRedmineAsync(string redmineProjectId, string redmineIssueId, WorkLog workLog)
        {
            await ValidateRedminePrerequisitesAsync(redmineProjectId, redmineIssueId);

            var createdTimeEntryIds = new List<string>();
            try
            {
                foreach (var workLogItem in workLog.Items)
                {
                    var redmineTimeEntry = Map(redmineProjectId, redmineIssueId, workLogItem);
                    var timeEntryId = await _redmineClient.CreateTimeEntryAsync(redmineTimeEntry);

                    createdTimeEntryIds.Add(timeEntryId);

                    _logger.LogInformation(
                        "[{0}/{1}] time entry logged in redmine - id: {2}, date: {3}, spentTime: {4}, comment: {5}",
                        createdTimeEntryIds.Count,
                        workLog.Items.Count,
                        timeEntryId,
                        redmineTimeEntry.WorkDate,
                        redmineTimeEntry.SpentTime, 
                        redmineTimeEntry.Comment);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "unable to log time in redmine, {0}/{1} time entries logged", createdTimeEntryIds.Count, workLog.Items.Count);
            }

            var needToRollbackLogging = createdTimeEntryIds.Count < workLog.Items.Count;
            if (needToRollbackLogging)
            {
                foreach (var timeEntryId in createdTimeEntryIds)
                {
                    try
                    {
                        await _redmineClient.DeleteTimeEntryAsync(timeEntryId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "unable to delete time entry id = {0} in redmine", timeEntryId);
                    }
                }
            }
        }

        private async Task ValidateRedminePrerequisitesAsync(string projectId, string issueId)
        {
            try
            {
                var projectFound = await _redmineClient.CheckProjectExistsAsync(projectId);
                if (!projectFound)
                {
                    throw new InvalidOperationException($"not found project with id = '{projectId}'");
                }

                var issueFound = await _redmineClient.CheckIssueExistsInProjectAsync(issueId, projectId);
                if (issueFound)
                {
                    throw new InvalidOperationException($"not found issue with id = '{issueId}' in project with id = '{projectId}'");
                }
            }
            catch (Exception ex)
            {
                throw new SyncException(ex);
            }
        }

        private static RedmineTimeEntry Map(string redmineProjectId, string redmineIssueId, WorkLogItem workLogItem)
        {
            return new RedmineTimeEntry(redmineProjectId, redmineIssueId, workLogItem.WorkDate, workLogItem.SpentTime, workLogItem.Issue.Key + " " + workLogItem.Issue.Summary);
        }
    }
}
