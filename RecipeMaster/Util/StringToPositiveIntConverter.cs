using System;
using System.Globalization;
using System.Windows.Data;

namespace RecipeMaster.Util
{
    /// <summary>
    /// Value converter between an integer value and the string representation of an integer value
    /// </summary>
    public class StringToPositiveIntConverter : IValueConverter
    {
        /// <summary>
        /// Converts from an integer to a string
        /// </summary>
        /// <param name="value">integer</param>
        /// <returns>Integer as a string if greater than 0, otherwise an empty string</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value > 0)
            {
                return value.ToString();
            }
            return "";
        }

        /// <summary>
        /// Converts from a string to an integer
        /// </summary>
        /// <param name="value">string</param>
        /// <returns>Parsed value of from the string (if greater than 0, otherwise returns 0), or -1 if parsing failed</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue;
            if (int.TryParse((string)value, out intValue))
            {
                return Math.Max(intValue,0);
            }
            return -1;
        }
    }
}
