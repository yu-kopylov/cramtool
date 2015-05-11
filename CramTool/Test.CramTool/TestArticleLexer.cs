using System.Collections.Generic;
using System.Linq;
using CramTool.Models;
using NUnit.Framework;

namespace Test.CramTool
{
    [TestFixture]
    public class TestArticleLexer
    {
        [Test]
        public void TestNewLines()
        {
            ArticleLexer parser = new ArticleLexer();
            List<Token> tokens;

            tokens = parser.Parse("\r\n");
            Assert.That(tokens.Select(t => t.Type).ToList(), Is.EqualTo(new List<TokenType> {TokenType.NewLine}));
            Assert.That(tokens.Select(t => t.Text).ToList(), Is.EqualTo(new List<string> {"\r\n"}));
            Assert.That(tokens.Select(t => t.Value).ToList(), Is.EqualTo(new List<string> {"\n"}));

            tokens = parser.Parse("\r\n\r\n");
            Assert.That(tokens.Select(t => t.Type).ToList(), Is.EqualTo(new List<TokenType> {TokenType.NewLine, TokenType.NewLine}));
            Assert.That(tokens.Select(t => t.Text).ToList(), Is.EqualTo(new List<string> {"\r\n", "\r\n"}));
            Assert.That(tokens.Select(t => t.Value).ToList(), Is.EqualTo(new List<string> {"\n", "\n"}));

            tokens = parser.Parse("abc\ncde");
            Assert.That(tokens.Select(t => t.Type).ToList(), Is.EqualTo(new List<TokenType> {TokenType.Translation, TokenType.NewLine, TokenType.Translation}));
            Assert.That(tokens.Select(t => t.Text).ToList(), Is.EqualTo(new List<string> {"abc", "\n", "cde"}));
            Assert.That(tokens.Select(t => t.Value).ToList(), Is.EqualTo(new List<string> {"abc", "\n", "cde"}));

            tokens = parser.Parse("abc\rcde");
            Assert.That(tokens.Select(t => t.Type).ToList(), Is.EqualTo(new List<TokenType> {TokenType.Translation, TokenType.NewLine, TokenType.Translation}));
            Assert.That(tokens.Select(t => t.Text).ToList(), Is.EqualTo(new List<string> {"abc", "\r", "cde"}));
            Assert.That(tokens.Select(t => t.Value).ToList(), Is.EqualTo(new List<string> {"abc", "\n", "cde"}));

            tokens = parser.Parse("abc\r\ncde");
            Assert.That(tokens.Select(t => t.Type).ToList(), Is.EqualTo(new List<TokenType> {TokenType.Translation, TokenType.NewLine, TokenType.Translation}));
            Assert.That(tokens.Select(t => t.Text).ToList(), Is.EqualTo(new List<string> {"abc", "\r\n", "cde"}));
            Assert.That(tokens.Select(t => t.Value).ToList(), Is.EqualTo(new List<string> {"abc", "\n", "cde"}));

            tokens = parser.Parse("\nabc\n");
            Assert.That(tokens.Select(t => t.Type).ToList(), Is.EqualTo(new List<TokenType> {TokenType.NewLine, TokenType.Translation, TokenType.NewLine}));
            Assert.That(tokens.Select(t => t.Text).ToList(), Is.EqualTo(new List<string> {"\n", "abc", "\n"}));
            Assert.That(tokens.Select(t => t.Value).ToList(), Is.EqualTo(new List<string> {"\n", "abc", "\n"}));
        }

        [Test]
        public void TestWordForms()
        {
            ArticleLexer parser = new ArticleLexer();
            List<Token> tokens;

            tokens = parser.Parse("#abc");
            Assert.That(tokens.Select(t => t.Type).ToList(), Is.EqualTo(new List<TokenType> {TokenType.WordForm}));
            Assert.That(tokens.Select(t => t.Value).ToList(), Is.EqualTo(new List<string> {"abc"}));

            tokens = parser.Parse("#abc\ncde");
            Assert.That(tokens.Select(t => t.Type).ToList(), Is.EqualTo(new List<TokenType> {TokenType.WordForm, TokenType.NewLine, TokenType.Translation}));
            Assert.That(tokens.Select(t => t.Value).ToList(), Is.EqualTo(new List<string> {"abc", "\n", "cde"}));

            tokens = parser.Parse("abc\n#cde");
            Assert.That(tokens.Select(t => t.Type).ToList(), Is.EqualTo(new List<TokenType> {TokenType.Translation, TokenType.NewLine, TokenType.WordForm}));
            Assert.That(tokens.Select(t => t.Value).ToList(), Is.EqualTo(new List<string> {"abc", "\n", "cde"}));

            tokens = parser.Parse("#abc\n#cde");
            Assert.That(tokens.Select(t => t.Type).ToList(), Is.EqualTo(new List<TokenType> {TokenType.WordForm, TokenType.NewLine, TokenType.WordForm}));
            Assert.That(tokens.Select(t => t.Value).ToList(), Is.EqualTo(new List<string> {"abc", "\n", "cde"}));
        }

        [Test]
        public void TestTranslations()
        {
            ArticleLexer parser = new ArticleLexer();
            List<Token> tokens;

            tokens = parser.Parse("abc");
            Assert.That(tokens.Select(t => t.Type).ToList(), Is.EqualTo(new List<TokenType> {TokenType.Translation}));
            Assert.That(tokens.Select(t => t.Value).ToList(), Is.EqualTo(new List<string> {"abc"}));

            tokens = parser.Parse("abc\n\ncde");
            Assert.That(tokens.Select(t => t.Type).ToList(), Is.EqualTo(new List<TokenType> {TokenType.Translation, TokenType.NewLine, TokenType.NewLine, TokenType.Translation}));
            Assert.That(tokens.Select(t => t.Value).ToList(), Is.EqualTo(new List<string> {"abc", "\n", "\n", "cde"}));
        }

        [Test]
        public void TestExamples()
        {
            ArticleLexer parser = new ArticleLexer();
            List<Token> tokens;

            tokens = parser.Parse("//abc");
            Assert.That(tokens.Select(t => t.Type).ToList(), Is.EqualTo(new List<TokenType> {TokenType.Example}));
            Assert.That(tokens.Select(t => t.Value).ToList(), Is.EqualTo(new List<string> {"abc"}));

            tokens = parser.Parse("//abc\ncde");
            Assert.That(tokens.Select(t => t.Type).ToList(), Is.EqualTo(new List<TokenType> {TokenType.Example, TokenType.NewLine, TokenType.Translation}));
            Assert.That(tokens.Select(t => t.Value).ToList(), Is.EqualTo(new List<string> {"abc", "\n", "cde"}));

            tokens = parser.Parse("abc\n//cde");
            Assert.That(tokens.Select(t => t.Type).ToList(), Is.EqualTo(new List<TokenType> {TokenType.Translation, TokenType.NewLine, TokenType.Example}));
            Assert.That(tokens.Select(t => t.Value).ToList(), Is.EqualTo(new List<string> {"abc", "\n", "cde"}));

            tokens = parser.Parse("//abc\n//cde");
            Assert.That(tokens.Select(t => t.Type).ToList(), Is.EqualTo(new List<TokenType> {TokenType.Example, TokenType.NewLine, TokenType.Example}));
            Assert.That(tokens.Select(t => t.Value).ToList(), Is.EqualTo(new List<string> {"abc", "\n", "cde"}));
        }
    }
}