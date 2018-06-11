using System;
using System.Collections.Generic;
using System.Text;

namespace Jira2Redmine.Jira
{
    interface IWorkLogProvider
    {
        IList<WorkLogItem> Get();
    }
}
