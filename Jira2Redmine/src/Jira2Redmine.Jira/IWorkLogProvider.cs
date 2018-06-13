using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Jira2Redmine.Jira.Domain;

namespace Jira2Redmine.Jira
{
    public interface IWorkLogProvider
    {
        WorkLog Get(Stream stream);
    }
}
