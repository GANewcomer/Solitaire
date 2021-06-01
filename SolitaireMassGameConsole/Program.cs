using Solitaire.Cards;
using Solitaire.Game;
using System;
using System.Threading.Tasks;

namespace SolitaireMassGameConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //SETUP
            int numGamesToRun = 1000;
            Player player = new Player();

            //OUTPUT
            Console.WriteLine("--SOLITAIRE MASS GAME SOLVER--");
            Console.WriteLine(string.Format(" running {0} games", numGamesToRun));
            Console.WriteLine();

            //Console.Write("Num games = ");
            //string input = Console.ReadLine();

            //RUNNING GAMES
            if (false)
            {
                Parallel.For(0, numGamesToRun, (i) =>
                {
                    //CREATING AND SHUFFLING DECK
                    Deck deck = new Deck();
                    deck.ShuffleDeck();

                    //CREATING TABLEAU
                    Tableau tableau = new Tableau(deck);

                    //SOLVING GAME
                    Console.WriteLine(string.Format(" --> Game {0,4} started. Deck = {1}", i, deck.ToString()));
                    GameSummary game = player.SolveGame(tableau);

                    //OUTPUT
                    Console.WriteLine(string.Format("Game {0,4}: {1,-10} - Win Ratio = {2:N3}%", i, game.Status, player.WinPercentage));

                });
            }
            else
            {
                for (int i = 0; i < numGamesToRun; i++)
                {
                    //CREATING AND SHUFFLING DECK
                    Deck deck = new Deck();
                    deck.ShuffleDeck();

                    //CREATING TABLEAU
                    Tableau tableau = new Tableau(deck);

                    //SOLVING GAME
                    Console.Write(string.Format(" --> Game {0,4} started. Deck = {1}", i, deck.ToString()));
                    GameSummary game = player.SolveGame(tableau);
                    Console.WriteLine(" ==> " + game.Status.ToString());

                    //OUTPUT
                    //Console.WriteLine(string.Format("Game {0,4}: {1,-10} - Win Ratio = {2:N3}%", i, game.Status, player.WinPercentage));

                }
            }
        }
    }
}
