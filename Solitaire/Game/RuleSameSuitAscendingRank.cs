using System;
using System.Collections.Generic;
using System.Text;
using Solitaire.Cards;

namespace Solitaire.Game
{
    public class RuleSameSuitAscendingRank : IStackAddRule
    {
        public string Name { get; } = "SameSuit-AscendingRank";

        public CardSuit FixedSuit { get; }

        public bool IsRuleTurnedOn { get; set; }

        public bool IsCardAllowedToBeAdded(Card cardToAdd, Card cardToStackOnto)
        {
            if (!IsRuleTurnedOn)    // if not turned on, then rule is skipped
                return true;

            if (cardToStackOnto != null)
                return (cardToAdd.Suit == FixedSuit) && (cardToAdd.Rank == cardToStackOnto.Rank + 1) && cardToAdd.IsFaceUp;
            else
                return (cardToAdd.Suit == FixedSuit) && (cardToAdd.Rank == Card.MinRank) && cardToAdd.IsFaceUp;

        }

        public RuleSameSuitAscendingRank(CardSuit fixedSuit)
        {
            FixedSuit = fixedSuit;
        }

    }
}
