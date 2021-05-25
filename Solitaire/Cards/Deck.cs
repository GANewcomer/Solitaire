using Solitaire.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solitaire.Cards
{
    /// <summary>
    /// A deck of 52 cards
    /// </summary>
    public class Deck
    {
        /// <summary>
        /// The cards that make up this deck
        /// </summary>
        public Card[] Cards { get; private set; }


        /// <summary>
        /// Create an order deck of cards
        /// </summary>
        public Deck()
        {
            //Creating ordered deck
            Cards = new Card[52];

            CardSuit[] suits = (CardSuit[])Enum.GetValues(typeof(CardSuit));
            int maxRank = Card.MaxRank;
            int iCount = 0;
            foreach (CardSuit suit in suits)
            {
                for (int rank = 1; rank <= maxRank; rank++)
                {
                    Cards[iCount] = new Card(suit, rank);
                    iCount++;
                }
            }

        }

        /// <summary>
        /// Create a deck from a deck summary string
        /// </summary>
        /// <param name="deckString">The summary string of a deck of cards</param>
        public Deck(string deckString)
        {
            string[] deckSplit = deckString.Split(';');
            if (deckSplit.Length != Card.MaxRank * Card.NumSuits)
                throw new ArgumentException("Cannot create deck from this string => " + deckString);

            // setting deck
            Cards = new Card[deckSplit.Length];
            for (int i = 0; i < deckSplit.Length; i++)
            {
                string cardString = deckSplit[i].Replace("{", "");
                cardString = cardString.Replace("}", "");

                Card card = new Card(cardString);
                Cards[i] = card;

            }

        }


        /// <summary>
        /// Shuffle this deck of cards into a new arrangement
        /// </summary>
        public void ShuffleDeck()
        {
            Random rn = new Random();
            List<Card> oldDeck = Cards.ToList();
            List<Card> newDeck = new List<Card>();
            while (newDeck.Count != 52)
            {
                // generating random index from the remaining cards in the old deck
                int index = rn.Next(0, oldDeck.Count);

                // adding the card of the random index into the new list
                Card card = oldDeck[index];
                if (card.IsFaceUp)
                    card.Flip();
                oldDeck.Remove(card);
                newDeck.Add(card);
            }

            //Setting new deck
            Cards = newDeck.ToArray();

        }


        /// <summary>
        /// Copy this deck into a new Deck object
        /// </summary>
        /// <returns></returns>
        public Deck Copy()
        {
            Deck copyDeck = new Deck();
            for (int i = 0; i < copyDeck.Cards.Length; i++)
            {
                copyDeck.Cards[i] = Cards[i].Copy();
            }

            return copyDeck;
        }

        /// <summary>
        /// Convert this deck to the string list of the card arrangement
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string deck = "{";
            for (int i = 0; i < Cards.Length; i++)
            {
                deck += Cards[i].ShortName + ";";
            }

            deck = deck.Substring(0, deck.Length - 1) + "}";

            return deck;
        }

        /// <summary>
        /// Compare 2 decks of cards
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Deck))
                return false;

            return obj.ToString() == this.ToString();
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
