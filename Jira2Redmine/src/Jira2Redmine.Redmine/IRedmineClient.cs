using System;
using System.Threading.Tasks;
using Jira2Redmine.Redmine.Domain;

namespace Jira2Redmine.Redmine
{
    public interface IRedmineClient
    {
        Task<string> CreateTimeEntryAsync(RedmineTimeEntry timeEntry);

        Task DeleteTimeEntryAsync(string id);

        Task<bool> CheckProjectExistsAsync(string id);

        Task<bool> CheckIssueExistsInProjectAsync(string issueId, string projectId);
    }
}
