using System.IO;
using System.Linq;
using CramTool.Formats;
using CramTool.Models;
using NUnit.Framework;

namespace Test.CramTool
{
    [TestFixture]
    public class TestWordList
    {
        [Test]
        public void Test()
        {
            WordList wordList = new WordList();

            wordList.Add("apple", "a round fruit", null);
            wordList.Add("oRRange", "a round citrus fruit", "fruit");

            Assert.That(wordList.GetAllWords().Select(w => w.Word.Name), Is.EquivalentTo(new[] { "apple", "oRRange" }));
            Assert.That(wordList.GetAllTags(), Is.EquivalentTo(new[] { "fruit" }));
            Assert.That(wordList.GetWordsWithTag("fruit").Select(w => w.Word.Name), Is.EquivalentTo(new[] { "oRRange" }));

            wordList.Update("apple", "apple", "a round fruit", "fruit");
            wordList.Update("oRRange", "orange", "a round citrus fruit", "fruit");

            Assert.That(wordList.GetAllWords().Select(w => w.Word.Name), Is.EquivalentTo(new[] {"apple", "orange"}));
            Assert.That(wordList.GetAllTags(), Is.EquivalentTo(new[] {"fruit"}));
            Assert.That(wordList.GetWordsWithTag("fruit").Select(w => w.Word.Name), Is.EquivalentTo(new[] { "apple", "orange" }));

            wordList.MarkTranslation("a round fruit", WordEventType.Remembered);
            WordEvent translationRemenberedEvent = wordList.GetAllTranslations().Single(t => t.Translation == "a round fruit").Events.First().WordEvent;
            Assert.That(translationRemenberedEvent.EventType, Is.EqualTo(WordEventType.Remembered));
            Assert.That(translationRemenberedEvent.Translation, Is.EqualTo("a round fruit"));

            MemoryStream mem = new MemoryStream();
            WordListFileParser fileParser = new WordListFileParser();
            fileParser.GenerateZip(wordList, mem);

            WordList wordList2 = fileParser.ParseZip(mem);
            Assert.That(wordList2.GetAllWords().Select(w => w.Word.Name), Is.EquivalentTo(new[] { "apple", "orange" }));
            Assert.That(wordList2.GetAllTags(), Is.EquivalentTo(new[] { "fruit" }));
            Assert.That(wordList2.GetWordsWithTag("fruit").Select(w => w.Word.Name), Is.EquivalentTo(new[] { "apple", "orange" }));

            translationRemenberedEvent = wordList2.GetAllTranslations().Single(t => t.Translation == "a round fruit").Events.First().WordEvent;
            Assert.That(translationRemenberedEvent.EventType, Is.EqualTo(WordEventType.Remembered));
            Assert.That(translationRemenberedEvent.Translation, Is.EqualTo("a round fruit"));
        }
    }
}