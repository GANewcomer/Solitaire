using System;
using System.Collections.Generic;
using System.Text;
using Solitaire.Cards;

namespace Solitaire.Game
{
    public interface IStackAddRule
    {

        public string Name { get; }

        /// <summary>
        /// Whether the rule is turned on
        /// </summary>
        public bool IsRuleTurnedOn { get; set; }

        /// <summary>
        /// The rule determining whether a card can be stacked onto another card
        /// </summary>
        /// <param name="cardToAdd">The card that would be added</param>
        /// <param name="cardToStackOnto">The card that would be stacked onto</param>
        /// <returns></returns>
        public bool IsCardAllowedToBeAdded(Card cardToAdd, Card cardToStackOnto);
    }
}
