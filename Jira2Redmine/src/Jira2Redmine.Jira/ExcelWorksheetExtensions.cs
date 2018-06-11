using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using OfficeOpenXml;

namespace Jira2Redmine.Jira
{
    internal static class ExcelWorksheetExtensions
    {
        public static int FindTimeTableHeaderRowNumber(this ExcelWorksheet self)
        {
            const int perProjectHeaderRowNumber = 2;

            if (self.Dimension.Rows == perProjectHeaderRowNumber)
            {
                throw new InvalidOperationException("invalid time report structure");
            }

            for (var rowNumber = perProjectHeaderRowNumber + 1; rowNumber <= self.Dimension.Rows; rowNumber++)
            {
                if (self.Cells[rowNumber, 1].GetValue<string>() == "Project")
                {
                    return rowNumber;
                }
            }

            throw new InvalidOperationException("invalid time report structure");
        }

        public static int FindTimeTableHeaderLastColumnNumber(this ExcelWorksheet self, int headerRowNumber)
        {
            for (var columnNumber = 1; columnNumber <= self.Dimension.Columns; columnNumber++)
            {
                if (self.Cells[headerRowNumber, columnNumber].GetValue<string>() == "Total")
                {
                    return columnNumber;
                }
            }

            throw new InvalidOperationException("invalid time report structure");
        }
    }
}
