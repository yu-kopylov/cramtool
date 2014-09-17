using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CramTool.Models;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for InvLookupPanel.xaml
    /// </summary>
    public partial class InvLookupPanel : UserControl
    {
        public static readonly DependencyProperty WordListProperty =
            DependencyProperty.Register("WordList", typeof(WordList), typeof(InvLookupPanel), new PropertyMetadata(default(WordList), (obj, args) => ((InvLookupPanel)obj).OnWordListChanged()));

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(InvLookupPanel), new PropertyMetadata(default(string), (obj, args) => ((InvLookupPanel)obj).MarkSearchPending()));

        public static readonly DependencyProperty MatchingTranslationsProperty =
            DependencyProperty.Register("MatchingTranslations", typeof(ObservableCollection<WordTranslation>), typeof(InvLookupPanel), new PropertyMetadata(default(ObservableCollection<WordTranslation>)));

        public static readonly DependencyProperty CurrentTranslationProperty =
            DependencyProperty.Register("CurrentTranslation", typeof(WordTranslation), typeof(InvLookupPanel), new PropertyMetadata(default(WordTranslation), (obj, args) => ((InvLookupPanel)obj).OnCurrentTranslationChanged()));

        public static readonly DependencyProperty MatchingWordsProperty =
            DependencyProperty.Register("MatchingWords", typeof(ObservableCollection<WordInfo>), typeof(InvLookupPanel), new PropertyMetadata(default(ObservableCollection<WordInfo>)));

        public static readonly DependencyProperty CurrentWordProperty =
            DependencyProperty.Register("CurrentWord", typeof(WordInfo), typeof(InvLookupPanel), new PropertyMetadata(default(WordInfo)));

        private readonly DispatcherTimer timer;
        private bool searchPending = true;

        public InvLookupPanel()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += Search;
            timer.IsEnabled = true;
        }

        public WordList WordList
        {
            get { return (WordList)GetValue(WordListProperty); }
            set { SetValue(WordListProperty, value); }
        }

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public ObservableCollection<WordTranslation> MatchingTranslations
        {
            get { return (ObservableCollection<WordTranslation>)GetValue(MatchingTranslationsProperty); }
            set { SetValue(MatchingTranslationsProperty, value); }
        }

        public WordTranslation CurrentTranslation
        {
            get { return (WordTranslation)GetValue(CurrentTranslationProperty); }
            set { SetValue(CurrentTranslationProperty, value); }
        }

        public ObservableCollection<WordInfo> MatchingWords
        {
            get { return (ObservableCollection<WordInfo>)GetValue(MatchingWordsProperty); }
            set { SetValue(MatchingWordsProperty, value); }
        }

        public WordInfo CurrentWord
        {
            get { return (WordInfo)GetValue(CurrentWordProperty); }
            set { SetValue(CurrentWordProperty, value); }
        }

        private void OnWordListChanged()
        {
            CurrentTranslation = null;
            CurrentWord = null;
            MarkSearchPending();
        }

        private void OnCurrentTranslationChanged()
        {
            CurrentWord = null;

            if (CurrentTranslation == null)
            {
                MatchingWords = new ObservableCollection<WordInfo>();
            }
            else
            {
                MatchingWords = new ObservableCollection<WordInfo>(CurrentTranslation.Words);
            }
        }

        private void MarkSearchPending()
        {
            searchPending = true;
        }

        private void Search(object sender, EventArgs eventArgs)
        {
            if (!searchPending)
            {
                return;
            }

            if (WordList == null)
            {
                return;
            }

            searchPending = false;

            string searchText = (SearchText ?? "").Trim();

            IEnumerable<WordTranslation> translations = WordList.GetAllTranslations();
            IEnumerable<WordTranslation> filteredTranslations = translations.Where(w => w.Name.StartsWith(searchText, true, CultureInfo.InvariantCulture)).OrderBy(w => w.Name);
            SortedDictionary<string, WordTranslation> groupedTranslations = new SortedDictionary<string, WordTranslation>();
            foreach (WordTranslation translation in filteredTranslations)
            {
                WordTranslation existingTranslation;
                if (!groupedTranslations.TryGetValue(translation.Name, out existingTranslation))
                {
                    //review why copy is used
                    groupedTranslations.Add(translation.Name, translation.Copy());
                }
                else
                {
                    //todo: make sure words are sorted within translation.Words
                    existingTranslation.Words.AddRange(translation.Words);
                }
            }

            MatchingTranslations = new ObservableCollection<WordTranslation>(groupedTranslations.Values);

            WordTranslation matchingTranslation;

            if (groupedTranslations.TryGetValue(searchText, out matchingTranslation))
            {
                CurrentTranslation = matchingTranslation;
            }
            else
            {
                CurrentTranslation = null;
            }
        }
    }
}
