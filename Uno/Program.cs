using Mono.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Uno
{
    internal class Program
    {
        private string[] specialCards = { "Wild", "Wild Draw Four", "Draw Twp", "Skip", "Reverse" };

        private const int totalCards = 108;

        public static int NumberOfHumanPlayers { get; set; }
        public static int NumberOfComputerPlayers { get; set; }
        public static int TotalPlayers { get; set; }
        public static int NumberOfGames { get; set; }
        public static string OutputPath { get; set; }
        public static bool DoubleDeck { get; set; }
        public static bool ShowHelpFlag { get; set; }

        public static List<Player> HumanPlayers { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            //var p = new OptionSet()
            //{
            //    {"p|players=", "Number of human players (1-10)", (int v) =>  NumberOfHumanPlayers = v},
            //    {"c|computer-players=", "Number of computer players (1-10)", (int v) =>  NumberOfComputerPlayers = v},
            //    {"g|games=", "Number of games (default is one.)", (int v) =>  NumberOfGames = v},
            //    {"w|write=[path]", "Write text output files for each game to [path]", (string v) => OutputPath = v},
            //    {"dd|double-deck", "Play game with 2 decks (216 cards) of 108 cards", (bool v) => DoubleDeck = v},
            //    { "h|help",  "show this message and exit", (bool? v) => ShowHelpFlag = v != null },
            //};

            //List<string> extra;
            //try
            //{
            //    extra = p.Parse(args);
            //}
            //catch (OptionException e)
            //{
            //    Console.Write("greet: ");
            //    Console.WriteLine(e.Message);
            //    Console.WriteLine("Try `greet --help' for more information.");
            //    return;
            //}

            //if (ShowHelpFlag)
            //{
            //    ShowHelp(p);
            //    return;
            //}

            if (NumberOfComputerPlayers == 0)
            {
                Random r = new Random();
                TotalPlayers = r.Next(3, 10) - NumberOfHumanPlayers;
            }

            if (NumberOfComputerPlayers > 0)
            {
                TotalPlayers = NumberOfComputerPlayers + NumberOfHumanPlayers;
            }

            if (TotalPlayers > 10)
            {
                Console.WriteLine("Total players cannot be greater than 10.");
                return;
            }

            //if (NumberOfGames == 0) NumberOfGames = 1;

            for (var i = 1; i <= 1; i++)
                StartNewGame();
        }

        private static void StartNewGame()
        {
            var players = new List<Player>();
            for (var i = 1; i <= TotalPlayers; i++)
                players.Add(CreatePlayer(i));

            var game = new Game(players);
            game.DoubleDeck = DoubleDeck;

            game.GameFinished += Game_GameFinished;
            game.GameTurnCompleted += Game_GameTurnCompleted;
            game.NewGame();
        }

        private static void Game_GameFinished(Game game)
        {
            game.GameFinished -= Game_GameFinished;
            SaveLogToFile(game);
        }

        private static void Game_GameTurnCompleted(GameTurn gameTurn)
        {
            Console.WriteLine(gameTurn.Print());
        }

        private static void SaveLogToFile(Game game)
        {
            var path = @"C:\Users\djeri\Downloads\Uno\";
            var sb = new StringBuilder();
            foreach (var line in game.GameLog)
                sb.Append(line.Print());

            File.WriteAllText(path + game.GameId + ".txt", sb.ToString());
        }

        private static Player CreatePlayer(int index)
        {
            var player = new Player(index) { Name = "AI Player " + index };
            Console.WriteLine($"AI Player {player.Number} {player.Id}");
            return player;
        }

        private static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: Uno [OPTIONS]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}