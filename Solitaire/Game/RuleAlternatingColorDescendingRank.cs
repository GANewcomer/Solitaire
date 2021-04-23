using Solitaire.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solitaire.Game
{
    public class RuleAlternatingColorDescendingRank : IStackAddRule
    {
        public bool IsRuleTurnedOn { get; set; }

        public string Name { get; } = "AlternatingColor-DescendingRank";

        public bool IsCardAllowedToBeAdded(Card cardToAdd, Card cardToStackOnto)
        {
            if (!IsRuleTurnedOn)
                return true;

            if (cardToStackOnto != null)
                return (cardToAdd.Color != cardToStackOnto.Color) && (cardToAdd.Rank == cardToStackOnto.Rank - 1) && cardToAdd.IsFaceUp;
            else
                return cardToAdd.Rank == Card.MaxRank && cardToAdd.IsFaceUp;

        }

    }
}
