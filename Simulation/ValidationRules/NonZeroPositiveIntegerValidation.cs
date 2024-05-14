using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Simulation.ValidationRules
{
    /// <summary>
    /// Validation rule for GUI wchich chcecks if a value is a positive integer.
    /// </summary>
    public class NonZeroPositiveIntegerValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Hodnota nesmie byť prázdna!");
            }

            var intParse = int.TryParse(value.ToString(), out int val1);

            if (intParse && val1 > 0)
            {
                return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "Hodnota nie je číslo alebo je záporné číslo alebo nula!");
        }
    }
}
