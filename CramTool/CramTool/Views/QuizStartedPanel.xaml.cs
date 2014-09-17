using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CramTool.Models;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for QuizStartedPanel.xaml
    /// </summary>
    public partial class QuizStartedPanel : UserControl
    {
        public static readonly DependencyProperty QuizProperty =
            DependencyProperty.Register("Quiz", typeof (Quiz), typeof (QuizStartedPanel), new PropertyMetadata(default(Quiz)));

        public QuizStartedPanel()
        {
            InitializeComponent();
        }

        public Quiz Quiz
        {
            get { return (Quiz)GetValue(QuizProperty); }
            set { SetValue(QuizProperty, value); }
        }

        private void CanShowAnswer(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Quiz != null && Quiz.CurrentWord != null && !Quiz.CurrentWord.IsShown;
        }

        private void ShowAnswer(object sender, ExecutedRoutedEventArgs e)
        {
            Quiz.CurrentWord.IsShown = true;
        }

        private void CanMoveToNextWord(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Quiz != null && Quiz.Words != null && (UxWords.SelectedIndex + 1) < Quiz.Words.Count;
        }

        private void MoveToNextWord(object sender, ExecutedRoutedEventArgs e)
        {
            UxWords.SelectedIndex = UxWords.SelectedIndex + 1;
        }

        private void CanMarkWordRemembered(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Quiz != null && Quiz.CurrentWord != null && Quiz.CurrentWord.IsShown && Quiz.CurrentWord.WordInfo.IsAdded;
        }

        private void MarkWordRemembered(object sender, ExecutedRoutedEventArgs args)
        {
            Quiz.MarkCurrentWord(WordEventType.Remembered);
        }

        private void CanMarkWordForgotten(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Quiz != null && Quiz.CurrentWord != null && Quiz.CurrentWord.IsShown && Quiz.CurrentWord.WordInfo.IsAdded;
        }

        private void MarkWordForgotten(object sender, ExecutedRoutedEventArgs args)
        {
            Quiz.MarkCurrentWord(WordEventType.Forgotten);
        }
    }
}
