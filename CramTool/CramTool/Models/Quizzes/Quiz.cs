using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CramTool.Models.Quizzes
{
    public class Quiz : INotifyPropertyChanged
    {
        private WordList wordList;
        private QuizStage quizStage;
        private ObservableCollection<QuizWord> words;
        private QuizWord currentWord;

        private readonly GeneralQuizSettings generalSettings = new GeneralQuizSettings();

        public WordList WordList
        {
            get { return wordList; }
            set
            {
                wordList = value;
                OnPropertyChanged();
                ResetQuiz();
            }
        }

        public QuizStage QuizStage
        {
            get { return quizStage; }
            private set
            {
                quizStage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<QuizWord> Words
        {
            get { return words; }
            private set
            {
                words = value;
                OnPropertyChanged();
            }
        }

        public QuizWord CurrentWord
        {
            get { return currentWord; }
            set
            {
                currentWord = value;
                OnPropertyChanged();
            }
        }

        public GeneralQuizSettings GeneralSettings
        {
            get { return generalSettings; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void StartQuiz(IQuizSettings settings)
        {
            List<WordInfo> selectedWords = settings.GetWords(wordList);
            List<QuizWord> quizWords = selectedWords.Select(w => new QuizWord(w)).ToList();
            Words = new ObservableCollection<QuizWord>(quizWords);
            CurrentWord = quizWords.FirstOrDefault();
            QuizStage = QuizStage.Started;
        }

        public void ResetQuiz()
        {
            Words = null;
            CurrentWord = null;
            QuizStage = QuizStage.Prepare;
        }

        public void MarkCurrentWord(WordEventType eventType)
        {
            WordList.Mark(CurrentWord.WordInfo.Word.Name, eventType);
            CurrentWord.Result = eventType;
        }
    }
}