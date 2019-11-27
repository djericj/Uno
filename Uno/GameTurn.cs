using System;
using System.Collections.Generic;
using System.Text;

namespace Uno
{
    public class GameTurn
    {
        // SETUP
        // DEAL
        // PLAYER
        // END
        public string Stage { get; set; }

        public Player Player { get; set; }
        public int Round { get; set; }
        public string Action { get; set; }
        public int CardsRemaining { get; set; }
        public List<Card> Hand { get; set; }
        public DateTime Timestamp { get; set; }
        public bool HasUno { get; set; }
        public bool IsWinner { get; set; }
        public Card FaceCard { get; set; }
        public string DrawPileCount { get; set; }
        public List<Card> DrawPileCards { get; set; }
        public string DiscardPileCount { get; set; }
        public List<Card> DiscardPileCards { get; set; }
    }

    public static class TurnLogExtensions
    {
        public static string Print(this GameTurn turn)
        {
            if (turn.Round < 0)
            {
                return $@"{turn.Stage}: {turn.Action}";
            }
            else
            {
                return turn != null ? $@"{turn.Stage}: {turn.Player?.Name}, Round {turn.Round}, FC: {CardExtensions.FormatCard(turn.FaceCard)}, {turn.Action}, Hand: {CardExtensions.FormatHand(turn.Player?.Hand)}" : "";
            }
        }
    }
}