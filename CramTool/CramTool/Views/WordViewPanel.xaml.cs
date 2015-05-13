using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using CramTool.Models;
using CramTool.Views.FlowDocuments;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for WordViewPanel.xaml
    /// </summary>
    public partial class WordViewPanel : UserControl
    {
        public static readonly DependencyProperty WordProperty = DependencyProperty.Register("Word", typeof(Word), typeof(WordViewPanel), new PropertyMetadata(default(Word), OnWordChanged));

        public WordViewPanel()
        {
            InitializeComponent();
        }

        public Word Word
        {
            get { return (Word) GetValue(WordProperty); }
            set { SetValue(WordProperty, value); }
        }

        private static void OnWordChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            WordViewPanel panel = (WordViewPanel) obj;

            WeakEventHelper.UpdateListener<Word, PropertyChangedEventArgs>(args, "PropertyChanged", panel.OnWordPropertyChanged);

            panel.UpdateText();
        }

        private void OnWordPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateText();
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

            Paragraph paragraph = FlowDocumentStyles.CreateParagraph();
            document.Blocks.Add(paragraph);

            {
                Run run = new Run(word.Name);
                paragraph.Inlines.Add(run);
                FlowDocumentStyles.FormatWord(run);
                paragraph.Inlines.Add(new LineBreak());
            }

            ArticleLexer parser = new ArticleLexer();
            List<Token> tokens = parser.Parse(word.Description);

            foreach (Token token in tokens)
            {
                string text = token.Value;
                if (token.Type == TokenType.Translation)
                {
                    text = "\u2022 " + text;
                }
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
                FlowDocumentStyles.FormatWordForm(text);
            }
            else if (tokenType == TokenType.Example)
            {
                FlowDocumentStyles.FormatExample(text);
            }
            else
            {
                FlowDocumentStyles.FormatTranslation(text);
            }
        }
    }
}
