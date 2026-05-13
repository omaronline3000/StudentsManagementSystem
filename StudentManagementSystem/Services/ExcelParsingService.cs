using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace StudentManagementSystem.Services;

public class ExcelParsingService
{
    /// <summary>
    /// Parses an Excel file and extracts student names from the first column.
    /// Expected format: Column A = Student Name
    /// </summary>
    public List<string> ParseAttendanceFile(IFormFile file)
    {
        var names = new List<string>();

        if (file == null || file.Length == 0)
            throw new ArgumentException("Excel file is required and must not be empty.");

        if (!IsValidExcelFile(file.FileName))
            throw new ArgumentException("Invalid file format. Please upload an Excel file (.xlsx).");

        try
        {
            using var stream = file.OpenReadStream();
            using var document = SpreadsheetDocument.Open(stream, false);

            var workbookPart = document.WorkbookPart
                ?? throw new InvalidOperationException("Excel file is corrupted or invalid.");

            var sharedStringTable = workbookPart.SharedStringTablePart?.SharedStringTable;
            var worksheetPart = workbookPart.WorksheetParts.FirstOrDefault()
                ?? throw new InvalidOperationException("No worksheet found in the Excel file.");

            var sheetData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault()
                ?? throw new InvalidOperationException("No data found in the worksheet.");

            foreach (var row in sheetData.Elements<Row>())
            {
                // Check every cell in the row instead of just Column A. 
                // This guarantees we find the name no matter which column they placed it in.
                foreach (var cell in row.Elements<Cell>())
                {
                    var name = GetCellText(cell, sharedStringTable);
                    
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        names.Add(name.Trim());
                    }
                }
            }

            if (names.Count == 0)
                throw new InvalidOperationException("No valid student names were found in the Excel file.");

            return names;
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException($"An unexpected error occurred while parsing the Excel file: {ex.Message}", ex);
        }
    }

    private static string GetCellText(Cell cell, SharedStringTable? sharedStringTable)
    {
        if (cell == null)
            return string.Empty;

        if (cell.DataType?.Value == CellValues.InlineString)
        {
            return cell.InnerText ?? string.Empty;
        }

        if (cell.CellValue == null)
            return cell.InnerText ?? string.Empty;

        var value = cell.CellValue.Text ?? string.Empty;

        if (cell.DataType?.Value == CellValues.SharedString)
        {
            if (int.TryParse(value, out var sharedIndex) && sharedStringTable != null)
            {
                if (sharedIndex >= 0 && sharedIndex < sharedStringTable.ChildElements.Count)
                {
                    return sharedStringTable.ChildElements[sharedIndex].InnerText ?? string.Empty;
                }
            }
        }

        return value;
    }

    private bool IsValidExcelFile(string fileName)
    {
        var validExtensions = new[] { ".xlsx", ".xlsm" };
        var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
        return validExtensions.Contains(fileExtension);
    }
}
