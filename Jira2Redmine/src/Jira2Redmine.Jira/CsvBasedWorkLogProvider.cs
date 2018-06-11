using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Jira2Redmine.Jira
{
    public class CsvBasedWorkLogProvider : IWorkLogProvider
    {
        private class TimeTableHeader
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

        private static readonly string[] DefaultSeparator = { ";" };
        private static readonly Regex TimeTableBorderRegex = new Regex(@"^Total\s\(\d+ issues\)$", RegexOptions.Compiled);

        private readonly string[] _separator;

        static CsvBasedWorkLogProvider()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public CsvBasedWorkLogProvider(string separator = null)
        {
            _separator = string.IsNullOrEmpty(separator) ? DefaultSeparator : new [] { separator };
        }

        public IList<WorkLogItem> Get()
        {
            var workLogItems = new List<WorkLogItem>();

            using (var reader = new StreamReader(@"C:\tmp\Timesheet Report.csv", Encoding.GetEncoding(1251)))
            {
                var line = reader.ReadLine();

                while (line != null)
                {
                    if (IsTimeTableBorderFound(line))
                    {
                        var rawHeader = ReadHeader(reader);
                        var header = ParseTimeTableHeader(rawHeader);

                        line = reader.ReadLine();
                        while (!IsTimeTableBorderFound(line))
                        {
                            workLogItems.AddRange(Create(header, line));

                            line = reader.ReadLine();
                        }
                    }

                    line = reader.ReadLine();
                }
            }

            return workLogItems;
        }

        private bool IsTimeTableBorderFound(string line)
        {
            var fields = line.Split(_separator, StringSplitOptions.None);
            return TimeTableBorderRegex.IsMatch(fields[0]);
        }

        private string ReadHeader(TextReader reader)
        {
            const string headerBordersMarker = "Project;Issue";

            var firstHeaderPart = reader.ReadLine();
            if (firstHeaderPart == null || !firstHeaderPart.Contains(headerBordersMarker))
            {
                throw new InvalidOperationException("can not read header");
            }

            var headerBuilder = new StringBuilder(firstHeaderPart.TrimEnd('\n'));

            var line = reader.ReadLine();
            while (line != null && !line.Contains(headerBordersMarker))
            {
                headerBuilder.Append(line.TrimEnd('\n'));
                line = reader.ReadLine();
            }

            if (line != null)
            {
                headerBuilder.Append(line);
            }

            return headerBuilder.ToString();
        }

        private TimeTableHeader ParseTimeTableHeader(string rawHeader)
        {
            TimeTableHeader header = null;
            
            var fields = rawHeader.Split(_separator, StringSplitOptions.None);

            int firstWorkDateIndex = -1;
            for (var index = 0; index < fields.Length; index++)
            {
                var value = fields[index];
                if (!string.IsNullOrEmpty(value) && TryParseWorkDate(fields[index], out var workDate))
                {
                    header = new TimeTableHeader(index);
                    header.Add(workDate);

                    firstWorkDateIndex = index;
                    break;
                }
            }

            if (header == null)
            {
                throw new InvalidOperationException("can not parse time table header");
            }

            for (var index = firstWorkDateIndex + 1; index < fields.Length; index++)
            {
                if (!TryParseWorkDate(fields[index], out var workDate))
                {
                    break;
                }

                header.Add(workDate);
            }

            return header;
        }

        private IEnumerable<WorkLogItem> Create(TimeTableHeader header, string line)
        {
            var csvFileds = line.Split(_separator, StringSplitOptions.None);

            var itemPrototype = new WorkLogItemPrototype(csvFileds[0], csvFileds[2].Trim(), csvFileds[3].Trim());
            
            foreach (var (columnIndex, workDate) in header.GetColumnIndexAndWorkDatePairs())
            {
                var rawSpentTime = csvFileds[columnIndex];
                if (!string.IsNullOrEmpty(rawSpentTime))
                {
                    yield return itemPrototype.Create(workDate, ParseSpentTime(rawSpentTime));
                }
            }
        }

        private static bool TryParseWorkDate(string rawDate, out DateTime date)
        {
            return DateTime.TryParse(rawDate.Replace("\"", string.Empty), out date);
        }

        private static TimeSpan ParseSpentTime(string rawSpentTime)
        {
            if (rawSpentTime.EndsWith("h"))
            {
                var rawHoursCount = rawSpentTime.Substring(0, rawSpentTime.Length - 1);
                if (decimal.TryParse(rawHoursCount, out var summHoursCount))
                {
                    var hoursCount = Convert.ToInt32(summHoursCount / 10);
                    int minutesCount = Convert.ToInt32(60 * (summHoursCount % 10));
                    return new TimeSpan(0, hoursCount, minutesCount);
                }
            }

            throw new ArgumentOutOfRangeException(nameof(rawSpentTime), rawSpentTime, "can not parse");
        }
    }
}
