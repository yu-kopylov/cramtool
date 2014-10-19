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

        public IEnumerable<WordTranslation> GetAllTranslations()
        {
            return wordsByName.Values.SelectMany(w => w.Translations);
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

            List<string> tagList = TagParser.ParseTags(tags);

            Word word = new Word();
            word.Name = name;
            word.Description = description;
            word.Tags = TagParser.FormatTags(tagList);
            word.Events.Add(new WordEvent(DateTime.UtcNow, WordEventType.Added));

            WordInfo wordInfo = WordInfo.Create(this, word);
            wordsByName.Add(name, wordInfo);

            foreach (string tag in tagList)
            {
                tagIndex.Add(name, tag);
            }

            UpdateStats();

            return word;
        }

        public void Update(string oldName, string newName, string description, string tags)
        {
            Contract.Assert(wordsByName.ContainsKey(oldName));
            Contract.Assert(newName == oldName || !wordsByName.ContainsKey(newName));

            Modified = true;

            WordInfo wordInfo = wordsByName[oldName];

            List<string> oldTagList = TagParser.ParseTags(wordInfo.Word.Tags);
            List<string> newTagList = TagParser.ParseTags(tags);

            wordInfo.Word.Name = newName;
            wordInfo.Word.Description = description;
            wordInfo.Word.Tags = TagParser.FormatTags(newTagList);
            wordInfo.Update();

            if (newName != oldName)
            {
                wordsByName.Remove(oldName);
                wordsByName.Add(newName, wordInfo);
            }

            tagIndex.Update(oldName, oldTagList, newName, newTagList);

            UpdateStats();
        }

        public void Mark(string name, WordEventType eventType)
        {
            Contract.Assert(wordsByName.ContainsKey(name));
            Modified = true;

            WordInfo wordInfo = wordsByName[name];
            wordInfo.Mark(eventType);

            UpdateStats();
        }

        public void ResetWordHistory(string name)
        {
            Contract.Assert(wordsByName.ContainsKey(name));
            Modified = true;

            WordInfo wordInfo = wordsByName[name];
            wordInfo.ResetHistory();

            UpdateStats();
        }

        public void ResetHistory()
        {
            Modified = true;

            foreach (WordInfo wordInfo in wordsByName.Values)
            {
                wordInfo.ResetHistory();
            }

            UpdateStats();
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
                wordsByName.Add(word.Name, WordInfo.Create(this, word));
                List<string> tagList = TagParser.ParseTags(word.Tags);
                foreach (string tag in tagList)
                {
                    tagIndex.Add(word.Name, tag);
                }
            }
            Modified = false;

            UpdateStats();
        }

        private void UpdateStats()
        {
            Stats = WordsListStats.BuildFrom(this);
        }
    }
}