using System;

namespace Jira2Redmine.Redmine
{
    public class RedmineOperationException : Exception
    {
        public RedmineOperationException(string message, Exception baseException) : base(message, baseException)
        {
        }
    }
}
