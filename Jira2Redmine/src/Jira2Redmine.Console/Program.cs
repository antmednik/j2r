using Jira2Redmine.Jira;
using System;

namespace Jira2Redmine.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            new CsvBasedWorkLogProvider().Get();

            System.Console.ReadLine();
        }
    }
}
