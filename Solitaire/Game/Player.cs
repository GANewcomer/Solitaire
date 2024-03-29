﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Data;
using Prism.Mvvm;
using Solitaire.Cards;

namespace Solitaire.Game
{
    public class Player : BindableBase
    {

        private ObservableCollection<Move> moves;
        private double winPercentage = 0;

        public ObservableCollection<Move> Moves
        {
            get => this.moves;
            set
            {
                SetProperty(ref this.moves, value);
            }
        }

        public double WinPercentage
        {
            get => this.winPercentage;
            set
            {
                SetProperty(ref this.winPercentage, value);
            }
        }

        public int gameStorageLimit = 1000;
        private int gameIDCounter = -1;
        private int gameWonCount = 0;

        public ObservableCollection<GameSummary> Games { get; set; } = new ObservableCollection<GameSummary>();

        private object gamesLock = new object();

        public Player()
        {
            BindingOperations.EnableCollectionSynchronization(Games, gamesLock);
            //Games.CollectionChanged += new NotifyCollectionChangedEventHandler(Games_CollectionChanged);
        }

        private void Games_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                lock (this.gamesLock)
                {
                    while (Games.Count > gameStorageLimit)
                    {
                        Games.RemoveAt(0);
                    }
                }
            }
        }

        public GameSummary SolveGame(Tableau tableau)
        {
            //Creating game
            GameSummary newGame;
            lock (this.gamesLock)
            {
                gameIDCounter++;
                newGame = new GameSummary(gameIDCounter, tableau.InitialDeck);
                Games.Add(newGame);
            }


            //Solving game
            BoardStatus status = EvaluateGameState(tableau, newGame);
            tableau.Status = status;
            newGame.FinishGame(tableau);

            if (status == BoardStatus.GameWon)
                gameWonCount++;

            //Updating win ratio
            lock (this.gamesLock)
            {
                WinPercentage = gameWonCount / (double)Games.Count * 100;
            }

            return newGame;
        }

        public BoardStatus EvaluateGameState(Tableau tableau, GameSummary game)
        {
            if (game == null)
            {
                lock (this.gamesLock)
                {
                    gameIDCounter++;
                    game = new GameSummary(gameIDCounter, tableau.InitialDeck);
                    Games.Add(game);
                }
            }

            //Check moves
            List<Move> moves = CheckAvailableMoves(tableau);

            //Looping through moves
            foreach (Move move in moves)
            {
                game.CycleCount++;

                //Thread.Sleep(50);

                if (game.CycleCount == 4000)
                    Console.Write(" OVER 4000!");

                // performing move
                BoardStatus status = PerformMove(tableau, move);

                if (status != BoardStatus.InProgress)
                    return status;

                // checking tableau state
                string state = tableau.GetSummary();
                if (game.GameStates.Contains(state))
                {
                    // undoing move for the next move to pick up with
                    tableau.UndoLastMove();
                    continue;
                }

                game.GameStates.Add(state);

                // analyzing tableau
                status = EvaluateGameState(tableau, game);

                if (status != BoardStatus.InProgress)
                    return status;

                // undoing move for the next move to pick up with
                tableau.UndoLastMove();

            }


            return BoardStatus.GameLost;

        }


        public BoardStatus PerformMove(Tableau tableau, Move move)
        {
            // from-to stack
            CardStack stackFrom = tableau.GetStack(move.StackFromName);
            CardStack stackTo = tableau.GetStack(move.StackToName);

            // moving
            if (stackFrom != null && stackTo != null)
            {
                // performing move
                bool successful = false;
                if (move.NumCards == 1)
                    successful = stackFrom.TransferTopCard(stackTo);
                else
                {
                    List<Card> movedCards;
                    if (move.ReverseCardOrder)
                        movedCards = stackFrom.DealTopMost(move.NumCards);
                    else
                        movedCards = stackFrom.TakeTopMost(move.NumCards);
                    successful = stackTo.AddStack(new CardStack(movedCards, new RuleNone()));
                }

                if (!successful)
                    throw new InvalidOperationException("Move could not be performed");

                // flipping all available cards
                foreach (CardStack stack in tableau.MainStacks.Values)
                {
                    if (stack.TopCard != null && !stack.TopCard.IsFaceUp)
                    {
                        stack.TopCard.Flip();
                        move.WasACardFlipped = true;
                    }
                }
                foreach (CardStack stack in tableau.AceStacks.Values)
                {
                    if (stack.TopCard != null && !stack.TopCard.IsFaceUp)
                        stack.TopCard.Flip();
                }
                foreach (Card card in tableau.Hand.Stack)
                {
                    if (card != null && card.IsFaceUp)
                        card.Flip();
                }
                foreach (Card card in tableau.HandFlip.Stack)
                {
                    if (card != null && !card.IsFaceUp)
                        card.Flip();
                }

                tableau.AddMove(move);
                BoardStatus status = tableau.UpdateBoard();
                return status;
            }
            else
                throw new InvalidOperationException("Move could not be completed");


        }

        public List<Move> CheckAvailableMoves(Tableau tableau)
        {
            List<Move> possibleMoves = new List<Move>();

            // check if all the kings have been planted yet
            bool allKingsPlanted = tableau.MainStacks.Values.ToArray().Where(stack => stack.BaseCard?.Rank == Card.MaxRank).Count() == 4;

            // main stacks
            foreach (CardStack stack in tableau.MainStacks.Values)
            {
                foreach (Card card in stack.Stack)
                {
                    if (card == null || !card.IsFaceUp)
                        continue;

                    int position = stack.GetPosition(card); 

                    //checking other main stacks
                    foreach (CardStack otherStack in tableau.MainStacks.Values)
                    {
                        if (otherStack.Name == stack.Name)
                            continue;

                        int ranking = 1;
                        if (otherStack.AddRule.IsCardAllowedToBeAdded(card, otherStack.TopCard))
                        {
                            if (card == stack.BaseCard && card.Rank == Card.MaxRank && otherStack.CardCount == 0)   //moving king into another empty spot (useless)
                                continue;
                            if (card == stack.BaseCard && allKingsPlanted)
                                ranking = 4;
                            if (position != stack.Stack.Where(c => c.IsFaceUp).Count())
                                ranking = 3;

                            possibleMoves.Add(new Move(position, stack, otherStack, ranking));
                        }
                    }

                    //checking ace stacks
                    if (card == stack.TopCard)
                    {
                        foreach (CardStack otherStack in tableau.AceStacks.Values)
                        {
                            int ranking = 1;
                            if (otherStack.AddRule.IsCardAllowedToBeAdded(card, otherStack.TopCard))
                                possibleMoves.Add(new Move(position, stack, otherStack, ranking));
                        }
                    }
                }
            }

            // ace stacks
            foreach (CardStack stack in tableau.AceStacks.Values)
            {
                Card card = stack.TopCard;
                int position = 1;

                if (card == null || !card.IsFaceUp)
                    continue;

                //checking main stacks
                foreach (CardStack otherStack in tableau.MainStacks.Values)
                {
                    int ranking = 3;
                    if (otherStack.AddRule.IsCardAllowedToBeAdded(card, otherStack.TopCard))
                        possibleMoves.Add(new Move(position, stack, otherStack, ranking));
                }

            }

            // hand flip
            Card flipCard = tableau.HandFlip.TopCard;

            if (flipCard != null && flipCard.IsFaceUp)
            {
                Card card = flipCard;
                CardStack stack = tableau.HandFlip;
                int position = 1;
                //checking main stacks
                foreach (CardStack otherStack in tableau.MainStacks.Values)
                {
                    int ranking = 1;
                    if (otherStack.AddRule.IsCardAllowedToBeAdded(card, otherStack.TopCard))
                        possibleMoves.Add(new Move(position, stack, otherStack, ranking));
                }

                //checking other ace stacks
                foreach (CardStack otherStack in tableau.AceStacks.Values)
                {
                    if (otherStack.Name == stack.Name)
                        continue;

                    int ranking = 1;
                    if (otherStack.AddRule.IsCardAllowedToBeAdded(card, otherStack.TopCard))
                        possibleMoves.Add(new Move(position, stack, otherStack, ranking));
                }
            }

            // hand flipping
            int handRanking = 2;
            if (tableau.Hand.CardCount != 0)
                possibleMoves.Add(new Move(tableau.HandIncrement, tableau.Hand, tableau.HandFlip, handRanking, true));           // regular hand increment
            else if (tableau.HandFlip.CardCount != 0)
                possibleMoves.Add(new Move(tableau.HandFlip.CardCount, tableau.HandFlip, tableau.Hand, handRanking, true));      // resetting hand

            possibleMoves.Sort();

            return possibleMoves;
        }

    }
}
