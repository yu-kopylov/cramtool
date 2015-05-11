using CramTool.Models;
using NUnit.Framework;

namespace Test.CramTool
{
    [TestFixture]
    public class TestArticleParser
    {
        [Test]
        public void Test()
        {
            ArticleParser parser = new ArticleParser();
            WordArticle article = parser.Parse("a", "b");
            Assert.That(article.FormGroups.Count, Is.EqualTo(1));
            Assert.That(article.FormGroups[0].Forms, Is.EquivalentTo(new string[] {"a"}));
            Assert.That(article.FormGroups[0].TranslationGroups.Count, Is.EqualTo(1));
            Assert.That(article.FormGroups[0].TranslationGroups[0].Translations, Is.EquivalentTo(new string[] {"b"}));
            Assert.That(article.Format(), Is.EqualTo("#a\nb\n"));
        }
    }
}