using Prism.Mvvm;
using Solitaire.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solitaire.Game
{
    public class GameSummary : BindableBase
    {

        private BoardStatus status = BoardStatus.InProgress;
        private int moveCount = -1;
        private int cycleCount = -1;
        private double difficulty = -1;

        public int GameID { get; }

        public Deck OriginalDeck { get; }

        public BoardStatus Status { get => this.status;
            set
            {
                SetProperty(ref this.status, value);
            }
        }

        public int MoveCount { get => this.moveCount;
            set 
            {
                SetProperty(ref this.moveCount, value);
                Difficulty = CycleCount / (double)MoveCount;
            }
        }

        public int CycleCount { get => this.cycleCount;
            set 
            {
                SetProperty(ref this.cycleCount, value);
                Difficulty = CycleCount / (double)MoveCount;
            }
        }

        public double Difficulty { get => this.difficulty;
            set 
            {
                SetProperty(ref this.difficulty, value);
            }
        }

        public List<string> GameStates { get; set; } = new List<string>();


        public GameSummary(int gameID, Deck deck)
        {
            GameID = gameID;
            OriginalDeck = deck.Copy();
        }

        public void FinishGame(Tableau tableau, int cycleCount)
        {
            Status = tableau.Status;
            MoveCount = tableau.Moves.Count;
            CycleCount = cycleCount;

        }

    }
}
