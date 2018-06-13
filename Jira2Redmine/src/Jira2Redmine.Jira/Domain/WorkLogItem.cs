using System;

namespace Jira2Redmine.Jira.Domain
{
    public class WorkLogItem
    {
        public Issue Issue { get; set; }

        public DateTime WorkDate { get; set; }

        public TimeSpan SpentTime { get; set; }
    }
}
