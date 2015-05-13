using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace CramTool.Views.FlowDocuments
{
    public static class FlowDocumentStyles
    {
        public static Paragraph CreateParagraph()
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Margin = new Thickness(0);
            paragraph.FontSize = 12;
            paragraph.LineHeight = 16;
            return paragraph;
        }

        public static void FormatWord(TextElement text)
        {
            text.Foreground = Brushes.Black;
            text.FontStyle = FontStyles.Normal;
            text.FontWeight = FontWeights.Bold;
        }

        public static void FormatWordForm(TextElement text)
        {
            text.Foreground = Brushes.Black;
            text.FontStyle = FontStyles.Oblique;
            text.FontWeight = FontWeights.Bold;
        }

        public static void FormatTranslation(TextElement text)
        {
            text.Foreground = Brushes.Black;
            text.FontStyle = FontStyles.Normal;
            text.FontWeight = FontWeights.Normal;
        }

        public static void FormatExample(TextElement text)
        {
            text.Foreground = Brushes.Black;
            text.FontStyle = FontStyles.Italic;
            text.FontWeight = FontWeights.Normal;
        }
    }
}