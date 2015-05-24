using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CramTool.Models.Quizzes
{
    public class QuizWord : INotifyPropertyChanged
    {
        private WordInfo wordInfo;
        private TranslationInfo translationInfo;

        private readonly string title;

        private bool isShown;
        private WordEventType? result;
        private WordState state;

        public QuizWord(WordInfo wordInfo)
        {
            this.wordInfo = wordInfo;
            this.title = wordInfo.Word.Name;
            UpdateState();
        }

        public QuizWord(TranslationInfo translationInfo)
        {
            this.translationInfo = translationInfo;
            this.title = translationInfo.Translation;
            UpdateState();
        }

        public WordInfo WordInfo
        {
            get { return wordInfo; }
            set
            {
                wordInfo = value;
                OnPropertyChanged();
            }
        }

        public TranslationInfo TranslationInfo
        {
            get { return translationInfo; }
            set
            {
                translationInfo = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get { return title; }
        }

        public WordEventType? Result
        {
            get { return result; }
            set
            {
                result = value;
                OnPropertyChanged();
            }
        }

        public WordState State
        {
            get { return state; }
            set
            {
                state = value;
                OnPropertyChanged();
            }
        }

        public bool IsShown
        {
            get { return isShown; }
            set
            {
                isShown = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateState()
        {
            if (wordInfo != null)
            {
                WordInfo updatedInfo = wordInfo.WordList.GetWord(wordInfo.Word.Name);
                if (updatedInfo != null)
                {
                    wordInfo = updatedInfo;
                    State = wordInfo.State;
                }
                else
                {
                    State = WordState.Unknown;
                }
            }
            if (translationInfo != null)
            {
                TranslationInfo updatedInfo = translationInfo.WordList.GetTranslation(translationInfo.Translation);
                if (updatedInfo != null)
                {
                    TranslationInfo = updatedInfo;
                    State = translationInfo.State;
                }
                else
                {
                    State = WordState.Unknown;
                }
            }
        }

        public void Mark(WordList wordList, WordEventType eventType)
        {
            if (WordInfo != null)
            {
                wordList.Mark(WordInfo.Word.Name, eventType);
            }
            if (TranslationInfo != null)
            {
                wordList.MarkTranslation(TranslationInfo.Translation, eventType);
            }
            Result = eventType;
            UpdateState();
        }
    }
}