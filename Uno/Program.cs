using System;
using System.Collections.Generic;

namespace Uno
{
    internal class Program
    {
        private string[] specialCards = { "Wild", "Wild Draw Four", "Draw Twp", "Skip", "Reverse" };

        private const int totalCards = 108;

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            //Usage();

            var players = new List<Player>();
            players.Add(CreatePlayer(1));
            players.Add(CreatePlayer(2));
            players.Add(CreatePlayer(3));

            new Game(players).NewGame();
        }

        private static Player CreatePlayer(int index)
        {
            var player = new Player(index);
            Console.WriteLine($"Player {player.Number} {player.Id}");
            return player;
        }

        private static void Usage()
        {
            var str = @"
                Usage: Uno [options]\n
                \n
                Options: \n
                \t -p|--players \t Number of players (1-10) \n
                \t -g|--games \t Number of games (default is one.) \n
                \t -wo|--write-output \t Write JSON Output files for each game to [path]
                ";
            Console.WriteLine(str);
        }
    }
}