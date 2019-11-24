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
        }

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

        public List<Card> Shuffle(List<Card> cards)
        {
            Console.WriteLine("Shuffling deck...");
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
            StartTime = DateTime.Now;
            Console.WriteLine("Game start");
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
                    Console.WriteLine($"Dealt card #{i} to Player {player.Number}");
                    player.Hand.Add(Dequeue());
                }
            }
        }

        public void End(Player winner, int round)
        {
            EndTime = DateTime.Now;
            TimeSpan ts = EndTime.Subtract(StartTime);
            Console.WriteLine("");
            Console.WriteLine("******************************");
            Console.WriteLine("Game end");
            Console.WriteLine("******************************");
            Console.WriteLine("");
            Console.WriteLine("Results:");
            Console.WriteLine($"Winner is Player {winner.Number}");
            foreach (var player in PlayerRotation.Players)
            {
                Console.WriteLine($"Player {player.Number} ended with {player.Hand.Count()} cards remaining");
            }

            Console.WriteLine($"Total Rounds: {round}");
            Console.WriteLine($"Game took {ts} seconds");
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
            Console.WriteLine("");
            Console.WriteLine("************************************");
            Console.WriteLine($"Starting Round {round}");
            Console.WriteLine("************************************");
            Console.WriteLine("");

            foreach (var player in PlayerRotation.Players)
            {
                if (Ended) continue;
                Player p = player;
                if (!SkipNextPlayer)
                {
                    if (DrawTwoCards)
                    {
                        p = DrawCard(player, 2);
                        DrawTwoCards = false;
                    }
                    else if (DrawFourCards)
                    {
                        p = DrawCard(player, 4);
                        DrawFourCards = false;
                    }
                    else
                    {
                        p = TakeTurn(player);
                    }
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine($"Player {p.Number} skipped.");
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

        private Player TakeTurn(Player player)
        {
            Console.WriteLine("");
            Console.WriteLine($"Player {player.Number} turn:");

            var card = PlayCard(player);

            Console.WriteLine($"Player {player.Number} has {player.Hand.Count()} cards remaining.");
            if (player.Hand.Count == 1)
                Console.WriteLine($"Player {player.Number} has UNO!!!!!!!!".Reversed());

            if (card == null)
            {
                Console.WriteLine($"Player {player.Number} has NO matches.  Draw 1 card.");
                player = DrawCard(player, 1);
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
                    player = DrawCard(player, 4);
                }
            }
            return player;
        }

        private Player DrawCard(Player player, int cards)
        {
            for (var i = 1; i <= cards; i++)
            {
                player.Hand.Add(Dequeue());
            }
            Console.WriteLine($"Player {player.Number} draws {cards} cards.");
            Console.WriteLine($"Player {player.Number} has {player.Hand.Count()} cards remaining.");
            return player;
        }

        private Card Dequeue()
        {
            if (GameDeck.Count == 0)
            {
                ReloadFromDiscard();
            }
            return GameDeck.Dequeue();
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
            Console.WriteLine($"FaceCard is {FaceCard.DisplayCard()}");
            Discard.Enqueue(card);
        }

        private Card MatchOnColor(Player player)
        {
            var colorCard = player.Hand.Where(x => x.Color == FaceCard.Color).FirstOrDefault();
            if (colorCard != null)
            {
                Console.WriteLine($"Player {player.Number} matched COLOR {colorCard.DisplayCard()} with FaceCard {FaceCard.DisplayCard()}");
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
                Console.WriteLine($"Player {player.Number} matched NUMBER {numberCard.DisplayCard()} with FaceCard {FaceCard.DisplayCard()} ");
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
            GameDeck = new Queue<Card>(Shuffle(Deck.BuildDeck()));
            Start();
        }
    }
}