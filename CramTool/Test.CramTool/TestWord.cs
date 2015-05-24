using System;
using CramTool.Models;
using NUnit.Framework;

namespace Test.CramTool
{
    [TestFixture]
    public class TestWord
    {
        [Test]
        public void TestEventOrdering1()
        {
            Word word = new Word();

            word.Mark(WordEventType.Added);
            Assert.That(word.Events.Count, Is.EqualTo(1));
            Assert.That(word.Events[0].EventType, Is.EqualTo(WordEventType.Added));

            word.Mark(WordEventType.Remembered);
            Assert.That(word.Events.Count, Is.EqualTo(2));
            Assert.That(word.Events[0].EventType, Is.EqualTo(WordEventType.Added));
            Assert.That(word.Events[1].EventType, Is.EqualTo(WordEventType.Remembered));
            Assert.That(word.Events[0].EventDate, Is.LessThanOrEqualTo(word.Events[1].EventDate));

            word.Mark(WordEventType.Forgotten);
            Assert.That(word.Events.Count, Is.EqualTo(2));
            Assert.That(word.Events[0].EventType, Is.EqualTo(WordEventType.Added));
            Assert.That(word.Events[1].EventType, Is.EqualTo(WordEventType.Forgotten));
            Assert.That(word.Events[0].EventDate, Is.LessThanOrEqualTo(word.Events[1].EventDate));
        }

        [Test]
        public void TestEventOrdering2()
        {
            Word word = new Word();

            word.Events.Add(new WordEvent(DateTime.UtcNow.AddDays(-2), WordEventType.Added));
            word.Events.Add(new WordEvent(DateTime.UtcNow.AddDays(-1), WordEventType.Remembered));

            word.Mark(WordEventType.Forgotten);
            Assert.That(word.Events.Count, Is.EqualTo(3));
            Assert.That(word.Events[0].EventType, Is.EqualTo(WordEventType.Added));
            Assert.That(word.Events[1].EventType, Is.EqualTo(WordEventType.Remembered));
            Assert.That(word.Events[2].EventType, Is.EqualTo(WordEventType.Forgotten));
            Assert.That(word.Events[0].EventDate, Is.LessThanOrEqualTo(word.Events[1].EventDate));
            Assert.That(word.Events[1].EventDate, Is.LessThanOrEqualTo(word.Events[2].EventDate));
        }

        [Test]
        public void TestEventOrderingWithFutureDates()
        {
            Word word = new Word();

            word.Events.Add(new WordEvent(DateTime.UtcNow.AddDays(1), WordEventType.Added));
            word.Events.Add(new WordEvent(DateTime.UtcNow.AddDays(2), WordEventType.Remembered));

            word.Mark(WordEventType.Forgotten);
            Assert.That(word.Events.Count, Is.EqualTo(2));
            Assert.That(word.Events[0].EventType, Is.EqualTo(WordEventType.Added));
            Assert.That(word.Events[1].EventType, Is.EqualTo(WordEventType.Forgotten));
            Assert.That(word.Events[0].EventDate, Is.LessThanOrEqualTo(word.Events[1].EventDate));
        }

        [Test]
        public void TestTranslationEvents()
        {
            Word word = new Word();

            word.Events.Add(new WordEvent(DateTime.UtcNow.AddDays(-1), WordEventType.Added));
            word.Events.Add(new WordEvent(DateTime.UtcNow.AddDays(1), WordEventType.Remembered));

            word.MarkTranslation(WordEventType.Forgotten, "a");
            Assert.That(word.Events.Count, Is.EqualTo(3));
            Assert.That(word.Events[0].EventType, Is.EqualTo(WordEventType.Added));
            Assert.That(word.Events[1].EventType, Is.EqualTo(WordEventType.Remembered));
            Assert.That(word.Events[2].EventType, Is.EqualTo(WordEventType.Forgotten));
            Assert.That(word.Events[0].Translation, Is.Null);
            Assert.That(word.Events[1].Translation, Is.Null);
            Assert.That(word.Events[2].Translation, Is.EqualTo("a"));

            word.MarkTranslation(WordEventType.Remembered, "b");
            Assert.That(word.Events.Count, Is.EqualTo(4));
            Assert.That(word.Events[0].EventType, Is.EqualTo(WordEventType.Added));
            Assert.That(word.Events[1].EventType, Is.EqualTo(WordEventType.Remembered));
            Assert.That(word.Events[2].EventType, Is.EqualTo(WordEventType.Forgotten));
            Assert.That(word.Events[3].EventType, Is.EqualTo(WordEventType.Remembered));
            Assert.That(word.Events[0].Translation, Is.Null);
            Assert.That(word.Events[1].Translation, Is.Null);
            Assert.That(word.Events[2].Translation, Is.EqualTo("a"));
            Assert.That(word.Events[3].Translation, Is.EqualTo("b"));

            word.MarkTranslation(WordEventType.Remembered, "a");
            Assert.That(word.Events.Count, Is.EqualTo(4));
            Assert.That(word.Events[0].EventType, Is.EqualTo(WordEventType.Added));
            Assert.That(word.Events[1].EventType, Is.EqualTo(WordEventType.Remembered));
            Assert.That(word.Events[2].EventType, Is.EqualTo(WordEventType.Remembered));
            Assert.That(word.Events[3].EventType, Is.EqualTo(WordEventType.Remembered));
            Assert.That(word.Events[0].Translation, Is.Null);
            Assert.That(word.Events[1].Translation, Is.Null);
            Assert.That(word.Events[2].Translation, Is.EqualTo("b"));
            Assert.That(word.Events[3].Translation, Is.EqualTo("a"));
        }
    }
}