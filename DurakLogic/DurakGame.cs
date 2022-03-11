using CardsModel;
using System;
using System.Collections.Generic;

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
        private Player mover;
        private Player defender;
        private Player attacker;
        public Player activePlayer;

        public Player ActivePlayer
        {
            get => activePlayer;
            set
            {
                activePlayer = value;
                message($"{activePlayer.Name}s Cards:");
                activePlayer.Hand.Sort();
                showCards(activePlayer.Hand);
            }
        }

        public DurakGame(List<Player> players, Action<CardSet> showCards, Action<string> message)
        {
            Players = players;
            this.showCards = showCards;
            this.message = message;
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
            }
            message("All right. Trump:");
            showCards(new CardSet(Trump));
            ActivePlayer = WhoFirst();
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
    }
}
