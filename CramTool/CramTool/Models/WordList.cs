﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CramTool.Models
{
    public class WordList : INotifyPropertyChanged
    {
        private readonly Dictionary<string, WordInfo> wordsByName = new Dictionary<string, WordInfo>();
        private readonly WordIndex tagIndex = new WordIndex();
        private readonly WordIndex translationIndex = new WordIndex();

        private bool modified;
        private WordsListStats stats;

        public bool Modified
        {
            get { return modified; }
            private set
            {
                modified = value;
                OnPropertyChanged();
            }
        }

        public WordsListStats Stats
        {
            get { return stats; }
            private set
            {
                stats = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<WordInfo> GetAllWords()
        {
            return wordsByName.Values;
        }

        public IEnumerable<WordForm> GetAllForms()
        {
            return wordsByName.Values.SelectMany(w => w.Forms);
        }

        public IEnumerable<string> GetAllTranslations()
        {
            return translationIndex.GetAttributes();
        }

        public IEnumerable<WordInfo> GetWordsWithTranslation(string translation)
        {
            return translationIndex.GetWordNames(translation).Select(wordName => wordsByName[wordName]);
        }

        public IEnumerable<string> GetAllTags()
        {
            return tagIndex.GetAttributes();
        }

        public IEnumerable<WordInfo> GetWordsWithTag(string tag)
        {
            return tagIndex.GetWordNames(tag).Select(wordName => wordsByName[wordName]);
        }

        public bool Contains(string name)
        {
            return wordsByName.ContainsKey(name);
        }

        public delegate void ContentsChangedEventHandler(object sender, EventArgs e);

        public event ContentsChangedEventHandler ContentsChanged;

        protected virtual void OnContentsChanged()
        {
            ContentsChangedEventHandler handler = ContentsChanged;
            if (handler != null) handler(this, new EventArgs());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public WordList()
        {
            UpdateStats();
        }

        public Word Add(string name, string description, string tags)
        {
            Contract.Assert(!wordsByName.ContainsKey(name));
            Modified = true;

            Word word = new Word();
            word.Name = name;
            word.Description = description;
            word.Tags = TagParser.ReformatTags(tags);
            word.Mark(WordEventType.Added);

            WordInfo wordInfo = WordInfo.Create(this, word);
            wordsByName.Add(name, wordInfo);

            tagIndex.Add(name, wordInfo.Tags);
            translationIndex.Add(name, wordInfo.Translations);

            UpdateStats();

            OnContentsChanged();

            return word;
        }

        public void Update(string oldName, string newName, string description, string tags)
        {
            Contract.Assert(wordsByName.ContainsKey(oldName));
            Contract.Assert(newName == oldName || !wordsByName.ContainsKey(newName));

            Modified = true;

            WordInfo wordInfo = wordsByName[oldName];

            List<string> oldTags = new List<string>(wordInfo.Tags);
            List<string> oldTranslations = new List<string>(wordInfo.Translations);

            wordInfo.Word.Name = newName;
            wordInfo.Word.Description = description;
            wordInfo.Word.Tags = TagParser.ReformatTags(tags);
            wordInfo.Update();

            if (newName != oldName)
            {
                wordsByName.Remove(oldName);
                wordsByName.Add(newName, wordInfo);
            }

            tagIndex.Update(oldName, oldTags, newName, wordInfo.Tags);
            translationIndex.Update(oldName, oldTranslations, newName, wordInfo.Translations);

            UpdateStats();

            OnContentsChanged();
        }

        public void Mark(string name, WordEventType eventType)
        {
            Contract.Assert(wordsByName.ContainsKey(name));
            Modified = true;

            WordInfo wordInfo = wordsByName[name];
            wordInfo.Mark(eventType);

            UpdateStats();

            OnContentsChanged();
        }

        public void ResetWordHistory(string name)
        {
            Contract.Assert(wordsByName.ContainsKey(name));
            Modified = true;

            WordInfo wordInfo = wordsByName[name];
            wordInfo.ResetHistory();

            UpdateStats();

            OnContentsChanged();
        }

        public void ResetHistory()
        {
            Modified = true;

            foreach (WordInfo wordInfo in wordsByName.Values)
            {
                wordInfo.ResetHistory();
            }

            UpdateStats();

            OnContentsChanged();
        }

        public void ResetModified()
        {
            Modified = false;
        }

        public void Populate(List<Word> words)
        {
            Contract.Assert(wordsByName.Count == 0);
            foreach (Word word in words)
            {
                WordInfo wordInfo = WordInfo.Create(this, word);
                wordsByName.Add(word.Name, wordInfo);

                tagIndex.Add(word.Name, wordInfo.Tags);
                translationIndex.Add(word.Name, wordInfo.Translations);
            }
            Modified = false;

            UpdateStats();

            OnContentsChanged();
        }

        private void UpdateStats()
        {
            Stats = WordsListStats.BuildFrom(this);
        }
    }
}