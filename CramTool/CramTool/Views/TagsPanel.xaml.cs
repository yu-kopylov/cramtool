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
    /// Interaction logic for TagsPanel.xaml
    /// </summary>
    public partial class TagsPanel : UserControl
    {
        public static readonly DependencyProperty WordListProperty =
            DependencyProperty.Register("WordList", typeof(WordList), typeof(TagsPanel), new PropertyMetadata(default(WordList), OnWordListChanged));

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(TagsPanel), new PropertyMetadata(default(string), (obj, args) => ((TagsPanel)obj).MarkSearchPending(true)));

        public static readonly DependencyProperty MatchingTagsProperty =
            DependencyProperty.Register("MatchingTags", typeof(ObservableCollection<string>), typeof(TagsPanel), new PropertyMetadata(default(ObservableCollection<string>)));

        public static readonly DependencyProperty CurrentTagProperty =
            DependencyProperty.Register("CurrentTag", typeof(string), typeof(TagsPanel), new PropertyMetadata(default(string), (obj, args) => ((TagsPanel)obj).OnCurrentTagChanged()));

        public static readonly DependencyProperty MatchingWordsProperty =
            DependencyProperty.Register("MatchingWords", typeof(ObservableCollection<WordInfo>), typeof(TagsPanel), new PropertyMetadata(default(ObservableCollection<WordInfo>)));

        public static readonly DependencyProperty CurrentWordProperty =
            DependencyProperty.Register("CurrentWord", typeof(WordInfo), typeof(TagsPanel), new PropertyMetadata(default(WordInfo)));

        private readonly DispatcherTimer timer;
        private bool searchPending = true;
        private bool filterChanged = true;

        public TagsPanel()
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

        public ObservableCollection<string> MatchingTags
        {
            get { return (ObservableCollection<string>)GetValue(MatchingTagsProperty); }
            set { SetValue(MatchingTagsProperty, value); }
        }

        public string CurrentTag
        {
            get { return (string)GetValue(CurrentTagProperty); }
            set { SetValue(CurrentTagProperty, value); }
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

        private static void OnWordListChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TagsPanel panel = (TagsPanel) obj;

            panel.CurrentTag = null;
            panel.CurrentWord = null;

            WeakEventHelper.UpdateListener<WordList, EventArgs>(args, "ContentsChanged", panel.OnWordListContentsChanged);

            panel.MarkSearchPending(false);
        }

        private void OnWordListContentsChanged(object sender, EventArgs e)
        {
            MarkSearchPending(false);
        }

        private void OnCurrentTagChanged()
        {
            CurrentWord = null;

            UpdateMatchingWords();
        }

        private void UpdateMatchingWords()
        {
            if (WordList == null || CurrentTag == null)
            {
                MatchingWords = new ObservableCollection<WordInfo>();
            }
            else
            {
                MatchingWords = new ObservableCollection<WordInfo>(WordList.GetWordsWithTag(CurrentTag));
            }
        }

        private void MarkSearchPending(bool markFilterChanged)
        {
            if (markFilterChanged)
            {
                filterChanged = true;
            }
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

            bool matchFilter = filterChanged;

            searchPending = false;
            filterChanged = false;

            Search(matchFilter);
        }

        private void Search(bool matchFilter)
        {
            string searchText = (SearchText ?? "").Trim();

            IEnumerable<string> tags = WordList.GetAllTags();
            IEnumerable<string> filteredTags = tags.Where(tag => tag.StartsWith(searchText, true, CultureInfo.InvariantCulture)).ToList();

            MatchingTags = new ObservableCollection<string>(filteredTags);

            if (matchFilter)
            {
                CurrentTag = filteredTags.Contains(searchText) ? searchText : null;
            }

            UpdateMatchingWords();
        }
    }
}
