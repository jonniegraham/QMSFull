using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QMSStyles.Converter
{
    /// <summary>
    /// Intended for indicating a changed (and unsaved) field in the UI.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class BoolToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(bool)value)
                return new SolidColorBrush(Colors.White);

            return Brushes.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
