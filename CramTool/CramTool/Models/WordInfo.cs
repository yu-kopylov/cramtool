using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CramTool.Models
{
    public class WordInfo : INotifyPropertyChanged
    {
        private static readonly TimeSpan TimeToLearn = TimeSpan.FromHours(12);
        public static readonly TimeSpan TimeToVerify = TimeSpan.FromHours(24*7 + 12);

        private WordList wordList;
        private Word word;

        private bool isAdded;
        private DateTime? dateAdded;
        private DateTime? lastEventDate;
        private DateTime? rememberedSince;
        private TimeSpan? rememberedFor;
        private bool isLearned;
        private bool isVerified;

        private WordState state = WordState.Unknown;

        private readonly ObservableCollection<WordEventInfo> events = new ObservableCollection<WordEventInfo>();
        private readonly ObservableCollection<WordForm> forms = new ObservableCollection<WordForm>();
        private readonly ObservableCollection<string> translations = new ObservableCollection<string>();
        private readonly ObservableCollection<string> tags = new ObservableCollection<string>();

        public WordList WordList
        {
            get { return wordList; }
            private set
            {
                wordList = value;
                OnPropertyChanged();
            }
        }

        public Word Word
        {
            get { return word; }
            private set
            {
                word = value;
                OnPropertyChanged();
            }
        }

        public bool IsAdded
        {
            get { return isAdded; }
            private set
            {
                isAdded = value;
                OnPropertyChanged();
            }
        }

        public DateTime? DateAdded
        {
            get { return dateAdded; }
            private set
            {
                dateAdded = value;
                OnPropertyChanged();
            }
        }

        public DateTime? LastEventDate
        {
            get { return lastEventDate; }
            private set
            {
                lastEventDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime? RememberedSince
        {
            get { return rememberedSince; }
            set
            {
                rememberedSince = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan? RememberedFor
        {
            get { return rememberedFor; }
            set
            {
                rememberedFor = value;
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
            private set
            {
                state = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<WordEventInfo> Events
        {
            get { return events; }
        }

        public ObservableCollection<WordForm> Forms
        {
            get { return forms; }
        }

        public ObservableCollection<string> Translations
        {
            get { return translations; }
        }

        public ObservableCollection<string> Tags
        {
            get { return tags; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public static WordInfo Create(WordList wordList, Word word)
        {
            WordInfo res = new WordInfo();

            res.WordList = wordList;
            res.Word = word;

            res.Update();

            return res;
        }

        public void Update()
        {
            IsAdded = word.Events.Count > 0;
            DateAdded = word.Events.Count > 0 ? word.Events.First().EventDate : (DateTime?) null;
            LastEventDate = word.Events.Count > 0 ? word.Events.Last().EventDate : (DateTime?) null;

            UpdatePeriods();
            UpdateEvents();
            
            IsLearned = RememberedFor >= TimeToLearn;
            IsVerified = RememberedFor >= TimeToVerify;

            State = EvalState(this);

            ParseWord(this);
            
            tags.Clear();
            foreach (string tag in TagParser.ParseTags(word.Tags))
            {
                tags.Add(tag);
            }
        }

        private static WordState EvalState(WordInfo wordInfo)
        {
            if (!wordInfo.IsAdded)
            {
                return WordState.Unknown;
            }

            if (wordInfo.IsVerified)
            {
                return WordState.Verified;
            }

            if (wordInfo.IsLearned)
            {
                return WordState.Learned;
            }

            return WordState.Studied;
        }

        private void UpdateEvents()
        {
            List<WordEventInfo> wordEventInfos = word.Events.Select(e => new WordEventInfo(e)).ToList();
            wordEventInfos.Reverse();
            Events.Clear();
            foreach (WordEventInfo wordEventInfo in wordEventInfos)
            {
                Events.Add(wordEventInfo);
            }
        }

        private void UpdatePeriods()
        {
            rememberedSince = null;
            rememberedFor = null;
            foreach (WordEvent wordEvent in word.Events)
            {
                if (wordEvent.EventType == WordEventType.Remembered)
                {
                    if (rememberedSince == null)
                    {
                        rememberedSince = wordEvent.EventDate;
                    }
                    rememberedFor = wordEvent.EventDate - rememberedSince;
                }
                if (wordEvent.EventType == WordEventType.Forgotten)
                {
                    rememberedSince = null;
                    rememberedFor = null;
                }
            }

            OnPropertyChanged("RememberedSince");
            OnPropertyChanged("RememberedFor");
        }

        //todo: move to parser
        private static void ParseWord(WordInfo info)
        {
            ISet<string> forms = new HashSet<string>();
            ISet<string> translations = new HashSet<string>();
            forms.Add(info.Word.Name);

            WordParser parser = new WordParser();
            var tokens = parser.Parse(info.Word.Description);
            foreach (Token token in tokens)
            {
                if (token.Type == TokenType.WordForm && !string.IsNullOrWhiteSpace(token.Value))
                {
                    forms.Add(token.Value);
                }
                if (token.Type == TokenType.Translation && !string.IsNullOrWhiteSpace(token.Value))
                {
                    translations.Add(token.Value);
                }
            }

            info.Forms.Clear();
            foreach (string form in forms)
            {
                info.Forms.Add(new WordForm(info, form));
            }

            info.Translations.Clear();
            foreach (string translation in translations)
            {
                info.Translations.Add(translation);
            }
        }

        //todo: is this wrapper required?
        public void Mark(WordEventType eventType)
        {
            Word.Mark(eventType);
            Update();
        }

        //todo: is this wrapper required?
        public void ResetHistory()
        {
            Word.ResetHistory();
            Update();
        }
    }
}