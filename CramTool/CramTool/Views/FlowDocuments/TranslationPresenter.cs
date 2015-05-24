using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using CramTool.Models;

namespace CramTool.Views.FlowDocuments
{
    public class TranslationPresenter : Freezable
    {
        public static readonly DependencyProperty WordListProperty = DependencyProperty.Register(
            "WordList", typeof(WordList), typeof(TranslationPresenter), new PropertyMetadata(default(WordList), OnWordListChanged));

        public WordList WordList
        {
            get { return (WordList)GetValue(WordListProperty); }
            set { SetValue(WordListProperty, value); }
        }

        public static readonly DependencyProperty TranslationProperty = DependencyProperty.Register(
            "Translation", typeof(string), typeof(TranslationPresenter), new PropertyMetadata(default(string), OnTranslationChanged));

        public string Translation
        {
            get { return (string)GetValue(TranslationProperty); }
            set { SetValue(TranslationProperty, value); }
        }

        public static readonly DependencyProperty IncludeUnknownWordsProperty = DependencyProperty.Register("IncludeUnknownWords", typeof(bool), typeof(TranslationPresenter), new PropertyMetadata(true));

        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(
            "Document", typeof(FlowDocument), typeof(TranslationPresenter), new PropertyMetadata(default(FlowDocument)));

        public bool IncludeUnknownWords
        {
            get { return (bool)GetValue(IncludeUnknownWordsProperty); }
            set { SetValue(IncludeUnknownWordsProperty, value); }
        }

        public FlowDocument Document
        {
            get { return (FlowDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        private static void OnWordListChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            TranslationPresenter presenter = (TranslationPresenter)depObj;

            WeakEventHelper.UpdateListener<WordList, EventArgs>(args, "ContentsChanged", presenter.OnWordListContentsChanged);

            presenter.Update();
        }

        private static void OnTranslationChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            TranslationPresenter presenter = (TranslationPresenter)depObj;
            presenter.Update();
        }

        private void OnWordListContentsChanged(object sender, EventArgs e)
        {
            Update();
        }

        private void Update()
        {
            if (WordList == null || Translation == null)
            {
                Document = null;
                return;
            }
            Document = FormatDocument();
        }

        private FlowDocument FormatDocument()
        {
            FlowDocument document = new FlowDocument();

            bool includeUnknownWords = IncludeUnknownWords;

            IEnumerable<WordInfo> words = WordList.GetWordsWithTranslation(Translation);
            foreach (WordInfo word in words)
            {
                if (includeUnknownWords || word.IsStudied)
                {
                    AppendWord(document, word);
                }
            }

            return document;
        }

        private void AppendWord(FlowDocument document, WordInfo word)
        {
            ArticleParser parser = new ArticleParser();
            WordArticle article = parser.Parse(word.Word.Name, word.Word.Description);

            Paragraph paragraph = FlowDocumentStyles.CreateParagraph();
            document.Blocks.Add(paragraph);

            IEnumerable<WordFormGroup> formGroups = article.FormGroups.Where(fg => fg.TranslationGroups.SelectMany(tg => tg.Translations).Contains(Translation));
            foreach (WordFormGroup formGroup in formGroups)
            {
                bool firstForm = true;
                foreach (string form in formGroup.Forms)
                {
                    Run run = new Run(form);
                    paragraph.Inlines.Add(run);
                    if (firstForm)
                    {
                        FlowDocumentStyles.FormatWord(run);
                        firstForm = false;
                    }
                    else
                    {
                        FlowDocumentStyles.FormatWordForm(run);
                    }

                    paragraph.Inlines.Add(new LineBreak());
                }
                foreach (WordTranslationGroup translationGroup in formGroup.TranslationGroups.Where(tg => tg.Translations.Contains(Translation)))
                {
                    foreach (string translation in translationGroup.Translations)
                    {
                        Run run = new Run("\u2022 " + translation);
                        paragraph.Inlines.Add(run);
                        FlowDocumentStyles.FormatTranslation(run);
                        paragraph.Inlines.Add(new LineBreak());
                    }
                    foreach (string example in translationGroup.Examples)
                    {
                        Run run = new Run("Example: " + example);
                        paragraph.Inlines.Add(run);
                        FlowDocumentStyles.FormatExample(run);
                        paragraph.Inlines.Add(new LineBreak());
                    }
                }
            }
        }
        
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}