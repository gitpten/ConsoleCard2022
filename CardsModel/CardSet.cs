using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardsModel
{
    public class CardSet:IEnumerable<Card>
    {
        private readonly static Random random = new Random();
        public CardSet(List<Card> cards)
        {
            Cards = cards;
        }

        public CardSet() : this(new List<Card>()) { }

        public CardSet(params Card[] cards):this(new List<Card>(cards)) { }

        public List<Card> Cards { get; set; }

        public int Count { get => Cards.Count; }

        public Card LastCard { get => Cards[Count - 1]; }

        public Card this[int i]
        {
            get => Cards[i];
            set => Cards[i] = value;
        }

        public Card Pull(int number = 0)
        {
            if (number < 0 || number >= Count)
                throw new Exception("The number of card was out of range");

            Card card = Cards[number];
            RemoveCard(number);
            return card;
        }

        public virtual void RemoveCard(int number)
        {
            Cards.RemoveAt(number);
        }

        public virtual void RemoveCard(Card card)
        {
            Cards.Remove(card);
        }


        public Card Pull(Card card)
        {
            Card foundCard = Cards.FirstOrDefault(c => c.Equals(card));
            if (foundCard != null) RemoveCard(foundCard);
            return foundCard;
        }

        public CardSet Deal(int amount)
        {
            if (amount < 1)
                throw new Exception("Amout of card for deal must be greater than zero");

            if (amount > Count) amount = Count;

            CardSet cardSet = GetBlankCardSet();
            for (int i = 0; i < amount; i++)
            {
                cardSet.Add(Pull());
            }
            return cardSet;
        }

        public virtual CardSet GetBlankCardSet()
        {
            return new CardSet();
        }

        public virtual void Add(params Card[] cards)
        {
            Cards.AddRange(cards);
        }

        public void Add(List<Card> cards)
        {
            Add(cards.ToArray());
        }

        public void Add(CardSet cards)
        {
            Add(cards.Cards);
        }

        public void Full()
        {
            foreach (CardFigure figure in Enum.GetValues(typeof(CardFigure)))
            {
                foreach (CardSuite suite in Enum.GetValues(typeof(CardSuite)))
                {
                    Add(GetCard(suite, figure));
                }
            }
        }

        public virtual Card GetCard(CardSuite suite, CardFigure figure)
        {
            return new Card(suite, figure);
        }

        public void Sort()
        {
            Cards.Sort(
                (card1, card2) => card1.Figure.CompareTo(card2.Figure) == 0 ?
                    card1.Suite.CompareTo(card2.Suite) :
                    card1.Figure.CompareTo(card2.Figure)
                );
        }

        public void Shuffle()
        {
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < Count; i++)
                {
                    int randNum = random.Next(Count);
                    Card temp = Cards[i];
                    Cards[i] = Cards[randNum];
                    Cards[randNum] = temp;
                }
            }
        }

        public void CutTo(int amount)
        {
            while (Count > amount)
            {
                RemoveCard(0);
            }
        }

        public IEnumerator<Card> GetEnumerator() => Cards.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Cards.GetEnumerator();

        public void Clear()
        {
            CutTo(0);
        }
    }
}
