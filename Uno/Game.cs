using System;
using System.Collections.Generic;
using System.Linq;

namespace Uno
{
    public class Game
    {
        public Game(List<Player> players)
        {
            Players = players;
        }

        public static List<Player> Players { get; set; }

        public static Queue<Card> GameDeck { get; set; }

        public static Queue<Card> Discard { get; set; }

        public static Card FaceCard { get; set; }

        public GameStatus Status { get; set; }

        public bool GoForward { get; set; }

        public static Player Winner { get; set; }

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
            Console.WriteLine("Game start");
            Status = GameStatus.Running;
            GoForward = true;
            Winner = null;
            Deal();
            FaceCard = GameDeck.Dequeue();
            Discard = new Queue<Card>(new List<Card>() { FaceCard });
            Console.Write($"FaceCard is ");
            FaceCard.DisplayCard();
            Console.WriteLine();
            Round = 1;
            ExecuteRound(Players[0], Round);
        }

        public void Deal()
        {
            for (var i = 1; i <= 7; i++)
            {
                foreach (var player in Players)
                {
                    Console.WriteLine($"Dealt card #{i} to Player {player.Number}");
                    player.Hand.Add(GameDeck.Dequeue());
                }
            }
        }

        public void End()
        {
            Status = GameStatus.Stopped;
            Console.WriteLine("Game end");
            Console.WriteLine($"Winner is {Winner.Id}");
            Reset();
        }

        public void Reset()
        {
            Console.WriteLine("Resetting");
            foreach (var player in Players)
                player.Hand.Clear();
        }

        private Player GetNextPlayer(int n, bool skip = false)
        {
            int next = 0;
            int players = Players.Count();
            var increment = 1;
            if (skip) increment = 2;
            if (GoForward)
            {
                if (n < players)
                {
                    next = n + increment;
                }
                if (n == players)
                {
                    next = increment;
                }
                if (next > players) next = next - players;
            }
            else
            {
                if (n < players)
                {
                    next = n - increment;
                }
                if (n == increment)
                {
                    next = players;
                }
                if (next < 1) next = next + players;
            }
            return Players.Where(x => x.Number == next).FirstOrDefault();
        }

        public void ExecuteRound(Player player, int round)
        {
            Console.WriteLine($"Starting Round {round}");
            if (player.Hand.Count == 0)
            {
                Winner = player;
                End();
            }
            else
            {
                var card = PlayCard(player);
                Discard.Enqueue(card);
                if (card.IsReverseCard)
                {
                    GoForward = !GoForward;
                    player = GetNextPlayer(player.Number);
                }
                else if (card.IsSkipCard)
                {
                    player = GetNextPlayer(player.Number, true);
                }
                else if (card.IsDrawTwoCard)
                {
                    player = GetNextPlayer(player.Number);
                    player = DrawCard(player, 2);
                }
                else if (card.IsWildCard)
                {
                    player = GetNextPlayer(player.Number);
                }
                else if (card.IsWildDrawFourCard)
                {
                    player = GetNextPlayer(player.Number);
                    player = DrawCard(player, 4);
                }
                if (player.Number == Players.Count)
                {
                    Console.WriteLine($"Round {Round} End");
                    round++;
                    ExecuteRound(player, round);
                }
            }
        }

        private Player DrawCard(Player player, int cards)
        {
            for (var i = 1; i <= cards; i++)
            {
                player.Hand.Add(GameDeck.Dequeue());
            }
            return player;
        }

        private Card MatchOnColor(Player player)
        {
            var colorCard = player.Hand.Where(x => x.Color == FaceCard.Color).FirstOrDefault();
            if (colorCard != null)
            {
                Discard.Enqueue(colorCard);
                player.Hand.Remove(colorCard);

                Console.Write($"Player {player.Number} matched ");
                colorCard.DisplayCard();
                Console.Write($" with Face Card ");
                FaceCard.DisplayCard();
                Console.WriteLine();
                FaceCard = colorCard;

                Console.WriteLine();
                return colorCard;
            }
            else
            {
                return null;
            }
        }

        private Card MatchOnNumber(Player player)
        {
            var numberCard = player.Hand.Where(x => x.Number > 0).FirstOrDefault();
            if (numberCard != null)
            {
                Discard.Enqueue(numberCard);
                player.Hand.Remove(numberCard);

                Console.Write($"Player {player.Number} matched ");
                numberCard.DisplayCard();
                Console.Write($" with Face Card ");
                FaceCard.DisplayCard();
                Console.WriteLine();
                FaceCard = numberCard;
                Console.WriteLine();
                return numberCard;
            }
            return numberCard;
        }

        public Card PlaySpecialCard(Player player)
        {
            if (HasSkipCard(player))
            {
                return player.Hand.Where(x => x.IsSkipCard).FirstOrDefault();
            }
            if (HasDrawTwoCard(player))
            {
                return player.Hand.Where(x => x.IsDrawTwoCard).FirstOrDefault();
            }
            if (HasReverseCard(player))
            {
                return player.Hand.Where(x => x.IsReverseCard).FirstOrDefault();
            }
            if (HasWildCard(player))
            {
                var card = player.Hand.Where(x => x.IsWildCard).FirstOrDefault();
                card.Color = GetRandomColor();
                return card;
            }
            if (HasWildDrawFourCard(player))
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
            if (card == null)
            {
                DrawCard(player, 1);
            }

            return card;
        }

        public GameResult NewGame()
        {
            Game game = new Game(Players);
            GameDeck = new Queue<Card>(Deck.BuildDeck());
            game.Start();

            return new GameResult();
        }
    }

    public enum GameStatus
    {
        Unknown = -1,
        Running = 1,
        Stopped = 2
    }

    public enum Direction
    {
        Unknown = -1,
        Forward = 1,
        Backward = 2
    }
}