using System;
using System.Collections.Generic;
using System.Linq;
using CramTool.Formats;
using CramTool.Models;
using NUnit.Framework;

namespace Test.CramTool
{
    [TestFixture]
    public class TestWordListXmlConverter
    {
        [Test]
        public void TestReversedEventOrder()
        {
            WordList wordList = new WordList();
            Word word = wordList.Add("test", "test", "");
            wordList.ResetHistory();

            DateTime utcNow = DateTime.UtcNow;
            word.Events.Add(new WordEvent(utcNow.AddDays(-1), WordEventType.Remembered));
            word.Events.Add(new WordEvent(utcNow.AddDays(-2), WordEventType.Forgotten));
            word.Events.Add(new WordEvent(utcNow.AddDays(-2), WordEventType.Added));

            var wordListXml = WordListXmlConverter.ConvertToXml(wordList);
            WordList wordList2 = WordListXmlConverter.ConvertToObject(wordListXml);

            List<WordInfo> words2 = wordList2.GetAllWords().ToList();
            Assert.That(words2.Count, Is.EqualTo(1));

            Word word2 = words2[0].Word;

            Assert.That(word2.Events.Count, Is.EqualTo(3));
            Assert.That(word2.Events[0].EventType, Is.EqualTo(WordEventType.Added));
            Assert.That(word2.Events[1].EventType, Is.EqualTo(WordEventType.Forgotten));
            Assert.That(word2.Events[2].EventType, Is.EqualTo(WordEventType.Remembered));
            Assert.That(word2.Events[0].EventDate, Is.EqualTo(utcNow.AddDays(-2)));
            Assert.That(word2.Events[1].EventDate, Is.EqualTo(utcNow.AddDays(-2)));
            Assert.That(word2.Events[2].EventDate, Is.EqualTo(utcNow.AddDays(-1)));
        }

        [Test]
        public void TestMixedEventOrder()
        {
            WordList wordList = new WordList();
            Word word = wordList.Add("test", "test", "");
            wordList.ResetHistory();

            DateTime utcNow = DateTime.UtcNow;
            word.Events.Add(new WordEvent(utcNow.AddDays(-1), WordEventType.Remembered));
            word.Events.Add(new WordEvent(utcNow.AddDays(-3), WordEventType.Forgotten));
            word.Events.Add(new WordEvent(utcNow.AddDays(-2), WordEventType.Added));

            var wordListXml = WordListXmlConverter.ConvertToXml(wordList);
            WordList wordList2 = WordListXmlConverter.ConvertToObject(wordListXml);

            List<WordInfo> words2 = wordList2.GetAllWords().ToList();
            Assert.That(words2.Count, Is.EqualTo(1));

            Word word2 = words2[0].Word;

            Assert.That(word2.Events.Count, Is.EqualTo(3));
            Assert.That(word2.Events[0].EventType, Is.EqualTo(WordEventType.Added));
            Assert.That(word2.Events[1].EventType, Is.EqualTo(WordEventType.Forgotten));
            Assert.That(word2.Events[2].EventType, Is.EqualTo(WordEventType.Remembered));
            Assert.That(word2.Events[0].EventDate, Is.EqualTo(utcNow.AddDays(-3)));
            Assert.That(word2.Events[1].EventDate, Is.EqualTo(utcNow.AddDays(-3)));
            Assert.That(word2.Events[2].EventDate, Is.EqualTo(utcNow.AddDays(-1)));
        }
    }
}