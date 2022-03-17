using CardsModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DurakLogic
{
    public class Player
    {
        public Player(string name, CardSet hand)
        {
            Name = name;
            Hand = hand;
        }

        public Player(string name):this(name, new CardSet())
        {
        }

        public string Name { get; set; }
        public CardSet Hand { get; set; }
        public bool IsInGame { get; set; } = true;
    }
}
