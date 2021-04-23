using System;
using System.Collections.Generic;
using System.Text;

namespace Solitaire.Cards
{
    public static class Extensions
    {
        /// <summary>
        /// Get the name of the card rank
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        public static string GetRankName(this int rank)
        {
            string rankString = rank.ToString();
            if (rank == 1)
                rankString = "Ace";
            else if (rank == 11)
                rankString = "Jack";
            else if (rank == 12)
                rankString = "Queen";
            else if (rank == 13)
                rankString = "King";

            return rankString;
        }

    }
}
