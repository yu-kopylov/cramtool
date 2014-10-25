namespace CramTool.Models
{
    public class WordEventInfo
    {
        private readonly WordEvent wordEvent;

        public WordEventInfo(WordEvent wordEvent)
        {
            this.wordEvent = wordEvent;
        }

        public WordEvent WordEvent
        {
            get { return wordEvent; }
        }
    }
}