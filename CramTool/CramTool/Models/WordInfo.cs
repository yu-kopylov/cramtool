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

        private readonly ObservableCollection<WordForm> forms = new ObservableCollection<WordForm>();
        private readonly ObservableCollection<WordTranslation> translations = new ObservableCollection<WordTranslation>();

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

        public ObservableCollection<WordForm> Forms
        {
            get { return forms; }
        }

        public ObservableCollection<WordTranslation> Translations
        {
            get { return translations; }
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
            DateAdded = word.Events.Min(e => (DateTime?)e.EventDate);
            LastEventDate = word.Events.Max(e => (DateTime?)e.EventDate);

            UpdatePeriods();
            
            IsLearned = RememberedFor >= TimeToLearn;
            IsVerified = RememberedFor >= TimeToVerify;

            State = EvalState(this);

            ParseWord(this);
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

        private void UpdatePeriods()
        {
            rememberedSince = null;
            rememberedFor = null;
            //todo: review word.Events ordering
            foreach (WordEvent wordEvent in word.Events.OrderBy(e => e.EventDate))
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
                info.Translations.Add(new WordTranslation(translation, info));
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