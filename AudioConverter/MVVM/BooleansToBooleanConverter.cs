using System;
using System.Globalization;
using System.Windows.Data;

namespace AudioConverter.MVVM
{
    /// <summary>
    /// Class which converts array of boolean 
    /// values into one boolean value for IsEnable property of Convert button
    /// </summary>
    public class BooleansToBooleanConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts array of boolean values into one boolean 
        /// value for IsEnable property of Convert button
        /// </summary>
        /// <param name="values">Array of boolean values</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.LongLength > 0)
            {
                foreach (var value in values)
                {
                    if (value is bool && (bool)value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
