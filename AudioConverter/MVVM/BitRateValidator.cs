using System.Globalization;
using System.Windows.Controls;

namespace AudioConverter.MVVM
{
    /// <summary>
    /// Class which validates values from bit rate textbox
    /// </summary>
    internal class BitRateValidator : ValidationRule
    {
        /// <summary>
        /// Creates instance of <see cref="BitRateValidator"/>
        /// </summary>
        public BitRateValidator()
        {
        }

        /// <summary>
        /// Validates value which comes from bit rate textbox
        /// </summary>
        /// <param name="value">Value from textbox</param>
        /// <param name="cultureInfo">Provides information about a specific culture</param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int bitRate = 0;

            int.TryParse(value.ToString(), out bitRate);

            return bitRate > 0 ? ValidationResult.ValidResult : new ValidationResult(false, string.Empty);
        }
    }
}
