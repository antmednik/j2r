using System;
using System.Collections.Generic;

namespace Jira2Redmine.Jira.Domain
{
    public class WorkLog
    {
        public WorkLog(ICollection<WorkLogItem> items)
        {
            Items = items ?? throw new ArgumentNullException();
        }

        public ICollection<WorkLogItem> Items { get; set; }
    }
}
