using System;
using System.Collections.Generic;

namespace Uno
{
    internal class Program
    {
        private string[] specialCards = { "Wild", "Wild Draw Four", "Draw Twp", "Skip", "Reverse" };

        private const int totalCards = 108;

        private static void Main(string[] args)
        {
            var players = new List<Player>();
            players.Add(CreatePlayer(1));
            players.Add(CreatePlayer(2));
            players.Add(CreatePlayer(3));

            var result = new Game(players).NewGame();
        }

        private static Player CreatePlayer(int index)
        {
            var player = new Player(index);
            Console.WriteLine($"Player {player.Number} {player.Id}");
            return player;
        }
    }
}