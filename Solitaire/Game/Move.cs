using System;
using System.Collections.Generic;
using System.Text;
using Solitaire.Cards;

namespace Solitaire.Game
{
    public struct Move
    {
        public string StackFromName { get; }

        public string StackToName { get; }

        public int NumCards { get; }

        public bool ReverseCardOrder { get; }

        public double Ranking { get; }

        public Move(int numCards, CardStack stackFrom, CardStack stackTo, double ranking, bool reverseCardOrder = false)
        {
            StackFromName = stackFrom.Name;
            StackToName = stackTo.Name;
            NumCards = numCards;
            Ranking = ranking;
            ReverseCardOrder = reverseCardOrder;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", StackFromName, StackToName);
        }

    }
}
