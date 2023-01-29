using System;

namespace MultiTF3_Strategy1
{
    internal class CrossEventArgs : EventArgs
    {
        public enum EventCrosArg
        {
            Gold_fast,
            Dead_fast,
            Gold_midt,
            Dead_mid,
            Gold_slow,
            Dead_slow,
            Unkown
        }

        public EventCrosArg Args { get; set; }

        public CrossEventArgs( EventCrosArg arg )
        {
            this.Args = arg;
        }
    }
}
