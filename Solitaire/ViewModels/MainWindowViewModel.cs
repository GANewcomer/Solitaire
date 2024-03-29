﻿using Prism.Commands;
using Prism.Mvvm;
using Solitaire.Cards;
using Solitaire.Game;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private int numGames = 10;

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


        public int NumGames
        { 
            get => this.numGames;
            set
            {
                SetProperty(ref this.numGames, value);
            }
        }



        private bool massGamesRunning = false;
        public bool MassGamesRunning
        { 
            get => this.massGamesRunning;
            set
            {
                SetProperty(ref this.massGamesRunning, value);
            }
        }


        #endregion Properties

        #region Commands

        public DelegateCommand NewGameCommand { get; set; }

        public DelegateCommand PerformMoveCommand { get; set; }
        public DelegateCommand UndoMoveCommand { get; set; }
        public DelegateCommand AutoSolveCommand { get; set; }
        public DelegateCommand RunGamesCommand { get; set; }

        #endregion Commands

        public MainWindowViewModel()
        {
            string winningDeck = "{S1;D5;D12;S11;D1;C5;H1;H9;C8;C11;D9;C1;C2;D3;C6;D11;H5;S6;H8;H7;S13;D6;C9;H11;S10;S5;C13;S9;H12;S3;H4;S12;H13;H3;S2;H2;D10;C4;D8;S4;S7;C10;D13;H10;D2;D4;H6;D7;S8;C12;C3;C7}";
            string badGame = "{C6;H9;C10;D10;H8;S9;S11;S6;D11;H10;C1;C11;D12;H3;C5;D1;S10;D13;D8;H4;S1;C3;C2;D3;H5;C12;S13;S7;C7;H2;C4;H7;H12;C13;C9;D2;H6;H1;D7;D9;S12;C8;D5;S2;H11;D6;S3;S8;H13;D4;S5;S4}";

            //Main objects
            //MainDeck = new Deck();
            //MainDeck = new Deck(winningDeck);
            MainDeck = new Deck(badGame);
            Player = new Player();

            Tableau = new Tableau(MainDeck);
            Player.Moves = new ObservableCollection<Move>(Player.CheckAvailableMoves(Tableau));

            //Commands
            PerformMoveCommand = new DelegateCommand(PerformMove);
            UndoMoveCommand = new DelegateCommand(UndoMove);
            NewGameCommand = new DelegateCommand(NewGame);
            AutoSolveCommand = new DelegateCommand(AutoSolve);
            RunGamesCommand = new DelegateCommand(RunGames);
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

        public void AutoSolve()
        {
            Thread solveThread = new Thread(() =>
            {
                GameSummary endGame = Player.SolveGame(Tableau);

                App.Current.Dispatcher.Invoke(() =>
                {
                    if (endGame.Status == BoardStatus.GameWon)
                        MessageBox.Show(string.Format("Congratulations human. You won the game in {0} moves and {1} cycles (though you used auto-solve...)", endGame.MoveCount, endGame.CycleCount ), "You Win!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    else if (endGame.Status == BoardStatus.GameLost)
                        MessageBox.Show(string.Format("Sorry human. It's too bad that you failed so terribly. But at least you've got an okay personality..."), "You Lost...", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    else
                        MessageBox.Show("strange...");
                });
            });

            //solveThread.ApartmentState = ApartmentState.STA;
            solveThread.Start();

        }

        public void RunGames()
        {
            Thread solveThread = new Thread(() =>
            {
                MassGamesRunning = true;

                //TextWriter tw = new StreamWriter("gamesLog.txt");
                //Stack<string> gamesToSave = new Stack<string>();

                //bool running = true;
                //Task saveTask = new Task(() =>
                //{
                //    while (running || gamesToSave.Count > 0)
                //    {
                //        Thread.Sleep(50);

                //        lock (gamesToSave)
                //        {
                //            if (gamesToSave.Count > 0)
                //            {
                //                string gameSummary = gamesToSave.Pop();
                //                tw.WriteLine(gameSummary);
                //            }
                //        }

                //    }
                //});
                //saveTask.Start();

                if (true)
                {
                    Parallel.For(0, NumGames, (i) =>
                    {
                        Deck newDeck = new Deck();
                        newDeck.ShuffleDeck();
                        Tableau newTableau = new Tableau(newDeck);

                        GameSummary game = Player.SolveGame(newTableau);

                        // write a line of text to the file
                        //gamesToSave.Push(game.SummaryCSV());

                    });
                }
                else
                {
                    for (int i = 0; i < NumGames; i++)
                    {
                        Deck newDeck = new Deck();
                        newDeck.ShuffleDeck();
                        Tableau newTableau = new Tableau(newDeck);

                        GameSummary game = Player.SolveGame(newTableau);

                        Console.WriteLine("Game finished " + Player.Games.Count);
                        // write a line of text to the file
                        //gamesToSave.Push(game.SummaryCSV());

                    }
                }

                //// close the stream
                //running = false;
                //saveTask.Wait();
                //tw.Close();

                MassGamesRunning = false;

            });

            //solveThread.ApartmentState = ApartmentState.STA;
            solveThread.Start();

        }


        #endregion Methods

    }
}
