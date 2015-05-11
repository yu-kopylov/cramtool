using System.Collections.Generic;

namespace CramTool.Models
{
    public class ArticleLexer
    {
        private string sourceText;
        private int sourceLength;

        private int currentOffset;
        private Token currentToken;

        private List<Token> result;

        public List<Token> Parse(string text)
        {
            sourceText = text ?? "";
            sourceLength = sourceText.Length;

            currentOffset = 0;
            currentToken = null;

            result = new List<Token>();

            while (currentOffset < sourceLength)
            {
                if (Matches("\r\n"))
                {
                    StartToken(TokenType.NewLine, 2);
                }
                else if (Matches("\n") || Matches("\r"))
                {
                    StartToken(TokenType.NewLine, 1);
                }
                else if (IsAtLineStart() && Matches("#"))
                {
                    StartToken(TokenType.WordForm, 1);
                }
                else if (IsAtLineStart() && Matches("//"))
                {
                    StartToken(TokenType.Example, 2);
                }
                else if (IsAtLineStart())
                {
                    StartToken(TokenType.Translation, 1);
                }
                else
                {
                    ExtendToken();
                }
            }

            return result;
        }

        private void StartToken(TokenType tokenType, int length)
        {
            currentToken = new Token(sourceText, tokenType, currentOffset, length);
            result.Add(currentToken);
            currentOffset += length;
        }

        private void ExtendToken()
        {
            currentToken.ExtendTo(currentOffset);
            currentOffset++;
        }

        private bool IsAtLineStart()
        {
            return currentToken == null || currentToken.Type == TokenType.NewLine;
        }

        private bool Matches(string key)
        {
            int keyLength = key.Length;
            if (currentOffset + keyLength > sourceLength)
            {
                return false;
            }
            return sourceText.Substring(currentOffset, keyLength) == key;
        }
    }

    public class Token
    {
        public string SourceText { get; private set; }
        public TokenType Type { get; private set; }
        public int Offset { get; set; }
        public int Length { get; set; }

        public Token(string sourceText, TokenType type, int offset, int length)
        {
            SourceText = sourceText;
            Type = type;
            Offset = offset;
            Length = length;
        }

        public string Text
        {
            get { return SourceText.Substring(Offset, Length); }
        }

        public string Value
        {
            get
            {
                if (Type == TokenType.WordForm)
                {
                    return SourceText.Substring(Offset + 1, Length - 1);
                }
                if (Type == TokenType.Example)
                {
                    return SourceText.Substring(Offset + 2, Length - 2);
                }
                if (Type == TokenType.NewLine)
                {
                    return "\n";
                }
                return Text;
            }
        }

        public void ExtendTo(int endOffset)
        {
            Length = endOffset - Offset + 1;
        }
    }

    public enum TokenType
    {
        WordForm,
        Translation,
        Example,
        NewLine
    }
}