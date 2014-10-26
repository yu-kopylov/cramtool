using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CramTool.Models.Quizzes
{
    public class QuizWord : INotifyPropertyChanged
    {
        private WordInfo wordInfo;
        private bool isShown;
        private WordEventType? result;

        public QuizWord(WordInfo wordInfo)
        {
            this.wordInfo = wordInfo;
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

        public WordEventType? Result
        {
            get { return result; }
            set
            {
                result = value;
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
    }
}