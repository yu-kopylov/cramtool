using System;

namespace CramTool.Models
{
    public class WordEventInfo
    {
        private readonly Word word;
        private readonly WordEvent wordEvent;
        private readonly WordState wordState;
        private readonly DateTime lastStateChange;

        public WordEventInfo(Word word, WordEvent wordEvent, WordState wordState, DateTime lastStateChange)
        {
            this.word = word;
            this.wordEvent = wordEvent;
            this.wordState = wordState;
            this.lastStateChange = lastStateChange;
        }

        public Word Word
        {
            get { return word; }
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