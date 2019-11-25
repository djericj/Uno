using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Crayon;

namespace Uno
{
    public class Game
    {
        public Game(List<Player> players)
        {
            PlayerRotation = new PlayerRotation();
            PlayerRotation.Players = new LinkedList<Player>(players);
            GameLog = new List<TurnLog>();
        }

        public Guid GameId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public static PlayerRotation PlayerRotation { get; set; }

        public static Queue<Card> GameDeck { get; set; }

        public static Queue<Card> Discard { get; set; }

        public static Card FaceCard { get; set; }

        public bool GoForward { get; set; }

        public bool SkipNextPlayer { get; set; }

        public bool DrawTwoCards { get; set; }

        public bool DrawFourCards { get; set; }

        public bool Ended { get; set; }

        public static int Round { get; set; }

        public List<TurnLog> GameLog { get; set; }

        public List<Card> Shuffle(List<Card> cards)
        {
            Log("SETUP", "Shuffling deck...");
            Random rng = new Random();
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }

            return cards;
        }

        public void Start()
        {
            GameLog = new List<TurnLog>();
            StartTime = DateTime.Now;
            Log("START", $"Game start, {PlayerRotation.Players.Count()} players, ID: " + GameId.ToString());
            GoForward = true;
            Discard = new Queue<Card>();
            Deal();
            SetFaceCard(Dequeue());
            //Round = 1;
            while (!Ended)
                ExecuteRound(1);
        }

        public void Deal()
        {
            for (var i = 1; i <= 7; i++)
            {
                foreach (var player in PlayerRotation.Players)
                {
                    Log("SETUP", $"Dealt card #{i} to Player {player.Name}");
                    player.Hand.Add(Dequeue());
                }
            }
        }

        public void End(Player winner, int round)
        {
            EndTime = DateTime.Now;
            TimeSpan ts = EndTime.Subtract(StartTime);
            Log("END", "Game end");
            Log("END", $"{PlayerRotation?.Players?.Count()} players");
            Log("END", "Results");
            Log("END", $"Winner is Player {winner?.Number}");
            foreach (var player in PlayerRotation?.Players)
            {
                Log("END", $"Player {player.Name} ended with {player.Hand.Count()} cards remaining");
            }

            Log("END", $"Total Rounds: {round}");
            Log("END", $"Game time {ts} seconds");

            Log("END", $"Game ID {GameId.ToString()}");
            PlayerRotation = null;
            return;
        }

        private PlayerRotation Reverse(PlayerRotation playerRotation)
        {
            var r = new PlayerRotation();
            r.Players = new LinkedList<Player>();
            for (var i = playerRotation.Players.Count() - 1; i >= 0; i--)
            {
                r.Players.AddLast(playerRotation.Players.ElementAt(i));
            }
            return r;
        }

        public void ExecuteRound(int round)
        {
            if (Ended) return;
            Log("INFO", $"*** Starting Round {round} ***");

            foreach (var player in PlayerRotation.Players)
            {
                if (Ended) continue;
                Player p = player;
                if (!SkipNextPlayer)
                {
                    if (DrawTwoCards)
                    {
                        p = DrawCard(player, 2, round);
                        DrawTwoCards = false;
                    }
                    else if (DrawFourCards)
                    {
                        p = DrawCard(player, 4, round);
                        DrawFourCards = false;
                    }
                    else
                    {
                        p = TakeTurn(player, round);
                    }
                }
                else
                {
                    Log("TURN", player, round, "Player skipped");
                    SkipNextPlayer = false;
                }
                if (HasWon(p))
                {
                    End(p, round);
                    Ended = true;
                    break;
                }
                else if (p == PlayerRotation.Players.Last())
                {
                    round++;
                }
            }
            ExecuteRound(round);
        }

        private bool HasWon(Player player)
        {
            return player.Hand.Count() == 0;
        }

        private Player TakeTurn(Player player, int round)
        {
            Log("TURN", player, round, $"Player {player.Number} turn:");

            var card = PlayCard(player);

            Log("TURN", player, round, $"Player {player.Number} has {player.CardsRemaining} cards remaining.");
            if (player.Hand.Count == 1)
            {
                Log("TURN", player, round, $"Player {player.Number} has UNO!!!!!!!!");
            }

            if (card == null)
            {
                Log("TURN", player, round, $"Player {player.Number} has NO matches.  Draw 1 card.");
                player = DrawCard(player, 1, round);
            }
            else
            {
                if (card.IsReverseCard)
                {
                    PlayerRotation = Reverse(PlayerRotation);
                }
                else if (card.IsSkipCard)
                {
                    SkipNextPlayer = true;
                }
                else if (card.IsDrawTwoCard)
                {
                    DrawTwoCards = true;
                }
                else if (card.IsWildCard)
                {
                }
                else if (card.IsWildDrawFourCard)
                {
                    player = DrawCard(player, 4, round);
                }
            }
            return player;
        }

        private Player DrawCard(Player player, int cards, int round)
        {
            for (var i = 1; i <= cards; i++)
            {
                if (!Ended)
                    player.Hand.Add(Dequeue());
            }
            Log("TURN", player, round, $"Player {player.Number} draws {cards} cards.");
            Log("TURN", player, round, $"Player {player.Number} has {player.CardsRemaining} cards remaining.");
            return player;
        }

