using System;
using System.Collections.Generic;
using System.Linq;

namespace CramTool.Models.Quizzes
{
    public class GeneralQuizSettings : IQuizSettings
    {
        public List<QuizWord> GetWords(WordList wordList)
        {
            List<WordInfo> wordsToLearn = wordList.GetAllWords().Where(w => w.IsStudied && !w.IsLearned).OrderBy(w => w.Word.Name).ToList();

            DateTime cutOffDate = DateTime.UtcNow - WordInfo.TimeToMarkVerified;
            List<WordInfo> wordsToVerify = wordList.GetAllWords().Where(w => w.IsLearned && !w.IsVerified && w.LastEvent.LastStateChange <= cutOffDate).OrderBy(w => w.Word.Name).ToList();

            List<WordInfo> words = wordsToLearn.Concat(wordsToVerify).ToList();
            
            return words.Select(w => new QuizWord(w)).ToList();
        }
    }
}