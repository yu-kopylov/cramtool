using System.Collections.Generic;

namespace CramTool.Models.Quizzes
{
    public interface IQuizSettings
    {
        List<WordInfo> GetWords(WordList wordList);
    }
}