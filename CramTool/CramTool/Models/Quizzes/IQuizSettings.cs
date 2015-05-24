using System.Collections.Generic;

namespace CramTool.Models.Quizzes
{
    public interface IQuizSettings
    {
        List<QuizWord> GetWords(WordList wordList);
    }
}