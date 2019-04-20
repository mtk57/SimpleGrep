using System.IO;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;
using System;

namespace GrepLib
{
    public static class MsOfficeHelper
    {
        private static Microsoft.Office.Interop.Excel.Application _excelApp = null;
        private static Microsoft.Office.Interop.Word.Application _wordApp = null;

        public static void CreateMsOfficeApp()
        {
            try
            {
                _excelApp = new Microsoft.Office.Interop.Excel.Application();
            }
            catch
            {
                throw new Exception("MS EXCEL app create failed! Is app installed?");
            }

            try
            {
                _wordApp = new Microsoft.Office.Interop.Word.Application();
            }
            catch
            {
                throw new Exception("MS WORD app create failed! Is app installed?");
            }
        }

        public static void ReleaseMsOfficeApp()
        {
            if(_excelApp != null)
            {
                _excelApp.Quit();
                Marshal.ReleaseComObject(_excelApp);
                _excelApp = null;
            }

            if(_wordApp != null)
            {
                _wordApp.Quit();
                Marshal.ReleaseComObject(_wordApp);
                _wordApp = null;
            }
        }

        public static ResultContainer SearchKeyword(FileInfo file, string keyword, Option option)
        {
            var results = new ResultContainer();

            switch(file.Extension)
            {
                case Const.EXT_XLS:
                case Const.EXT_XLSX:
                    var resultExcel = searchExcel(file, keyword, option);

                    // TBD

                    break;

                case Const.EXT_DOC:
                case Const.EXT_DOCX:
                    var resultWord = searchWord(file, keyword, option);

                    // TBD

                    break;

                default:
                    break;
            }

            return results;
        }

        private static string searchExcel(FileInfo file, string keyword, Option option)
        {
            if(_excelApp == null)
            {
                return string.Empty;
            }

            Workbook wb = null;
            Worksheet ws = null;
            Microsoft.Office.Interop.Excel.Range aRange = null;
            Microsoft.Office.Interop.Excel.Range firstFind = null;
            var ret = string.Empty;

            try
            {
                //xlApp.Visible = true; //This code will show you Excel window.

                wb = _excelApp.Workbooks.Open(
                        file.FullName,  // オープンするExcelファイル名
                        Type.Missing, // （省略可能）UpdateLinks (0 / 1 / 2 / 3)
                        Type.Missing, // （省略可能）ReadOnly (True / False )
                        Type.Missing, // （省略可能）Format
                                        // 1:タブ / 2:カンマ (,) / 3:スペース / 4:セミコロン (;)
                                        // 5:なし / 6:引数 Delimiterで指定された文字
                        Type.Missing, // （省略可能）Password
                        Type.Missing, // （省略可能）WriteResPassword
                        Type.Missing, // （省略可能）IgnoreReadOnlyRecommended
                        Type.Missing, // （省略可能）Origin
                        Type.Missing, // （省略可能）Delimiter
                        Type.Missing, // （省略可能）Editable
                        Type.Missing, // （省略可能）Notify
                        Type.Missing, // （省略可能）Converter
                        Type.Missing, // （省略可能）AddToMru
                        Type.Missing, // （省略可能）Local
                        Type.Missing  // （省略可能）CorruptLoad
                    );

                ws = (Worksheet)wb.Sheets[1];

                aRange = ws.UsedRange;

                firstFind = aRange.Find(
                        keyword,
                        Type.Missing,
                        XlFindLookIn.xlValues,
                        XlLookAt.xlPart,
                        XlSearchOrder.xlByRows,
                        XlSearchDirection.xlNext,
                        false,
                        Type.Missing,
                        Type.Missing);

                ret = firstFind.get_Address(
                        Type.Missing, 
                        Type.Missing,
                        XlReferenceStyle.xlA1, 
                        Type.Missing, 
                        Type.Missing);

                return ret;
            }
            catch
            {
                throw;
            }
            finally
            {
                if(aRange != null)
                {
                    Marshal.ReleaseComObject(aRange);
                    aRange = null;
                }

                if(firstFind != null)
                {
                    Marshal.ReleaseComObject(firstFind);
                    firstFind = null;
                }

                if(ws != null)
                {
                    Marshal.ReleaseComObject(ws);
                    ws = null;
                }

                if(wb != null)
                {
                    wb.Close(true, Type.Missing, Type.Missing);
                    Marshal.ReleaseComObject(wb);
                    wb = null;
                }

                //GC.Collect();
            }
        }

        private static string searchWord(FileInfo file, string keyword, Option option)
        {
            return string.Empty;
        }
    }
}
