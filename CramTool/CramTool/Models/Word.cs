using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CramTool.Models
{
    public class Word : INotifyPropertyChanged
    {
        private string name;
        private string description;
        private readonly ObservableCollection<WordEvent> events = new ObservableCollection<WordEvent>();

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

        public ObservableCollection<WordEvent> Events
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
            var lastEvent = Events.OrderByDescending(e => e.EventDate).FirstOrDefault(e => e.IsLearningEvent);
            DateTime utcNow = DateTime.UtcNow;
            if (lastEvent != null && lastEvent.EventDate >= utcNow.AddMinutes(-5))
            {
                Events.Remove(lastEvent);
            }

            WordEvent wordEvent = new WordEvent(utcNow, eventType);
            Events.Insert(0, wordEvent);
        }

        public void ResetHistory()
        {
            Events.Clear();
        }
    }
}
