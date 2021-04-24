using Prism.Mvvm;
using Solitaire.Cards;
using Solitaire.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solitaire.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private CardStack testStack;

        public CardStack TestStack { 
            get => this.testStack;
            set
            {
                SetProperty(ref this.testStack, value);
            }
        }


        public MainWindowViewModel()
        {

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
            //player.StartGame();

            TestStack = new CardStack(deck1, null);
            TestStack = game2.MainStacks["Main6"];
        }

    }
}
