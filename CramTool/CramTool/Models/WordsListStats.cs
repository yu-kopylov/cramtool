using System.Collections.Generic;
using System.Linq;

namespace CramTool.Models
{
    public class WordsListStats
    {
        public int WordCount { get; private set; }
        public int StudiedWordCount { get; private set; }
        public int LearnedWordCount { get; private set; }
        public int VerifiedWordCount { get; private set; }

        public static WordsListStats BuildFrom(WordList wordList)
        {
            List<WordInfo> words = wordList.GetAllWords().ToList();

            WordsListStats stats = new WordsListStats();
            stats.WordCount = words.Count;
            stats.StudiedWordCount = words.Count(w => w.IsAdded);
            stats.LearnedWordCount = words.Count(w => w.IsLearned);
            stats.VerifiedWordCount = words.Count(w => w.IsVerified);
            return stats;
        }
    }
}