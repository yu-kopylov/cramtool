using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CramTool.Models
{
    public class ArticleParser
    {
        private readonly ArticleLexer lexer = new ArticleLexer();

        public WordArticle Parse(string word, string description)
        {
            List<Token> tokens = lexer.Parse(description);

            WordArticle article = new WordArticle();

            WordFormGroup formGroup = new WordFormGroup();
            article.FormGroups.Add(formGroup);
            formGroup.Forms.Add(word);

            WordTranslationGroup translationGroup = null;

            TokenType lastTokenType1 = TokenType.WordForm;
            TokenType lastTokenType2 = TokenType.NewLine;

            foreach (Token token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Example:
                        if (translationGroup == null)
                        {
                            translationGroup = new WordTranslationGroup();
                            formGroup.TranslationGroups.Add(translationGroup);
                        }
                        else
                        {
                            translationGroup.Examples.Add(token.Value);
                        }
                        break;
                    case TokenType.Translation:
                        if (translationGroup == null || translationGroup.Examples.Any() || (lastTokenType1 == TokenType.NewLine && lastTokenType2 == TokenType.NewLine))
                        {
                            translationGroup = new WordTranslationGroup();
                            formGroup.TranslationGroups.Add(translationGroup);
                        }
                        translationGroup.Translations.Add(token.Value);
                        break;
                    case TokenType.WordForm:
                        if (formGroup.TranslationGroups.Any())
                        {
                            formGroup = new WordFormGroup();
                            article.FormGroups.Add(formGroup);
                            translationGroup = null;
                        }
                        formGroup.Forms.Add(token.Value);
                        break;
                }
                lastTokenType2 = lastTokenType1;
                lastTokenType1 = token.Type;
            }

            return article;
        }
    }
}