using System;

namespace CramTool.Models
{
    public class WordEventInfo
    {
        private readonly WordEvent wordEvent;
        private readonly WordState wordState;
        private readonly DateTime lastStateChange;

        public WordEventInfo(WordEvent wordEvent, WordState wordState, DateTime lastStateChange)
        {
            this.wordEvent = wordEvent;
            this.wordState = wordState;
            this.lastStateChange = lastStateChange;
        }

        public WordEvent WordEvent
        {
            get { return wordEvent; }
        }

        public WordState WordState
        {
            get { return wordState; }
        }

        public DateTime LastStateChange
        {
            get { return lastStateChange; }
        }
    }
}