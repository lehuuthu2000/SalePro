using System;
using System.Data;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using System.Windows.Forms;

namespace helloworld.Services
{
    public class ExcelService
    {
        public ExcelService()
        {
            // Set license context for EPPlus (Free for non-commercial)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        /// <summary>
        /// Exports a DataTable to an Excel file.
        /// </summary>
        /// <param name="dt">The DataTable to export.</param>
        /// <param name="sheetName">The name of the worksheet.</param>
        /// <param name="filePath">The file path to save to.</param>
        public void ExportToExcel(DataTable dt, string sheetName, string filePath)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                
                // Format header
                using (var range = worksheet.Cells[1, 1, 1, dt.Columns.Count])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                worksheet.Cells.AutoFitColumns();
                package.SaveAs(new FileInfo(filePath));
            }
        }

        /// <summary>
        /// Opens a SaveFileDialog to let user choose where to save the Excel file.
        /// </summary>
        public string? ShowSaveFileDialog(string defaultFileName)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook|*.xlsx";
                sfd.Title = "Save as Excel";
                sfd.FileName = defaultFileName;
                
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    return sfd.FileName;
                }
            }
            return null;
        }

        /// <summary>
        /// Reads an Excel file into a DataTable.
        /// Assumes first row is header.
        /// </summary>
        public DataTable ImportFromExcel(string filePath)
        {
            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    if (package.Workbook.Worksheets.Count == 0)
                        throw new Exception("File Excel không có sheet nào.");

                    var worksheet = package.Workbook.Worksheets[0]; // First sheet
                    DataTable dt = new DataTable();

                    if (worksheet.Dimension == null)
                        return dt; // Empty sheet

                    int colCount = worksheet.Dimension.End.Column;
                    int rowCount = worksheet.Dimension.End.Row;

                    // Header row
                    for (int col = 1; col <= colCount; col++)
                    {
                        string colName = worksheet.Cells[1, col].Value?.ToString() ?? $"Column{col}";
                        dt.Columns.Add(colName);
                    }

                    // Data rows
                    for (int row = 2; row <= rowCount; row++)
                    {
                        DataRow dr = dt.NewRow();
                        bool hasData = false;
                        for (int col = 1; col <= colCount; col++)
                        {
                            object val = worksheet.Cells[row, col].Value;
                            dr[col - 1] = val;
                            if (val != null && !string.IsNullOrWhiteSpace(val.ToString()))
                                hasData = true;
                        }
                        if (hasData)
                            dt.Rows.Add(dr);
                    }
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đọc file Excel: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Opens an OpenFileDialog to let user choose an Excel file to import.
        /// </summary>
        public string? ShowOpenFileDialog()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Excel Workbook|*.xlsx|All Files|*.*";
                ofd.Title = "Select Excel File";
                
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    return ofd.FileName;
                }
            }
            return null;
        }
    }
}
