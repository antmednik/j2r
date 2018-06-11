using System;

namespace Jira2Redmine.Jira
{
    internal class WorkLogItemPrototype
    {
        private readonly Issue _issue;

        public WorkLogItemPrototype(string projectName, string issueKey, string summary)
        {
            if (string.IsNullOrEmpty(projectName)) throw new ArgumentNullException(nameof(projectName));
            if (string.IsNullOrEmpty(issueKey)) throw new ArgumentNullException(nameof(issueKey));
            if (string.IsNullOrEmpty(summary)) throw new ArgumentNullException(nameof(summary));

            _issue = new Issue
            {
                ProjectName = projectName,
                Key = issueKey,
                Summary = summary
            };
        }

        public WorkLogItem Create(DateTime workDate, TimeSpan spentTime)
        {
            if (workDate == default(DateTime)) throw new ArgumentOutOfRangeException(nameof(workDate));
            if (spentTime == default(TimeSpan)) throw new ArgumentOutOfRangeException(nameof(spentTime));

            return new WorkLogItem
            {
                Issue = _issue,
                WorkDate = workDate,
                SpentTime = spentTime
            };
        }
    }
}
