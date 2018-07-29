using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace WorksheetWrapper
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWorksheet
    {
        void Initialize(string workbookPath, string worksheetName);
        void Initialize(_Worksheet worksheet);
        Rows Rows { get; }
        bool ShowUpdating { get; set; }
        void Save(bool silent = false);
        Worksheet New(string workbookName = "New");
        string Name { get; set; }
        _Workbook Workbook { get; }
        void Dispose();
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class Worksheet : IWorksheet, IDisposable
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private static readonly Application ExApp = new Application();
        private static _Workbook _workbook;
        private _Worksheet _worksheet;

        private Worksheet(string workbookName)
        {
            _worksheet = (_Worksheet)_workbook.Worksheets.Add();
            Name = workbookName;
            Rows = new Rows(ref _worksheet);
        }

        public Worksheet() { }

        [ComVisible(true)]
        public void Initialize(string workbookPath, string worksheetName)
        {
            _workbook = ExApp.Workbooks.Open(workbookPath);
            if (_workbook.ReadOnly)
                HandleReadOnly();
            try
            {
                _worksheet = _workbook.Worksheets[worksheetName];
            }
            catch (COMException e)
            {
                if (e.HResult.ToString("X").Equals("8002000B"))
                {
                    MessageBox.Show($"Cannot Find File{worksheetName}");
                }
            }
            Rows = new Rows(ref _worksheet);
        }

        [ComVisible(true)]
        public void Initialize(_Worksheet worksheet)
        {
            try
            {
                _worksheet = worksheet;
                _workbook = _worksheet.Application.ActiveWorkbook;
                if (_workbook.ReadOnly)
                    HandleReadOnly();
                Rows = new Rows(ref _worksheet);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            KillRedundantProcess();
        }

        [ComVisible(true)]
        public Rows Rows { get; private set; }

        [ComVisible(true)]
        public bool ShowUpdating
        {
            get => _worksheet.Application.ScreenUpdating;
            set => _worksheet.Application.ScreenUpdating = value;
        }

        [ComVisible(true)]
        public void Save(bool silent = false)
        {
            if (silent)
                ExApp.DisplayAlerts = false;
            _workbook.Save();
        }

        [ComVisible(true)]
        public Worksheet New(string workbookName = "New")
        {
            return new Worksheet(workbookName);
        }

        [ComVisible(true)]
        public string Name
        {
            get => _worksheet.Name;
            set
            {
                var append = "";
                var num = 1;
                while (true)
                {
                    if (!SheetNameExists(value + append))
                    {
                        _worksheet.Name = value + append;
                        break;
                    }
                    else
                    {
                        append = num + "";
                        num++;
                    }
                }
            }
        }

        [ComVisible(false)]
        public _Workbook Workbook => _workbook;

        [ComVisible(false)]
        public async void Dispose()
        {
            await Task.Delay(100);
            _workbook.Close();
            ExApp.Quit();
        }

        private static void HandleReadOnly()
        {
            throw new Exception("Cannot process takeoff as it is read only. Make sure it is closed properly.");
        }

        private static bool SheetNameExists(string sheetName)
        {
            foreach (_Worksheet sheet in _workbook.Sheets)
                if (sheet.Name.Equals(sheetName))
                    return true;
            return false;
        }

        private static void KillRedundantProcess()
        {
            try
            {
                GetWindowThreadProcessId(new IntPtr(ExApp.Hwnd), out var processId);
                if (processId == 0) return;
                var excelProcess = Process.GetProcessById((int)processId);
                excelProcess.CloseMainWindow();
                excelProcess.Refresh();
                excelProcess.Kill();
            }
            catch (Exception e)
            {
                MessageBox.Show("MESSAGE: " + e.Message + Environment.NewLine + "STACKTRACE: " + e.StackTrace);
            }
        }
    }
}
