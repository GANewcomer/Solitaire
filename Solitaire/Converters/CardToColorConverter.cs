﻿using Solitaire.Cards;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Solitaire.Converters
{
    public class CardToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(CardColor))
                return Brushes.Black;

            if ((CardColor)value == CardColor.Black)
                return Brushes.Black;
            else if ((CardColor)value == CardColor.Red)
                return Brushes.Red;
            else
                return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
