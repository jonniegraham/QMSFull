using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities;

namespace QMSStyles.Control
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class CustomDataGrid : DataGrid
    {
        public CustomDataGrid()
        {
            MouseDown += EventSetter_OnHandler;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomDataGridRow();
        }

        /// <summary>
        /// Row is selected irrespective of where on the datagrid row the mouse is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is Border originalSource))
                return;

            var dataGridRow = Utilities.Utilities.FindVisualParent<DataGridRow>(originalSource);

            if (!(dataGridRow is DataGridRow row))
                return;

            row.IsSelected = true;
        }
    }
}
