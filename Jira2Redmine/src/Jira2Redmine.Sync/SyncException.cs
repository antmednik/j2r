using System;

namespace Jira2Redmine.Sync
{
    internal class SyncException : Exception
    {
        public SyncException(string message, Exception baseException = null) : base(message, baseException)
        {
        }

        public SyncException(Exception baseException) : base(baseException.Message, baseException)
        {
        }
    }
}
