namespace HardwareToSerialWriter.WPF.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToCollapsingVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a boolean");

            var valueBool = (bool) value;
            if (valueBool)
            {
                return Visibility.Visible;
            }
            
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
