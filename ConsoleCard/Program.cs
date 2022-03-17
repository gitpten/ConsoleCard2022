using CardsModel;
using DurakLogic;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleCard
{
    class Program
    {
        static DurakGame game;
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            List<Player> players = new List<Player>()
            {
                new Player("Vasia"),
                new Player("Petia"),
                new Player("Oleg")
            };
            game = new DurakGame(players, ShowState);
            game.Prepare();
            game.Deal();
            while (!game.IsGameOver)
            {
                WaitAction();
            }
        }

        private static void WaitAction()
        {
            string answer = Console.ReadLine();
            int number;
            CardSet hand = game.ActivePlayer.Hand;
            while(!int.TryParse(answer, out number) || number - 1 < 0 || number - 1 >= hand.Count)
            {
                if(answer.ToLower() == "g")
                {
                    game.GiveUp();
                    return;
                }

                if (answer.ToLower() == "p")
                {
                    game.Pass();
                    return;
                }
                Console.WriteLine("Not correct! Enter nunmber or letter: ");
                answer = Console.ReadLine();
            }
            game.Turn(hand[number - 1]);
        }

        private static void ShowState()
        {
            Console.Clear();
            Console.WriteLine(game.ResultInfo);
            Console.WriteLine($"{game.StateInfo}...");
            Console.WriteLine($"*{CardSymbol(game.Trump)}*");
            if(game.Table.Count > 0)
            {
                Console.WriteLine("-----------------");
                ShowCards(game.Table);
                Console.WriteLine("-----------------");
            }
            string pa = game.GetPossibleAction();
            Console.WriteLine($"To {pa} enter {pa[0]}");
            Console.WriteLine($"Or enter number of card to turn:");
            for (int i = 1; i <= game.ActivePlayer.Hand.Count; i++)
            {
                Console.Write($"{i, -4}");
            }
            Console.WriteLine();
            ShowCards(game.ActivePlayer.Hand);

        }

        private static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        private static void ShowCards(CardSet cards)
        {
            foreach (var item in cards)
            {
                Console.Write($"{CardSymbol(item), -4}");
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
