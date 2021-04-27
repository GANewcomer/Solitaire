using Solitaire.Cards;
using Solitaire.Game;
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
    /// Interaction logic for CardStackControl.xaml
    /// </summary>
    public partial class CardStackControl : UserControl
    {
        public CardStackControl()
        {
            InitializeComponent();
        }

        public CardStack CardStack
        {
            get { return (CardStack)GetValue(CardStackProperty); }
            set { SetValue(CardStackProperty, value); }
        }


        public static readonly DependencyProperty CardStackProperty =
            DependencyProperty.Register("CardStack", typeof(CardStack), typeof(CardStackControl));


        public Card SelectedCard
        {
            get { return (Card)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }


        public static readonly DependencyProperty CardProperty =
            DependencyProperty.Register("SelectedCard", typeof(Card), typeof(CardStackControl));


    }
}
