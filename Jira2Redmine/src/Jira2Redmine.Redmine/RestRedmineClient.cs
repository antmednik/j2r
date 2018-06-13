using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Jira2Redmine.Redmine.Domain;
using RedmineApi.Core;
using RedmineApi.Core.Types;

namespace Jira2Redmine.Redmine
{
    internal class RestRedmineClient : IRedmineClient
    {
        private static readonly NameValueCollection EmptyNameValueCollection = new NameValueCollection();

        private readonly RedmineManager _client;
        private readonly ConcurrentDictionary<string, IdentifiableName> _objectIdToInternalIdMap;

        public RestRedmineClient(RedmineManager client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _objectIdToInternalIdMap = new ConcurrentDictionary<string, IdentifiableName>();
        }

        public async Task<string> CreateTimeEntryAsync(RedmineTimeEntry timeEntry)
        {
            if (timeEntry == null) throw new ArgumentNullException(nameof(timeEntry));
            
            var projectInternalId = await GetProjectInternalIdAsync(timeEntry.ProjectId);
            var issueInternalId = await GetIssueInternalIdAsync(timeEntry.IssueId);

            var internalTimeEntry = new TimeEntry
            {
                Project = projectInternalId,
                Issue = issueInternalId,
                SpentOn = timeEntry.WorkDate,
                Hours = Convert.ToDecimal(timeEntry.SpentTime.TotalHours),
                Comments = timeEntry.Comment
            };

            var createdEntry = await _client.Create(internalTimeEntry);

            return createdEntry.Id.ToString();
        }

        public async Task DeleteTimeEntryAsync(string id)
        {
            try
            {
                var result = await _client.Delete<TimeEntry>(id);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            
        }

        public async Task<bool> CheckProjectExistsAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

            try
            {
                var project = await _client.Get<Project>(id, EmptyNameValueCollection);
                return project != null;
            }
            catch (Exception ex)
            {
                throw new RedmineOperationException("unable to check project", ex);
            }
        }

        public async Task<bool> CheckIssueExistsInProjectAsync(string issueId, string projectId)
        {
            if (string.IsNullOrWhiteSpace(issueId)) throw new ArgumentNullException(nameof(issueId));
            if (string.IsNullOrWhiteSpace(projectId)) throw new ArgumentNullException(nameof(projectId));

            try
            {
                var issue = await _client.Get<Issue>(issueId, EmptyNameValueCollection);
                if (issue == null)
                {
                    return false;
                }

                return issue.Project?.Id.ToString() == projectId;
            }
            catch (Exception ex)
            {
                throw new RedmineOperationException("unable to check issue", ex);
            }
        }

        private Task<IdentifiableName> GetProjectInternalIdAsync(string projectId) => GetObjectInternalIdAsync(
            RedmineKeys.PROJECT,
            projectId,
            async () => await _client.Get<Project>(projectId, new NameValueCollection()) as IdentifiableName);

        private Task<IdentifiableName> GetIssueInternalIdAsync(string issueId) => GetObjectInternalIdAsync(
            RedmineKeys.ISSUE,
            issueId,
            () => _client.Get<Issue>(issueId, new NameValueCollection()));

        private async Task<IdentifiableName> GetObjectInternalIdAsync<T>(
            string objectType, 
            string objectId,
            Func<Task<T>> objectProvider) where T : Identifiable<T>, IEquatable<T>
        {
            var cacheKey = GetCacheKey(objectType, objectId);

            if (_objectIdToInternalIdMap.TryGetValue(cacheKey, out var id))
            {
                return id;
            }

            var targetObject = await objectProvider();
            if (targetObject == null)
            {
                throw new InvalidOperationException($"not found object with type = {objectType} with id = {objectId}");
            }

            return _objectIdToInternalIdMap.GetOrAdd(cacheKey, new IdentifiableName { Id = targetObject.Id});
        }

        private static string GetCacheKey(string category, string id) => $"{category}::{id}";
    }
}
