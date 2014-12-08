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
            DateTime utcNow = DateTime.UtcNow;

            Word word = new Word();
            word.Name = "name";
            word.Description = "descr";
            word.Tags = "tag1, tag2";
            word.Events.Add(new WordEvent(utcNow.AddDays(-20), WordEventType.Added));
            word.Events.Add(new WordEvent(utcNow.AddDays(-15), WordEventType.Remembered));

            WordList wordList = new WordList();
            wordList.Populate(new List<Word> {word});

            WordInfo wordInfo = WordInfo.Create(wordList, word);
            Assert.That(wordInfo.LastEvent.LastStateChange, Is.EqualTo(utcNow.AddDays(-15)));
            Assert.That(wordInfo.State, Is.EqualTo(WordState.Repeated));
            Assert.That(wordInfo.IsStudied);
            Assert.That(!wordInfo.IsLearned);
            Assert.That(!wordInfo.IsVerified);

            word.Events.Add(new WordEvent(utcNow.AddDays(-10), WordEventType.Remembered));
            wordInfo.Update();
            Assert.That(wordInfo.LastEvent.LastStateChange, Is.EqualTo(utcNow.AddDays(-10)));
            Assert.That(wordInfo.State, Is.EqualTo(WordState.Learned));
            Assert.That(wordInfo.IsStudied);
            Assert.That(wordInfo.IsLearned);
            Assert.That(!wordInfo.IsVerified);

            word.Events.Add(new WordEvent(utcNow.AddDays(-1), WordEventType.Remembered));
            wordInfo.Update();
            Assert.That(wordInfo.LastEvent.LastStateChange, Is.EqualTo(utcNow.AddDays(-1)));
            Assert.That(wordInfo.State, Is.EqualTo(WordState.Verified));
            Assert.That(wordInfo.IsStudied);
            Assert.That(wordInfo.IsLearned);
            Assert.That(wordInfo.IsVerified);
        }
    }
}