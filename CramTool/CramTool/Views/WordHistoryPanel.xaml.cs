﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CramTool.Models;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for WordHistoryPanel.xaml
    /// </summary>
    public partial class WordHistoryPanel : UserControl
    {
        public static readonly DependencyProperty WordInfoProperty = DependencyProperty.Register("WordInfo", typeof(WordInfo), typeof(WordHistoryPanel), new PropertyMetadata(default(WordInfo)));

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(WordHistoryPanel), new PropertyMetadata(false));

        public WordHistoryPanel()
        {
            InitializeComponent();
        }

        public WordInfo WordInfo
        {
            get { return (WordInfo)GetValue(WordInfoProperty); }
            set { SetValue(WordInfoProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        private void CanMarkWordAdded(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsReadOnly && WordInfo != null && !WordInfo.IsStudied;
        }

        private void MarkWordAdded(object sender, ExecutedRoutedEventArgs e)
        {
            WordInfo.WordList.Mark(WordInfo.Word.Name, WordEventType.Added);
        }

        private void CanResetWordHistory(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsReadOnly && WordInfo != null && WordInfo.IsStudied;
        }

        private void ResetWordHistory(object sender, ExecutedRoutedEventArgs e)
        {
            if (!ConfirmationWindow.Confirm(Window.GetWindow(this), string.Format("Reset history for word '{0}'?", WordInfo.Word.Name)))
            {
                return;
            }
            WordInfo.WordList.ResetWordHistory(WordInfo.Word.Name);
        }

        private void CanMarkWordRemembered(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsReadOnly && WordInfo != null && WordInfo.IsStudied;
        }

        private void MarkWordRemembered(object sender, ExecutedRoutedEventArgs args)
        {
            WordInfo.WordList.Mark(WordInfo.Word.Name, WordEventType.Remembered);
        }

        private void CanMarkWordForgotten(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsReadOnly && WordInfo != null && WordInfo.IsStudied;
        }

        private void MarkWordForgotten(object sender, ExecutedRoutedEventArgs args)
        {
            WordInfo.WordList.Mark(WordInfo.Word.Name, WordEventType.Forgotten);
        }
    }
}
