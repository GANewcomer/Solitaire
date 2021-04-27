using Prism.Commands;
using Prism.Mvvm;
using Solitaire.Cards;
using Solitaire.Game;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace Solitaire.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {

        #region Properties

        private Deck mainDeck;
        private Move selectedMove;
        private Tableau tableau;
        private Player player;

        public Deck MainDeck
        {
            get => this.mainDeck;
            set
            {
                SetProperty(ref this.mainDeck, value);
            }
        }


        public Tableau Tableau
        { 
            get => this.tableau;
            set
            {
                SetProperty(ref this.tableau, value);
            }
        }

        public Player Player
        { 
            get => this.player;
            set
            {
                SetProperty(ref this.player, value);
            }
        }


        public Move SelectedMove
        { 
            get => this.selectedMove;
            set
            {
                SetProperty(ref this.selectedMove, value);
            }
        }

        #endregion Properties

        #region Commands

        public DelegateCommand NewGameCommand { get; set; }

        public DelegateCommand PerformMoveCommand { get; set; }

        #endregion Commands

        public MainWindowViewModel()
        {
            //Main objects
            MainDeck = new Deck();
            Player = new Player();


            Tableau = new Tableau(MainDeck);
            Player.Moves = Player.CheckAvailableMoves(Tableau);

            //Commands
            PerformMoveCommand = new DelegateCommand(PerformMove);
            NewGameCommand = new DelegateCommand(NewGame);
        }


        #region Methods

        public void NewGame()
        {
            MainDeck.ShuffleDeck();
            Tableau = new Tableau(MainDeck);
            Player.Moves = Player.CheckAvailableMoves(Tableau);
        }

        public void PerformMove()
        {
            if (SelectedMove.NumCards == 0)
            {
                MessageBox.Show("No move selected");
                return;
            }

            bool successful = Player.PerformMove(Tableau, SelectedMove);
            Player.Moves = new ObservableCollection<Move>(Player.CheckAvailableMoves(Tableau));

        }

        #endregion Methods

    }
}
