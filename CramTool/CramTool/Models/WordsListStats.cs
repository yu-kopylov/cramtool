using System.Collections.Generic;
using System.Linq;

namespace CramTool.Models
{
    public class WordsListStats
    {
        public int WordCount { get; private set; }
        public int StudiedWordCount { get; private set; }
        public int RepeatedWordCount { get; private set; }
        public int LearnedWordCount { get; private set; }
        public int VerifiedWordCount { get; private set; }

        public int TranslationCount { get; private set; }
        public int StudiedTranslationCount { get; private set; }
        public int RepeatedTranslationCount { get; private set; }
        public int LearnedTranslationCount { get; private set; }
        public int VerifiedTranslationCount { get; private set; }

        public static WordsListStats BuildFrom(WordList wordList)
        {
            List<WordInfo> words = wordList.GetAllWords().ToList();
            List<TranslationInfo> translations = wordList.GetAllTranslations().ToList();

            WordsListStats stats = new WordsListStats();

            stats.WordCount = words.Count;
            stats.StudiedWordCount = words.Count(w => w.IsStudied);
            stats.RepeatedWordCount = words.Count(w => w.IsRepeated);
            stats.LearnedWordCount = words.Count(w => w.IsLearned);
            stats.VerifiedWordCount = words.Count(w => w.IsVerified);

            stats.TranslationCount = translations.Count;
            stats.StudiedTranslationCount = translations.Count(w => w.IsStudied);
            stats.RepeatedTranslationCount = translations.Count(w => w.IsRepeated);
            stats.LearnedTranslationCount = translations.Count(w => w.IsLearned);
            stats.VerifiedTranslationCount = translations.Count(w => w.IsVerified);
            
            return stats;
        }
    }
}