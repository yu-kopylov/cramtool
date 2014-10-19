using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using CramTool.Models;
using System.Linq;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for LookupPanel.xaml
    /// </summary>
    public partial class LookupPanel : UserControl
    {
        public static readonly DependencyProperty WordListProperty =
            DependencyProperty.Register("WordList", typeof (WordList), typeof (LookupPanel), new PropertyMetadata(default(WordList), (obj, args) => ((LookupPanel) obj).OnWordListChanged()));

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof (string), typeof (LookupPanel), new PropertyMetadata(default(string), (obj, args) => ((LookupPanel) obj).MarkSearchPending()));

        public static readonly DependencyProperty MatchingWordsProperty =
            DependencyProperty.Register("MatchingWords", typeof (ObservableCollection<WordForm>), typeof (LookupPanel), new PropertyMetadata(default(ObservableCollection<WordForm>)));

        public static readonly DependencyProperty CurrentWordFormProperty =
            DependencyProperty.Register("CurrentWordForm", typeof (WordForm), typeof (LookupPanel), new PropertyMetadata(default(WordForm)));

        public static readonly DependencyProperty EditableWordProperty =
            DependencyProperty.Register("EditableWord", typeof(Word), typeof(LookupPanel), new PropertyMetadata(default(Word), (obj, args) => ((LookupPanel)obj).OnEditableWordChanged()));

        public static readonly DependencyProperty SearchEnabledProperty =
            DependencyProperty.Register("SearchEnabled", typeof (bool), typeof (LookupPanel), new PropertyMetadata(true));

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register("IsEditing", typeof(bool), typeof(LookupPanel), new PropertyMetadata(false));

        private readonly DispatcherTimer timer;
        private bool searchPending = true;

        public LookupPanel()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += Search;
            timer.IsEnabled = true;
        }

        public WordList WordList
        {
            get { return (WordList) GetValue(WordListProperty); }
            set { SetValue(WordListProperty, value); }
        }

        public string SearchText
        {
            get { return (string) GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public ObservableCollection<WordForm> MatchingWords
        {
            get { return (ObservableCollection<WordForm>) GetValue(MatchingWordsProperty); }
            set { SetValue(MatchingWordsProperty, value); }
        }

        public WordForm CurrentWordForm
        {
            get { return (WordForm) GetValue(CurrentWordFormProperty); }
            set { SetValue(CurrentWordFormProperty, value); }
        }

        public Word EditableWord
        {
            get { return (Word) GetValue(EditableWordProperty); }
            set { SetValue(EditableWordProperty, value); }
        }

        public bool SearchEnabled
        {
            get { return (bool) GetValue(SearchEnabledProperty); }
            set { SetValue(SearchEnabledProperty, value); }
        }

        public bool IsEditing
        {
            get { return (bool) GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        private void CanAddWord(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditableWord == null;
        }

        private void AddWord(object sender, ExecutedRoutedEventArgs e)
        {
            CurrentWordForm = null;
            EditableWord = new Word();
            EditableWord.Name = (SearchText ?? "").Trim();
            EditableWord.Description = "";
            EditableWord.Tags = "";
        }

        private void CanEditWord(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentWordForm != null && EditableWord == null;
        }

        private void EditWord(object sender, ExecutedRoutedEventArgs e)
        {
            EditableWord = new Word();
            EditableWord.Name = CurrentWordForm.WordInfo.Word.Name;
            EditableWord.Description = CurrentWordForm.WordInfo.Word.Description;
            EditableWord.Tags = CurrentWordForm.WordInfo.Word.Tags;
        }

        private void CanCancelEditWord(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditableWord != null;
        }

        private void CancelEditWord(object sender, ExecutedRoutedEventArgs e)
        {
            EditableWord = null;
        }

        private void CanSaveWord(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditableWord != null && !string.IsNullOrWhiteSpace(EditableWord.Name);
        }

        private void SaveWord(object sender, ExecutedRoutedEventArgs e)
        {
            string oldName = CurrentWordForm == null ? null : CurrentWordForm.WordInfo.Word.Name;
            string newName = (EditableWord.Name ?? "").Trim();
            string description = EditableWord.Description;
            string tags = EditableWord.Tags;

            if (oldName != newName && WordList.Contains(newName))
            {
                throw new Exception(string.Format("Word '{0}' is present in dictionary already.", newName));
            }

            if (oldName == null)
            {
                WordList.Add(newName, description, tags);
            }
            else
            {
                WordList.Update(oldName, newName, description, tags);
            }

            EditableWord = null;
            SearchText = newName;
            MarkSearchPending();
        }

        private void OnWordListChanged()
        {
            CurrentWordForm = null;
            EditableWord = null;
            MarkSearchPending();
        }

        private void OnEditableWordChanged()
        {
            SearchEnabled = EditableWord == null;
            IsEditing = EditableWord != null;
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

            if (!SearchEnabled)
            {
                return;
            }

            if (WordList == null)
            {
                return;
            }

            searchPending = false;

            string searchText = (SearchText ?? "").Trim();

            IEnumerable<WordForm> forms = WordList.GetAllForms();
            IEnumerable<WordForm> filteredForms = forms.Where(w => w.Name.StartsWith(searchText, true, CultureInfo.InvariantCulture)).OrderBy(w => w.Name);
            WordForm matchingWordForm = null;
            foreach (WordForm wordForm in filteredForms)
            {
                if (wordForm.Name.Equals(searchText, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (matchingWordForm == null || wordForm.WordInfo.Word.Name.Equals(searchText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        matchingWordForm = wordForm;
                    }
                }
            }
            MatchingWords = new ObservableCollection<WordForm>(filteredForms);
            if (matchingWordForm != null)
            {
                CurrentWordForm = matchingWordForm;
            }
        }
    }
}