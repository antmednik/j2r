using System;
using System.Collections.Generic;
using System.Text;

namespace Jira2Redmine.Jira
{
    public class Issue
    {
        public string ProjectName { get; set; }

        public string Key { get; set; }

        public string Summary { get; set; }
    }
}
