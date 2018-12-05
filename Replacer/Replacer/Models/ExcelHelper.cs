using Replacer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Replacer.Models
{
    public static class ExcelHelper
    {
        public static string[,] GetData(FileModel file)
        {
            var path = GetPathToTempFile(file);

            if (String.IsNullOrWhiteSpace(path))
                throw new Exception("Возникли проблемы с созданием временного файла excel. Обратитесь к администратору");
            
            var excelApp = new Microsoft.Office.Interop.Excel.Application();

            //Book
            var workBookExcel = excelApp.Workbooks.Open(path);

            //Table
            var workSheetExcel = (Microsoft.Office.Interop.Excel.Worksheet)workBookExcel.Sheets[1];

            var lastCell = workSheetExcel.Cells.SpecialCells(Microsoft.Office.Interop.Excel.XlCellType.xlCellTypeLastCell);
            var result = new string[lastCell.Column, lastCell.Row];

            for (int i = 0; i < (int)lastCell.Column; i++)
            {
                for (int j = 0; j < (int)lastCell.Row; j++)
                {
                    result[i, j] = workSheetExcel.Cells[j + 1, i + 1].Text.ToString();
                }
            }

            workBookExcel.Close(false, Type.Missing, Type.Missing); // Close without save
            excelApp.Quit(); // Exit from Excel
            GC.Collect(); // Remove your trash

            if (File.Exists(path))
                File.Delete(path); //Remove temp Excel file

            return result;
        }

        private static string GetPathToTempFile(FileModel file)
        {
            var fullName = $"{Environment.CurrentDirectory}\\{new Random().Next(100000, 1000000)}_{file.FileName}"; 
            File.WriteAllBytes(fullName, file.Data);

            return fullName;
        }
    }
}
