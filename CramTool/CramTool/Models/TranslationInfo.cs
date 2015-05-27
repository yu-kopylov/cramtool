using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CramTool.Models
{
    public class TranslationInfo : INotifyPropertyChanged
    {
        private WordList wordList;
        private string translation;
        private List<WordInfo> words;

        private bool isStudied;
        private bool isRepeated;
        private bool isLearned;
        private bool isVerified;

        private WordState state = WordState.Unknown;
        private DateTime lastStateChange = DateTime.MinValue;

        private readonly ObservableCollection<WordEventInfo> events = new ObservableCollection<WordEventInfo>();

        public WordList WordList
        {
            get { return wordList; }
            private set
            {
                wordList = value;
                OnPropertyChanged();
            }
        }

        public string Translation
        {
            get { return translation; }
            private set
            {
                translation = value;
                OnPropertyChanged();
            }
        }

        public bool IsStudied
        {
            get { return isStudied; }
            private set
            {
                isStudied = value;
                OnPropertyChanged();
            }
        }

        public bool IsRepeated
        {
            get { return isRepeated; }
            set
            {
                isRepeated = value;
                OnPropertyChanged();
            }
        }

        public bool IsLearned
        {
            get { return isLearned; }
            set
            {
                isLearned = value;
                OnPropertyChanged();
            }
        }

        public bool IsVerified
        {
            get { return isVerified; }
            set
            {
                isVerified = value;
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

        public DateTime LastStateChange
        {
            get { return lastStateChange; }
            set
            {
                lastStateChange = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<WordEventInfo> Events
        {
            get { return events; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public static TranslationInfo Create(WordList wordList, string translation)
        {
            TranslationInfo res = new TranslationInfo();

            res.WordList = wordList;
            res.Translation = translation;

            res.Update();

            return res;
        }

        public void Update()
        {
            words = WordList.GetWordsWithTranslation(Translation).ToList();

            UpdateEvents();

            IsStudied = State >= WordState.Studied;
            IsRepeated = State >= WordState.Repeated;
            IsLearned = State >= WordState.Learned;
            IsVerified = State >= WordState.Verified;
        }

        private void UpdateEvents()
        {
            WordState lastState = WordState.Unknown;
            DateTime lastStateChange = DateTime.MinValue;

            List<WordEventInfo> eventInfos = new List<WordEventInfo>();
            foreach (WordInfo wordInfo in words)
            {
                WordEventInfo lastEventInfo = null;
                foreach (WordEvent wordEvent in wordInfo.Word.Events)
                {
                    if (wordEvent.EventType == WordEventType.Added || wordEvent.Translation == Translation)
                    {
                        lastEventInfo = CreateEventInfo(wordInfo.Word, lastEventInfo, wordEvent);
                        eventInfos.Add(lastEventInfo);
                    }
                }

                if (lastEventInfo != null && (lastState == WordState.Unknown || lastEventInfo.WordState < lastState))
                {
                    lastState = lastEventInfo.WordState;
                    lastStateChange = lastEventInfo.LastStateChange;
                }
            }

            State = lastState;
            LastStateChange = lastStateChange;

            eventInfos.Reverse();
            eventInfos = eventInfos.OrderByDescending(e => e.WordEvent.EventDate).ToList();

            Events.Clear();
            foreach (WordEventInfo eventInfo in eventInfos)
            {
                Events.Add(eventInfo);
            }
        }

        private static WordEventInfo CreateEventInfo(Word word, WordEventInfo prevEventInfo, WordEvent wordEvent)
        {
            if (prevEventInfo == null || wordEvent.EventType != WordEventType.Remembered)
            {
                return new WordEventInfo(word, wordEvent, WordState.Studied, wordEvent.EventDate);
            }

            TimeSpan timeSinceLastStateChange = wordEvent.EventDate - prevEventInfo.LastStateChange;

            if (prevEventInfo.WordState == WordState.Studied && timeSinceLastStateChange >= WordInfo.TimeToMarkRepeated)
            {
                return new WordEventInfo(word, wordEvent, WordState.Repeated, wordEvent.EventDate);
            }

            if (prevEventInfo.WordState == WordState.Repeated && timeSinceLastStateChange >= WordInfo.TimeToMarkLearned)
            {
                return new WordEventInfo(word, wordEvent, WordState.Learned, wordEvent.EventDate);
            }

            if (prevEventInfo.WordState == WordState.Learned && timeSinceLastStateChange >= WordInfo.TimeToMarkVerified)
            {
                return new WordEventInfo(word, wordEvent, WordState.Verified, wordEvent.EventDate);
            }

            return new WordEventInfo(word, wordEvent, prevEventInfo.WordState, prevEventInfo.LastStateChange);
        }

        //todo: is this wrapper required?
        public void Mark(WordEventType eventType)
        {
            foreach (WordInfo word in words)
            {
                if (word.IsStudied)
                {
                    word.MarkTranslation(eventType, Translation);
                }
            }
            Update();
        }
    }
}