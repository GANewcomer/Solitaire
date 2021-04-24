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
            typeof(CardControl), new PropertyMetadata(null, OnCardChanged));

        public Card Card
        {
            get { return (Card)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }

        public static readonly DependencyProperty SuitImagePathProperty =
            DependencyProperty.Register("SuitImagePath", typeof(string),
            typeof(CardControl));

        public string SuitImagePath
        {
            get { return (string)GetValue(SuitImagePathProperty); }
            set { SetValue(SuitImagePathProperty, value); }
        }

        public static void OnCardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CardControl control = (CardControl)d;
            Card card = control.Card;
            if (card != null)
            {
                switch (card.Suit)
                {
                    case CardSuit.Spade:
                        control.SuitImagePath = "\\Icons\\spades.png";
                        break;
                    case CardSuit.Club:
                        control.SuitImagePath = "\\Icons\\clubs.png";
                        break;
                    case CardSuit.Diamond:
                        control.SuitImagePath = "\\Icons\\diamonds.png";
                        break;
                    case CardSuit.Heart:
                        control.SuitImagePath = "\\Icons\\hearts.png";
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
