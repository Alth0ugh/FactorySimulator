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
    /// Validation rule for GUI which checks if value is a non-negative integer.
    /// </summary>
    public class PositiveIntegerValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Hodnota nesmie byť prázdna!");
            }

            var intParse = int.TryParse(value.ToString(), out int result);

            ValidationResult validationResult;
            if (intParse && result >= 0)
            {
                validationResult = ValidationResult.ValidResult;
            }
            else
            {
                validationResult = new ValidationResult(false, "Hodnota nie je číslo alebo je záporné číslo!");
            }

            return validationResult;
        }
    }
}
