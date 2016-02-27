using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using CramTool.Models;

namespace CramTool.Formats.Html
{
    public class HtmlGenerator
    {
        private const string CssWordArticle = "word-article";
        private const string CssWordForm = "word-form";
        private const string CssWordTranslation = "word-translation";
        private const string CssWordExample = "word-example";
        private const string CssWordRef = "word-ref";

        private const string WordIdPrefix = "word";

        public byte[] Generate(Models.WordList wordList)
        {
            MemoryStream mem = new MemoryStream();

            Encoding encoding = new UTF8Encoding(false);

            using (StreamWriter streamWriter = new StreamWriter(mem, encoding))
            {
                using (HtmlTextWriter writer = new HtmlTextWriter(streamWriter))
                {
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    writer.WriteLine("<!DOCTYPE html>");
                    writer.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
                    writer.WriteLine("<head>");
                    WriteStyleSection(writer);
                    writer.WriteLine("</head>");
                    writer.WriteLine("<body>");

                    foreach (WordForm wordForm in wordList.GetAllForms().OrderBy(wf => wf.Title))
                    {
                        WriteWordForm(writer, wordForm);
                    }

                    writer.WriteLine("</body>");
                    writer.WriteLine("</html>");
                    writer.Flush();
                }
            }

            return mem.ToArray();
        }

        private void WriteWordForm(HtmlTextWriter writer, WordForm wordForm)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, CssWordArticle);
            if (wordForm.Name == wordForm.WordInfo.Word.Name)
            {
                writer.AddAttribute("id", CreateId(WordIdPrefix, wordForm.WordInfo.Word.Name));
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, CssWordForm);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.WriteLine(wordForm.Title);

            writer.RenderEndTag();

            if (wordForm.Name != wordForm.WordInfo.Word.Name)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssWordRef);
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.Write("see: ");

                writer.AddAttribute(HtmlTextWriterAttribute.Href, "#" + CreateId(WordIdPrefix, wordForm.WordInfo.Word.Name));
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(wordForm.WordInfo.Word.Name);
                writer.RenderEndTag();

                writer.RenderEndTag();
            }
            else
            {
                ArticleLexer parser = new ArticleLexer();
                List<Token> tokens = parser.Parse(wordForm.WordInfo.Word.Description);
                bool afterNewLine = false;
                foreach (Token token in tokens)
                {
                    if (token.Type == TokenType.NewLine)
                    {
                        if (afterNewLine)
                        {
                            writer.WriteLine("<br/>");
                        }
                        afterNewLine = true;
                    }
                    else
                    {
                        afterNewLine = false;
                        WriteToken(writer, token);
                    }
                }
            }

            writer.RenderEndTag();
        }

        private void WriteToken(HtmlTextWriter writer, Token token)
        {
            string divClass = null;

            if (token.Type == TokenType.WordForm)
            {
                divClass = CssWordForm;
            }

            if (token.Type == TokenType.Translation)
            {
                divClass = CssWordTranslation;
            }

            if (token.Type == TokenType.Example)
            {
                divClass = CssWordExample;
            }

            if (divClass != null)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, divClass);
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.WriteLine(token.Value);
            writer.RenderEndTag();
        }

        private void WriteStyleSection(HtmlTextWriter writer)
        {
            writer.WriteLine("<style>");

            using (Stream stream = typeof (HtmlGenerator).Assembly.GetManifestResourceStream(typeof (HtmlGenerator), "style.css"))
            {
                using (TextReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string content = reader.ReadToEnd();
                    writer.WriteLine(content);
                }
            }

            writer.WriteLine("</style>");
        }

        private string CreateId(string prefix, string name)
        {
            StringBuilder sb = new StringBuilder(prefix.Length + name.Length + 1);
            sb.Append(prefix);
            sb.Append("-");
            foreach (char c in name)
            {
                if (c == '-')
                {
                    sb.Append("--");
                }
                else if (IsValidIdCharacter(c))
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append("-");
                    sb.AppendFormat("{0:X4}", (uint)c);
                }
            }
            return sb.ToString();
        }

        private bool IsValidIdCharacter(char c)
        {
            return ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z') || ('0' <= c && c <= '9') || c == '-' || c == '_' || c == '.';
        }
    }
}