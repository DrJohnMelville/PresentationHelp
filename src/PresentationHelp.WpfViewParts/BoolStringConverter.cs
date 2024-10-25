using System.Globalization;
using System.Windows.Data;
using Melville.INPC;

namespace PresentationHelp.WpfViewParts;

[StaticSingleton]
public partial class BoolStringConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var options = parameter?.ToString()?.Split('|')??["No parameter for binding"];
        return options[(value is false && options.Length > 1) ? 1 : 0];
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}