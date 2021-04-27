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

        public DelegateCommand PerformMoveCommand { get; set; }

        #endregion Commands

        public MainWindowViewModel()
        {
            //Main objects
            MainDeck = new Deck();
            Tableau = new Tableau(MainDeck);

            Player = new Player();
            Player.Moves = new ObservableCollection<Move>(Player.CheckAvailableMoves(Tableau));

            //Commands
            PerformMoveCommand = new DelegateCommand(PerformMove);

        }


        #region Methods

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
