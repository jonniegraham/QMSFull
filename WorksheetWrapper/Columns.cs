using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

namespace WorksheetWrapper
{
    [ComVisible(true)]
    public enum Alignment { Left, Right, Center, None }
    [ComVisible(true)]
    public enum Direction { Vertical, Horizontal }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IColumns
    {
        int Number { get; set; }
        Rows Rows { get; }
        Columns this[int i] { get; }
        int Last { get; }
        void Clear();
        void Insert(Direction direction, string[] entries);
        void Insert(string entry);
        dynamic Value { get; set; }
        Alignment HorizontalAlignment { get; set; }
        bool IsBold { get; }
        bool IsShaded { get; }
        int FirstNonBoldCharNum { get; }
        int Width { get; set; }
        void SetBoldTo(int charNum);
        void Underline(uint length);
        void SetBold(bool value, uint length, uint height);
        void SetItalic(bool value, uint length, uint height);
        void Shade(uint length, uint height);
        string NumberFormat { get; set; }
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class Columns : IColumns
    {
        private readonly Dictionary<int, string> _indexToAlphabet;
        private Dictionary<string, int> _alphabetToIndex;
        private readonly _Worksheet _worksheet;

        public Columns(ref _Worksheet worksheet, Rows rows)
        {
            _indexToAlphabet = new Dictionary<int, string>
            {
                {1, "A"},{2, "B"},{3, "C"},{4, "D"},{5, "E"},{6, "F"},{7, "G"},{8, "H"},{9, "I"},
                {10, "J"},{11, "K"},{12, "L"},{13, "M"},{14, "N"},{15, "O"},{16, "P"},{ 17, "Q"},
                {18, "R"},{19, "S"},{20, "T"},{21, "U"},{22, "V"},{23, "W"},{24, "X"},{25, "Y"},{26, "Z"}
            };
            _alphabetToIndex = new Dictionary<string, int>
            {
                {"A", 1},{"B", 2},{"C", 3},{"D", 4},{"E", 5},{"F", 6},{"G", 7},{"H", 8},{"I", 9},
                {"J", 10},{"K", 11},{"L", 12},{"M", 13},{"N", 14},{"O", 15},{"P", 16},{"Q", 17},
                {"R", 18},{"S", 19},{"T", 20},{"U", 21},{"V", 22},{"W", 23},{"X", 24},{"Y", 25},{"Z", 26}
            };
            _worksheet = worksheet;
            Rows = rows;
            Number = 1;
        }

        [ComVisible(true)]
        public int Number { get; set; }

        [ComVisible(true)]
        public Rows Rows { get; }

        [ComVisible(true)]
        public Columns this[int columnNumber]
        {
            get
            {
                Number = columnNumber;
                return this;
            }
        }

        [ComVisible(true)]
        public int Last => _worksheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell, Type.Missing).Column;

        [ComVisible(true)]
        public void Clear()
        {
            if (Rows.Number == -1)
                _worksheet.Cells[1, Number].EntireColumn.Clear();
            else
                _worksheet.Cells[Rows.Number, Number].Clear();
        }

        [ComVisible(true)]
        public void Insert(Direction direction, string[] entries)
        {
            if (direction == Direction.Horizontal)
            {
                var currentColumn = Number;
                foreach (var entry in entries)
                {
                    _worksheet.Cells[Rows.Number, currentColumn++] = entry;
                }
            }
            else
            {
                var currentRow = Rows.Number;
                foreach (var entry in entries)
                {
                    _worksheet.Cells[currentRow++, Number] = entry;
                }
            }
        }

        [ComVisible(true)]
        public void Insert(string entry)
        {
            _worksheet.Cells[Rows.Number, Number] = entry;
        }

        [ComVisible(true)]
        public dynamic Value
        {
            get => _worksheet.Cells[Rows.Number, Number].Value;
            set => _worksheet.Cells[Rows.Number, Number].Value = value;
        }

