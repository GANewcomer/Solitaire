using System;
using System.Collections.Generic;
using System.Text;
using Solitaire.Cards;

namespace Solitaire.Game
{
    public struct Move : IComparable
    {
        public string StackFromName { get; private set; }

        public string StackToName { get; private set; }

        public int NumCards { get; private set; }

        public bool ReverseCardOrder { get; private set; }

        public double Ranking { get; private set; }

        public Move(int numCards, CardStack stackFrom, CardStack stackTo, double ranking, bool reverseCardOrder = false)
        {
            StackFromName = stackFrom.Name;
            StackToName = stackTo.Name;
            NumCards = numCards;
            Ranking = ranking;
            ReverseCardOrder = reverseCardOrder;
        }

        /// <summary>
        /// Create a move that is the opposite to this move
        /// </summary>
        /// <returns></returns>
        public Move Opposite()
        {
            return new Move()
            {
                StackFromName = StackToName,
                StackToName = StackFromName,
                NumCards = NumCards,
                ReverseCardOrder = ReverseCardOrder,
                Ranking = Ranking,
            };
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", StackFromName, StackToName);
        }

        public int CompareTo(object obj)
        {
            if (obj is Move)
                return Ranking.CompareTo(((Move)obj).Ranking);
            else
                return 0;
        }
    }
}
