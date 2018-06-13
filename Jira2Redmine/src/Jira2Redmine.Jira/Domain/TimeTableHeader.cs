using System;
using System.Collections.Generic;

namespace Jira2Redmine.Jira.Domain
{
    internal class TimeTableHeader
    {
        private readonly List<DateTime> _workDates = new List<DateTime>();
        private readonly int _baseColumnOffset;

        public TimeTableHeader(int baseColumnOffset)
        {
            _baseColumnOffset = baseColumnOffset;
        }

        public void Add(DateTime workDate)
        {
            _workDates.Add(workDate);
        }

        public IEnumerable<(int columnIndex, DateTime workDate)> GetColumnIndexAndWorkDatePairs()
        {
            for (var index = 0; index < _workDates.Count; index++)
            {
                yield return (index + _baseColumnOffset, _workDates[index]);
            }
        }
    }
}
