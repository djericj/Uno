using System.Collections.Generic;

namespace Uno
{
    internal static class Deck
    {
        private static string[] colors = { "Blue", "Red", "Yellow", "Green" };
        private static int[] numbers = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        public static List<Card> BuildDeck()
        {
            var cards = new List<Card>();
            for (var i = 1; i <= 2; i++)
            {
                foreach (var color in colors)
                {
                    foreach (var number in numbers)
                    {
                        if (!(i == 2 && number == 0))
                            cards.Add(CreateNumberCard(color, number));
                    }

                    cards.Add(CreateSkipCard(color));

                    cards.Add(CreateReverseCard(color));

                    cards.Add(CreateDrawTwoCard(color));

                    if (i == 1)
                        cards.Add(CreateWildCard());
                    else
                        cards.Add(CreateWildDrawFourCard());
                }
            }
            return cards;
        }

        private static Card CreateNumberCard(string color, int number) => new Card() { Color = color, Number = number };

        private static Card CreateDrawTwoCard(string color) => new Card() { Color = color, Number = -1, IsDrawTwoCard = true, IsSpecialCard = true };

        private static Card CreateReverseCard(string color) => new Card() { Color = color, Number = -1, IsReverseCard = true, IsSpecialCard = true };

        private static Card CreateSkipCard(string color) => new Card() { Color = color, Number = -1, IsSkipCard = true, IsSpecialCard = true };

        private static Card CreateWildCard() => new Card() { Color = "Black", Number = -1, IsWildCard = true, IsSpecialCard = true };

        private static Card CreateWildDrawFourCard() => new Card() { Color = "Black", Number = -1, IsWildDrawFourCard = true, IsSpecialCard = true };
    }
}