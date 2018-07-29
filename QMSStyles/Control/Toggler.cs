using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace QMSStyles.Control
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class Toggler : CheckBox
    {
        public static readonly DependencyProperty TitleProperty =
    DependencyProperty.Register("Title", typeof(string), typeof(Toggler), new UIPropertyMetadata(""));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
    }
}
