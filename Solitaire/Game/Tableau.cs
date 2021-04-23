using System;
using System.Collections.Generic;
using System.Text;
using Solitaire.Cards;

namespace Solitaire.Game
{
    public class Tableau
    {
        public int HandIncrement = 3;

        public Deck InitialDeck { get; }

        public Dictionary<string, CardStack> MainStacks { get; private set; }

        public Dictionary<string, CardStack> AceStacks { get; private set; }

        public CardStack Hand { get; private set; }

        public CardStack HandFlip { get; private set; }

        public List<Move> Moves { get; private set; } = new List<Move>();
        public List<string> History { get; private set; } = new List<string>();

        public bool WasWon { get; }


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

            // check
            if (Hand.CardCount != 24)
                throw new InvalidOperationException("Stack dealing was not correct");

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
            newTableau.Moves = new List<Move>(Moves);


            return newTableau;
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
