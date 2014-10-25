using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            var lastEvent = Events.LastOrDefault();
            while (lastEvent != null && EventsOverllap(lastEvent, wordEvent))
            {
                Events.RemoveAt(Events.Count - 1);
                lastEvent = Events.LastOrDefault();
            }

            if (eventType != WordEventType.Added && Events.Count == 0)
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
            return newEvent.EventDate.AddMinutes(-5) < oldEvent.EventDate;
        }

        public void ResetHistory()
        {
            Events.Clear();
        }
    }
}
