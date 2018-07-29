using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace QMSStyles.Control
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class CustomDataGridRow : DataGridRow
    {
        public static DependencyProperty IsChangedProperty = DependencyProperty.Register(
            "IsChanged", typeof(bool), typeof(CustomDataGridRow), new PropertyMetadata(false));
        public bool IsChanged
        {
            get => (bool)GetValue(IsChangedProperty);
            set => SetValue(IsChangedProperty, value);
        }
    }
}