        private Card Dequeue()
        {
            if (GameDeck.Count == 0)
            {
                ReloadFromDiscard();
            }
            if (GameDeck.Count == 0)
            {
                Ended = true;
                Log("END", $"Game deck ran out of cards.");
                return null;
            }
            else
            {
                return GameDeck.Dequeue();
            }
        }

        private void ReloadFromDiscard()
        {
            Object obj = new Object();
            lock (obj)
            {
                Card[] discardCards = new Card[Discard.Count()];
                Discard.CopyTo(discardCards, 0);
                foreach (var card in discardCards)
                {
                    GameDeck.Enqueue(card);
                }
                Discard.Clear();
            }
        }

        private void SetFaceCard(Card card)
        {
            FaceCard = card;
            Log("INFO", $"FaceCard is {FaceCard.DisplayCard()}");
            Discard.Enqueue(card);
        }

        private Card MatchOnColor(Player player)
        {
            var colorCard = player.Hand.Where(x => x.Color == FaceCard.Color).FirstOrDefault();
            if (colorCard != null)
            {
                Log("TURN", $"Player {player.Number} matched COLOR {colorCard.DisplayCard()} with FaceCard {FaceCard.DisplayCard()}");
                return colorCard;
            }
            else
            {
                return null;
            }
        }

        private Card MatchOnNumber(Player player)
        {
            var numberCard = player.Hand.Where(x => !x.IsSpecialCard && x.Number == FaceCard.Number).FirstOrDefault();
            if (numberCard != null)
            {
                Log("TURN", $"Player {player.Number} matched NUMBER {numberCard.DisplayCard()} with FaceCard {FaceCard.DisplayCard()} ");
                return numberCard;
            }
            return numberCard;
        }

        public Card PlaySpecialCard(Player player)
        {
            if (HasSkipCard(player))
            {
                var card = player.Hand.Where(x => x.IsSkipCard).FirstOrDefault();
                if (card.Color == FaceCard.Color) return card;
            }
            else if (HasDrawTwoCard(player))
            {
                var card = player.Hand.Where(x => x.IsDrawTwoCard).FirstOrDefault();
                if (card.Color == FaceCard.Color) return card;
            }
            else if (HasReverseCard(player))
            {
                var card = player.Hand.Where(x => x.IsReverseCard).FirstOrDefault();
                if (card.Color == FaceCard.Color) return card;
            }
            else if (HasWildCard(player))
            {
                var card = player.Hand.Where(x => x.IsWildCard).FirstOrDefault();
                card.Color = GetRandomColor();
                return card;
            }
            else if (HasWildDrawFourCard(player))
            {
                var card = player.Hand.Where(x => x.IsWildDrawFourCard).FirstOrDefault();
                card.Color = GetRandomColor();
                return card;
            }
            return null;
        }

        private string GetRandomColor()
        {
            string[] colors = { "Blue", "Green", "Yellow", "Red" };
            Random r = new Random();
            return colors[r.Next(0, 3)];
        }

        public bool HasSkipCard(Player player) => player.Hand.Where(x => x.IsSkipCard).Count() > 0;

        public bool HasReverseCard(Player player) => player.Hand.Where(x => x.IsReverseCard).Count() > 0;

        public bool HasDrawTwoCard(Player player) => player.Hand.Where(x => x.IsDrawTwoCard).Count() > 0;

        public bool HasWildCard(Player player) => player.Hand.Where(x => x.IsWildCard).Count() > 0;

        public bool HasWildDrawFourCard(Player player) => player.Hand.Where(x => x.IsWildDrawFourCard).Count() > 0;

        public Card PlayCard(Player player)
        {
            var card = MatchOnColor(player);
            if (card == null)
            {
                card = MatchOnNumber(player);
            }
            if (card == null)
            {
                card = PlaySpecialCard(player);
            }
            if (card != null)
            {
                SetFaceCard(card);
                player.Hand.Remove(card);
            }

            return card;
        }

        public void NewGame()
        {
            GameId = Guid.NewGuid();
            GameDeck = new Queue<Card>(Shuffle(Deck.BuildDeck()));
            Start();
        }

        private void Log(string stage, string action)
        {
            var log = new TurnLog()
            {
                Stage = stage,
                Player = null,
                Round = -1,
                Action = action,
                CardsRemaining = -1,
                Hand = null,
                HasUno = false,
                IsWinner = false,
                FaceCard = null
            };
            GameLog.Add(log);
            Console.WriteLine(log.Print());
        }

        private void Log(string stage, Player player, int round, string action)
        {
            var log = new TurnLog()
            {
                Stage = stage,
                Player = player,
                Round = round,
                Action = action,
                CardsRemaining = player.CardsRemaining,
                Hand = player.Hand,
                HasUno = player.Hand != null ? player.Hand.Count() == 1 : false,
                FaceCard = FaceCard
            };
            GameLog.Add(log);
            Console.WriteLine(log.Print());
        }
    }
}