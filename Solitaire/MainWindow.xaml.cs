using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Solitaire.Cards;
using Solitaire.Game;

namespace Solitaire
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // creating some decks
            Deck deck1 = new Deck();
            Deck deck2 = new Deck();
            Deck deck3 = new Deck();
            deck2.ShuffleDeck();

            bool equal1 = deck2.Equals(deck1);
            bool equal2 = deck3.Equals(deck1);

            // creating a tableau
            Tableau game1 = new Tableau(deck1);
            Tableau game2 = new Tableau(deck2);
            Tableau game3 = new Tableau(deck3);

            string game1Summary = game1.GetSummary();
            string game2Summary = game2.GetSummary();
            string game3Summary = game3.GetSummary();

            bool gameEqual1 = game1.Equals(game2);
            bool gameEqual2 = game1.Equals(game3);

            // player
            Player player = new Player();
            player.StartGame();

        }
    }
}
