using System.Runtime.InteropServices;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
// this 'using' is needed.
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using ProgressBar;
using Application = System.Windows.Application;

namespace TakeoffWrapper
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITakeoff
    {
        void Initialize(string path, string sheet);
        void Initialize(WorksheetWrapper.Worksheet worksheet);
        void BuildSoq();
        string FileNumber { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Contractor { get; set; }
        string Job { get; set; }
        string UserId { get; set; }
        WorksheetWrapper.Worksheet Worksheet { get; set; }
        void Save(bool silent = false);
        void Dispose();
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public partial class Takeoff : ITakeoff, IDisposable
    {
        private int _progressIncrementsRemaining = 100;
        private Animation _progressBar;

        [ComVisible(true)]
        public void Initialize(string path, string sheet)
        {
            SetUpTakeoff(path, sheet);
            SetUpProgressBar();
        }

        [ComVisible(true)]
        public void Initialize(WorksheetWrapper.Worksheet worksheet)
        {
            Worksheet = worksheet;
            SetUpProgressBar();
        }

        [ComVisible(true)]
        public void Initialize(_Worksheet worksheet)
        {
            Worksheet = new WorksheetWrapper.Worksheet();
            Worksheet.Initialize(worksheet);
        }

        [ComVisible(true)]
        public void BuildSoq()
        {
            if (Application.Current == null)
            {
                new Application();
            }

            Debug.Assert(Application.Current != null, "Application.Current != null");
            Application.Current.Dispatcher.Invoke(_progressBar.Show);
        }

        [ComVisible(true)]
        public string FileNumber
        {
            get
            {
                var hiddenValue = Worksheet.Rows[TAKEOFF.ROW.FileNumber_HIDDEN].Columns[TAKEOFF.COLUMN.FileNumber_HIDDEN].Value;
                var visibleValue = Worksheet.Rows[TAKEOFF.ROW.FileNumber_VISIBLE].Columns[TAKEOFF.COLUMN.FileNumber_VISIBLE].Value;
                if (!hiddenValue.Equals(visibleValue))
                    throw new ArgumentException("File number values in takeoff document don't match.");
                if (string.IsNullOrEmpty(visibleValue))
                    throw new ArgumentException("File number value(s) in takeoff document is blank.");
                return visibleValue;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("File number value in takeoff document cannot be set to null or empty.");
                Worksheet.Rows[TAKEOFF.ROW.FileNumber_HIDDEN].Columns[TAKEOFF.COLUMN.FileNumber_HIDDEN].Value = value;
                Worksheet.Rows[TAKEOFF.ROW.FileNumber_VISIBLE].Columns[TAKEOFF.COLUMN.FileNumber_VISIBLE].Value = value;
            }
        }

        [ComVisible(true)]
        public string FirstName
        {
            get => Worksheet.Rows[TAKEOFF.ROW.FirstName].Columns[TAKEOFF.COLUMN.FirstName].Value;
            set => Worksheet.Rows[TAKEOFF.ROW.FirstName].Columns[TAKEOFF.COLUMN.FirstName].Value = value;
        }

        [ComVisible(true)]
        public string LastName
        {
            get => Worksheet.Rows[TAKEOFF.ROW.LastName].Columns[TAKEOFF.COLUMN.LastName].Value;
            set => Worksheet.Rows[TAKEOFF.ROW.LastName].Columns[TAKEOFF.COLUMN.LastName].Value = value;
        }

        [ComVisible(true)]
        public string Contractor
        {
            get => Worksheet.Rows[TAKEOFF.ROW.Contractor].Columns[TAKEOFF.COLUMN.Contractor].Value;
            set => Worksheet.Rows[TAKEOFF.ROW.Contractor].Columns[TAKEOFF.COLUMN.Contractor].Value = value;
        }

        [ComVisible(true)]
        public string Job
        {
            get => Worksheet.Rows[TAKEOFF.ROW.Job].Columns[TAKEOFF.COLUMN.Job].Value;
            set => Worksheet.Rows[TAKEOFF.ROW.Job].Columns[TAKEOFF.COLUMN.Job].Value = value;
        }

        [ComVisible(false)]
        public string UserId
        {
            get
            {
                var userId = Worksheet.Rows[TAKEOFF.ROW.UserId].Columns[TAKEOFF.COLUMN.UserId].Value;
                if (string.IsNullOrEmpty(userId))
                    throw new ArgumentException("User I.D. value in takeoff document is blank.");

                return userId;
            }
            set => Worksheet.Rows[TAKEOFF.ROW.UserId].Columns[TAKEOFF.COLUMN.UserId].Value = value;
        }

        [ComVisible(false)]
        public WorksheetWrapper.Worksheet Worksheet { get; set; }

        [ComVisible(true)]
        public void Save(bool silent = false)
        {
            Worksheet.Save(silent);
        }

        [ComVisible(false)]
        public async void Dispose()
        {
            await Task.Delay(100);
            Worksheet.Dispose();
        }
    }
}
