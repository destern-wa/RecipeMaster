using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RecipeMaster.Util
{
    /// <summary>
    /// ValueConverter to convert prep time from integer (minutes) to a
    /// formatted string with hours and minutes.
    /// 
    /// Based on: http://www.blackwasp.co.uk/WPFCustomValueConverter.aspx
    /// </summary>
    public class PrepTimeIntToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts integer minutes into a formatted string
        /// </summary>
        /// <param name="value">integer minutes</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Formatted string with either just minutes or hours and minutes as appliacble</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int hours = (int)((int)value / 60);
            int minutes = (int)((int)value % 60);
            if (hours > 0)
            {
                return $"{hours} hr {minutes} min";
            }
            else
            {
                return minutes + " min";
            }
        }
        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <remarks>
        /// Converting back is not required since this is only used in one-way bindings
        /// </remarks>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
