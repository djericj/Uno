using Crayon;
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
        private static string SetColor(Card card)
        {
            if (card.Color == "Red") return "Red".BrightRed().Reversed();
            if (card.Color == "Yellow") return "Yellow".Yellow().Reversed();
            if (card.Color == "Green") return "Green".BrightGreen().Reversed();
            if (card.Color == "Blue") return "Blue".BrightBlue().Reversed();
            return "Black".BrightBlack();
        }

        public static string DisplayCard(this Card card)
        {
            if (card.Number > -1)
            {
                return $"{SetColor(card)} {card.Number}";
            }
            else if (card.IsDrawTwoCard)
            {
                return $"{SetColor(card)} Draw Two";
            }
            else if (card.IsReverseCard)
            {
                return $"{SetColor(card)} Reverse";
            }
            else if (card.IsSkipCard)
            {
                return $"{SetColor(card)} Skip";
            }
            else if (card.IsWildCard)
            {
                return $"{SetColor(card)} Wild Card";
            }
            else if (card.IsWildDrawFourCard)
            {
                return $"{SetColor(card)} Wild Card Draw Four";
            }
            return "";
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