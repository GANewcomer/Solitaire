using System;
using System.Collections.Generic;
using System.Text;
using Solitaire.Cards;
using System.Linq;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Solitaire.Game
{
    public class CardStack : BindableBase
    {
        #region Properties

        private ObservableCollection<Card> stack = new ObservableCollection<Card>();

        public string Name { get; }

        /// <summary>
        /// The rule that cards must follow to be allowed to be added to this stack
        /// </summary>
        public IStackAddRule AddRule { get; }

        /// <summary>
        /// The list of cards that comprise this card stack
        /// </summary>
        public ObservableCollection<Card> Stack 
        { 
            get => this.stack;
            private set
            {
                SetProperty(ref this.stack, value);
                UpdateThreadSyncing();

            }
        }

        private int cardCount;

        /// <summary>
        /// The number of cards in this stack
        /// </summary>
        public int CardCount { 
            get => this.cardCount;
            private set
            {
                SetProperty(ref this.cardCount, value);
            }

        }

        /// <summary>
        /// The top card of this stack
        /// </summary>
        public Card TopCard { get 
            {
                lock (this.stackLock)
                {
                    if (Stack == null || Stack.Count == 0)
                        return null;

                    return Stack.Last();
                }
            }
        }
        
        /// <summary>
        /// The bottom card of this stack
        /// </summary>
        public Card BaseCard { get 
            {
                lock (this.stackLock)
                {
                    if (Stack == null || Stack.Count == 0)
                        return null;

                    return Stack.First();
                }
            }
        }

        private object stackLock = new object();

        #endregion Properties

        #region Methods

        /// <summary>
        /// Add a card to this stack
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public bool AddCard(Card card)
        {
            if (!AddRule.IsCardAllowedToBeAdded(card, TopCard))
                return false;

            lock (this.stackLock)
            {
                Stack.Add(card);
                CardCount = Stack.Count;
                return true;
            }
        }

        /// <summary>
        /// Add a stack of cards to this stack
        /// </summary>
        /// <param name="stack">Stack to add to the current stack. This stack will be empty when this is completed</param>
        /// <returns></returns>
        public bool AddStack(CardStack stack)
        {
            lock (this.stackLock)
            {
                if (stack.Stack.Count == 0)
                    return false;
            }

            // checking base card for transfer
            if (!AddRule.IsCardAllowedToBeAdded(stack.BaseCard, TopCard))
                return false;

            // transferring this stack
            while (stack.TransferBaseCard(this))
            {}

            if (stack.CardCount != 0)
                throw new InvalidOperationException("All the cards of this stack could not be added even though the base card could be added");

            return true;
        }


        /// <summary>
        /// Take a card from this stack
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Card TakeCard(int position)
        {
            lock (this.stackLock)
            {
                if (Stack == null || Stack.Count == 0)
                    return null;
                if (position <= 0 || Stack.Count < position)
                    throw new ArgumentException("Position must be >= 1 and <= " + Stack.Count);

                //Getting the position from the end
                int reversePosition = Stack.Count - position;

                Card removedCard = Stack[reversePosition];
                Stack.RemoveAt(reversePosition);
                CardCount = Stack.Count;
                return removedCard;
            }
        }


        /// <summary>
        /// Take a stack of cards from the top of this stack
        /// </summary>
        /// <param name="length">The number of cards to take from the top</param>
        /// <returns>Stack of cards in the same order that they were in this stack</returns>
        public List<Card> TakeTopMost(int length)
        {
            lock (this.stackLock)
            {
                if (Stack.Count == 0)
                    return new List<Card>();
            }
            if (length <= 0)
                throw new ArgumentException("Length must be >= 1");

            // transferring cards
            CardStack topmost = new CardStack(new RuleNone());
            for (int i = 0; i < length; i++)
            {
                TransferTopCard(topmost);
            }

            // reversing stack (to match original order)
            topmost.ReverseStack();

            return topmost.Stack.ToList();
        }


        /// <summary>
        /// Deal a number of cards out from the top of this stack
        /// </summary>
        /// <param name="length">The number of cards to deal out</param>
        /// <returns>Stack of cards</returns>
        public List<Card> DealTopMost(int length)
        {
            lock (this.stackLock)
            {
                if (Stack.Count == 0)
                    return new List<Card>();
            }
            if (length <= 0)
                throw new ArgumentException("Length must be >= 1");

            // transferring cards
            CardStack topmost = new CardStack(new RuleNone());
            for (int i = 0; i < length; i++)
            {
                TransferTopCard(topmost);
            }

            return topmost.Stack.ToList();
        }

        /// <summary>
        /// See a card from this stack
        /// </summary>
        /// <param name="position">The position of the card to grab from the top (one-based)</param>
        /// <returns></returns>
        public Card SeeCard(int position)
        {
            lock (this.stackLock)
            {
                if (Stack == null || Stack.Count == 0)
                    return null;
                if (position <= 0)
                    throw new ArgumentException("Position must be > 0");

                //Getting the position from the end
                int reversePosition = Stack.Count - position;

                if (reversePosition < 0)
                    return Stack.First();
                else
                    return Stack[reversePosition];
            }

        }


        /// <summary>
        /// Get the position of a card within this stack
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public int GetPosition(Card card)
        {
            lock (this.stackLock)
            {
                int pos = -1;
                for (int i = 0; i < Stack.Count; i++)
                {
                    if (Stack[i].ShortName == card.ShortName)
                    {
                        pos = i;
                        break;
                    }
                }

                if (pos != -1)
                    return CardCount - pos;
                else
                    return pos;
            }
        }

        /// <summary>
        /// Transfer the top card of this deck to the top of another stack
        /// </summary>
        /// <param name="transferStack">The stack to transfer the top card to</param>
        public bool TransferTopCard(CardStack transferStack)
        {
            //Removing card
            Card transferredCard = TakeCard(1);
            //Giving card to other stack
            if (transferredCard != null)
            {
                bool wasTransferred =  transferStack.AddCard(transferredCard);
                if (!wasTransferred)
                    AddCard(transferredCard);   // returning card to the original stack
                return wasTransferred;
            }
            else
                return false;
        }

        /// <summary>
        /// Transfer the bottom card of this deck to the top of another stack
        /// </summary>
        /// <param name="transferStack">The stack to transfer the bottom card to</param>
        public bool TransferBaseCard(CardStack transferStack)
        {
            //Removing card
            Card transferredCard = TakeCard(CardCount);
            //Giving card to other stack
            if (transferredCard != null)
            {
                bool wasTransferred = transferStack.AddCard(transferredCard);
                if (!wasTransferred)
                    AddCard(transferredCard);   // returning card to the original stack
                return wasTransferred;
            }
            else
                return false;
        }

        /// <summary>
        /// Reverse the order of the cards in this stack 
        /// </summary>
        /// <returns></returns>
        public void ReverseStack()
        {
            lock (this.stackLock)
            {
                // reversing stack
                CardStack reversed = new CardStack(AddRule);
                while (this.TransferTopCard(reversed))
                { }

                // setting
                Stack.Clear();
                foreach (Card card in reversed.Stack)
                {
                    Stack.Add(card);
                }
                CardCount = Stack.Count;
            }
        }

        public void UpdateStackCount()
        {
            lock (this.stackLock)
            {
                CardCount = Stack.Count;
            }
        }

        public void UpdateThreadSyncing()
        {
            BindingOperations.EnableCollectionSynchronization(Stack, this.stackLock);
        }

        #endregion Methods

        #region Constructors

        /// <summary>
        /// Create an empty card stack
        /// </summary>
        /// <param name="addingRule">The adding rule</param>
        public CardStack(IStackAddRule addingRule, string name = "")
        {
            AddRule = addingRule;
            Name = name;
            Stack = new ObservableCollection<Card>();
            CardCount = Stack.Count;
        
        }


        /// <summary>
        /// Create a card stack from an existing stack
        /// </summary>
        /// <param name="stack">The stack of cards to create this stack from</param>
        /// <param name="addingRule">The adding rule</param>
        public CardStack(List<Card> stack, IStackAddRule addingRule, string name = "")
        {
            AddRule = addingRule;
            Stack = new ObservableCollection<Card>(stack);
            Name = name;
            CardCount = Stack.Count;

        }

        /// <summary>
        /// Create a card stack from a deck of cards
        /// </summary>
        /// <param name="stack">The deck of cards to create this stack from</param>
        /// <param name="addingRule">The adding rule</param>
        public CardStack(Deck deck, IStackAddRule addingRule, string name = "")
        {
            AddRule = addingRule;
            Stack = new ObservableCollection<Card>(deck.Cards.ToList());
            Name = name;
            CardCount = Stack.Count;

        }

        #endregion Constructors

        /// <summary>
        /// Create a summary string of this stack
        /// </summary>
        /// <returns></returns>
        public string GetSummary()
        {
            lock (this.stackLock)
            {
                string summary = Name + ":{";
                foreach (Card card in Stack)
                {
                    if (card.IsFaceUp)
                        summary += card.ShortName + ",";
                    else
                        summary += "(H)" + ",";
                }

                if (Stack.Count > 0)
                    summary = summary.Substring(0, summary.Length - 1) + "}";

                return summary;
            }
        }

        public override string ToString()
        {
            lock (this.stackLock)
            {
                if (Name != "")
                    return string.Format("Stack[{0}] Name={1}", CardCount, Name);
                else
                    return string.Format("Stack[{0}]", CardCount);
            }

        }

        /// <summary>
        /// Copy this card stack into a new stack
        /// </summary>
        /// <returns></returns>
        public CardStack Copy()
        {
            lock (this.stackLock)
            {
                ObservableCollection<Card> newStack = new ObservableCollection<Card>();
                foreach (Card card in Stack)
                {
                    newStack.Add(card.Copy());
                }

                var newCardStack = new CardStack(AddRule, Name);
                newCardStack.Stack = newStack;

                return newCardStack;
            }
        }

    }
}
