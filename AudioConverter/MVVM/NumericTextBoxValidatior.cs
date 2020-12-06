using System.Globalization;
using System.Windows.Controls;

namespace AudioConverter.MVVM
{
    /// <summary>
    /// Class which validates values from bit rate textbox
    /// </summary>
    internal class NumericTextBoxValidatior : ValidationRule
    {
        /// <summary>
        /// Creates instance of <see cref="NumericTextBoxValidatior"/>
        /// </summary>
        public NumericTextBoxValidatior()
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
            int digit = 0;

            int.TryParse(value.ToString(), out digit);

            return digit > 0 && value.ToString().Length > 0 ? ValidationResult.ValidResult : new ValidationResult(false, string.Empty);
        }
    }
}
