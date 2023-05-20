using System.Diagnostics;
using System.Globalization;

namespace ViewModelToolkit.Converters;

public class DebugConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        Debug.WriteLine($"DebugConverter.Convert: value = [{value?.ToString() ?? "null"}] {(parameter != null ? $", parameter = [{parameter}]" : string.Empty)}");
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        Debug.WriteLine($"DebugConverter.ConvertBack: value = [{value?.ToString() ?? "null"}] {(parameter != null ? $", parameter = [{parameter}]" : string.Empty)}");
        return value;
    }
}
