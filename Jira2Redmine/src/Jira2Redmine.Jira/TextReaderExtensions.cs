using System.Collections.Generic;
using System.IO;

namespace Jira2Redmine.Jira
{
    internal static class TextReaderExtensions
    {
        public static IEnumerable<string> ReadLines(this TextReader self, char delimiter)
        {
            var chars = new List<char>();

            while (self.Peek() >= 0)
            {
                var c = (char)self.Read();

                if (c != delimiter)
                {
                    chars.Add(c);
                }

                yield return new string(chars.ToArray());

                chars.Clear();
            }
        }
    }
}
