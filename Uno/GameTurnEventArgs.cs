using System;

namespace Uno
{
    public class GameTurnEventArgs : EventArgs
    {
        public GameTurn GameTurn { get; set; }
    }
}