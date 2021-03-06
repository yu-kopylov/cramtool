﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CramTool.Models
{
    public class WordInfo : INotifyPropertyChanged
    {
        public static readonly TimeSpan TimeToMarkRepeated = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan TimeToMarkLearned = TimeSpan.FromHours(12);
        public static readonly TimeSpan TimeToMarkVerified = TimeSpan.FromHours(24*4 + 12);

        private WordList wordList;
        private Word word;

        private bool isStudied;
        private bool isRepeated;
        private bool isLearned;
        private bool isVerified;

        private WordState state = WordState.Unknown;

        private WordEventInfo lastEvent = null;

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
            private set
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
            UpdateEvents();
            
            State = LastEvent == null ? WordState.Unknown : LastEvent.WordState;

            IsStudied = State >= WordState.Studied; 
            IsRepeated = State >= WordState.Repeated; 
            IsLearned = State >= WordState.Learned; 
            IsVerified = State >= WordState.Verified; 

            ParseWord(this);
            
            tags.Clear();
            foreach (string tag in TagParser.ParseTags(word.Tags))
            {
                tags.Add(tag);
            }
        }

        private void UpdateEvents()
        {
            WordEventInfo lastEventInfo = null;
            List<WordEventInfo> eventInfos = new List<WordEventInfo>();
            foreach (WordEvent wordEvent in word.Events)
            {
                if (wordEvent.Translation == null)
                {
                    lastEventInfo = CreateEventInfo(word, lastEventInfo, wordEvent);
                    eventInfos.Add(lastEventInfo);
                }
            }

            eventInfos.Reverse();

            LastEvent = eventInfos.FirstOrDefault();
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

            if (prevEventInfo.WordState == WordState.Studied && timeSinceLastStateChange >= TimeToMarkRepeated)
            {
                return new WordEventInfo(word, wordEvent, WordState.Repeated, wordEvent.EventDate);
            }

            if (prevEventInfo.WordState == WordState.Repeated && timeSinceLastStateChange >= TimeToMarkLearned)
            {
                return new WordEventInfo(word, wordEvent, WordState.Learned, wordEvent.EventDate);
            }

            if (prevEventInfo.WordState == WordState.Learned && timeSinceLastStateChange >= TimeToMarkVerified)
            {
                return new WordEventInfo(word, wordEvent, WordState.Verified, wordEvent.EventDate);
            }

            return new WordEventInfo(word, wordEvent, prevEventInfo.WordState, prevEventInfo.LastStateChange);
        }

        private static void ParseWord(WordInfo info)
        {
            ArticleParser parser = new ArticleParser();
            WordArticle article = parser.Parse(info.Word.Name, info.Word.Description);

            List<string> forms = article.FormGroups.SelectMany(g => g.Forms).Distinct().ToList();
            info.Forms.Clear();
            foreach (string form in forms)
            {
                info.Forms.Add(new WordForm(info, form));
            }

            List<string> translations = article.FormGroups.SelectMany(g => g.TranslationGroups).SelectMany(tg => tg.Translations).Distinct().ToList();
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
        public void MarkTranslation(WordEventType eventType, string translation)
        {
            Word.MarkTranslation(eventType, translation);
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