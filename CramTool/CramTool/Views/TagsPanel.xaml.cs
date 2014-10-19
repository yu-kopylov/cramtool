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
            DependencyProperty.Register("WordList", typeof(WordList), typeof(TagsPanel), new PropertyMetadata(default(WordList), (obj, args) => ((TagsPanel)obj).OnWordListChanged()));

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(TagsPanel), new PropertyMetadata(default(string), (obj, args) => ((TagsPanel)obj).MarkSearchPending()));

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

        private void OnWordListChanged()
        {
            CurrentTag = null;
            CurrentWord = null;
            MarkSearchPending();
        }

        private void OnCurrentTagChanged()
        {
            CurrentWord = null;

            if (CurrentTag == null)
            {
                MatchingWords = new ObservableCollection<WordInfo>();
            }
            else
            {
                MatchingWords = new ObservableCollection<WordInfo>(WordList.GetWordsWithTag(CurrentTag));
            }
        }

        private void MarkSearchPending()
        {
            searchPending = true;
        }

        private void Search(object sender, EventArgs eventArgs)
        {
            //todo: consider adding "clear" button for search string
            //todo: change other panels to use WordIndex
            //todo: update search when dictionary is upated
            //todo: update articles when they are updated
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

            IEnumerable<string> tags = WordList.GetAllTags();
            IEnumerable<string> filteredTags = tags.Where(tag => tag.StartsWith(searchText, true, CultureInfo.InvariantCulture)).ToList();

            MatchingTags = new ObservableCollection<string>(filteredTags);

            if (filteredTags.Contains(searchText))
            {
                CurrentTag = searchText;
            }
            else
            {
                CurrentTag = null;
            }
        }
    }
}
