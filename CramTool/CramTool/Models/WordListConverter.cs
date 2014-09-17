using System;
using System.Collections.Generic;
using System.Linq;

namespace CramTool.Models
{
    public static class WordListConverter
    {
        public static Formats.WordList.WordList ConvertToXml(WordList wordList)
        {
            Formats.WordList.WordList wordListXml = new Formats.WordList.WordList();
            List<Word> words = wordList.GetAllWords().Select(w => w.Word).ToList();
            wordListXml.Words = new Formats.WordList.Word[words.Count];

            int idx = 0;
            foreach (Word word in words)
            {
                wordListXml.Words[idx++] = ConvertToXml(word);
            }

            return wordListXml;
        }

        private static Formats.WordList.Word ConvertToXml(Word word)
        {
            Formats.WordList.Word wordXml = new Formats.WordList.Word();

            wordXml.Name = word.Name;
            wordXml.Description = word.Description;
            wordXml.Events = new Formats.WordList.WordEvent[word.Events.Count];
            int idx = 0;
            foreach (WordEvent wordEvent in word.Events)
            {
                wordXml.Events[idx++] = ConvertToXml(wordEvent);
            }

            return wordXml;
        }

        private static Formats.WordList.WordEvent ConvertToXml(WordEvent wordEvent)
        {
            Formats.WordList.WordEvent wordEventXml = new Formats.WordList.WordEvent();

            wordEventXml.EventDate = wordEvent.EventDate;
            wordEventXml.EventType = ConvertToXml(wordEvent.EventType);

            return wordEventXml;
        }

        public static WordList ConvertToObject(Formats.WordList.WordList wordListXml)
        {
            List<Word> words = new List<Word>();

            foreach (Formats.WordList.Word wordXml in wordListXml.Words)
            {
                words.Add(ConvertToObject(wordXml));
            }

            WordList wordList = new WordList();
            wordList.Populate(words);

            return wordList;
        }

        private static Word ConvertToObject(Formats.WordList.Word wordXml)
        {
            Word word = new Word();

            word.Name = wordXml.Name;
            word.Description = wordXml.Description;

            foreach (var wordEventXml in wordXml.Events)
            {
                word.Events.Add(ConvertToObject(wordEventXml));
            }

            return word;
        }

        private static WordEvent ConvertToObject(Formats.WordList.WordEvent wordEventXml)
        {
            DateTime eventDate = wordEventXml.EventDate;
            WordEventType eventType = ConvertToObject(wordEventXml.EventType);
            return new WordEvent(eventDate, eventType);
        }

        private static Formats.WordList.WordEventType ConvertToXml(WordEventType eventType)
        {
            if (eventType == WordEventType.Added)
            {
                return Formats.WordList.WordEventType.Added;
            }
            if (eventType == WordEventType.Remembered)
            {
                return Formats.WordList.WordEventType.Remembered;
            }
            if (eventType == WordEventType.Forgotten)
            {
                return Formats.WordList.WordEventType.Forgot;
            }
            throw new Exception(string.Format("Unrecognized eventType: {0}", eventType));
        }

        private static WordEventType ConvertToObject(Formats.WordList.WordEventType eventType)
        {
            if (eventType == Formats.WordList.WordEventType.Added)
            {
                return WordEventType.Added;
            }
            if (eventType == Formats.WordList.WordEventType.Remembered)
            {
                return WordEventType.Remembered;
            }
            if (eventType == Formats.WordList.WordEventType.Forgot)
            {
                return WordEventType.Forgotten;
            }
            throw new Exception(string.Format("Unrecognized eventType: {0}", eventType));
        }
    }
}