using System;
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

        private int checkedCount;
        private int rememberedCount;
        private int forgottenCount;

        private readonly GeneralQuizSettings generalSettings = new GeneralQuizSettings();
        private readonly InverseQuizSettings inverseSettings = new InverseQuizSettings();

        public WordList WordList
        {
            get { return wordList; }
            set
            {
                WeakEventHelper.UpdateListener<WordList, EventArgs>(wordList, value, "ContentsChanged", OnWordListContentsChanged);
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

        public int CheckedCount
        {
            get { return checkedCount; }
            set
            {
                checkedCount = value;
                OnPropertyChanged();
            }
        }

        public int RememberedCount
        {
            get { return rememberedCount; }
            set
            {
                rememberedCount = value;
                OnPropertyChanged();
            }
        }

        public int ForgottenCount
        {
            get { return forgottenCount; }
            set
            {
                forgottenCount = value;
                OnPropertyChanged();
            }
        }

        public GeneralQuizSettings GeneralSettings
        {
            get { return generalSettings; }
        }

        public InverseQuizSettings InverseSettings
        {
            get { return inverseSettings; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnWordListContentsChanged(object sender, EventArgs e)
        {
            if (words != null)
            {
                foreach (QuizWord word in Words)
                {
                    word.UpdateState();
                }
            }
        }

        public void StartQuiz(IQuizSettings settings)
        {
            List<QuizWord> quizWords = settings.GetWords(wordList);
            Words = new ObservableCollection<QuizWord>(quizWords);
            CurrentWord = quizWords.FirstOrDefault();
            QuizStage = QuizStage.Started;
            UpdateCounts();
        }

        public void ResetQuiz()
        {
            Words = null;
            CurrentWord = null;
            QuizStage = QuizStage.Prepare;
            UpdateCounts();
        }

        public void MarkCurrentWord(WordEventType eventType)
        {
            CurrentWord.Mark(WordList, eventType);
            UpdateCounts();
        }

        private void UpdateCounts()
        {
            if (Words == null)
            {
                CheckedCount = 0;
                RememberedCount = 0;
                ForgottenCount = 0;
                return;
            }

            CheckedCount = Words.Count(w => w.Result != null);
            RememberedCount = Words.Count(w => w.Result == WordEventType.Remembered);
            ForgottenCount = Words.Count(w => w.Result == WordEventType.Forgotten);
        }
    }
}