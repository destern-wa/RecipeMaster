using System;
using System.Globalization;
using System.Windows.Data;

namespace RecipeMaster.Util
{
    /// <summary>
    /// Value converter from a boolean into a symbol (string)
    /// </summary>
    public class BoolToEditSymbolConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean into an edit symbol (pencil) if true,
        /// or a cancel edit symbol (x-mark) if false
        /// </summary>
        /// <param name="value">Boolean</param>
        /// <returns>✎ or ╳</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "✎" : "╳";
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <remarks>
        /// This converter is only used with one-way bindings, so there is no need to convert back
        /// </remarks>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
