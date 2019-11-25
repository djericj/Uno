using System;
using System.Collections.Generic;
using System.Text;

namespace Uno
{
    public class TurnLog
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
        public static string Print(this TurnLog log)
        {
            if (log.Round < 0)
            {
                return $@"{log.Stage}: {log.Action}";
            }
            else
            {
                return log != null ? $@"{log.Stage}: {log.Player?.Name}, Round {log.Round}, FC: {CardExtensions.FormatCard(log.FaceCard)}, {log.Action}, Hand: {CardExtensions.FormatHand(log.Player?.Hand)}" : "";
            }
        }
    }
}