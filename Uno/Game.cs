﻿using System;
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
            Players = players;
        }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public static List<Player> Players { get; set; }

        public static Queue<Card> GameDeck { get; set; }

        public static Queue<Card> Discard { get; set; }

        public static Card FaceCard { get; set; }

        public bool GoForward { get; set; }

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
            Reset();
            GoForward = true;
            Discard = new Queue<Card>();
            Deal();
            SetFaceCard(GameDeck.Dequeue());
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

        public void End(Player winner)
        {
            EndTime = DateTime.Now;
            TimeSpan ts = EndTime.Subtract(StartTime);
            Console.WriteLine("Game end");
            foreach (var player in Players)
            {
                Console.WriteLine($"Player {player.Number} had {player.Hand.Count()} cards remaining");
            }

            Console.WriteLine($"Winner is Player {winner.Number}");
            Console.WriteLine($"Game took {ts} seconds");
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

            var nextPlayer = Players.Where(x => x.Number == next).FirstOrDefault();
            qreturn nextPlayer;
        }

        public void ExecuteRound(Player player, int round)
        {
            var index = 0;
            Console.WriteLine("");
            Console.WriteLine("************************************");
            Console.WriteLine($"Starting Round {round}");
            Console.WriteLine("************************************");
            Console.WriteLine("");
            if (player.Hand.Count == 0)
            {
                End(player);
            }
            else
            {
                index++;
                player = TakeTurn(player);

                if (player.Hand.Count() == 0)
                {
                    End(player);
                }
                else
                {
                    while (player.Number > 1)
                    {
                        index++;
                        player = TakeTurn(player);

                        if (player.Hand.Count() == 0)
                        {
                            End(player);
                            break;
                        }
                    }
                    if (index == 3)
                    {
                        Console.WriteLine($"Round {Round} End");
                        index = 0;
                        round++;
                    }
                    ExecuteRound(player, round);
                }
            }
        }

        private Player TakeTurn(Player player)
        {
            Console.WriteLine("");
            Console.WriteLine($"Player {player.Number} turn:");
            var card = PlayCard(player);
            Console.WriteLine($"Player {player.Number} has {player.Hand.Count()} cards remaining.");
            if (player.Hand.Count == 1)
                Console.WriteLine($"Player {player.Number} has UNO!!!!!!!!".Reversed());
            Player nextPlayer = null;
            if (card == null)
            {
                Console.WriteLine($"Player {player.Number} has NO matches.  Draw 1 card.");
                player = DrawCard(player, 1);
                nextPlayer = GetNextPlayer(player.Number);
            }
            else
            {
                if (card.IsReverseCard)
                {
                    GoForward = !GoForward;
                    nextPlayer = GetNextPlayer(player.Number);
                }
                else if (card.IsSkipCard)
                {
                    nextPlayer = GetNextPlayer(player.Number, true);
                }
                else if (card.IsDrawTwoCard)
                {
                    player = GetNextPlayer(player.Number);

                    Console.WriteLine("");
                    Console.WriteLine($"Player {player.Number} turn:");
                    player = DrawCard(player, 2);
                    Console.WriteLine($"Player {player.Number} has {player.Hand.Count()} cards remaining.");
                    player = GetNextPlayer(player.Number);
                    nextPlayer = player;
                }
                else if (card.IsWildCard)
                {
                    nextPlayer = GetNextPlayer(player.Number);
                }
                else if (card.IsWildDrawFourCard)
                {
                    player = GetNextPlayer(player.Number);
                    player = DrawCard(player, 4);
                    nextPlayer = player;
                }
                else
                {
                    nextPlayer = GetNextPlayer(player.Number);
                }
            }
            return nextPlayer;
        }

        private Player DrawCard(Player player, int cards)
        {
            for (var i = 1; i <= cards; i++)
            {
                player.Hand.Add(GameDeck.Dequeue());
            }
            Console.WriteLine($"Player {player.Number} draws {cards} cards.");
            Console.WriteLine($"Player {player.Number} has {player.Hand.Count()} cards remaining.");
            return player;
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
            var numberCard = player.Hand.Where(x => x.Number == FaceCard.Number).FirstOrDefault();
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
            if (card != null)
            {
                SetFaceCard(card);
                player.Hand.Remove(card);
            }

            return card;
        }

        public GameResult NewGame()
        {
            Game game = new Game(Players);
            GameDeck = new Queue<Card>(Shuffle(Deck.BuildDeck()));
            game.Start();

            return new GameResult();
        }
    }
}