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
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            List<Player> players = new List<Player>()
            {
                new Player("Vasia"),
                new Player("Petia"),
                new Player("Oleg")
            };
            DurakGame game = new DurakGame(players, ShowCards, ShowMessage, ChooseCard);
            while(true)
            {
                game.NextMove();
            }
        }

        private static Card ChooseCard(Player player)
        {
            CardSet hand = player.Hand;
            Console.WriteLine($"Cards of {player.Name}:");
            for (int i = 0; i < hand.Count; i++)
            {
                Console.Write($"{i + 1, -4}");
            }
            Console.WriteLine();
            WriteCards(hand);
            Console.Write("Enter number of card or '-' to pass: ");
            int number;
            string answ = Console.ReadLine();
            while (!int.TryParse(answ, out number) || number - 1 < 0 || number - 1 >= hand.Count)
            {
                if (answ == "-")
                    return null;
                Console.Write("Not correct! Enter number or '-': ");
                answ = Console.ReadLine();
            } 
            return hand[number - 1];           
        }

        private static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        private static void ShowCards(CardSet cards)
        {
            Console.WriteLine("------------");
            WriteCards(cards);
            Console.WriteLine("------------");
        }

        private static void WriteCards(CardSet cards)
        {
            foreach (var item in cards.Cards)
            {
                Console.Write($"{CardSymbol(item),-4}");
            }
            Console.WriteLine();
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
