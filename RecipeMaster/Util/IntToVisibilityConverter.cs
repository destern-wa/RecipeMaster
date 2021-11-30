using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RecipeMaster.Util
{
    /// <summary>
    /// Value converter between integer and visibility
    /// </summary>
    public class IntToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts an integer value into a Visibility
        /// </summary>
        /// <param name="value">integer</param>
        /// <returns>Visible if integer is above 0, Hidden otheriwse</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = (int)value;
            return count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <remarks>
        /// It isn't possible, nor necessary, to convert a visibility back to a specific number 
        /// </remarks>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
