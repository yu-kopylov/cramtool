using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CramTool.Models;
using CramTool.Models.Quizzes;

namespace CramTool.Views
{
    /// <summary>
    /// Interaction logic for QuizPanel.xaml
    /// </summary>
    public partial class QuizPanel : UserControl
    {
        public static readonly DependencyProperty WordListProperty = DependencyProperty.Register("WordList", typeof (WordList), typeof (QuizPanel),
                                                                                                 new PropertyMetadata(default(WordList), (obj, args) => ((QuizPanel) obj).OnWordListChanged()));

        public static readonly DependencyProperty QuizProperty = DependencyProperty.Register("Quiz", typeof (Quiz), typeof (QuizPanel), new PropertyMetadata(new Quiz()));

        public QuizPanel()
        {
            InitializeComponent();
        }

        public WordList WordList
        {
            get { return (WordList) GetValue(WordListProperty); }
            set { SetValue(WordListProperty, value); }
        }

        public Quiz Quiz
        {
            get { return (Quiz) GetValue(QuizProperty); }
            set { SetValue(QuizProperty, value); }
        }

        private void OnWordListChanged()
        {
            Quiz.WordList = WordList;
        }

        private void CanStartQuiz(object sender, CanExecuteRoutedEventArgs e)
        {
            IQuizSettings quizSettings = e.Parameter as IQuizSettings;
            e.CanExecute = Quiz != null && Quiz.QuizStage == QuizStage.Prepare && quizSettings != null;
        }

        private void StartQuiz(object sender, ExecutedRoutedEventArgs e)
        {
            IQuizSettings quizSettings = e.Parameter as IQuizSettings;
            if (quizSettings == null)
            {
                return;
            }
            Quiz.StartQuiz(quizSettings);
        }

        private void CanEndQuiz(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Quiz != null && Quiz.QuizStage == QuizStage.Started;
        }

        private void EndQuiz(object sender, ExecutedRoutedEventArgs e)
        {
            Quiz.ResetQuiz();
        }
    }
}