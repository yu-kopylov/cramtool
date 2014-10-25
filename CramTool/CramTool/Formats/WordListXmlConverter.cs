using System;
using System.Collections.Generic;
using System.Linq;
using CramTool.Formats.WordList;

namespace CramTool.Formats
{
    public static class WordListXmlConverter
    {
        public static WordList.WordList ConvertToXml(Models.WordList wordList)
        {
            var wordListXml = new WordList.WordList();
            List<Models.Word> words = wordList.GetAllWords().Select(w => w.Word).ToList();
            wordListXml.Words = new Word[words.Count];

            int idx = 0;
            foreach (Models.Word word in words)
            {
                wordListXml.Words[idx++] = ConvertToXml(word);
            }

            return wordListXml;
        }

        private static Word ConvertToXml(Models.Word word)
        {
            var wordXml = new Word();

            wordXml.Name = word.Name;
            wordXml.Description = word.Description;
            wordXml.Tags = string.IsNullOrEmpty(word.Tags) ? null : word.Tags;
            wordXml.Events = new WordEvent[word.Events.Count];
            int idx = 0;
            foreach (Models.WordEvent wordEvent in word.Events)
            {
                wordXml.Events[idx++] = ConvertToXml(wordEvent);
            }

            return wordXml;
        }

        private static WordEvent ConvertToXml(Models.WordEvent wordEvent)
        {
            var wordEventXml = new WordEvent();

            wordEventXml.EventDate = wordEvent.EventDate;
            wordEventXml.EventType = ConvertToXml(wordEvent.EventType);

            return wordEventXml;
        }

        public static Models.WordList ConvertToObject(WordList.WordList wordListXml)
        {
            var words = new List<Models.Word>();

            foreach (Word wordXml in wordListXml.Words)
            {
                words.Add(ConvertToObject(wordXml));
            }

            var wordList = new Models.WordList();
            wordList.Populate(words);

            return wordList;
        }

        private static Models.Word ConvertToObject(Word wordXml)
        {
            var word = new Models.Word();

            word.Name = wordXml.Name;
            word.Description = wordXml.Description;
            word.Tags = wordXml.Tags;

            List<Models.WordEvent> events = ConvertToObject(wordXml.Events);

            foreach (Models.WordEvent wordEvent in events)
            {
                word.Events.Add(wordEvent);
            }

            return word;
        }

        private static List<Models.WordEvent> ConvertToObject(WordEvent[] wordEventsXml)
        {
            List<Models.WordEvent> events = new List<Models.WordEvent>();

            foreach (WordEvent wordEventXml in wordEventsXml)
            {
                events.Add(ConvertToObject(wordEventXml));
            }

            NormalizeEventsOrder(events);

            return events;
        }

        private static void NormalizeEventsOrder(List<Models.WordEvent> events)
        {
            if (events.Count == 0)
            {
                return;
            }
            
            events.Sort(CompareEventDates);

            Models.WordEvent firstEvent = events[0];

            if (firstEvent.EventType != Models.WordEventType.Added)
            {
                firstEvent = new Models.WordEvent(firstEvent.EventDate, Models.WordEventType.Added);
                events.Insert(0, firstEvent);
            }

            Models.WordEvent secondEvent = events.Count > 1 ? events[1] : null;

            if (secondEvent != null && secondEvent.EventDate < firstEvent.EventDate)
            {
                firstEvent = new Models.WordEvent(secondEvent.EventDate, firstEvent.EventType);
                events[0] = firstEvent;
            }
        }

        private static int CompareEventDates(Models.WordEvent e1, Models.WordEvent e2)
        {
            if (e1.EventType == Models.WordEventType.Added && e2.EventType != Models.WordEventType.Added)
            {
                return -1;
            }
            if (e1.EventType != Models.WordEventType.Added && e2.EventType == Models.WordEventType.Added)
            {
                return 1;
            }
            return e1.EventDate.CompareTo(e2.EventDate);
        }

        private static Models.WordEvent ConvertToObject(WordEvent wordEventXml)
        {
            DateTime eventDate = wordEventXml.EventDate;
            Models.WordEventType eventType = ConvertToObject(wordEventXml.EventType);
            return new Models.WordEvent(eventDate, eventType);
        }

        private static WordEventType ConvertToXml(Models.WordEventType eventType)
        {
            if (eventType == Models.WordEventType.Added)
            {
                return WordEventType.Added;
            }
            if (eventType == Models.WordEventType.Remembered)
            {
                return WordEventType.Remembered;
            }
            if (eventType == Models.WordEventType.Forgotten)
            {
                return WordEventType.Forgot;
            }
            throw new Exception(string.Format("Unrecognized eventType: {0}", eventType));
        }

        private static Models.WordEventType ConvertToObject(WordEventType eventType)
        {
            if (eventType == WordEventType.Added)
            {
                return Models.WordEventType.Added;
            }
            if (eventType == WordEventType.Remembered)
            {
                return Models.WordEventType.Remembered;
            }
            if (eventType == WordEventType.Forgot)
            {
                return Models.WordEventType.Forgotten;
            }
            throw new Exception(string.Format("Unrecognized eventType: {0}", eventType));
        }
    }
}