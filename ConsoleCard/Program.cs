using CardsModel;
using DurakLogic;
using System;
using System.Collections.Generic;

namespace ConsoleCard
{
    class Program
    {
        static DurakGame game;
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            List<Player> players = new List<Player>()
            {
                new Player("Vasia"),
                new Player("Petia"),
                new Player("Oleg")
            };
            game = new DurakGame(players, ShowState);
            game.Prepare();
            game.Deal();
            while(!game.IsGameOver)
            {           
                WaitAction();
            }
        }

        private static void WaitAction()
        {
            CardSet hand = game.ActivePlayer.Hand;
            Console.Write("Enter letter to action or number of card to move: ");
            int number;
            string answ = Console.ReadLine();
            while (!int.TryParse(answ, out number) || number - 1 < 0 || number - 1 >= hand.Count)
            {
                if (answ.ToLower() == "t")
                {
                    game.GiveUp();
                    return;
                }
                if (answ.ToLower() == "p")
                {
                    game.Pass();
                    return;
                }
                Console.Write("Not correct! Enter number or letter: ");
                answ = Console.ReadLine();
            }
            game.Turn(hand[number - 1]);
        }

        private static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        private static void ShowState()
        {
            Console.Clear();
            Console.WriteLine(game.ResultInfo);
            Console.WriteLine(game.StateInfo);
            Console.WriteLine($"Trump: {CardSymbol(game.Trump)}");

            if (game.Table.Count > 0)
            {
                Console.WriteLine("------------");
                WriteCards(game.Table);
                Console.WriteLine("------------");
            }

            foreach (var command in game.GetPossibleActions())
            {
                Console.WriteLine($"{command[0]} — {command}");
            }

            CardSet hand = game.ActivePlayer.Hand;
            Console.WriteLine($"Cards of {game.ActivePlayer.Name}:");
            for (int i = 0; i < hand.Count; i++)
            {
                Console.Write($"{i + 1,-4}");
            }
            Console.WriteLine();
            WriteCards(hand);
        }

        private static void WriteCards(CardSet cards)
        {
            foreach (var item in cards)
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
