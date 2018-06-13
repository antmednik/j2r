using System;

namespace Jira2Redmine.Redmine.Domain
{
    public class RedmineTimeEntry
    {
        public RedmineTimeEntry(string projectId, string issueId, DateTime workDate, TimeSpan spentTime, string comment)
        {
            ProjectId = string.IsNullOrWhiteSpace(projectId) ? throw new ArgumentNullException(nameof(projectId)) : projectId;
            IssueId = string.IsNullOrWhiteSpace(issueId) ? throw new ArgumentNullException(nameof(issueId)) : issueId;
            WorkDate = workDate;
            SpentTime = spentTime;
            Comment = comment ?? throw new ArgumentNullException(nameof(comment));
        }

        public string ProjectId { get; }

        public string IssueId { get; }

        public DateTime WorkDate { get; }

        public TimeSpan SpentTime { get; }

        public string Comment { get; }
    }
}
