using CardsModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DurakLogic
{
    public class DurakGame
    {
        enum Mode{
            Attack,
            Defending,
            PickingUp
        }

        private const string GIVEUPSTR = "Take cards";
        private const string PASSSTR = "Pass the move";

        private readonly static Random random = new Random();
        public List<Player> Players { get; set; }
        public CardSet Deck { get; set; }
        public CardSet Table { get; set; }
        public Card Trump { get; private set; }
        public Player ActivePlayer { get; private set; }
        public string ResultInfo { get; private set; }
        public string StateInfo {
            get 
            {
                if (IsGameOver)
                    return "";
                else if (mode == Mode.PickingUp)
                    return $"{ActivePlayer.Name} is choose card {defender.Name} is piching up. He can put {countCardToTurn} card";
                else if (mode == Mode.Attack)
                    return $"{ActivePlayer.Name} is moving to {defender.Name}";
                else if (mode == Mode.Defending)
                    return $"{ActivePlayer.Name} is defending";
                return "";
            }
        }

        private Mode mode;
        private Player mover;
        private Player defender;
        private Player attacker;
        private Player firstPasser; 

        private Action ShowState;


        public bool IsGameOver { get; set; } = false;
        private int countCardToTurn;

        public List<string> GetPossibleActions()
        {
            List<string> actions = new List<string>();
            if (mode == Mode.Defending)
                actions.Add(GIVEUPSTR);

            if (mode != Mode.Defending && Table.Count > 0)
                actions.Add(PASSSTR);

            return actions;
        }

        public DurakGame(List<Player> players,
            Action showState)
        {
            Players = players;
            ShowState = showState;
            Table = new CardSet();
            Deck = new CardSet();
        }

        public void Prepare()
        {
            foreach (var player in Players)
            {
                player.IsInGame = true;
            }
            Deck.Full();
            Deck.CutTo(36);

            Deck.Shuffle();
        }

        public void Deal()
        {
            Trump = Deck.LastCard;
            foreach (var player in Players)
            {
                player.Hand.Add(Deck.Deal(6));
                player.Hand.Sort();
            }
            ResultInfo = "All right!";
            mode = Mode.Attack;
            attacker = WhoFirst();
            defender = NextPlayer(attacker);
            mover = attacker;
            ActivePlayer = mover;
            countCardToTurn = Math.Min(defender.Hand.Count, 6);
            ShowState();
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

        public void Turn(Card movingCard)
        {
            if (ImpossibleMove(movingCard)) return;

            Table.Add(ActivePlayer.Hand.Pull(movingCard));
            ResultInfo = "";

            SwitchMode();

            ShowState();
        }

        private bool ImpossibleMove(Card movingCard)
        {
            return IsGameOver ||
            (mode != Mode.Defending && ActivePlayer == defender) ||
            (!ActivePlayer.Hand.Contains(movingCard)) ||
            (mode != Mode.Defending && Table.Count > 0 && Table.FirstOrDefault(c => c.Figure == movingCard.Figure) == null) ||
            (mode == Mode.Defending && !IsBeat(movingCard, Table.LastCard));
        }

        private void SwitchMode()
        {
            switch (mode)
            {
                case Mode.Attack:
                    SwitchAfterAttack();
                    break;
                case Mode.Defending:
                    SwitchAfterDefending();
                    break;
                case Mode.PickingUp:
                    SwitchAfterPickedUp();
                    break;
                default:
                    throw new Exception("We don't now this mode");
            }
        }

        private void SwitchAfterDefending()
        {
            if (defender.Hand.Count == 0 || countCardToTurn == 0)
                Beat();
            else
            {
                firstPasser = null;
                TurnAttackMode();                
            }
            ShowState();
        }

        private void SwitchAfterAttack()
        {
            countCardToTurn--;
            TurnDefendingMode();
            ShowState();
        }

        private void SwitchAfterPickedUp()
        {
            countCardToTurn--;
            if (countCardToTurn == 0) PickUp();
            else if (mover.Hand.Count == 0)
            {
                NextMover();
                ActivateMover();
            }
            ShowState();
        }

        private void TurnAttackMode()
        {
            ActivateMover(); 
            mode = Mode.Attack;
        }

        private void TurnDefendingMode()
        {
            mode = Mode.Defending;
            ActivePlayer = defender;
        }
        public void GiveUp()
        {
            mode = Mode.PickingUp;
            if (countCardToTurn == 0)
                PickUp();
            ActivateMover();
            ShowState();
        }
        private void ActivateMover()
        {
            if (mover.Hand.Count == 0) NextMover();
            ActivePlayer = mover;
        }


        private void PickUp()
        {
            defender.Hand.Add(Table.Deal(Table.Count));
            ResultInfo = $"{defender.Name} has taken cards.";
            NewTurn();
        }

        private void Beat()
        {
            Table.Clear();
            ResultInfo = $"There is beat";
            NewTurn();
        }

        private void NewTurn()
        {
            CardDraw();
            CheckWinner();
            if (IsGameOver) return;
            if (mode == Mode.PickingUp)
                attacker = NextPlayer(defender);
            else
                attacker = defender.IsInGame ? defender : NextPlayer(defender);
            defender = NextPlayer(attacker);
            mover = attacker;
            firstPasser = null;
            countCardToTurn = Math.Min(defender.Hand.Count, 6);
            TurnAttackMode();
        }


        public void Pass()
        {
            if (firstPasser == null)
                firstPasser = ActivePlayer;
            NextMover();
            ActivateMover();
            ShowState();
        }
        private void CheckWinner()
        {
            int countInGame = Players.Count(p => p.IsInGame);
            if (countInGame == 1)
            {
                IsGameOver = true;
                ResultInfo = $"{Players.FirstOrDefault(p => p.IsInGame).Name} loose! Game over!";
            }

            if (countInGame == 0)
            {
                IsGameOver = true;
                ResultInfo = $"There is draw";
            }
            ShowState();
        }

        private void CardDraw()
        {
            CardDraw(attacker);
            var player = NextPlayer(attacker, p => p == defender);
            while (player != null && player != attacker)
            {
                CardDraw(player);
                player = NextPlayer(player, p => p == defender);
            }
            CardDraw(defender);
            foreach (var p in Players)
            {
                if (p.Hand.Count == 0)
                {
                    p.IsInGame = false;
                }
            }
        }   
        private void CardDraw(Player player)
        {
            int count = player.Hand.Count;
            if (count < 6) player.Hand.Add(Deck.Deal(6 - count));
            player.Hand.Sort();
        }
        public Player NextPlayer(Player player)
        {
            var applicant = Players[Players.Count - 1] == player ? Players[0] : Players[Players.IndexOf(player) + 1];
            if (applicant.IsInGame) return applicant;
            return NextPlayer(applicant);
        }

        public Player NextPlayer(Player player, Predicate<Player> except, Player stopPlayer = null)
        {
            Player applicant = NextPlayer(player);
            if (stopPlayer == applicant) return null;
            if (except(applicant)) return NextPlayer(applicant, except, player);
            return applicant;
        }
        private void NextMover()
        {
            Player applicant = NextPlayer(mover, p => p == defender || p.Hand.Count == 0);
            if(applicant == null || applicant == firstPasser)
            {
                if (mode == Mode.PickingUp)
                    PickUp();
                else
                    Beat();
                return;
            }
            mover = applicant;
        }
        public Player PreviousPlayer(Player player)
        {
            return Players[0] == player ? Players[Players.Count - 1] : Players[Players.IndexOf(player) - 1];
        }
        public bool IsBeat(Card attacking, Card defending)
        {
            if (attacking.Suite == defending.Suite) return attacking.Figure > defending.Figure;
            if (attacking.Suite == Trump.Suite) return true;
            return false;
        }
    }
}
