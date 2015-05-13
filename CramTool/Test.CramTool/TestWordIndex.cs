using System;
using System.Collections.Generic;
using CramTool.Models;
using NUnit.Framework;

namespace Test.CramTool
{
    [TestFixture]
    public class TestWordIndex
    {
        [Test]
        public void TestSingleAttribute()
        {
            var index = new WordIndex();

            Assert.That(index.GetAttributes(), Is.EquivalentTo(new string[0]));

            index.Add("w2", "attr1");
            index.Add("w1", "attr1");
            index.Add("w3", "attr1");

            Assert.Throws<ArgumentException>(() => index.Add("w1", "attr1"));

            Assert.That(index.GetAttributes(), Is.EquivalentTo(new string[] {"attr1"}));
            Assert.That(index.GetWordNames("attr1"), Is.EquivalentTo(new string[] {"w1", "w2", "w3"}));

            Assert.Throws<KeyNotFoundException>(() => index.Remove("wX", "attr1"));
            Assert.Throws<KeyNotFoundException>(() => index.Remove("w1", "attrX"));

            index.Remove("w2", "attr1");
            Assert.That(index.GetAttributes(), Is.EquivalentTo(new string[] {"attr1"}));
            Assert.That(index.GetWordNames("attr1"), Is.EquivalentTo(new string[] {"w1", "w3"}));


            index.Remove("w1", "attr1");
            index.Remove("w3", "attr1");
            Assert.That(index.GetAttributes(), Is.EquivalentTo(new string[0]));
            Assert.That(index.GetWordNames("attr1"), Is.EquivalentTo(new string[0]));
        }

        [Test]
        public void TestSingleWord()
        {
            var index = new WordIndex();

            Assert.That(index.GetAttributes(), Is.EquivalentTo(new string[0]));

            index.Add("w1", "attr2");
            index.Add("w1", "attr1");
            index.Add("w1", "attr3");

            Assert.Throws<ArgumentException>(() => index.Add("w1", "attr1"));

            Assert.That(index.GetAttributes(), Is.EquivalentTo(new string[] {"attr1", "attr2", "attr3"}));
            Assert.That(index.GetWordNames("attr1"), Is.EquivalentTo(new string[] {"w1"}));
            Assert.That(index.GetWordNames("attr2"), Is.EquivalentTo(new string[] {"w1"}));
            Assert.That(index.GetWordNames("attr3"), Is.EquivalentTo(new string[] {"w1"}));

            Assert.Throws<KeyNotFoundException>(() => index.Remove("wX", "attr1"));
            Assert.Throws<KeyNotFoundException>(() => index.Remove("w1", "attrX"));

            index.Remove("w1", "attr2");
            Assert.That(index.GetAttributes(), Is.EquivalentTo(new string[] {"attr1", "attr3"}));
            Assert.That(index.GetWordNames("attr1"), Is.EquivalentTo(new string[] {"w1"}));
            Assert.That(index.GetWordNames("attr2"), Is.EquivalentTo(new string[0]));
            Assert.That(index.GetWordNames("attr3"), Is.EquivalentTo(new string[] {"w1"}));


            index.Remove("w1", "attr1");
            index.Remove("w1", "attr3");
            Assert.That(index.GetAttributes(), Is.EquivalentTo(new string[0]));
            Assert.That(index.GetWordNames("attr1"), Is.EquivalentTo(new string[0]));
            Assert.That(index.GetWordNames("attr2"), Is.EquivalentTo(new string[0]));
            Assert.That(index.GetWordNames("attr3"), Is.EquivalentTo(new string[0]));
        }

        [Test]
        public void TestUpdate()
        {
            var index = new WordIndex();

            index.Update(null, new List<string>(), "w1", new List<string> {"attr1"});

            Assert.That(index.GetAttributes(), Is.EquivalentTo(new string[] {"attr1"}));
            Assert.That(index.GetWordNames("attr1"), Is.EquivalentTo(new string[] {"w1"}));

            index.Update("w1", new List<string> {"attr1"}, "w1", new List<string> {"attr2", "attr3"});

            Assert.That(index.GetAttributes(), Is.EquivalentTo(new string[] {"attr2", "attr3"}));
            Assert.That(index.GetWordNames("attr1"), Is.EquivalentTo(new string[0]));
            Assert.That(index.GetWordNames("attr2"), Is.EquivalentTo(new string[] {"w1"}));
            Assert.That(index.GetWordNames("attr3"), Is.EquivalentTo(new string[] {"w1"}));

            index.Update("w1", new List<string> {"attr2", "attr3"}, "w2", new List<string> {"attr3", "attr1"});

            Assert.That(index.GetAttributes(), Is.EquivalentTo(new string[] {"attr1", "attr3"}));
            Assert.That(index.GetWordNames("attr1"), Is.EquivalentTo(new string[] {"w2"}));
            Assert.That(index.GetWordNames("attr2"), Is.EquivalentTo(new string[0]));
            Assert.That(index.GetWordNames("attr3"), Is.EquivalentTo(new string[] {"w2"}));
        }
    }
}