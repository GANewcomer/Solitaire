using Solitaire.Cards;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Solitaire.Converters
{
    public class CardToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Card) && value != null)
            {
                if (((Card)value).Color == CardColor.Black)
                    return ConsoleColor.Black;
                else
                    return ConsoleColor.Red;
            }
            else
                return ConsoleColor.Yellow;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
