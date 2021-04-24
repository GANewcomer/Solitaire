using System;
using System.Collections.Generic;
using System.Text;
using Solitaire.Cards;

namespace Solitaire.Game
{
    public class Player
    {


        public Player()
        {

        }


        public void StartGame()
        {
            //Shuffling deck
            Deck deck = new Deck();
            //deck.ShuffleDeck();

            //Creating tableau
            Tableau tableau = new Tableau(deck);

            //Beginning solving
            EvaluateGameState(tableau);

        }

        public int CycleCounter = 0;

        public void EvaluateGameState(Tableau tableau)
        {
            CycleCounter++;

            //Check moves
            List<Move> moves = CheckAvailableMoves(tableau);

            //Looping through moves
            foreach (Move move in moves)
            {
                // copying tableau
                Tableau copyTableau = tableau.Copy();

                // performing move
                if (!PerformMove(copyTableau, move))
                    throw new InvalidOperationException("Move could not be performed even though it was detected");

                // checking tableau state
                string state = copyTableau.GetSummary();
                if (copyTableau.History.Contains(state))
                    continue;

                copyTableau.History.Add(state);

                // analyzing tableau
                EvaluateGameState(copyTableau);

            }

        }


        public bool PerformMove(Tableau tableau, Move move)
        {
            // from stack
            CardStack stackFrom = null;
            if (move.StackFromName.Contains("Main"))
                stackFrom = tableau.MainStacks[move.StackFromName];
            else if (move.StackFromName.Contains("Ace"))
                stackFrom = tableau.AceStacks[move.StackFromName];
            else if (move.StackFromName == "Hand")
                stackFrom = tableau.Hand;
            else if (move.StackFromName == "HandFlip")
                stackFrom = tableau.HandFlip;

            // to stack
            CardStack stackTo = null;
            if (move.StackToName.Contains("Main"))
                stackTo = tableau.MainStacks[move.StackToName];
            else if (move.StackToName.Contains("Ace"))
                stackTo = tableau.AceStacks[move.StackToName];
            else if (move.StackToName == "Hand")
                stackTo = tableau.Hand;
            else if (move.StackToName == "HandFlip")
                stackTo = tableau.HandFlip;

            // moving
            if (stackFrom != null && stackTo != null)
            {
                bool successful = false;
                if (move.NumCards == 1)
                    successful = stackFrom.TransferTopCard(stackTo);
                else
                {
                    CardStack movedCards = stackFrom.TakeTopMost(move.NumCards);
                    successful = stackTo.AddStack(movedCards);
                }

                // flipping all available cards
                foreach (CardStack stack in tableau.MainStacks.Values)
                {
                    if (stack.TopCard != null && !stack.TopCard.IsFaceUp)
                        stack.TopCard.Flip();
                }
                foreach (CardStack stack in tableau.AceStacks.Values)
                {
                    if (stack.TopCard != null && !stack.TopCard.IsFaceUp)
                        stack.TopCard.Flip();
                }
                if (tableau.HandFlip.TopCard != null && !tableau.HandFlip.TopCard.IsFaceUp)
                    tableau.HandFlip.TopCard.Flip();

                tableau.Moves.Add(move);
                return successful;
            }
            else
                throw new InvalidOperationException("Move could not be completed");


        }

        public List<Move> CheckAvailableMoves(Tableau tableau)
        {
            List<Move> possibleMoves = new List<Move>();

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

                        if (otherStack.AddRule.IsCardAllowedToBeAdded(card, otherStack.TopCard))
                            possibleMoves.Add(new Move(position, stack, otherStack));
                    }

                    //checking ace stacks
                    foreach (CardStack otherStack in tableau.AceStacks.Values)
                    {
                        if (otherStack.AddRule.IsCardAllowedToBeAdded(card, otherStack.TopCard))
                            possibleMoves.Add(new Move(position, stack, otherStack));
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
                    if (otherStack.AddRule.IsCardAllowedToBeAdded(card, otherStack.TopCard))
                        possibleMoves.Add(new Move(position, stack, otherStack));
                }

                //checking other ace stacks
                foreach (CardStack otherStack in tableau.AceStacks.Values)
                {
                    if (otherStack.Name == stack.Name)
                        continue;

                    if (otherStack.AddRule.IsCardAllowedToBeAdded(card, otherStack.TopCard))
                        possibleMoves.Add(new Move(position, stack, otherStack));
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
                    if (otherStack.AddRule.IsCardAllowedToBeAdded(card, otherStack.TopCard))
                        possibleMoves.Add(new Move(position, stack, otherStack));
                }

                //checking other ace stacks
                foreach (CardStack otherStack in tableau.AceStacks.Values)
                {
                    if (otherStack.Name == stack.Name)
                        continue;

                    if (otherStack.AddRule.IsCardAllowedToBeAdded(card, otherStack.TopCard))
                        possibleMoves.Add(new Move(position, stack, otherStack));
                }
            }

            // hand flipping
            if (tableau.Hand.CardCount != 0)
                possibleMoves.Add(new Move(tableau.HandIncrement, tableau.Hand, tableau.HandFlip, true));           // regular hand increment
            else
                possibleMoves.Add(new Move(tableau.HandFlip.CardCount, tableau.HandFlip, tableau.Hand, true));      // resetting hand


            return possibleMoves;
        }

    }
}
