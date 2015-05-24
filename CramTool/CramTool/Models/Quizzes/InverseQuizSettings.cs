using System;
using System.Collections.Generic;
using System.Linq;

namespace CramTool.Models.Quizzes
{
    public class InverseQuizSettings : IQuizSettings
    {
        public List<QuizWord> GetWords(WordList wordList)
        {
            List<TranslationInfo> wordsToLearn = wordList.GetAllTranslations().Where(w => w.IsStudied && !w.IsLearned).OrderBy(tr => tr.Translation).ToList();

            DateTime cutOffDate = DateTime.UtcNow - WordInfo.TimeToMarkVerified;
            List<TranslationInfo> wordsToVerify = wordList.GetAllTranslations().Where(w => w.IsLearned && !w.IsVerified && w.LastStateChange <= cutOffDate).OrderBy(tr => tr.Translation).ToList();

            List<TranslationInfo> words = wordsToLearn.Concat(wordsToVerify).ToList();
            
            return words.Select(w => new QuizWord(w)).ToList();
        }
    }
}