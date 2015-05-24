using System;
using System.Collections.Generic;
using System.Linq;
using CramTool.Formats.WordList;
using CramTool.Models;

namespace CramTool.Formats
{
    public static class WordListXmlConverter
    {
        public static WordListXml ConvertToXml(Models.WordList wordList)
        {
            var wordListXml = new WordListXml();
            List<Word> words = wordList.GetAllWords().Select(w => w.Word).ToList();
            wordListXml.Words = new WordXml[words.Count];

            int idx = 0;
            foreach (Word word in words)
            {
                wordListXml.Words[idx++] = ConvertToXml(word);
            }

            return wordListXml;
        }

        private static WordXml ConvertToXml(Word word)
        {
            var wordXml = new WordXml();

            wordXml.Name = word.Name;
            wordXml.Description = word.Description;
            wordXml.Tags = string.IsNullOrEmpty(word.Tags) ? null : word.Tags;
            wordXml.Events = new WordEventXml[word.Events.Count];
            int idx = 0;
            foreach (WordEvent wordEvent in word.Events)
            {
                wordXml.Events[idx++] = ConvertToXml(wordEvent);
            }

            return wordXml;
        }

        private static WordEventXml ConvertToXml(WordEvent wordEvent)
        {
            var wordEventXml = new WordEventXml();

            wordEventXml.EventDate = wordEvent.EventDate;
            wordEventXml.EventType = ConvertToXml(wordEvent.EventType);
            wordEventXml.Translation = wordEvent.Translation;

            return wordEventXml;
        }

        public static Models.WordList ConvertToObject(WordListXml wordListXml)
        {
            var words = new List<Word>();

            foreach (WordXml wordXml in wordListXml.Words)
            {
                words.Add(ConvertToObject(wordXml));
            }

            var wordList = new Models.WordList();
            wordList.Populate(words);

            return wordList;
        }

        private static Word ConvertToObject(WordXml wordXml)
        {
            var word = new Word();

            word.Name = wordXml.Name;
            word.Description = wordXml.Description;
            word.Tags = wordXml.Tags;

            List<WordEvent> events = ConvertToObject(wordXml.Events);

            foreach (WordEvent wordEvent in events)
            {
                word.Events.Add(wordEvent);
            }

            return word;
        }

        private static List<WordEvent> ConvertToObject(WordEventXml[] wordEventsXml)
        {
            List<WordEvent> events = new List<WordEvent>();

            foreach (WordEventXml wordEventXml in wordEventsXml)
            {
                events.Add(ConvertToObject(wordEventXml));
            }

            NormalizeEventsOrder(events);

            return events;
        }

        private static void NormalizeEventsOrder(List<WordEvent> events)
        {
            if (events.Count == 0)
            {
                return;
            }

            events.Sort(CompareEventDates);

            WordEvent firstEvent = events[0];

            if (firstEvent.EventType != WordEventType.Added)
            {
                firstEvent = new WordEvent(firstEvent.EventDate, WordEventType.Added);
                events.Insert(0, firstEvent);
            }

            WordEvent secondEvent = events.Count > 1 ? events[1] : null;

            if (secondEvent != null && secondEvent.EventDate < firstEvent.EventDate)
            {
                firstEvent = new WordEvent(secondEvent.EventDate, firstEvent.EventType);
                events[0] = firstEvent;
            }
        }

        private static int CompareEventDates(WordEvent e1, WordEvent e2)
        {
            if (e1.EventType == WordEventType.Added && e2.EventType != WordEventType.Added)
            {
                return -1;
            }
            if (e1.EventType != WordEventType.Added && e2.EventType == WordEventType.Added)
            {
                return 1;
            }
            return e1.EventDate.CompareTo(e2.EventDate);
        }

        private static WordEvent ConvertToObject(WordEventXml wordEventXml)
        {
            DateTime eventDate = wordEventXml.EventDate;
            WordEventType eventType = ConvertToObject(wordEventXml.EventType);
            return new WordEvent(eventDate, eventType, wordEventXml.Translation);
        }

        private static WordEventTypeXml ConvertToXml(WordEventType eventType)
        {
            if (eventType == WordEventType.Added)
            {
                return WordEventTypeXml.Added;
            }
            if (eventType == WordEventType.Remembered)
            {
                return WordEventTypeXml.Remembered;
            }
            if (eventType == WordEventType.Forgotten)
            {
                return WordEventTypeXml.Forgotten;
            }
            throw new Exception(string.Format("Unrecognized eventType: {0}", eventType));
        }

        private static WordEventType ConvertToObject(WordEventTypeXml eventType)
        {
            if (eventType == WordEventTypeXml.Added)
            {
                return WordEventType.Added;
            }
            if (eventType == WordEventTypeXml.Remembered)
            {
                return WordEventType.Remembered;
            }
            if (eventType == WordEventTypeXml.Forgotten)
            {
                return WordEventType.Forgotten;
            }
            //todo: remove this value from format
            if (eventType == WordEventTypeXml.Forgot)
            {
                return WordEventType.Forgotten;
            }
            throw new Exception(string.Format("Unrecognized eventType: {0}", eventType));
        }
    }
}