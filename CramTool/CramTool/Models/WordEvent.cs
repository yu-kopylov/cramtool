using System;
using System.Diagnostics.Contracts;

namespace CramTool.Models
{
    public class WordEvent
    {
        public DateTime EventDate { get; private set; }
        public WordEventType EventType { get; private set; }
        public string Translation { get; private set; }

        public WordEvent(DateTime eventDate, WordEventType eventType)
        {
            Contract.Assert(eventDate.Kind == DateTimeKind.Utc);
            EventDate = eventDate;
            EventType = eventType;
            Translation = null;
        }

        public WordEvent(DateTime eventDate, WordEventType eventType, string translation)
        {
            Contract.Assert(eventDate.Kind == DateTimeKind.Utc);
            EventDate = eventDate;
            EventType = eventType;
            Translation = translation;
        }

        public DateTime LocalDate
        {
            get { return EventDate.ToLocalTime(); }
        }

        public bool IsLearningEvent
        {
            get { return EventType == WordEventType.Remembered || EventType == WordEventType.Forgotten; }
        }
    }
}