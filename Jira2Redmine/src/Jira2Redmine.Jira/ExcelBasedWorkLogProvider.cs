using OfficeOpenXml;
using System;
using System.IO;

namespace Jira2Redmine.Jira
{
    public class ExcelBasedWorkLogProvider 
    {
        public void Read()
        {
            var path = @"C:\tmp\Timesheet Report.xlsx";

            var file = new FileInfo(path);
            using (var excelPackage = new ExcelPackage(file))
            {
                var sheet = excelPackage.Workbook.Worksheets["Timesheet Report"];

                var headerRowNumber = sheet.FindTimeTableHeaderRowNumber();
                var lastColumnNumber = sheet.FindTimeTableHeaderLastColumnNumber(headerRowNumber);

                for(var rowNumber = headerRowNumber + 1; rowNumber < sheet.Dimension.Rows; rowNumber++)
                {
                    for(var columnNumber = 1; columnNumber < lastColumnNumber; columnNumber++)
                    {
                    //    sheet.Cells[]
                    }
                }
            }
        }
    }
}
