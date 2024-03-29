﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Prism.Mvvm;
using Solitaire.Cards;

namespace Solitaire.Game
{
    public enum BoardStatus
    {
        InProgress,
        GameLost,
        GameWon
    }

    public class Tableau : BindableBase
    {
        private ObservableCollection<Move> moves = new ObservableCollection<Move>();
        private int faceDownCards;
        private int cardsLeft = 0;
        private int maxUnflippedCards;

        public int HandIncrement = 3;

        public Deck InitialDeck { get; }

        public Dictionary<string, CardStack> MainStacks { get; private set; }

        public Dictionary<string, CardStack> AceStacks { get; private set; }

        public CardStack Hand { get; private set; }

        public CardStack HandFlip { get; private set; }

        public ObservableCollection<Move> Moves { get => this.moves;
            private set
            {
                SetProperty(ref this.moves, value);    
            }
        }

        public List<string> History { get; private set; } = new List<string>();

        public BoardStatus Status { get; set; }

        public int FaceDownCards { get => this.faceDownCards;
            private set
            {
                SetProperty(ref this.faceDownCards, value);
            }
        }

        public int CardsProgress { get => this.cardsLeft;
            private set
            {
                SetProperty(ref this.cardsLeft, value);
            }
        }

        public int MaxUnflippedCards { get => this.maxUnflippedCards;
            private set
            {
                SetProperty(ref this.maxUnflippedCards, value);
            }
        }

        private object movesLock = new object();
        
        public Tableau(Deck deck)
        {
            InitialDeck = deck;

            //Creating stack arrays
            MainStacks = new Dictionary<string, CardStack>()
            {
                { "Main1", new CardStack(new RuleAlternatingColorDescendingRank(), "Main1")},
                { "Main2", new CardStack(new RuleAlternatingColorDescendingRank(), "Main2")},
                { "Main3", new CardStack(new RuleAlternatingColorDescendingRank(), "Main3")},
                { "Main4", new CardStack(new RuleAlternatingColorDescendingRank(), "Main4")},
                { "Main5", new CardStack(new RuleAlternatingColorDescendingRank(), "Main5")},
                { "Main6", new CardStack(new RuleAlternatingColorDescendingRank(), "Main6")},
                { "Main7", new CardStack(new RuleAlternatingColorDescendingRank(), "Main7")}
            };

            AceStacks = new Dictionary<string, CardStack>()
            {
                { "AceSpade", new CardStack(new RuleSameSuitAscendingRank(CardSuit.Spade), "AceSpade") },
                { "AceClub", new CardStack(new RuleSameSuitAscendingRank(CardSuit.Club), "AceClub") },
                { "AceDiamond", new CardStack(new RuleSameSuitAscendingRank(CardSuit.Diamond), "AceDiamond") },
                { "AceHeart",  new CardStack(new RuleSameSuitAscendingRank(CardSuit.Heart), "AceHeart") },
            };


            //Moving deck into hand
            Hand = new CardStack(deck, new RuleNone(), "Hand");
            HandFlip = new CardStack(new RuleNone(), "HandFlip");

            //Dealing out hand into the main stacks
            for (int i = 0; i < MainStacks.Count; i++)
            {
                int j = 0;
                foreach (string key in MainStacks.Keys)
                {
                    if (j >= i)
                    {
                        Hand.TransferTopCard(MainStacks[key]);
                    }
                    j++;
                }
            }

            //Turning rules on
            foreach (CardStack cardStack in MainStacks.Values)
            {
                cardStack.AddRule.IsRuleTurnedOn = true;
            }
            foreach (CardStack cardStack in AceStacks.Values)
            {
                cardStack.AddRule.IsRuleTurnedOn = true;
            }
            Hand.AddRule.IsRuleTurnedOn = true;
            HandFlip.AddRule.IsRuleTurnedOn = true;


            //Flipping top cards up
            foreach (CardStack stack in MainStacks.Values)
            {
                stack.TopCard.Flip();
            }

            UpdateBoard();
            MaxUnflippedCards = FaceDownCards;

            // sync
            BindingOperations.EnableCollectionSynchronization(Moves, this.movesLock);

            // check
            if (Hand.CardCount != 24)
                throw new InvalidOperationException("Stack dealing was not correct");

        }

        public void AddMove(Move move)
        {
            lock (this.movesLock)
            {
                Moves.Add(move);
            }
        }

