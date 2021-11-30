using System;
using System.Globalization;
using System.Windows.Data;

namespace RecipeMaster.Util
{
    /// <summary>
    /// Value converter between nullable string and non-nullable string
    /// </summary>
    public class NullableStringToNonNullableStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts null into an empty string, otherwise returns string as-is
        /// </summary>
        /// <param name="value">String that could be null</param>
        /// <returns>A string that is not null</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            return value as string;
        }

        /// <summary>
        /// Converts empty string into null, otherwise returns string as-is
        /// </summary>
        /// <param name="value">String that will not be null</param>
        /// <returns>A string or null</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value as string)) return null;
            return value as string;
        }
    }
}
