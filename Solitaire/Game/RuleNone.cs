using Solitaire.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solitaire.Game
{
    public class RuleNone : IStackAddRule
    {
        public string Name { get; } = "None";

        public bool IsRuleTurnedOn { get; set; }

        public bool IsCardAllowedToBeAdded(Card cardToAdd, Card cardToStackOnto)
        {
            return true;
        }
    }
}
