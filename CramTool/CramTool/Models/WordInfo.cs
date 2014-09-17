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
        private WordList wordList;
        private Word word;

        private bool isAdded;
        private DateTime? dateAdded;
        private DateTime? lastEventDate;
        private bool isLearned;

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

        public bool IsLearned
        {
            get { return isLearned; }
            private set
            {
                isLearned = value;
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
            DateAdded = EvalDateAdded(word);
            IsLearned = EvalIsLearned(word);
            LastEventDate = EvalLastEventDate(word);

            State = EvalState(this);

            ParseWord(this);
        }

        private static WordState EvalState(WordInfo wordInfo)
        {
            if (!wordInfo.IsAdded)
            {
                return WordState.Unknown;
            }

            if (wordInfo.IsLearned)
            {
                return WordState.Learned;
            }

            return WordState.Studied;
        }

        private static DateTime? EvalDateAdded(Word word)
        {
            return word.Events.Where(e => e.EventType == WordEventType.Added).Min(e => (DateTime?)e.EventDate);
        }

        private static DateTime? EvalLastEventDate(Word word)
        {
            return word.Events.Max(e => (DateTime?)e.EventDate);
        }

        private static bool EvalIsLearned(Word word)
        {
            List<WordEvent> learningEvents = word.Events.Where(e => e.IsLearningEvent).OrderByDescending(e => e.EventDate).ToList();
            if (learningEvents.Count == 0)
            {
                return false;
            }

            if (learningEvents[0].EventType != WordEventType.Remembered)
            {
                return false;
            }

            DateTime learningStart = learningEvents[0].EventDate.AddHours(-12);

            foreach (var learningEvent in learningEvents)
            {
                if (learningEvent.EventType != WordEventType.Remembered)
                {
                    return false;
                }
                if (learningEvent.EventDate <= learningStart)
                {
                    return true;
                }
            }

            return false;
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