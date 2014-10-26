using System;
using System.Collections.Generic;
using System.Linq;

namespace CramTool.Models.Quizzes
{
    public class GeneralQuizSettings : IQuizSettings
    {
        public List<WordInfo> GetWords(WordList wordList)
        {
            List<WordInfo> wordsToLearn = wordList.GetAllWords().Where(w => w.IsAdded && !w.IsLearned).OrderBy(w => w.Word.Name).ToList();

            DateTime cutOffDate = DateTime.UtcNow - WordInfo.TimeToVerify;
            List<WordInfo> wordsToVerify = wordList.GetAllWords().Where(w => w.IsLearned && !w.IsVerified && w.RememberedSince <= cutOffDate).OrderBy(w => w.Word.Name).ToList();

            List<WordInfo> words = wordsToLearn.Concat(wordsToVerify).ToList();
            
            return words;
        }
    }
}