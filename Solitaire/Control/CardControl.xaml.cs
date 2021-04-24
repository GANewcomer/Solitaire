using Solitaire.Cards;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Solitaire.Control
{
    /// <summary>
    /// Interaction logic for CardControl.xaml
    /// </summary>
    public partial class CardControl : UserControl
    {
        public CardControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CardProperty =
            DependencyProperty.Register("Card", typeof(Card),
            typeof(CardControl));

        public Card Card
        {
            get { return (Card)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }

    }
}
