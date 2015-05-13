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
            DependencyProperty.Register("WordList", typeof(WordList), typeof(InvLookupPanel), new PropertyMetadata(default(WordList), OnWordListChanged));

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(InvLookupPanel), new PropertyMetadata(default(string), (obj, args) => ((InvLookupPanel)obj).MarkSearchPending(true)));

        public static readonly DependencyProperty MatchingTranslationsProperty =
            DependencyProperty.Register("MatchingTranslations", typeof(ObservableCollection<string>), typeof(InvLookupPanel), new PropertyMetadata(default(ObservableCollection<string>)));

        public static readonly DependencyProperty CurrentTranslationProperty = 
            DependencyProperty.Register("CurrentTranslation", typeof(string), typeof(InvLookupPanel), new PropertyMetadata(default(string)));

        private readonly DispatcherTimer timer;
        private bool searchPending = true;
        private bool filterChanged = true;

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

        public ObservableCollection<string> MatchingTranslations
        {
            get { return (ObservableCollection<string>)GetValue(MatchingTranslationsProperty); }
            set { SetValue(MatchingTranslationsProperty, value); }
        }

        public string CurrentTranslation
        {
            get { return (string)GetValue(CurrentTranslationProperty); }
            set { SetValue(CurrentTranslationProperty, value); }
        }

        private static void OnWordListChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            InvLookupPanel panel = (InvLookupPanel)obj;

            panel.CurrentTranslation = null;

            WeakEventHelper.UpdateListener<WordList, EventArgs>(args, "ContentsChanged", panel.OnWordListContentsChanged);

            panel.MarkSearchPending(false);
        }

        private void OnWordListContentsChanged(object sender, EventArgs e)
        {
            MarkSearchPending(false);
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
            //todo: consider adding "clear" button for search string
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

            IEnumerable<string> translations = WordList.GetAllTranslations();
            IEnumerable<string> filteredTranslations = translations.Where(tr => tr.StartsWith(searchText, true, CultureInfo.InvariantCulture)).ToList();

            MatchingTranslations = new ObservableCollection<string>(filteredTranslations);

            if (matchFilter)
            {
                CurrentTranslation = filteredTranslations.Contains(searchText) ? searchText : null;
            }
        }
    }
}
