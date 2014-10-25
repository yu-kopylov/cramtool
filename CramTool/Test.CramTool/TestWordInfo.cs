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
            word.Events.Add(new WordEvent(utcNow.AddDays(-2), WordEventType.Added));
            word.Events.Add(new WordEvent(utcNow.AddDays(-1), WordEventType.Remembered));

            WordList wordList = new WordList();
            wordList.Populate(new List<Word> {word});

            WordInfo wordInfo = WordInfo.Create(wordList, word);
            Assert.That(wordInfo.DateAdded, Is.EqualTo(utcNow.AddDays(-2)));
            Assert.That(wordInfo.LastEventDate, Is.EqualTo(utcNow.AddDays(-1)));
        }
    }
}