using System;
using System.Collections.Generic;

namespace Uno
{
    public class Player
    {
        public Player(int number)
        {
            Hand = new List<Card>();
            Id = Guid.NewGuid();
            Number = number;
        }

        public Guid Id { get; }
        public int Number { get; set; }
        public List<Card> Hand { get; set; }
    }

    public class PlayerRotation
    {
        public LinkedList<Player> Players { get; set; }
    }
}