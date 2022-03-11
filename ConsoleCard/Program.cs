using CardsModel;
using DurakLogic;
using System;
using System.Collections.Generic;

namespace ConsoleCard
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Player> players = new List<Player>()
            {
                new Player("Vasia"),
                new Player("Petia"),
                new Player("Oleg")
            };
            DurakGame game = new DurakGame(players, ShowCards, ShowMessage);

        }

        private static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        private static void ShowCards(CardSet cards)
        {
            foreach (var item in cards.Cards)
            {
                Console.Write($"{CardSymbol(item)} ");
            }
        }

        private static string CardSymbol(Card card)
        {
            string s = "";
            if ((int)card.Figure <= 10)
                s += ((int)card.Figure).ToString();
            else
                switch (card.Figure)
                {
                    case CardFigure.Jack:
                        s += "J";
                        break;
                    case CardFigure.Queen:
                        s += "Q";
                        break;
                    case CardFigure.King:
                        s += "K";
                        break;
                    case CardFigure.Ace:
                        s += "A";
                        break;
                    default:
                        break;
                }

            switch (card.Suite)
            {
                case CardSuite.Diamond:
                    s += "♦";
                    break;
                case CardSuite.Club:
                    s += "♣";
                    break;
                case CardSuite.Heart:
                    s += "♥";
                    break;
                case CardSuite.Spade:
                    s += "♠";
                    break;
                default:
                    break;
            }

            return s;
        }
    }
}
