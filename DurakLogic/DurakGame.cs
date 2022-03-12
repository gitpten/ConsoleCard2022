using CardsModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DurakLogic
{
    public class DurakGame
    {
        private readonly static Random random = new Random();
        public List<Player> Players { get; set; }
        public CardSet Deck { get; set; }
        public CardSet Table { get; set; }
        public Card Trump { get; set; }

        private Action<CardSet> showCards;
        private Action<string> message;
        private Func<Player, Card> chooseCard;
        private Player mover;
        private Player defender;
        private Player attacker;
        public Player activePlayer;
        public bool IsGameOver { get; set; } = false;

        public Player ActivePlayer
        {
            get => activePlayer;
            set
            {
                activePlayer = value;
                //message($"{activePlayer.Name}s Cards:");
                //activePlayer.Hand.Sort();
                //showCards(activePlayer.Hand);
            }
        }

        public DurakGame(List<Player> players,
            Action<CardSet> showCards, Action<string> message, Func<Player, Card> chooseCard)
        {
            Players = players;
            this.showCards = showCards;
            this.message = message;
            this.chooseCard = chooseCard;
            Deck = new CardSet();
            Deck.Full();
            Deck.CutTo(36);
            Table = new CardSet();
            StartGame();            
        }


        private void StartGame()
        {
            Deck.Shuffle();

            Trump = Deck.LastCard;
            foreach (var player in Players)
            {
                player.Hand = Deck.Deal(6);
                player.Hand.Sort();
            }
            message("All right. Trump:");
            showCards(new CardSet(Trump));
            attacker = WhoFirst();
            defender = NextPlayer(attacker);
            mover = attacker;
            ActivePlayer = mover;
        }

        private Player WhoFirst()
        {
            Card minCard = new Card(Trump.Suite, CardFigure.Ace);
            Player active = Players[0];
            foreach (var player in Players)
            {
                foreach (var card in player.Hand)
                {
                    if (card.Suite == Trump.Suite && card.Figure < minCard.Figure)
                    {
                        minCard = card;
                        active = player;
                    }
                }
            }
            return active;
        }

        public void Move(Card movingCard)
        {
            if (IsGameOver) return;

            if (!activePlayer.Hand.Cards.Contains(movingCard)) return;

            if (ActivePlayer == mover && Table.Count > 0 && Table.Cards.FirstOrDefault(c => c.Figure == movingCard.Figure) == null) return;

            if (ActivePlayer == defender && !IsBeat(movingCard, Table.LastCard)) return;

            Table.Add(ActivePlayer.Hand.Pull(movingCard));
            ActivePlayer = ActivePlayer == mover ? defender : mover;
        }

        public void DefenderTake()
        {
            if (IsGameOver) return;

            message($"Player{defender.Name} is taking");
            PutForTaking(defender.Hand.Count);
            defender.Hand.Add(Table.Deal(Table.Count));            
            CardDraw();
            attacker = NextPlayer(defender);
            defender = NextPlayer(attacker);
            mover = attacker;
            ActivePlayer = mover;            
        }

        private void PutForTaking(int count)
        {
            Player player = mover;
            while(count > 0)
            {
                Card cardForAdding = chooseCard(player);
                if (cardForAdding == null)
                {
                    player = NextPlayer(player);
                    if (player == defender) player = NextPlayer(player);
                    if (player == mover) return;
                }
                else
                {
                    defender.Hand.Add(player.Hand.Pull(cardForAdding));
                    count--;
                }
            }
        }

        public void NextMover()
        {
            if (IsGameOver) return;
            if (ActivePlayer == defender) return;            
            Player nextMover = NextPlayer(mover);
            if (nextMover == defender) nextMover = NextPlayer(nextMover);
            if (nextMover == attacker) 
                Beat();
            else
            {
                mover = nextMover;
                message($"Player{mover} is moving");
                ActivePlayer = mover;
            }
        }
        public void Beat()
        {
            if (IsGameOver) return;
            Table.Cards.Clear();
            message($"There is beat");
            CardDraw();
            attacker = defender;
            defender = NextPlayer(attacker);
            mover = attacker;
            ActivePlayer = mover;
        }

        public void NextMove()
        {
            if (IsGameOver) return;
            showCards(Table);
            Card movingCard = chooseCard(ActivePlayer);
            while(movingCard == null && Table.Count == 0)
            {
                message("You must select card");
                movingCard = chooseCard(ActivePlayer);
            }
            if (movingCard == null)
                DefaultAction(ActivePlayer);
            Move(movingCard);
        }

        private void DefaultAction(Player player)
        {
            if (player == defender)
                DefenderTake();
            else if (player == mover)
                NextMover();
        }

        private void CardDraw()
        {
            CardDraw(attacker);
            foreach (var player in Players)
            {
                if (player != attacker && player != defender)
                    CardDraw(player);
            }
            CardDraw(defender);
            if(Players.Count == 1)
            {
                IsGameOver = true;
                message($"{Players[0].Name} loose! Game over!");
            }
        }

        private void CardDraw(Player player)
        {
            if(Deck.Count == 0 && player.Hand.Count == 0)
            {
                Players.Remove(player);
                return;
            }
            int count = player.Hand.Count;
            if (count < 6) player.Hand.Add(Deck.Deal(6 - count));
            player.Hand.Sort();
        }

        public Player NextPlayer(Player player)
        {
            if (Players[Players.Count - 1] == player) return Players[0];
            return Players[Players.IndexOf(player) + 1];
        }

        public Player PeviousPlayer(Player player)
        {
            if (Players[0] == player) return Players[Players.Count - 1];
            return Players[Players.IndexOf(player) - 1];
        }
        public bool IsBeat(Card attacking, Card defending)
        {
            if (attacking.Suite == defending.Suite) return attacking.Figure > defending.Figure;
            if (attacking.Suite == Trump.Suite) return true;
            return false;
        }
    }
}