        public CardStack GetStack(string stackName)
        {
            CardStack stack = null;
            if (stackName.Contains("Main"))
                stack = MainStacks[stackName];
            else if (stackName.Contains("Ace"))
                stack = AceStacks[stackName];
            else if (stackName == "Hand")
                stack = Hand;
            else if (stackName == "HandFlip")
                stack = HandFlip;

            return stack;
        }


        /// <summary>
        /// Create a copy of this current tableau
        /// </summary>
        /// <returns></returns>
        public Tableau Copy()
        {
            var newTableau = new Tableau(InitialDeck.Copy());

            // main stacks
            foreach (string stackName in MainStacks.Keys)
            {
                newTableau.MainStacks[stackName] = MainStacks[stackName].Copy();
            }

            // ace stacks
            foreach (string stackName in AceStacks.Keys)
            {
                newTableau.AceStacks[stackName] = AceStacks[stackName].Copy();
            }

            // hands
            newTableau.Hand = Hand.Copy();
            newTableau.HandFlip = HandFlip.Copy();
            newTableau.Moves = new ObservableCollection<Move>(Moves);


            return newTableau;
        }

        public void UndoLastMove()
        {
            if (Moves.Count == 0)
                return;

            Move lastMove = Moves.Last();
            Move undoMove = lastMove.Opposite();

            // getting stacks
            CardStack stackTakeFrom = GetStack(undoMove.StackFromName);
            CardStack stackReturnTo = GetStack(undoMove.StackToName);

            // main or ace stack move
            if ((undoMove.StackFromName.Contains("Ace") || undoMove.StackFromName.Contains("Main")) && (undoMove.StackToName.Contains("Ace") || undoMove.StackToName.Contains("Main")))
            {
                // flipping top card back over
                if (stackReturnTo.CardCount > 0 && stackReturnTo.TopCard.IsFaceUp && undoMove.WasACardFlipped)
                    stackReturnTo.TopCard.Flip();
            }

            // hand and handflip move
            bool setFaceDown = false;
            bool setFaceUp = false;
            if (stackTakeFrom.Name == "HandFlip" && stackReturnTo.Name == "Hand")
                setFaceDown = true;
            if (stackTakeFrom.Name == "Hand" && stackReturnTo.Name == "HandFlip")
                setFaceUp = true;

            // undoing the move
            List<Card> cards = stackTakeFrom.TakeTopMost(undoMove.NumCards);
            for (int i = 0; i < cards.Count; i++)
            {
                int iCard = undoMove.ReverseCardOrder ? cards.Count - i - 1 : i;

                // flipping if necessary
                if (setFaceDown && cards[iCard].IsFaceUp)
                    cards[iCard].Flip();
                else if (setFaceUp && !cards[iCard].IsFaceUp)
                    cards[iCard].Flip();

                stackReturnTo.Stack.Add(cards[iCard]);
            }
            stackReturnTo.UpdateStackCount();

            // updating
            UpdateBoard();
            Moves.RemoveAt(Moves.Count - 1);
        }

        public BoardStatus UpdateBoard()
        {
            // updating number of face down cards
            FaceDownCards = 0;
            foreach (CardStack stack in MainStacks.Values)   
            {
                foreach (Card card in stack.Stack)
                {
                    if (!card.IsFaceUp)
                        FaceDownCards++;
                }
            }

            FaceDownCards += Hand.CardCount;
            FaceDownCards += HandFlip.CardCount - (HandFlip.CardCount > 0 ? 1 : 0);

            CardsProgress = MaxUnflippedCards - FaceDownCards;

            if (FaceDownCards == 0)
                Status = BoardStatus.GameWon;
            else
                Status = BoardStatus.InProgress;
            return Status;
        }


        /// <summary>
        /// Generate a summary of this tableau
        /// </summary>
        /// <returns></returns>
        public string GetSummary()
        {
            string summary = "";

            // main stacks
            foreach (string stackName in MainStacks.Keys)
            {
                summary += MainStacks[stackName].GetSummary() + ";";
            }

            // ace stacks
            foreach (string stackName in AceStacks.Keys)
            {
                summary += AceStacks[stackName].GetSummary() + ";";
            }

            // hand stack
            summary += Hand.GetSummary() + ";";
            summary += HandFlip.GetSummary() + ";";

            return summary;
        }

        /// <summary>
        /// Compare another object to see if it equals this Tableau
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Tableau))
                return false;

            return GetSummary() == ((Tableau)obj).GetSummary();
        }

    }
}
