using CramTool.Models;
using NUnit.Framework;

namespace Test.CramTool
{
    [TestFixture]
    public class TestTagParser
    {
        [Test]
        public void Test()
        {
            Assert.That(TagParser.ReformatTags(null), Is.EqualTo(""));
            Assert.That(TagParser.ReformatTags(""), Is.EqualTo(""));
            Assert.That(TagParser.ReformatTags(","), Is.EqualTo(""));
            Assert.That(TagParser.ReformatTags(";"), Is.EqualTo(""));
            Assert.That(TagParser.ReformatTags(",\t \t,"), Is.EqualTo(""));
            Assert.That(TagParser.ReformatTags(";\t \t;"), Is.EqualTo(""));
            Assert.That(TagParser.ReformatTags("; , ;"), Is.EqualTo(""));
            
            Assert.That(TagParser.ReformatTags("A,B"), Is.EqualTo("A, B"));
            Assert.That(TagParser.ReformatTags("A;B"), Is.EqualTo("A, B"));
            
            Assert.That(TagParser.ReformatTags(" A , \tB\t "), Is.EqualTo("A, B"));
            
            Assert.That(TagParser.ReformatTags("B,A"), Is.EqualTo("A, B"));
            Assert.That(TagParser.ReformatTags(" B ,A"), Is.EqualTo("A, B"));
            Assert.That(TagParser.ReformatTags("B, A "), Is.EqualTo("A, B"));
            
            Assert.That(TagParser.ReformatTags("a, A"), Is.EqualTo("a, A"));
            Assert.That(TagParser.ReformatTags("A, a"), Is.EqualTo("a, A"));
            
            Assert.That(TagParser.ReformatTags("a1, A2"), Is.EqualTo("a1, A2"));
            Assert.That(TagParser.ReformatTags("A1, a2"), Is.EqualTo("A1, a2"));
        }
    }
}