using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace WorksheetWrapper
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IRows
    {
        int Number { get; set; }
        Columns Columns { get; }
        Rows this[int rowNumber] { get; }
        int Last { get; }
        void Insert(XlInsertShiftDirection direction);
        void Delete();
        void Clear();
        double Height { get; set; }
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class Rows : IRows
    {
        private readonly _Worksheet _worksheet;

        public Rows(ref _Worksheet worksheet)
        {
            _worksheet = worksheet;
            Columns = new Columns(ref _worksheet, this);
            Number = 1;
        }

        [ComVisible(true)]
        public int Number { get; set; }

        [ComVisible(true)]
        public Columns Columns { get; }

        [ComVisible(true)]
        public Rows this[int rowNumber]
        {
            get
            {
                Number = rowNumber;
                return this;
            }
        }

        [ComVisible(true)]
        public int Last => _worksheet.UsedRange.Rows.Count;

        [ComVisible(true)]
        public void Insert(XlInsertShiftDirection direction)
        {
            _worksheet.Cells[Number, 1].EntireRow.Insert(direction);
        }

        [ComVisible(true)]
        public void Delete()
        {
            _worksheet.Cells[Number, 1].EntireRow.Delete();
        }

        [ComVisible(true)]
        public void Clear()
        {
            _worksheet.Cells[Number, 1].EntireRow.Clear();
        }

        [ComVisible(true)]
        public double Height
        {
            get => _worksheet.Cells[Number, 1].RowHeight;
            set => _worksheet.Cells[Number, 1].RowHeight = value;
        }
    }
}
