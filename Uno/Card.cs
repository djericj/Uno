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

        public bool IsSpecialCard { get; set; }

        public bool IsSkipCard { get; set; }

        public bool IsReverseCard { get; set; }

        public bool IsDrawTwoCard { get; set; }

        public bool IsWildCard { get; set; }

        public bool IsWildDrawFourCard { get; set; }

        public string Print()
        {
            return "";
        }
    }

    public static class CardExtensions
    {
        public static string SetColor(Card card)
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

        public static string FormatHand(List<Card> cards)
        {
            if (cards == null) return "";
            var sb = new StringBuilder();
            foreach (var card in cards)
            {
                sb.Append(FormatCard(card) + ";");
            }
            return sb.ToString();
        }

        public static string FormatCard(Card card)
        {
            if (card.IsDrawTwoCard)
            {
                return "D" + card.Color.Substring(0, 1);
            }
            else if (card.IsReverseCard)
            {
                return "R" + card.Color.Substring(0, 1);
            }
            else if (card.IsSkipCard)
            {
                return "S" + card.Color.Substring(0, 1);
            }
            else if (card.IsWildCard)
            {
                return "W" + card.Color.Substring(0, 1);
            }
            else if (card.IsWildDrawFourCard)
            {
                return "X" + card.Color.Substring(0, 1);
            }
            else
            {
                return card.Number + card.Color.Substring(0, 1);
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