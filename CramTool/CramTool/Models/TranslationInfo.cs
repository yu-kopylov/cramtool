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

        private WordState state = WordState.Unknown;

        private WordEventInfo lastEvent = null; 
        
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

        public WordState State
        {
            get { return state; }
            set
            {
                state = value;
                OnPropertyChanged();
            }
        }

        public WordEventInfo LastEvent
        {
            get { return lastEvent; }
            set
            {
                lastEvent = value;
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
            List<WordInfo> words = WordList.GetWordsWithTranslation(Translation).ToList();

            UpdateEvents(words);

            State = LastEvent == null ? WordState.Unknown : LastEvent.WordState;
            
            //            IsStudied = State >= WordState.Studied;
            //            IsLearned = State >= WordState.Learned;
            //            IsVerified = State >= WordState.Verified;
            
            //            ParseWord(this);
        }

        private void UpdateEvents(List<WordInfo> words)
        {
            WordEventInfo lastEventInfo = null;
            List<WordEventInfo> eventInfos = new List<WordEventInfo>();
            List<WordEvent> rawEvents = words.SelectMany(w => w.Word.Events).OrderBy(e => e.EventDate).ToList();
            foreach (WordEvent wordEvent in rawEvents)
            {
                lastEventInfo = CreateEventInfo(lastEventInfo, wordEvent);
                eventInfos.Add(lastEventInfo);
            }

            eventInfos.Reverse();

            LastEvent = eventInfos.FirstOrDefault();
            Events.Clear();
            foreach (WordEventInfo eventInfo in eventInfos)
            {
                Events.Add(eventInfo);
            }
        }

        private static WordEventInfo CreateEventInfo(WordEventInfo prevEventInfo, WordEvent wordEvent)
        {
            if (prevEventInfo == null || wordEvent.EventType != WordEventType.Remembered)
            {
                return new WordEventInfo(wordEvent, WordState.Studied, wordEvent.EventDate);
            }

            TimeSpan timeSinceLastStateChange = wordEvent.EventDate - prevEventInfo.LastStateChange;

            if (prevEventInfo.WordState == WordState.Studied && timeSinceLastStateChange >= WordInfo.TimeToMarkRepeated)
            {
                return new WordEventInfo(wordEvent, WordState.Repeated, wordEvent.EventDate);
            }

            if (prevEventInfo.WordState == WordState.Repeated && timeSinceLastStateChange >= WordInfo.TimeToMarkLearned)
            {
                return new WordEventInfo(wordEvent, WordState.Learned, wordEvent.EventDate);
            }

            if (prevEventInfo.WordState == WordState.Learned && timeSinceLastStateChange >= WordInfo.TimeToMarkVerified)
            {
                return new WordEventInfo(wordEvent, WordState.Verified, wordEvent.EventDate);
            }

            return new WordEventInfo(wordEvent, prevEventInfo.WordState, prevEventInfo.LastStateChange);
        }
    }
}