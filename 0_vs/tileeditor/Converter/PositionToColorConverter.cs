using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using NintendoLand.DataFormats;

namespace tileeditor.Converter
{
    [ValueConversion(typeof(Point), typeof(SolidColorBrush))]
    public class PositionToColorConverter : IValueConverter
    {
        public static readonly SolidColorBrush InRangeColor = new SolidColorBrush(Color.FromArgb(0x00, 0x18, 0x18, 0x18));
        public static readonly SolidColorBrush OutOfRangeColor = new SolidColorBrush(Color.FromArgb(0x80, 0xFF, 0x00, 0x00));
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Point position = (Point)value;
            if (position.X >= MapData.COLUMNS_VISIBLE)
            {
                return OutOfRangeColor;
            }
            if (position.Y >= MapData.ROWS_VISIBLE)
            {
                return OutOfRangeColor;
            }

            return InRangeColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}