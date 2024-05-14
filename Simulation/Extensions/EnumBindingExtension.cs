using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Simulation.Extensions
{
    /// <summary>
    /// XML markup extension used for binding to C# enums.
    /// </summary>
    public class EnumBindingExtension : MarkupExtension
    {
        private bool _isConverterPresent;
        private IValueConverter _converter;
        public Type Value { get; set; }
        public IValueConverter Converter
        {
            get => _converter;
            set
            {
                _converter = value;
                if (value == null)
                {
                    _isConverterPresent = false;
                    return;
                }

                _isConverterPresent = value is IValueConverter ? true : false;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Value == null)
            {
                return Binding.DoNothing;
            }
            object retVal;

            if (Value.IsEnum)
            {
                retVal = Value.GetEnumValues();
            }
            else
            {
                return Binding.DoNothing;
            }

            if (_isConverterPresent)
            {
                retVal = ((IValueConverter)Converter).Convert(retVal, null, null, null);
            }

            return retVal;
        }
    }
}
