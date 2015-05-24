using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CramTool.Models;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for HistoryPanel.xaml
    /// </summary>
    public partial class TranslationHistoryPanel : UserControl
    {
        public static readonly DependencyProperty TranslationInfoProperty = DependencyProperty.Register("TranslationInfo", typeof(TranslationInfo), typeof(TranslationHistoryPanel), new PropertyMetadata(default(TranslationInfo)));

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TranslationHistoryPanel), new PropertyMetadata(false));

        public TranslationHistoryPanel()
        {
            InitializeComponent();
        }

        public TranslationInfo TranslationInfo
        {
            get { return (TranslationInfo)GetValue(TranslationInfoProperty); }
            set { SetValue(TranslationInfoProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        private void CanMarkWordRemembered(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsReadOnly && TranslationInfo != null && TranslationInfo.IsStudied;
        }

        private void MarkWordRemembered(object sender, ExecutedRoutedEventArgs args)
        {
            TranslationInfo.WordList.MarkTranslation(WordEventType.Remembered, TranslationInfo.Translation);
        }

        private void CanMarkWordForgotten(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsReadOnly && TranslationInfo != null && TranslationInfo.IsStudied;
        }

        private void MarkWordForgotten(object sender, ExecutedRoutedEventArgs args)
        {
            TranslationInfo.WordList.MarkTranslation(WordEventType.Forgotten, TranslationInfo.Translation);
        }
    }
}
