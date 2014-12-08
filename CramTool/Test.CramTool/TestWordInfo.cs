using System;
using System.Collections.Generic;
using CramTool.Models;
using NUnit.Framework;

namespace Test.CramTool
{
    [TestFixture]
    public class TestWordInfo
    {
        [Test]
        public void Test()
        {
            DateTime dateAdded = DateTime.UtcNow.AddDays(-100);
            DateTime dateRepeated1 = dateAdded + new TimeSpan(1, 0, 0, 0);
            DateTime dateForgotten = dateAdded + new TimeSpan(2, 0, 0, 0);
            DateTime dateRepeated2 = dateAdded + new TimeSpan(2, 0, 1, 0);
            DateTime dateRepeated3 = dateAdded + new TimeSpan(3, 0, 0, 0);
            DateTime dateLearned = dateAdded + new TimeSpan(4, 0, 0, 0);
            DateTime dateVerified = dateAdded + new TimeSpan(10, 0, 0, 0);

            Word word = new Word();
            word.Name = "name";
            word.Description = "descr";
            word.Tags = "tag1, tag2";
            word.Events.Add(new WordEvent(dateAdded, WordEventType.Added));
            word.Events.Add(new WordEvent(dateRepeated1, WordEventType.Remembered));
            word.Events.Add(new WordEvent(dateForgotten, WordEventType.Forgotten));
            word.Events.Add(new WordEvent(dateRepeated2, WordEventType.Remembered));

            WordList wordList = new WordList();
            wordList.Populate(new List<Word> {word});

            WordInfo wordInfo = WordInfo.Create(wordList, word);
            Assert.That(wordInfo.LastEvent.LastStateChange, Is.EqualTo(dateForgotten));
            Assert.That(wordInfo.State, Is.EqualTo(WordState.Studied));
            Assert.That(wordInfo.IsStudied);
            Assert.That(!wordInfo.IsLearned);
            Assert.That(!wordInfo.IsVerified);

            word.Events.Add(new WordEvent(dateRepeated3, WordEventType.Remembered));
            wordInfo.Update();
            Assert.That(wordInfo.LastEvent.LastStateChange, Is.EqualTo(dateRepeated3));
            Assert.That(wordInfo.State, Is.EqualTo(WordState.Repeated));
            Assert.That(wordInfo.IsStudied);
            Assert.That(!wordInfo.IsLearned);
            Assert.That(!wordInfo.IsVerified);

            word.Events.Add(new WordEvent(dateLearned, WordEventType.Remembered));
            wordInfo.Update();
            Assert.That(wordInfo.LastEvent.LastStateChange, Is.EqualTo(dateLearned));
            Assert.That(wordInfo.State, Is.EqualTo(WordState.Learned));
            Assert.That(wordInfo.IsStudied);
            Assert.That(wordInfo.IsLearned);
            Assert.That(!wordInfo.IsVerified);

            word.Events.Add(new WordEvent(dateVerified, WordEventType.Remembered));
            wordInfo.Update();
            Assert.That(wordInfo.LastEvent.LastStateChange, Is.EqualTo(dateVerified));
            Assert.That(wordInfo.State, Is.EqualTo(WordState.Verified));
            Assert.That(wordInfo.IsStudied);
            Assert.That(wordInfo.IsLearned);
            Assert.That(wordInfo.IsVerified);
        }
    }
}