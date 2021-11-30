using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RecipeMaster.Util
{
    /// <summary>
    /// Value converter between boolean and hidden visibility
    /// </summary>
    public class BoolToInvisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value into a Visibility
        /// </summary>
        /// <param name="value">boolean</param>
        /// <returns>Hidden if true, Visible if false</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Hidden : Visibility.Visible;
        }

        /// <summary>
        /// Converts a Visibility value into a boolean
        /// </summary>
        /// <param name="value">Visibility</param>
        /// <returns>false if visible, true otherwise</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value != Visibility.Visible;
        }
    }
}
