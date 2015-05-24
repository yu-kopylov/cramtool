using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CramTool.Models
{
    public class Word : INotifyPropertyChanged
    {
        private string name;
        private string description;
        private string tags;
        private readonly List<WordEvent> events = new List<WordEvent>();

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged();
            }
        }

        public string Tags
        {
            get { return tags; }
            set
            {
                tags = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Events in ascending EventDate order.
        /// Event with type 'Added' should always be the first event.
        /// </summary>
        public List<WordEvent> Events
        {
            get { return events; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Mark(WordEventType eventType)
        {
            Contract.Assert(eventType != WordEventType.Added || Events.Count == 0);
            Contract.Assert(eventType == WordEventType.Added || Events.Count > 0);
            
            WordEvent wordEvent = new WordEvent(DateTime.UtcNow, eventType);
            AddEvent(wordEvent);
        }

        public void MarkTranslation(WordEventType eventType, string translation)
        {
            Contract.Assert(eventType != WordEventType.Added);
            Contract.Assert(Events.Count > 0);

            WordEvent wordEvent = new WordEvent(DateTime.UtcNow, eventType, translation);
            AddEvent(wordEvent);
        }

        private void AddEvent(WordEvent wordEvent)
        {
            List<WordEvent> cleanEvents = Events.Where(e => !EventsOverllap(e, wordEvent)).ToList();
            events.Clear();
            events.AddRange(cleanEvents);
            
            if (wordEvent.EventType != WordEventType.Added && Events.Count == 0)
            {
                Events.Add(new WordEvent(wordEvent.EventDate, WordEventType.Added));
            }

            Events.Add(wordEvent);
        }

        private bool EventsOverllap(WordEvent oldEvent, WordEvent newEvent)
        {
            if (oldEvent.EventType == WordEventType.Added)
            {
                return newEvent.EventDate < oldEvent.EventDate;
            }
            if (oldEvent.Translation != newEvent.Translation)
            {
                return false;
            }
            return newEvent.EventDate.AddMinutes(-5) < oldEvent.EventDate;
        }

        public void ResetHistory()
        {
            Events.Clear();
        }

        public void FilterTranslationEvents()
        {
            ArticleParser parser = new ArticleParser();
            WordArticle article = parser.Parse(name, description);
            HashSet<string> translations = new HashSet<string>(article.GetAllTranslations());
            for (int i = 0; i < Events.Count;)
            {
                WordEvent wordEvent = events[i];
                if (wordEvent.Translation == null || translations.Contains(wordEvent.Translation))
                {
                    i++;
                }
                else
                {
                    events.RemoveAt(i);
                }
            }
        }
    }
}
