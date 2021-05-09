﻿using Prism.Commands;
using Prism.Mvvm;
using Solitaire.Cards;
using Solitaire.Game;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        public DelegateCommand UndoMoveCommand { get; set; }

        #endregion Commands

        public MainWindowViewModel()
        {
            string winningDeck = "{S1,D5,D12,S11,D1,C5,H1,H9,C8,C11,D9,C1,C2,D3,C6,D11,H5,S6,H8,H7,S13,D6,C9,H11,S10,S5,C13,S9,H12,S3,H4,S12,H13,H3,S2,H2,D10,C4,D8,S4,S7,C10,D13,H10,D2,D4,H6,D7,S8,C12,C3,C7}";

            //Main objects
            //MainDeck = new Deck();
            MainDeck = new Deck(winningDeck);
            Player = new Player();


            Tableau = new Tableau(MainDeck);
            Player.Moves = new ObservableCollection<Move>(Player.CheckAvailableMoves(Tableau));

            //Commands
            PerformMoveCommand = new DelegateCommand(PerformMove);
            UndoMoveCommand = new DelegateCommand(UndoMove);
            NewGameCommand = new DelegateCommand(NewGame);
        }


        #region Methods

        public void NewGame()
        {
            MainDeck.ShuffleDeck();
            Tableau = new Tableau(MainDeck);
            Player.Moves = new ObservableCollection<Move>(Player.CheckAvailableMoves(Tableau));
        }

        public void PerformMove()
        {
            if (SelectedMove.NumCards == 0)
            {
                MessageBox.Show("No move selected");
                return;
            }

            BoardStatus status = Player.PerformMove(Tableau, SelectedMove);
            if (status == BoardStatus.GameWon)
                MessageBox.Show(string.Format("Congratulations human. You won the game in {0} moves", tableau.Moves.Count), "You Win!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else if (status == BoardStatus.GameLost)
                MessageBox.Show(string.Format("Sorry human. It's too bad that you failed so terribly. But at least you've got an okay personality..."), "You Lost...", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            Player.Moves = new ObservableCollection<Move>(Player.CheckAvailableMoves(Tableau));
            SelectedMove = Player.Moves.First();

        }

        public void UndoMove()
        {
            Tableau.UndoLastMove();

            Player.Moves = new ObservableCollection<Move>(Player.CheckAvailableMoves(Tableau));

        }

        #endregion Methods

    }
}
