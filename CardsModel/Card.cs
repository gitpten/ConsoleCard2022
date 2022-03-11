using System;
using System.Collections.Generic;
using System.Text;

namespace CardsModel
{
    public class Card
    {
        public Card(CardSuite suite, CardFigure figure)
        {
            Suite = suite;
            Figure = figure;
        }

        public CardSuite Suite { get; set; }
        public CardFigure Figure { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is Card))
                return false;

            return Equals((Card)obj);
        }

        public bool Equals(Card other)
        {
            if (other == null) return false;

            return other.Figure == Figure && other.Suite == Suite;
        }

        public override int GetHashCode()
        {
            var hashCode = Suite.GetHashCode();
            hashCode = hashCode * -1521134295 + Figure.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"{Figure} {Suite}";
        }
    }
}
