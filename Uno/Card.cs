using System;
using System.Collections.Generic;
using System.Text;

namespace Uno
{
    public interface ICard
    {
        string Color { get; }

        CardStatus Status { get; set; }
    }

    public class Card : ICard
    {
        public string Color { get; set; }

        public int Number { get; set; }

        public CardStatus Status { get; set; }

        public bool IsSkipCard { get; set; }

        public bool IsReverseCard { get; set; }

        public bool IsDrawTwoCard { get; set; }

        public bool IsWildCard { get; set; }

        public bool IsWildDrawFourCard { get; set; }
    }

    public static class CardExtensions
    {
        public static void DisplayCard(this Card card)
        {
            if (card.Number > 0)
            {
                SetColor(card.Color);
                Console.Write($"{card.Color} {card.Number}");
                Console.ResetColor();
            }
            else if (card.IsDrawTwoCard)
            {
                SetColor(card.Color);
                Console.Write($"{card.Color} Draw Two");
                Console.ResetColor();
            }
            else if (card.IsReverseCard)
            {
                SetColor(card.Color);
                Console.Write($"{card.Color} Reverse");
                Console.ResetColor();
            }
            else if (card.IsSkipCard)
            {
                SetColor(card.Color);
                Console.Write($"{card.Color} Skip");
                Console.ResetColor();
            }
            else if (card.IsWildCard)
            {
                SetColor("Black");
                Console.Write($"Wild Card");
                Console.ResetColor();
            }
            else if (card.IsWildDrawFourCard)
            {
                SetColor("Black");
                Console.Write($"Wild Card Draw Four");
                Console.ResetColor();
            }
        }

        private static void SetColor(string color)
        {
            Console.ForegroundColor = ConsoleColor.White;
            if (color == "Red")
            {
                Console.BackgroundColor = ConsoleColor.Red;
            }
            else if (color == "Yellow")
            {
                Console.BackgroundColor = ConsoleColor.DarkYellow;
            }
            else if (color == "Blue")
            {
                Console.BackgroundColor = ConsoleColor.Blue;
            }
            else if (color == "Green")
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
            }
            else if (color == "Black")
            {
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }
    }

    public enum CardStatus
    {
        Unknown = -1,
        Available = 1,
        InHand = 2,
        Discarded = 3
    }
}