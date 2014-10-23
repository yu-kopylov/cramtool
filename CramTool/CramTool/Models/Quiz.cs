using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CramTool.Models
{
    public class Quiz : INotifyPropertyChanged
    {
        private WordList wordList;
        private QuizStage quizStage;
        private ObservableCollection<QuizWord> words;
        private QuizWord currentWord;

        private readonly QuizUnlearnedSettings unlearnedSettings = new QuizUnlearnedSettings();
        private readonly QuizUnverifiedSettings unverifiedSettings = new QuizUnverifiedSettings();
        private readonly QuizUnrepeatedSettings unrepeatedSettings = new QuizUnrepeatedSettings();

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

        public QuizUnlearnedSettings UnlearnedSettings
        {
            get { return unlearnedSettings; }
        }

        public QuizUnverifiedSettings UnverifiedSettings
        {
            get { return unverifiedSettings; }
        }

        public QuizUnrepeatedSettings UnrepeatedSettings
        {
            get { return unrepeatedSettings; }
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

    public enum QuizStage
    {
        Prepare,
        Started
    }

    public interface IQuizSettings
    {
        List<WordInfo> GetWords(WordList wordList);
    }

    public class QuizUnlearnedSettings : IQuizSettings
    {
        public List<WordInfo> GetWords(WordList wordList)
        {
            List<WordInfo> wordsToLearn = wordList.GetAllWords().Where(w => w.IsAdded && !w.IsLearned).ToList();
            return wordsToLearn.OrderBy(w => w.DateAdded ?? DateTime.MaxValue).ThenBy(w => w.Word.Name).ToList();
        }
    }

    public class QuizUnverifiedSettings : IQuizSettings
    {
        public List<WordInfo> GetWords(WordList wordList)
        {
            DateTime cutOffDate = DateTime.UtcNow - WordInfo.TimeToVerify;
            List<WordInfo> wordsToLearn = wordList.GetAllWords().Where(w => w.IsLearned && !w.IsVerified && w.RememberedSince <= cutOffDate).ToList();
            return wordsToLearn.OrderBy(w => w.Word.Name).ToList();
        }
    }

    public class QuizUnrepeatedSettings : IQuizSettings, INotifyPropertyChanged
    {
        private DateTime unrepeatedSince;

        public DateTime UnrepeatedSince
        {
            get { return unrepeatedSince; }
            set
            {
                unrepeatedSince = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<WordInfo> GetWords(WordList wordList)
        {
            DateTime unrepeatedSinceUtc = unrepeatedSince.Date.ToUniversalTime(); 
            List<WordInfo> wordsToLearn = wordList.GetAllWords().Where(w => w.IsLearned && w.LastEventDate < unrepeatedSinceUtc).ToList();
            return wordsToLearn.OrderBy(w => w.DateAdded ?? DateTime.MaxValue).ThenBy(w => w.Word.Name).ToList();
        }
    }
}