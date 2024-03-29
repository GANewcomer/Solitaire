﻿using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solitaire.Cards
{
    public class Card : BindableBase
    {
        #region Properties

        public const int MinRank = 1;
        public const int MaxRank = 13;
        public const int NumSuits = 4;

        private CardSuit suit;
        private int rank;
        private bool isFaceUp;

        /// <summary>
        /// This card's suit
        /// </summary>
        public CardSuit Suit { get => this.suit;
            private set
            {
                this.suit = value;
                switch (Suit)
                {
                    case CardSuit.Spade:
                    case CardSuit.Club:
                        Color = CardColor.Black;
                        break;
                    case CardSuit.Diamond:
                    case CardSuit.Heart:
                        Color = CardColor.Red;
                        break;
                    default:
                        throw new NotImplementedException();
                }

            }
        }

        /// <summary>
        /// This card's rank
        /// </summary>
        public int Rank { get => this.rank;
            private set
            {
                if (value < MinRank || MaxRank < value)
                    throw new ArgumentException(string.Format("Card rank must be from {0} to {1}", MinRank, MaxRank));

                this.rank = value;
            }
        }

        /// <summary>
        /// The name of this card's rank
        /// </summary>
        public string RankName { get; }

        /// <summary>
        /// The color of this card
        /// </summary>
        public CardColor Color { get; private set; }

        /// <summary>
        /// Whether this card is exposed
        /// </summary>
        public bool IsFaceUp { get => this.isFaceUp;
            private set
            {
                SetProperty(ref this.isFaceUp, value);
            }

        }

        /// <summary>
        /// The short name of this card ("Suit"-"Rank")
        /// </summary>
        public string ShortName { get; }

        /// <summary>
        /// The full name of this card ("Rank"-"of"-"Suit"s)
        /// </summary>
        public string FullName { get; }

        #endregion Properties

        /// <summary>
        /// Create a new Card
        /// </summary>
        /// <param name="suit"></param>
        /// <param name="rank"></param>
        public Card(CardSuit suit, int rank, bool faceUp = false)
        {
            Suit = suit;
            Rank = rank;
            IsFaceUp = faceUp;
            RankName = rank.GetRankName();

            //Short name
            ShortName = Suit.ToString().Substring(0, 1) + Rank.ToString();

            //Full name
            FullName = RankName + " of " + Suit.ToString() + "s";

        }

        /// <summary>
        /// Create a new card from a short name
        /// </summary>
        /// <param name="shortName"></param>
        /// <param name="isFaceUp"></param>
        public Card(string shortName, bool isFaceUp = false)
        {
            string suitLetter = shortName.Substring(0, 1);
            string rankNumber = shortName.Substring(1);

            switch (suitLetter)
            {
                case "S":
                    Suit = CardSuit.Spade;
                    break;
                case "C":
                    Suit = CardSuit.Club;
                    break;
                case "H":
                    Suit = CardSuit.Heart;
                    break;
                case "D":
                    Suit = CardSuit.Diamond;
                    break;
                default:
                    throw new ArgumentException("Suit letter does not match any suit (must be S, C, H, or D)");
            }

            Rank = Convert.ToInt32(rankNumber);
            IsFaceUp = isFaceUp;
            RankName = Rank.GetRankName();

            //Short name
            ShortName = Suit.ToString().Substring(0, 1) + Rank.ToString();

            //Full name
            FullName = RankName + " of " + Suit.ToString() + "s";


        }

        /// <summary>
        /// Flip this card
        /// </summary>
        public void Flip()
        {
            IsFaceUp = !IsFaceUp;
        }

         /// <summary>
        /// Convert this Card to its string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (IsFaceUp)
                return FullName;
            else
                return "(Hidden)";
        }


        /// <summary>
        /// Copy this card into a new card object
        /// </summary>
        /// <returns></returns>
        public Card Copy()
        {
            return new Card(Suit, Rank, IsFaceUp);
        }

    }
}
