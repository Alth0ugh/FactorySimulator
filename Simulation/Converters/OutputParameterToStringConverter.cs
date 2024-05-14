using Simulation.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Simulation.Converters
{
    /// <summary>
    /// WPF converter used to convert OutputParameterEnum to human-readable text.
    /// </summary>
    public class OutputParameterToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            var val = (OutputParameterEnum)value;
            var returnValue = "";

            switch (val)
            {
                case OutputParameterEnum.Downtime:
                    returnValue = "Časové stráty";
                    break;
                case OutputParameterEnum.None:
                    returnValue = "Vyberte požadovaný výstup";
                    break;
                case OutputParameterEnum.PartCount:
                    returnValue = "Počet použitých súčiastok";
                    break;
                case OutputParameterEnum.TotalFG:
                    returnValue = "Objem výroby";
                    break;
                case OutputParameterEnum.UnloadValues:
                    returnValue = "Objem vyprázdnenia linky";
                    break;
                default:
                    break;
            }
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