        [ComVisible(true)]
        public Alignment HorizontalAlignment
        {
            get
            {
                if (_worksheet.Cells[Rows.Number, Number].HorizontalAlignment == XlHAlign.xlHAlignCenter)
                    return Alignment.Center;
                else if (_worksheet.Cells[Rows.Number, Number].HorizontalAlignment == XlHAlign.xlHAlignLeft)
                    return Alignment.Left;
                else if (_worksheet.Cells[Rows.Number, Number].HorizontalAlignment == XlHAlign.xlHAlignRight)
                    return Alignment.Right;
                return Alignment.None;
            }
            set
            {
                switch (value)
                {
                    case Alignment.Center:
                        _worksheet.Cells[Rows.Number, Number].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                        break;
                    case Alignment.Left:
                        _worksheet.Cells[Rows.Number, Number].HorizontalAlignment = XlHAlign.xlHAlignLeft;
                        break;
                    case Alignment.Right:
                        _worksheet.Cells[Rows.Number, Number].HorizontalAlignment = XlHAlign.xlHAlignRight;
                        break;
                }
            }
        }

        [ComVisible(true)]
        public bool IsBold => _worksheet.Cells[Rows.Number, Number].Font.Bold is bool &&
                              _worksheet.Cells[Rows.Number, Number].Font.Bold == true;

        [ComVisible(true)]
        public bool IsShaded => _worksheet.Cells[Rows.Number, Number].Interior.ColorIndex != null && _worksheet.Cells[Rows.Number, Number].Interior.ColorIndex == 40;

        [ComVisible(true)]
        public int FirstNonBoldCharNum
        {
            get
            {
                var num = 1;
                while (num <= Value.Length && _worksheet.Cells[Rows.Number, Number].Characters[1, num].Font.Bold is bool && _worksheet.Cells[Rows.Number, Number].Characters[1, num].Font.Bold == true)
                    num++;

                return num;
            }
        }

        [ComVisible(true)]
        public int Width
        {
            get => _worksheet.Cells[Rows.Number, Number].ColumnWidth;
            set => _worksheet.Cells[Rows.Number, Number].ColumnWidth = value;
        }

        [ComVisible(true)]
        public void SetBoldTo(int charNum)
        {
            _worksheet.Cells[Rows.Number, Number].Characters[1, charNum].Font.Bold = true;
        }

        [ComVisible(true)]
        public void Underline(uint length)
        {
            length--;
            _worksheet.Range[_indexToAlphabet[Number > 26 ? 26 : Number] + Rows.Number, _indexToAlphabet[Number + (int)length > 26 ? 26 : Number + (int)length] + Rows.Number].Borders[XlBordersIndex.xlEdgeBottom].LineStyle = Constants.xlSolid;
        }

        [ComVisible(true)]
        public void SetBold(bool value, uint length, uint height)
        {
            length--;
            height--;
            _worksheet.Range[_indexToAlphabet[Number > 26 ? 26 : Number] + Rows.Number, _indexToAlphabet[Number + (int)length > 26 ? 26 : Number + (int)length] + (Rows.Number + height)].Font.Bold = value;
        }

        [ComVisible(true)]
        public void SetItalic(bool value, uint length, uint height)
        {
            length--;
            height--;
            _worksheet.Range[_indexToAlphabet[Number > 26 ? 26 : Number] + Rows.Number, _indexToAlphabet[Number + (int)length > 26 ? 26 : Number + (int)length] + (Rows.Number + height)].Font.Italic = value;
        }

        [ComVisible(true)]
        public void Shade(uint length, uint height)
        {
            length--;
            height--;

            _worksheet.Range[_indexToAlphabet[Number > 26 ? 26 : Number] + Rows.Number, _indexToAlphabet[Number + (int)length > 26 ? 26 : Number + (int)length] + (Rows.Number + height)].Interior.Color =
                XlRgbColor.rgbLightGray;
        }

        [ComVisible(true)]
        public string NumberFormat
        {
            get => _worksheet.Cells[Rows.Number, Number].NumberFormat;
            set => _worksheet.Cells[Rows.Number, Number].NumberFormat = value;
        }
    }
}
