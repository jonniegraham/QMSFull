using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QMSStyles.Converter
{
    /// <summary>
    /// Intended for indicating a changed (and unsaved) field in the UI.
    /// </summary>
    [ValueConversion(typeof(SolidColorBrush), typeof(SolidColorBrush))]
    public class BackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SolidColorBrush backgroundColor)) return Binding.DoNothing;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
