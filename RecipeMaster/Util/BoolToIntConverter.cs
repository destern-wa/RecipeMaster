using System;
using System.Globalization;
using System.Windows.Data;

namespace RecipeMaster.Util
{
    /// <summary>
    /// Value converter between boolean and integer (1 or 0)
    /// </summary>
    public class BoolToIntConverter : IValueConverter
    {
        /// <summary>
        /// Converts from boolean to integer
        /// </summary>
        /// <param name="value">boolean</param>
        /// <returns>1 if value is true, 0 if value is false</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 1 : 0;  
        }

        /// <summary>
        /// Converts from integer to boolean
        /// </summary>
        /// <param name="value">integer</param>
        /// <returns>true if integer is above 0, false otherise</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value > 0;
        }
    }
}
