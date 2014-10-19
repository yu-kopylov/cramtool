using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using CramTool.Models;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for WordViewPanel.xaml
    /// </summary>
    public partial class WordViewPanel : UserControl
    {
        public static readonly DependencyProperty WordProperty = DependencyProperty.Register("Word", typeof(Word), typeof(WordViewPanel), new PropertyMetadata(default(Word), WordChanged));

        public WordViewPanel()
        {
            InitializeComponent();
        }

        public Word Word
        {
            get { return (Word) GetValue(WordProperty); }
            set { SetValue(WordProperty, value); }
        }

        private static void WordChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            WordViewPanel panel = (WordViewPanel) obj;
            panel.UpdateText();
        }

        public void UpdateText()
        {
            UxRichText.Document = FormatWord(Word);
        }

        private static FlowDocument FormatWord(Word word)
        {
            FlowDocument document = new FlowDocument();

            if (word == null)
            {
                return document;
            }

            Paragraph paragraph = new Paragraph();
            document.Blocks.Add(paragraph);
            paragraph.Margin = new Thickness(0);

            {
                Run run = new Run(word.Name);
                paragraph.Inlines.Add(run);
                run.FontWeight = FontWeights.Bold;
                paragraph.Inlines.Add(new LineBreak());
            }

            WordParser parser = new WordParser();
            List<Token> tokens = parser.Parse(word.Description);

            foreach (Token token in tokens)
            {
                string text = token.Value;
                if (token.Type == TokenType.Example)
                {
                    text = "Example: " + text;
                }
                Run run = new Run(text);
                paragraph.Inlines.Add(run);
                FormatText(run, token.Type);
            }

            //todo: handle different number of new lines in the end of the artcle
            if (!string.IsNullOrEmpty(word.Tags))
            {
                Paragraph tagsParagraph = new Paragraph();
                document.Blocks.Add(tagsParagraph);
                tagsParagraph.Margin = new Thickness(0, 5, 0, 0);

                Run run = new Run("Tags: ");
                tagsParagraph.Inlines.Add(run);
                run.FontWeight = FontWeights.Bold;

                run = new Run(word.Tags);
                tagsParagraph.Inlines.Add(run);
            }

            return document;
        }

        private static void FormatText(TextElement text, TokenType tokenType)
        {
            if (tokenType == TokenType.WordForm)
            {
                text.Foreground = Brushes.Black;
                text.FontStyle = FontStyles.Oblique;
                text.FontWeight = FontWeights.Bold;
            }
            else if (tokenType == TokenType.Example)
            {
                text.Foreground = Brushes.Black;
                text.FontStyle = FontStyles.Italic;
                text.FontWeight = FontWeights.Normal;
            }
            else
            {
                text.Foreground = Brushes.Black;
                text.FontStyle = FontStyles.Normal;
                text.FontWeight = FontWeights.Normal;
            }
        }
    }
}
