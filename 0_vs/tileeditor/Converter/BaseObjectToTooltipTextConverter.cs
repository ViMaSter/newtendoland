using System;
using System.Globalization;
using System.Windows.Data;
using NintendoLand.DataFormats;
using tileeditor.GridObjects;

namespace tileeditor.Converter
{
    [ValueConversion(typeof(BaseObject), typeof(string))]
    public class BaseObjectToTooltipTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BaseObject baseObject = (BaseObject)value;
            string output = baseObject.DisplayName;
            if (!string.IsNullOrEmpty(baseObject.DisplayData))
            {
                output += ": " + baseObject.DisplayData;
            }

            output += $" [{baseObject.Position.X}, {baseObject.Position.Y}]";
            if (baseObject.Position.X >= MapData.COLUMNS_VISIBLE || baseObject.Position.Y >= MapData.ROWS_VISIBLE)
            {
                output += " | OUT OF PLAYAREA";
            }

            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}